using System.Diagnostics;
using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running.Displays
{
    internal class IdleViewModel : Screen, IHandle<GameState>
    {
        private readonly IEventAggregator _agg;
        private string _greeting;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public IdleViewModel(IEventAggregator agg)
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

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameState message)
        {
            Greeting = $"Hello {message.Player.Name}";
        }

        public void GithubButton()
        {
            Process.Start(@"https://github.com/patchandthat/BroDoYouEvenStack");
        }
    }
}