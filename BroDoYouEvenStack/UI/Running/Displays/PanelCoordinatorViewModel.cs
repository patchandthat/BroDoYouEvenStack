using System.Collections.Generic;
using Caliburn.Micro;
using Dota2GSI;
using Dota2GSI.Nodes;

namespace BroDoYouEvenStack.UI.Running.Displays
{
    class PanelCoordinatorViewModel : Screen, IHandle<GameState>
    {
        private readonly Dictionary<DOTA_GameState, IScreen> _stateScreens;
        private IScreen _activeScreen;

        public PanelCoordinatorViewModel(IEventAggregator agg, IdleViewModel inactive, LoadingViewModel loading, TimersViewModel timers)
        {
            agg.Subscribe(this);

            _stateScreens = new Dictionary<DOTA_GameState, IScreen>();
            
            _stateScreens.Add(DOTA_GameState.Undefined, inactive);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_INIT, loading);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_WAIT_FOR_PLAYERS_TO_LOAD, loading);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_STRATEGY_TIME, loading);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_HERO_SELECTION, loading);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_PRE_GAME, timers);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_GAME_IN_PROGRESS, timers);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_POST_GAME, inactive);
            _stateScreens.Add(DOTA_GameState.DOTA_GAMERULES_STATE_DISCONNECT, inactive);

            ActiveScreen = inactive;
        }

        public IScreen ActiveScreen
        {
            get { return _activeScreen; }
            set
            {
                if (Equals(value, _activeScreen)) return;
                _activeScreen = value;
                NotifyOfPropertyChange(() => ActiveScreen);
            }
        }

        public void Handle(GameState message)
        {
            //Todo: check that states actually are what you expect

            ActiveScreen = _stateScreens[message.Map.GameState];
        }
    }
}
