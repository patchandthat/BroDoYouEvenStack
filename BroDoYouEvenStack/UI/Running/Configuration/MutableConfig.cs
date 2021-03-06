﻿namespace BroDoYouEvenStack.UI.Running.Configuration
{
    internal class MutableConfig
    {
        public bool RuneToggle { get; set; }
        public bool CreepToggle { get; set; }
        public int RuneSecondsWarning { get; set; }
        public int CreepSecondsWarning { get; set; }
        public int RuneStopWarningAfterMinutes { get; set; }
        public int CreepStopWarningAfterMinutes { get; set; }
        public int GameStateIntegrationPort { get; set; }
        public double RuneWarningVolume { get; set; }
        public double CreepWarningVolume { get; set; }

        public Config ToImmutable()
        {
            return new Config(
                RuneToggle,
                CreepToggle,
                RuneSecondsWarning,
                CreepSecondsWarning,
                RuneStopWarningAfterMinutes,
                CreepStopWarningAfterMinutes,
                GameStateIntegrationPort,
                RuneWarningVolume,
                CreepWarningVolume);
        }
    }
}