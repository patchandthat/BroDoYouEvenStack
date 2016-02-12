using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running.Displays
{
    internal class IdleViewModel : Screen, IHandle<GameState>
    {
        private readonly IEventAggregator _agg;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public IdleViewModel(IEventAggregator agg)
        {
            _agg = agg;
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameState message)
        {
            
        }
    }
}