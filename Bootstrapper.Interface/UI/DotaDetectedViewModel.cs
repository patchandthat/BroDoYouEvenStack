using Bootstrapper.Interface.Messages;
using Caliburn.Micro;

namespace Bootstrapper.Interface.UI
{
    class DotaDetectedViewModel : Screen, IHandle<DirectorySearchMessages.DotaDirectoryFound>
    {
        private readonly IEventAggregator _agg;
        private string _dotaConfigDirectory;

        public DotaDetectedViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }

        public string DotaConfigDirectory
        {
            get { return _dotaConfigDirectory; }
            set
            {
                if (value == _dotaConfigDirectory) return;
                _dotaConfigDirectory = value;
                NotifyOfPropertyChange(() => DotaConfigDirectory);
            }
        }

        public void Handle(DirectorySearchMessages.DotaDirectoryFound message)
        {
            DotaConfigDirectory = message.Path;
        }
    }
}
