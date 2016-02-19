using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using Bootstrapper.Interface.Messages;
using Bootstrapper.Interface.UI.Terminal;
using Bootstrapper.Interface.Util;
using Caliburn.Micro;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;
using Application = System.Windows.Application;
using ErrorEventArgs = Microsoft.Tools.WindowsInstallerXml.Bootstrapper.ErrorEventArgs;
using MessageBox = System.Windows.MessageBox;
using Screen = Caliburn.Micro.Screen;

namespace Bootstrapper.Interface.UI
{
    public enum InstallationState
    {
        Initializing,
        DetectedAbsent,
        DetectedPresent,
        DetectedNewer,
        Applying,
        Applied,
        Failed,
    }

    class ShellViewModel : Screen,
        IHandle<DirectorySearchMessages.SearchForDotaDirectory>,
        IHandle<DirectorySearchMessages.DotaDirectoryFound>,
        IHandle<DirectorySearchMessages.DotaDirectoryNotFound>,
        IHandle<TerminationMessages.Success>,
        IHandle<TerminationMessages.Error>
    {
        private const int ERROR_UserCancelled = 1223;
        private IntPtr hwnd;
        private Dictionary<string, int> downloadRetries = new Dictionary<string, int>();
        private LaunchAction _plannedAction;
        private InstallationState _state;

        private readonly BootstrapperApplication _ba;
        private readonly IEventAggregator _agg;
        private PropertyChangedBase _activeViewModel;
        private readonly List<IScreen> _screens;

        private bool _dotaDirectoryKnown = false;

        public ShellViewModel(BootstrapperApplication bootStrapper, IEventAggregator agg, 
            DotaFinderViewModel finder,
            DotaDetectedViewModel detected,
            BusyViewModel busy,
            ErrorViewModel error,
            InstallSuccessViewModel installSuccess,
            UninstallSuccessViewModel uninstallSuccess)
        {
            DisplayName = "Bro do you even stack? Installer";

            _ba = bootStrapper;
            _agg = agg;
            _agg.Subscribe(this);

            //engine events
            _ba.DetectBegin += DetectBegin;
            _ba.DetectPackageComplete += DetectedPackage;
            _ba.DetectRelatedBundle += DetectedRelatedBundle;
            _ba.DetectComplete += DetectComplete;

            _ba.PlanPackageBegin += PlanPackageBegin;
            _ba.PlanComplete += PlanComplete;

            _ba.ApplyBegin += ApplyBegin;
            _ba.ApplyComplete += ApplyComplete;

            _ba.ResolveSource += ResolveSource;
            _ba.Error += ExecuteError;

            _ba.ExecuteMsiMessage += this.ExecuteMsiMessage;
            _ba.ExecuteProgress += this.ApplyExecuteProgress;
            _ba.Progress += this.ApplyProgress;
            _ba.CacheAcquireProgress += this.CacheAcquireProgress;
            _ba.CacheComplete += this.CacheComplete;

            _screens = new List<IScreen>()
            {
                finder,
                detected,
                busy,
                error,
                installSuccess,
                uninstallSuccess
            };

            _agg.PublishOnBackgroundThread(new DirectorySearchMessages.SearchForDotaDirectory());
        }

        public PropertyChangedBase ActiveViewModel
        {
            get { return _activeViewModel; }
            set
            {
                if (Equals(value, _activeViewModel)) return;
                _activeViewModel = value;
                NotifyOfPropertyChange(() => ActiveViewModel);
            }
        }

        public void Handle(DirectorySearchMessages.SearchForDotaDirectory message)
        {
            ActiveViewModel = _screens.OfType<DotaFinderViewModel>().First();
            NotifyStateChanged();
        }

        public void Handle(DirectorySearchMessages.DotaDirectoryFound message)
        {
            _dotaDirectoryKnown = true;
            _ba.Engine.StringVariables["DotaConfigDir"] = message.Path;

            ActiveViewModel = _screens.OfType<DotaDetectedViewModel>().First();
            NotifyStateChanged();
        }

        public void Handle(DirectorySearchMessages.DotaDirectoryNotFound message)
        {
            _agg.PublishOnBackgroundThread(new TerminationMessages.Error("Unable to locate your Dota 2 installation directory"));
            NotifyStateChanged();
        }

        public void Handle(TerminationMessages.Success message)
        {
            ActiveViewModel = _plannedAction == LaunchAction.Install
                ? (PropertyChangedBase)_screens.OfType<InstallSuccessViewModel>().First()
                : (PropertyChangedBase)_screens.OfType<UninstallSuccessViewModel>().First();
            NotifyStateChanged();
        }

        public void Handle(TerminationMessages.Error message)
        {
            ActiveViewModel = _screens.OfType<ErrorViewModel>().First();
            NotifyStateChanged();
        }

        public bool CanInstall => State == InstallationState.DetectedAbsent && _dotaDirectoryKnown;
        public bool CanUninstall => State == InstallationState.DetectedPresent;
        public bool CanUpdate => State == InstallationState.DetectedPresent && _dotaDirectoryKnown;
        public bool CanExitAndLaunch => State == InstallationState.Applied && _plannedAction == LaunchAction.Install;

        private void NotifyStateChanged()
        {
            NotifyOfPropertyChange(() => State);
            NotifyOfPropertyChange(() => ProgressEnabled);

            NotifyOfPropertyChange(() => CompleteEnabled);
            NotifyOfPropertyChange(() => CanInstall);
            NotifyOfPropertyChange(() => CanUninstall);
            NotifyOfPropertyChange(() => CanUpdate);
            NotifyOfPropertyChange(() => CanExitAndLaunch);
            NotifyOfPropertyChange(() => CanExit);
        }

        public void Install()
        {
            ActiveViewModel = _screens.OfType<BusyViewModel>().First();
            _plannedAction = LaunchAction.Install;
            _ba.Engine.Plan(LaunchAction.Install);

        }

        public void Uninstall()
        {
            ActiveViewModel = _screens.OfType<BusyViewModel>().First();
            _plannedAction = LaunchAction.Uninstall;
            this.Plan(LaunchAction.Uninstall);
        }

        public void Update()
        {
            ActiveViewModel = _screens.OfType<BusyViewModel>().First();
            _plannedAction = LaunchAction.Install;
            Plan(LaunchAction.UpdateReplace);
        }

        public void ExitAndLaunch()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                "BroDoYouEvenStack\\BroDoyouevenStack.exe");
            if (File.Exists(path))
            {
                var info = new ProcessStartInfo(path)
                {
                    WorkingDirectory = Path.GetDirectoryName(path)
                };

                Process.Start(info);
            }

