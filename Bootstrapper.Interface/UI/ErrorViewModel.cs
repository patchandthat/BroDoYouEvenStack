using Bootstrapper.Interface.Messages;
using Caliburn.Micro;

namespace Bootstrapper.Interface.UI
{
    class ErrorViewModel : Screen, IHandle<TerminationMessages.Error>
    {
        private readonly IEventAggregator _agg;
        private string _errorMessage;

        public ErrorViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value == _errorMessage) return;
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public void Handle(TerminationMessages.Error message)
        {
            ErrorMessage = message.Message;
        }
    }
}
