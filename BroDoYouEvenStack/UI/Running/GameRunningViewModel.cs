using BroDoYouEvenStack.Messages;
using Caliburn.Micro;
using Dota2GSI;
using Dota2GSI.Nodes;

namespace BroDoYouEvenStack.UI.Running
{
    class GameRunningViewModel : Screen, IHandle<GameOpened>, IHandle<GameClosed>
    {
        private readonly IEventAggregator _agg;
        private readonly GameStateListener _listener;
        private string _message;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public GameRunningViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
            _listener = new GameStateListener(4000);

            _listener.NewGameState += OnNewGameState;
        }

        private void OnNewGameState(GameState state)
        {
            if (state.Map.GameState == DOTA_GameState.DOTA_GAMERULES_STATE_PRE_GAME)
            {
                Message = $"{state.Map.ClockTime} seconds until game starts.";
                return;
            }

            if (state.Map.GameState == DOTA_GameState.DOTA_GAMERULES_STATE_GAME_IN_PROGRESS)
            {
                //Don't alert if the player is dead...

                var mins = state.Map.ClockTime / 60;
                var secs = state.Map.ClockTime % 60;
                Message = $"Game time is {mins}:{secs.ToString("D2")}";
                return;
            }

            Message = $"{state.Player.Name}: {state.Player.Kills}/{state.Player.Deaths}/{state.Player.Assists}";
        }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value == _message) return;
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
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