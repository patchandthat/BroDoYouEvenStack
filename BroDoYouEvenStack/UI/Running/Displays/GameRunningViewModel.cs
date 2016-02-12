using BroDoYouEvenStack.Messages;
using BroDoYouEvenStack.UI.Running.Configuration;
using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running.Displays
{
    class GameRunningViewModel : Screen, IHandle<GameOpened>, IHandle<GameClosed>
    {
        private readonly IEventAggregator _agg;
        private IScreen _panels;
        private IScreen _config;
        private readonly GameStateListener _listener;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public GameRunningViewModel(IEventAggregator agg, PanelCoordinatorViewModel panels, ConfigViewModel config)
        {
            _agg = agg;
            _panels = panels;
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

        public IScreen Panels
        {
            get { return _panels; }
            set
            {
                if (Equals(value, _panels)) return;
                _panels = value;
                NotifyOfPropertyChange(() => Panels);
            }
        }

        private void OnNewGameState(GameState state)
        {
            _agg.PublishOnBackgroundThread(state);
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