using BroDoYouEvenStack.Messages;
using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running
{
    class GameRunningViewModel : Screen, IHandle<GameOpened>, IHandle<GameClosed>
    {
        private readonly IEventAggregator _agg;
        private IScreen _timers;
        private IScreen _config;
        private readonly GameStateListener _listener;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public GameRunningViewModel(IEventAggregator agg, TimersViewModel timers, ConfigViewModel config)
        {
            _agg = agg;
            _timers = timers;
            _config = config;
            _agg.Subscribe(this);
            _listener = new GameStateListener(4000);

            _listener.NewGameState += OnNewGameState;
        }

        public IScreen Config
        {
            get { return _config; }
            set
            {
                if (Equals(value, _config)) return;
                _config = value;
                NotifyOfPropertyChange(() => Config);
            }
        }

        public IScreen Timers
        {
            get { return _timers; }
            set
            {
                if (Equals(value, _timers)) return;
                _timers = value;
                NotifyOfPropertyChange(() => Timers);
            }
        }

        private void OnNewGameState(GameState state)
        {
            _agg.PublishOnBackgroundThread(state);
            //if (state.Map.GameState == DOTA_GameState.DOTA_GAMERULES_STATE_PRE_GAME)
            //{
            //    Message = $"{state.Map.ClockTime} seconds until game starts.";
            //    return;
            //}

            //if (state.Map.GameState == DOTA_GameState.DOTA_GAMERULES_STATE_GAME_IN_PROGRESS)
            //{
            //    //Don't alert if the player is dead...

            //    var mins = state.Map.ClockTime / 60;
            //    var secs = state.Map.ClockTime % 60;
            //    Message = $"Game time is {mins}:{secs.ToString("D2")}";
            //    return;
            //}

            //Message = $"{state.Player.Name}: {state.Player.Kills}/{state.Player.Deaths}/{state.Player.Assists}";
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameOpened message)
        {
            _listener.Start();
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameClosed message)
        {
            _listener.Stop();
        }
    }
}