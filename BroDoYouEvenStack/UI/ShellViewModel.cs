﻿using System.Diagnostics;
using System.Linq;
using System.Timers;
using BroDoYouEvenStack.Messages;
using BroDoYouEvenStack.UI.Idle;
using BroDoYouEvenStack.UI.Running.Displays;
using Caliburn.Micro;

namespace BroDoYouEvenStack.UI
{
    class ShellViewModel : Screen
    {
        private readonly IScreen _idle;
        private readonly IScreen _running;
        private readonly IEventAggregator _agg;


        private IScreen _windowContent;
        private double INTERVAL = 3 * 1000;
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

            _timer = new Timer(INTERVAL);
            _timer.Elapsed += (sender, args) => Detect();
            _timer.AutoReset = true;

            agg.PublishOnBackgroundThread(new FirstLoad());
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            WindowContent = _idle;

            _timer.Start();
        }

        private void Detect()
        {
            var isRunning = Process.GetProcessesByName("dota2").Any();

            if (isRunning != _lastRunning)
            {
                if (isRunning)
                {
                    WindowContent = _running;
                    _agg.PublishOnBackgroundThread(new GameOpened());
                }
                else
                {
                    WindowContent = _idle;
                    _agg.PublishOnBackgroundThread(new GameClosed());
                }
            }

            _lastRunning = isRunning;
        }

        public IScreen WindowContent
        {
            get { return _windowContent; }
            set
            {
                if (Equals(value, _windowContent)) return;
                _windowContent = value;
                NotifyOfPropertyChange(() => WindowContent);
            }
        }
    }
}
