using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Bootstrapper.Interface.Util
{
    class DotaDirectoryLocator
    {
        private const string LikelyLocationsText = "Default install locations";

        private const string RegistryKeyLocation = @"Software\BroDoYouEvenStack";
        private const string ConfigPathValueName = "DotaConfigDir";

        private const string DefaultSteamLibraryInstallDir = @"SteamLibrary\Steamapps\common\dota 2 beta";
        private const string DefaultProgramFilesSteamInstallDir = @"Steam\Steamapps\common\dota 2 beta";

        private const string DotaInstallDir = "dota 2 beta";
        private const string DotaConfigFolderSubPath = @"game\dota\cfg";

        private readonly ConcurrentQueue<string> _dirsToSearch = new ConcurrentQueue<string>();
        private readonly CancellationTokenSource _cts;
        private string _currentSearchLocation;

        public DotaDirectoryLocator()
        {
            IsBusy = false;
            CurrentSearchLocation = "";
            IsFound = false;
            HasStarted = false;

            _cts = new CancellationTokenSource();
        }

        //Todo: lock prop backing vars
        public bool IsFound { get; private set; }
        public bool IsBusy { get; private set; }

        private readonly object _locationLock = new object();
        private volatile bool _doneFindingDirs = false;

        public string CurrentSearchLocation
        {
            get
            {
                lock (_locationLock)
                {
                    return _currentSearchLocation;
                }
            }
            private set
            {
                lock (_locationLock)
                {
                    _currentSearchLocation = value;
                }
            }
        }

        public string UserPath { get; private set; }
        public bool HasStarted { get; private set; }

        public DirectorySearchResult SearchForDotaDirectory()
        {
            if (!IsBusy)
            {
                var result = Task.Run(() =>
                                      {
                                          try
                                          {
                                              HasStarted = true;
                                              IsBusy = true;
                                              return PerformSearch(_cts.Token);
                                          }
                                          catch (OperationCanceledException)
                                          {
                                              CurrentSearchLocation = "Cancelled";
                                              //User specified the directory manually, and it was valid.
                                              return new DirectorySearchResult(SearchOutcome.Found, UserPath);
                                          }
                                          catch (UnauthorizedAccessException)
                                          {
                                              //todo: friendly failure message
                                              throw;
                                          }
                                          catch (IOException)
                                          {
                                              //Todo: Can probably retry in this case
                                              throw;
                                          }
                                          catch (SecurityException)
                                          {
                                              //todo: friendly failure message
                                              throw;
                                          }
                                          finally
                                          {
                                              IsBusy = false;
                                          }
                                      }, _cts.Token);
                return result.Result;
            }

            return new DirectorySearchResult(SearchOutcome.NotFound);
        }

        public bool SpecifyDirectoryManually(string path)
        {
            bool valid = IsValidDota2Directory(path);

            if (valid && !IsFound)
            {
                IsFound = true;
                UserPath = Path.Combine(path, DotaConfigFolderSubPath);
                try
                {
                    _cts.Cancel();
                }
                catch(Exception ex)
                { }
            }

            return valid;
        }

        private bool IsValidDota2Directory(string path)
        {
            if (!Directory.Exists(path)) return false;

            if (path.EndsWith("dota 2 beta", StringComparison.InvariantCultureIgnoreCase))
            {
                return true;
            }
            
            //If we have a subdir, backtrack to dota2beta
            DirectoryInfo parent = Directory.GetParent(path);
            while (parent != null)
            {
                path = parent.FullName;
                if (path.EndsWith("dota 2 beta", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }

                parent = Directory.GetParent(path);
            }

            return false;
        }

        private DirectorySearchResult PerformSearch(CancellationToken token)
        {
            var cts = new CancellationTokenSource();
            EnqueueDirsToSearchAsync(cts.Token);

            string configPath;

            configPath = GetConfigPathFromRegistryOrNull(token);
            if (!string.IsNullOrEmpty(configPath))
            {
                cts.CancelAfter(1000);
                return new DirectorySearchResult(SearchOutcome.Found, configPath);
            }

            configPath = CheckDefaultSteamDirectoryOrNull(token);
            if (!string.IsNullOrEmpty(configPath))
            {
                cts.CancelAfter(1000);
                return new DirectorySearchResult(SearchOutcome.Found, configPath);
            }

            configPath = CheckAllDrivesForSteamLibrariesOrNull(token);
            if (!string.IsNullOrEmpty(configPath))
            {
                cts.CancelAfter(1000);
                return new DirectorySearchResult(SearchOutcome.Found, configPath);
            }

            configPath = SearchAllFixedDrivesOrNull(token);
            if (!string.IsNullOrEmpty(configPath))
            {
                cts.CancelAfter(1000);
                return new DirectorySearchResult(SearchOutcome.Found, configPath);
            }

            return new DirectorySearchResult(SearchOutcome.NotFound);
        }

        private string GetConfigPathFromRegistryOrNull(CancellationToken token)
        {
            CurrentSearchLocation = "Checking for previous installations";
            string path = null;

            using (var key = Registry.LocalMachine.OpenSubKey(RegistryKeyLocation, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey))
            {
                object o = key?.GetValue(ConfigPathValueName);
                if (o != null)
                {
                    path = o as string;
                    if (path != null) IsFound = true;
                }
            }

            return path;
        }

        private string CheckDefaultSteamDirectoryOrNull(CancellationToken token)
        {
            CurrentSearchLocation = LikelyLocationsText;
            token.ThrowIfCancellationRequested();

            var basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), DefaultProgramFilesSteamInstallDir);

            if (Directory.Exists(basePath))
            {
                IsFound = true;
                return Path.Combine(basePath, DotaConfigFolderSubPath);
            }

            basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), DefaultProgramFilesSteamInstallDir);

            if (Directory.Exists(basePath))
            {
                IsFound = true;
                return Path.Combine(basePath, DotaConfigFolderSubPath);
            }

            return null;
        }

        private string CheckAllDrivesForSteamLibrariesOrNull(CancellationToken token)
        {
            CurrentSearchLocation = LikelyLocationsText;

            foreach (var driveInfo in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed))
            {
                token.ThrowIfCancellationRequested();

                var path = Path.Combine(driveInfo.Name, DefaultSteamLibraryInstallDir);
                
                if (Directory.Exists(path))
                    return path;
            }

            return null;
        }

        private string SearchAllFixedDrivesOrNull(CancellationToken token)
        {
            var culture = CultureInfo.InvariantCulture;
            string combinedSubPath = Path.Combine(DotaInstallDir, DotaConfigFolderSubPath);

            while (_dirsToSearch.IsEmpty)
            {
                Task.Delay(100).Wait();
            }

            while (true)
            {
                token.ThrowIfCancellationRequested();

                string dir;
                _dirsToSearch.TryDequeue(out dir);

                if (dir != null)
                {
                    CurrentSearchLocation = dir;

                    if (culture.CompareInfo.IndexOf(dir, combinedSubPath, CompareOptions.IgnoreCase) >= 0)
                    {
                        IsFound = true;
                        return dir;
                    }
                }

                if (_doneFindingDirs && _dirsToSearch.IsEmpty)
                {
                    return null;
                }
            }
        }

        private void EnqueueDirsToSearchAsync(CancellationToken token)
        {
            Task.Run(() =>
                     {
                         foreach (var driveInfo in DriveInfo.GetDrives().Where(d => d.DriveType == DriveType.Fixed))
                         {
                             var rootDirs = Directory.EnumerateDirectories(driveInfo.Name);

                             foreach (string dir in rootDirs)
                             {
                                 try
                                 {
                                     if (token.IsCancellationRequested)
                                     {
                                         return;
                                     }

                                     _dirsToSearch.Enqueue(dir);
                                     GetAllFoldersUnder(dir, token);
                                 }
                                 catch (UnauthorizedAccessException ex)
                                 {
                                     //Keep going for dirs that we do have permission to
                                 }
                                 catch (OperationCanceledException)
                                 { }
                             }
                         }
                     }, token)
                .ContinueWith((t) =>
                              {
                                  _doneFindingDirs = true;
                              }, token);
        }

        private void GetAllFoldersUnder(string path, CancellationToken token)
        {
            if (token.IsCancellationRequested) return;

            try
            {
                foreach (string folder in Directory.GetDirectories(path))
                {
                    _dirsToSearch.Enqueue(folder);
                    GetAllFoldersUnder(folder, token);
                }
            }
            catch (UnauthorizedAccessException ex) { }
        }
    }

    internal class SearchLocation
    {
        public SearchLocation(string value)
        {
            Location = value;
        }

        public string Location { get; }
    }
}
