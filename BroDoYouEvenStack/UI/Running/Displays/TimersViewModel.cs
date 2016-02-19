using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Media;
using BroDoYouEvenStack.Messages;
using BroDoYouEvenStack.UI.Running.Configuration;
using BroDoYouEvenStack.Util;
using Caliburn.Micro;
using Dota2GSI;

namespace BroDoYouEvenStack.UI.Running.Displays
{
    internal class TimersViewModel : Screen, IHandle<ConfigChanged>, IHandle<GameState>
    {
        private readonly IEventAggregator _agg;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public TimersViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);
        }

        //Todo: allow user to override with less obnoxious sounds
        private const string DefaultRuneAlarmPath = "Resources\\horn.wav";
        private const string DefaultCreepAlarmPath = "Resources\\clown.wav";
        public string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        private WarningEvaluator _runeWarning;
        private WarningEvaluator _creepWarning;

        private Config _config;

        private int _runeProgress;
        private int _creepProgress;
        private bool _creepsMuted;
        private bool _runesMuted;
        private string _gameTime;

        //todo: interlock
        private bool _debounceRuneAlarm;
        private bool _debounceCreepAlarm;
        private bool _dead;

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ConfigChanged message)
        {
            _config = message.NewConfig;

            _runeWarning = new WarningEvaluator(
                TimeSpan.FromSeconds(_config.RuneSecondsWarning), 
                WarningType.RuneSpawn);
            _creepWarning = new WarningEvaluator(
                TimeSpan.FromSeconds(_config.CreepSecondsWarning), 
                WarningType.NeutralCreepSpawn);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(GameState message)
        {
            int clocktime = message.Map.ClockTime;
            _dead = !message.Hero.IsAlive;

            //Todo: this will be wrong for games over 60 mins
            int positiveTime = Math.Abs(clocktime);
            var time = TimeSpan.FromSeconds(positiveTime);
            GameTime = time.ToString("mm\\:ss");

            if (clocktime > 0)
            {
                RuneCountdown(clocktime);
                CreepCountdown(clocktime);
            }
        }

        private void CreepCountdown(int clocktime)
        {
            if (_config.CreepToggle && !WarningDurationExpired(clocktime, _config.CreepStopWarningAfterMinutes))
            {
                CreepProgress = _creepWarning.ProgressPercent(clocktime);

                if (_creepWarning.IsTimeForWarning(clocktime))
                {
                    PlayCreepWarningIfNotMuted();
                }
            }
            else
            {
                CreepProgress = 0;
            }
        }

        private void RuneCountdown(int clocktime)
        {
            if (_config.RuneToggle && !WarningDurationExpired(clocktime, _config.RuneStopWarningAfterMinutes))
            {
                RuneProgress = _runeWarning.ProgressPercent(clocktime);

                if (_runeWarning.IsTimeForWarning(clocktime))
                {
                    PlayRuneAlarmIfNotMuted();
                }
            }
            else
            {
                RuneProgress = 0;
            }
        }

        private bool WarningDurationExpired(int clocktime, int maxAlarmMinutes)
        {
            return clocktime / 60 > maxAlarmMinutes;
        }

        private void PlayRuneAlarmIfNotMuted()
        {
            if (!RunesMuted && !_debounceRuneAlarm && !_dead)
            {
                _debounceRuneAlarm = true;
                PlaySound(Path.Combine(AssemblyDirectory, DefaultRuneAlarmPath), _config.RuneWarningVolume);

                Task.Run(async () =>
                {
                    await Task.Delay(950);
                    _debounceRuneAlarm = false;
                });
            }
        }

        private void PlayCreepWarningIfNotMuted()
        {
            if (!CreepsMuted && !_debounceCreepAlarm && !_dead)
            {
                _debounceCreepAlarm = true;
                PlaySound(Path.Combine(AssemblyDirectory, DefaultCreepAlarmPath), _config.CreepWarningVolume);

                Task.Run(async () =>
                {
                    await Task.Delay(950);
                    _debounceCreepAlarm = false;
                });
            }
        }
        
        private void PlaySound(string filePath, double volume)
        {
            try
            {
                var player = new MediaPlayer();
                player.Open(new Uri(filePath));

                player.Volume = volume;

                player.Play();
            }
            catch (Exception ex)
            {
                //Todo:
                //UriformatException
                //TimeoutException -- too slow to load file
                //FileNotFoundException
                //InvalidOperationException -- file is corrupt
                _agg.PublishOnBackgroundThread(new ErrorMessage($"Error trying to play sound {filePath}."));
            }
        }

        public void MuteRunes()
        {
            RunesMuted = !RunesMuted;
        }

        public void MuteCreeps()
        {
            CreepsMuted = !CreepsMuted;
        }

        public string GameTime
        {
            get { return _gameTime; }
            set
            {
                if (value.Equals(_gameTime)) return;
                _gameTime = value;
                NotifyOfPropertyChange(() => GameTime);
            }
        }

        public bool RunesMuted
        {
            get { return _runesMuted; }
            set
            {
                if (value == _runesMuted) return;
                _runesMuted = value;
                NotifyOfPropertyChange(() => RunesMuted);
            }
        }

        public bool CreepsMuted
        {
            get { return _creepsMuted; }
            set
            {
                if (value == _creepsMuted) return;
                _creepsMuted = value;
                NotifyOfPropertyChange(() => CreepsMuted);
            }
        }

        public int CreepProgress
        {
            get { return _creepProgress; }
            set
            {
                if (value == _creepProgress) return;
                _creepProgress = value;
                NotifyOfPropertyChange(() => CreepProgress);
            }
        }

        public int RuneProgress
        {
            get { return _runeProgress; }
            set
            {
                if (value == _runeProgress) return;
                _runeProgress = value;
                NotifyOfPropertyChange(() => RuneProgress);
            }
        }
    }
}