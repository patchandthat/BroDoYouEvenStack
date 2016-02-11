using BroDoYouEvenStack.Messages;
using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running
{
    class TimersViewModel : Screen, IHandle<ConfigChanged>, IHandle<GameState>
    {
        private readonly IEventAggregator _agg;
        private ConfigChanged _config;

        public TimersViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }

        public void Handle(ConfigChanged message)
        {
            _config = message;
        }

        public void Handle(GameState message)
        {
            //Set VM properties using message with respect to config object
        }
    }
}