            this.TryClose();
        }

        #region Engine Events

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _ba.Engine.Detect();
        }

        public InstallationState PreApplyState { get; set; }

        public InstallationState State
        {
            get { return _state; }
            set
            {
                if (value == _state) return;
                _state = value;
                NotifyStateChanged();
            }
        }

        public bool CanExit { get { return !this.ProgressEnabled; } }
        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public bool Cancelled { get; set; }
        public bool Downgrade { get; set; }
        public string Message { get; set; }
        public int CacheProgress { get; set; }
        public int ExecuteProgress { get; set; }

        public int Progress { get { return (this.CacheProgress + this.ExecuteProgress) / 2; } }
        public bool ProgressEnabled { get { return this.State == InstallationState.Applying; } }
        public bool CompleteEnabled { get { return this.State == InstallationState.Applied; } }

        private void Plan(LaunchAction action)
        {
            this._plannedAction = action;
            this.hwnd = (Application.Current.MainWindow == null) ? IntPtr.Zero : new WindowInteropHelper(Application.Current.MainWindow).Handle;

            this.Cancelled = false;
            _ba.Engine.Plan(action);
        }

        public static bool HResultSucceeded(int status)
        {
            return status >= 0;
        }

        private void DetectBegin(object sender, DetectBeginEventArgs e)
        {
            this.State = InstallationState.Initializing;
        }

        private void DetectedPackage(object sender, DetectPackageCompleteEventArgs e)
        {
            // The Package ID from the Bootstrapper chain.
            if (e.PackageId.Equals("bdyesInstaller", StringComparison.Ordinal))
            {
                this.State = (e.State == PackageState.Present) ? InstallationState.DetectedPresent : InstallationState.DetectedAbsent;
            }
        }

        private void DetectedRelatedBundle(object sender, DetectRelatedBundleEventArgs e)
        {
            if (e.Operation == RelatedOperation.Downgrade)
            {
                this.Downgrade = true;
            }
        }

        private void DetectComplete(object sender, DetectCompleteEventArgs e)
        {
            if (_ba.Command.Action == LaunchAction.Uninstall)
            {
                _ba.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for uninstall");
                Execute.OnUIThread(() =>
                {
                    _plannedAction = LaunchAction.Uninstall;
                    this.Plan(LaunchAction.Uninstall);
                });
            }
            else if (HResultSucceeded(e.Status))
            {
                if (this.Downgrade)
                {
                    // Downgrades are not allowed we just display and error and let the user exit.
                    this.State = InstallationState.DetectedNewer;
                }

                // If we're not waiting for the user to click install, dispatch plan with the default action.
                if (_ba.Command.Display != Display.Full)
                {
                    _ba.Engine.Log(LogLevel.Verbose, "Invoking automatic plan for non-interactive mode.");
                    Execute.OnUIThread(() =>
                    {
                        this.Plan(_ba.Command.Action);
                    });
                }
            }
            else
            {
                this.State = InstallationState.Failed;
            }
        }

        private void PlanPackageBegin(object sender, PlanPackageBeginEventArgs e)
        {
            // Turns off .NET install when setting up the install plan as we already have it.
            //if (e.PackageId.Equals(_ba.Engine.StringVariables["WixMbaPrereqPackageId"], StringComparison.Ordinal))
            if (e.PackageId.Equals("NetFx46Web", StringComparison.Ordinal))
            {
                e.State = RequestState.None;
            }
        }

        private void PlanComplete(object sender, PlanCompleteEventArgs e)
        {
            if (HResultSucceeded(e.Status))
            {
                this.PreApplyState = this.State;
                this.State = InstallationState.Applying;
                _ba.Engine.Apply(this.hwnd);
            }
            else
            {
                this.State = InstallationState.Failed;
            }
        }

        private void ApplyBegin(object sender, ApplyBeginEventArgs e)
        {
            this.downloadRetries.Clear();
        }

        private void ApplyComplete(object sender, ApplyCompleteEventArgs e)
        {

            // Set the state to applied or failed unless the state has already been set back to the preapply state
            // which means we need to show the UI as it was before the apply started.
            if (this.State != this.PreApplyState)
            {
                this.State = HResultSucceeded(e.Status) ? InstallationState.Applied : InstallationState.Failed;

                if (State == InstallationState.Applied)
                {
                    _agg.PublishOnBackgroundThread(new TerminationMessages.Success("Installed successfully. GLHF."));
                }
                else
                {
                    _agg.PublishOnBackgroundThread(new TerminationMessages.Error("Installation failed. No changes have been made to your computer."));
                }
            }

            // If we're not in Full UI mode, we need to alert the dispatcher to stop and close the window for passive.
            if (_ba.Command.Display != Display.Full)
            {
                // If its passive, send a message to the window to close.
                if (_ba.Command.Display == Display.Passive || _ba.Command.Display == Display.Embedded)
                {
                    _ba.Engine.Log(LogLevel.Verbose, "Automatically closing the window for non-interactive install");
                    this.TryClose();
                }
                else
                {
                    Application.Current.Shutdown();
                }
            }

        }

        private void ResolveSource(object sender, ResolveSourceEventArgs e)
        {
            int retries = 0;

            this.downloadRetries.TryGetValue(e.PackageOrContainerId, out retries);
            this.downloadRetries[e.PackageOrContainerId] = retries + 1;

            e.Result = retries < 3 && !String.IsNullOrEmpty(e.DownloadSource) ? Result.Download : Result.Ok;
        }

        private void ExecuteError(object sender, ErrorEventArgs e)
        {
            lock (this)
            {
                if (!this.Cancelled)
                {
                    // If the error is a cancel coming from the engine during apply we want to go back to the preapply state.
                    if (this.State == InstallationState.Applying && e.ErrorCode == ERROR_UserCancelled)
                    {
                        this.State = this.PreApplyState;
                    }
                    else
                    {
                        this.Message = e.ErrorMessage;
                        Execute.OnUIThread(() =>
                        {
                            MessageBox.Show(Application.Current.MainWindow, e.ErrorMessage, "WiX Toolset", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                }

                e.Result = this.Cancelled ? Result.Cancel : Result.Ok;
            }
        }

        private void ExecuteMsiMessage(object sender, ExecuteMsiMessageEventArgs e)
        {
            lock (this)
            {
                this.Message = e.Message;
                e.Result = this.Cancelled ? Result.Cancel : Result.Ok;
            }
        }

        private void ApplyProgress(object sender, ProgressEventArgs e)
        {
            lock (this)
            {
                e.Result = this.Cancelled ? Result.Cancel : Result.Ok;
            }
        }

        private void CacheAcquireProgress(object sender, CacheAcquireProgressEventArgs e)
        {
            lock (this)
            {
                this.CacheProgress = e.OverallPercentage;
                e.Result = this.Cancelled ? Result.Cancel : Result.Ok;
            }
        }

        private void CacheComplete(object sender, CacheCompleteEventArgs e)
        {
            lock (this)
            {
                this.CacheProgress = 100;
            }
        }

        private void ApplyExecuteProgress(object sender, ExecuteProgressEventArgs e)
        {
            lock (this)
            {
                this.ExecuteProgress = e.OverallPercentage;

                if (_ba.Command.Display == Display.Embedded)
                {
                    _ba.Engine.SendEmbeddedProgress(e.ProgressPercentage, this.Progress);
                }

                e.Result = this.Cancelled ? Result.Cancel : Result.Ok;
            }
        }

        #endregion
    }
}
