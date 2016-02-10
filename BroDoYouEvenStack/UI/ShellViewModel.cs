using System.Diagnostics;
using System.Linq;
using System.Timers;
using BroDoYouEvenStack.Messages;
using BroDoYouEvenStack.UI.Idle;
using BroDoYouEvenStack.UI.Running;
using Caliburn.Micro;

namespace BroDoYouEvenStack.UI
{
    class ShellViewModel : Screen
    {
        private readonly IScreen _idle;
        private readonly IScreen _running;
        private readonly IEventAggregator _agg;


        private IScreen _content;
        private double FIVE_SECONDS = 5 * 1000;
        private readonly Timer _timer;
        private bool _lastRunning;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public ShellViewModel(GameNotRunningViewModel idle, GameRunningViewModel running, IEventAggregator agg)
        {
            _idle = idle;
            _running = running;
            _agg = agg;

            DisplayName = "Bro do you even stack?";

            _timer = new Timer(FIVE_SECONDS);
            _timer.Elapsed += (sender, args) => Detect();
            _timer.AutoReset = true;
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            Content = _idle;

            _timer.Start();
        }

        private void Detect()
        {
            var running = Process.GetProcessesByName("dota2").Any();

            if (running != _lastRunning)
            {
                if (running)
                {
                    Content = _running;
                    _agg.PublishOnBackgroundThread(new GameOpened());
                }
                else
                {
                    Content = _idle;
                    _agg.PublishOnBackgroundThread(new GameClosed());
                }
            }

            _lastRunning = running;
        }

        public IScreen Content
        {
            get { return _content; }
            set
            {
                if (Equals(value, _content)) return;
                _content = value;
                NotifyOfPropertyChange(() => Content);
            }
        }
    }
}
