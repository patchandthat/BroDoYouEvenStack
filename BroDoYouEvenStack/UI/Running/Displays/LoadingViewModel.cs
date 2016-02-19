using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running.Displays
{
    internal class LoadingViewModel : Screen, IHandle<GameState>
    {
        private readonly IEventAggregator _agg;
        private string _flavourText;
        private string _greeting;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public LoadingViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }

        public string Greeting
        {
            get { return _greeting; }
            set
            {
                if (value == _greeting) return;
                _greeting = value;
                NotifyOfPropertyChange(() => Greeting);
            }
        }

        public string FlavourText
        {
            get { return _flavourText; }
            set
            {
                if (value == _flavourText) return;
                _flavourText = value;
                NotifyOfPropertyChange(() => FlavourText);
            }
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameState message)
        {
            Greeting = $"Hello {message.Player.Name}";
            FlavourText = $"Your game will start soon...";
        }
    }
}