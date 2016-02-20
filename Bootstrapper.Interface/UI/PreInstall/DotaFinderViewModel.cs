using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Bootstrapper.Interface.Messages;
using Bootstrapper.Interface.Util;
using Caliburn.Micro;
using Screen = Caliburn.Micro.Screen;

namespace Bootstrapper.Interface.UI
{
    class DotaFinderViewModel : Screen, IHandle<DirectorySearchMessages.SearchForDotaDirectory>
    {
        private readonly IEventAggregator _agg;
        private readonly DotaDirectoryLocator _locator;
        private readonly System.Timers.Timer _timer;
        private string _currentSearchLocation;
        private DirectorySearchResult _searchResult;

        public DotaFinderViewModel(IEventAggregator agg, DotaDirectoryLocator locator)
        {
            _agg = agg;
            _locator = locator;

            _agg.Subscribe(this);

            _timer = new System.Timers.Timer();
            _timer.AutoReset = true;
            _timer.Interval = 100d;
            _timer.Enabled = false;

            _timer.Elapsed += TimerOnTick;
        }

        public string CurrentSearchLocation
        {
            get { return _currentSearchLocation; }
            set
            {
                if (value == _currentSearchLocation) return;
                _currentSearchLocation = value;
                NotifyOfPropertyChange(() => CurrentSearchLocation);
            }
        }

        private void TimerOnTick(object sender, EventArgs eventArgs)
        {
            _timer.Stop();
            CurrentSearchLocation = _locator.CurrentSearchLocation ?? "";

            if (_locator.HasStarted)
            {
                if (_locator.IsFound)
                {
                    //Todo:
                    //Theres a race condition here, it's all a bit of a mess.
                    //Consider just waiting for the task to run to completion, and post the result message in a continuation
                    //If the result is not found, give the option to pick manually
                    //Also stop doing this tasks running other tasks rubbish
                    //Also this method is reentrant
                    if (_searchResult == null)
                    {
                        _timer.Start();
                        return;
                    }

                    _agg.PublishOnBackgroundThread(new DirectorySearchMessages.DotaDirectoryFound(_searchResult.Path));
                    return;
                }

                if (!_locator.IsBusy)
                {
                    _agg.PublishOnBackgroundThread(new DirectorySearchMessages.DotaDirectoryNotFound());
                    return;
                }
            }

            _timer.Start();
        }

        public void Handle(DirectorySearchMessages.SearchForDotaDirectory message)
        {
            Task.Run(() =>
            {
                //Put an artifical delay in here, as in most cases it's going to almost instantly finish and just flicker up that viewmodel
                var t = Task.Delay(2500);
                t.Wait(2000);

                _searchResult = _locator.SearchForDotaDirectory();
            });

            _timer.Start();
        }

        public void SpecifyManually()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                bool correctFolder = _locator.SpecifyDirectoryManually(dialog.SelectedPath);

                if (!correctFolder)
                {
                    MessageBox.Show("That is the wrong directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
