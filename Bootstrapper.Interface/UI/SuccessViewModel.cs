using Bootstrapper.Interface.Messages;
using Caliburn.Micro;

namespace Bootstrapper.Interface.UI
{
    class SuccessViewModel : Screen, IHandle<TerminationMessages.Success>
    {
        private readonly IEventAggregator _agg;
        private string _successMessage;

        public SuccessViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }

        public string SuccessMessage
        {
            get { return _successMessage; }
            set
            {
                if (value == _successMessage) return;
                _successMessage = value;
                NotifyOfPropertyChange(() => SuccessMessage);
            }
        }

        public void Handle(TerminationMessages.Success message)
        {
            SuccessMessage = message.Message;
        }
    }
}
