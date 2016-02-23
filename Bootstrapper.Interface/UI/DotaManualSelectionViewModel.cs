using System.Windows.Forms;
using Bootstrapper.Interface.Messages;
using Caliburn.Micro;
using Screen = Caliburn.Micro.Screen;

namespace Bootstrapper.Interface.UI
{
    class DotaManualSelectionViewModel : Screen
    {
        private readonly IEventAggregator _agg;
        private string _selectedDirectory;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public DotaManualSelectionViewModel(IEventAggregator agg)
        {
            _agg = agg;

            SelectedDirectory = "";
        }

        /// <summary>
        /// I'm not validating this, if the user selects wrong, they can uninstall and do it right
        /// Given that we're at this point to start with, it might be hard to validate the location exists, as we couldn't find it in the first place
        /// </summary>
        public void PickDir()
        {
            var dialog = new FolderBrowserDialog();
            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                SelectedDirectory = dialog.SelectedPath;
            }
        }

        public string SelectedDirectory
        {
            get { return _selectedDirectory; }
            set
            {
                if (value == _selectedDirectory) return;
                _selectedDirectory = value;
                NotifyOfPropertyChange(() => SelectedDirectory);
                NotifyOfPropertyChange(() => CanConfirm);
            }
        }

        public void Confirm()
        {
            _agg.PublishOnBackgroundThread(new DirectorySearchMessages.DotaDirectoryFound(SelectedDirectory));
        }

        public bool CanConfirm => !string.IsNullOrWhiteSpace(SelectedDirectory);
    }
}
