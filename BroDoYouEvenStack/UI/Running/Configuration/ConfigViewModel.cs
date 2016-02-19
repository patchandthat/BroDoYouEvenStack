using System;
using System.IO;
using System.Threading.Tasks;
using BroDoYouEvenStack.Messages;
using Caliburn.Micro;
using Newtonsoft.Json;

namespace BroDoYouEvenStack.UI.Running.Configuration
{
    class ConfigViewModel : Screen, IHandle<FirstLoad>
    {
        private readonly IEventAggregator _agg;
        private int _creepStopWarningAfterMinutes;
        private int _runeStopWarningAfterMinutes;
        private int _creepSecondsWarning;
        private int _runeSecondsWarning;
        private bool _creepToggle;
        private bool _runeToggle;
        private int _gsiPort;
        private const int DEFAULT_PORT = 4000;

        //Todo: interlock
        private bool _isLoaded = false;

        private const string ConfigFileName = "Bdyes.config";

        private readonly string _appDataDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BroDoYouEvenStack");

        private int _creepWarningVolume;
        private int _runeWarningVolume;

        public ConfigViewModel(IEventAggregator agg)
        {
            _agg = agg;
            _agg.Subscribe(this);

            _gsiPort = DEFAULT_PORT;
        }

        public bool RuneToggle
        {
            get { return _runeToggle; }
            set
            {
                if (value == _runeToggle) return;
                _runeToggle = value;
                NotifyOfPropertyChange(() => RuneToggle);

                if (!_isLoaded) return;
                ConfigChanged();
            }
        }

        public bool CreepToggle
        {
            get { return _creepToggle; }
            set
            {
                if (value == _creepToggle) return;
                _creepToggle = value;
                NotifyOfPropertyChange(() => CreepToggle);

                if (!_isLoaded) return;
                ConfigChanged();
            }
        }

        public int RuneSecondsWarning
        {
            get { return _runeSecondsWarning; }
            set
            {
                if (value == _runeSecondsWarning) return;
                _runeSecondsWarning = value;
                NotifyOfPropertyChange(() => RuneSecondsWarning);

                if (!_isLoaded) return;
                ConfigChanged();
            }
        }

        public int CreepSecondsWarning
        {
            get { return _creepSecondsWarning; }
            set
            {
                if (value == _creepSecondsWarning) return;
                _creepSecondsWarning = value;
                NotifyOfPropertyChange(() => CreepSecondsWarning);

                if (!_isLoaded) return;
                ConfigChanged();
            }
        }

        public int RuneStopWarningAfterMinutes
        {
            get { return _runeStopWarningAfterMinutes; }
            set
            {
                if (value == _runeStopWarningAfterMinutes) return;
                _runeStopWarningAfterMinutes = value;
                NotifyOfPropertyChange(() => RuneStopWarningAfterMinutes);

                if (!_isLoaded) return;
                ConfigChanged();
            }
        }

        public int CreepStopWarningAfterMinutes
        {
            get { return _creepStopWarningAfterMinutes; }
            set
            {
                if (value == _creepStopWarningAfterMinutes) return;
                _creepStopWarningAfterMinutes = value;
                NotifyOfPropertyChange(() => CreepStopWarningAfterMinutes);

                if (!_isLoaded) return;
                ConfigChanged();
            }
        }

        public int RuneWarningVolume
        {
            get { return _runeWarningVolume; }
            set
            {
                if (value == _runeWarningVolume) return;
                _runeWarningVolume = value;
                NotifyOfPropertyChange(() => RuneWarningVolume);
                ConfigChanged();
            }
        }

        public int CreepWarningVolume
        {
            get { return _creepWarningVolume; }
            set
            {
                if (value == _creepWarningVolume) return;
                _creepWarningVolume = value;
                NotifyOfPropertyChange(() => CreepWarningVolume);
                ConfigChanged();
            }
        }

        private void ConfigChanged()
        {
            var config = GetConfigObject();

            _agg.PublishOnBackgroundThread(new ConfigChanged(config));

            if (!_isLoaded) return;
            //Todo: test, maybe bang these into a concurrent queue, may get loads of these requests when you whizz the slider up and down.
            Task.Run(() => Save(config));
        }

        private Config GetConfigObject()
        {
            var config = new Config(
                _runeToggle,
                _creepToggle,
                _runeSecondsWarning,
                _creepSecondsWarning,
                _runeStopWarningAfterMinutes,
                _creepStopWarningAfterMinutes,
                _gsiPort, 
                _runeWarningVolume / 100.0d,
                _creepWarningVolume / 100.0d);
            return config;
        }

        private void Save(Config config)
        {
            try
            {
                var path = Path.Combine(_appDataDir, ConfigFileName);

                if (!Directory.Exists(_appDataDir))
                    Directory.CreateDirectory(_appDataDir);

                string json = JsonConvert.SerializeObject(config, Formatting.Indented);
                File.WriteAllText(path, json);
            }
            catch (Exception)
            {
                //Todo:
                //_agg.Publish(new ErrorMessage("Unable to save changes"));
            }
        }

        private void Load()
        {
            try
            {
                var path = Path.Combine(_appDataDir, ConfigFileName);
                if (!File.Exists(path)) return;

                string json = File.ReadAllText(path);
                var settings = JsonConvert.DeserializeObject<MutableConfig>(json);

                CreepSecondsWarning = settings.CreepSecondsWarning;
                CreepStopWarningAfterMinutes = settings.CreepStopWarningAfterMinutes;
                CreepToggle = settings.CreepToggle;

                RuneToggle = settings.RuneToggle;
                RuneSecondsWarning = settings.RuneSecondsWarning;
                RuneStopWarningAfterMinutes = settings.RuneStopWarningAfterMinutes;

                RuneWarningVolume = (int)(settings.RuneWarningVolume * 100);
                CreepWarningVolume = (int)(settings.CreepWarningVolume * 100);

                _gsiPort = settings.GameStateIntegrationPort;
            }
            catch (Exception)
            {
                //It's ok, just fall back to defaults if the file is garbage, will overwrite it anyway
            }
            finally
            {
                //Publish settings to relevant parties
                ConfigFirstLoaded();
                ConfigChanged(); //todo make obsolete
                _isLoaded = true;
            }
        }

        private void ConfigFirstLoaded()
        {
            var config = GetConfigObject();

            _agg.PublishOnBackgroundThread(new ConfigFirstLoaded(config));
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(FirstLoad message)
        {
            Load();
        }
    }
}
