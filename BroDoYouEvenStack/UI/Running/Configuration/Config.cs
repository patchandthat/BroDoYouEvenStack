namespace BroDoYouEvenStack.UI.Running.Configuration
{
    class Config
    {
        public Config(bool runeToggle, bool creepToggle, int runeSecondsWarning, int creepSecondsWarning, int runeStopWarningAfterMinutes, int creepStopWarningAfterMinutes, int gameStateIntegrationPort, double runeWarningVolume, double creepWarningVolume)
        {
            RuneToggle = runeToggle;
            CreepToggle = creepToggle;
            RuneSecondsWarning = runeSecondsWarning;
            CreepSecondsWarning = creepSecondsWarning;
            RuneStopWarningAfterMinutes = runeStopWarningAfterMinutes;
            CreepStopWarningAfterMinutes = creepStopWarningAfterMinutes;
            GameStateIntegrationPort = gameStateIntegrationPort;
            RuneWarningVolume = runeWarningVolume;
            CreepWarningVolume = creepWarningVolume;
        }

        public int GameStateIntegrationPort { get; }
        public bool RuneToggle { get; }
        public bool CreepToggle { get; }
        public int RuneSecondsWarning { get; }
        public int CreepSecondsWarning { get; }
        public int RuneStopWarningAfterMinutes { get; }
        public int CreepStopWarningAfterMinutes { get; }
        public double RuneWarningVolume { get; }
        public double CreepWarningVolume { get; }
    }
}