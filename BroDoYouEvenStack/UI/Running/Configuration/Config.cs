namespace BroDoYouEvenStack.UI.Running
{
    class Config
    {
        public Config(bool runeToggle, bool creepToggle, int runeSecondsWarning, int creepSecondsWarning, int runeStopWarningAfterMinutes, int creepStopWarningAfterMinutes)
        {
            RuneToggle = runeToggle;
            CreepToggle = creepToggle;
            RuneSecondsWarning = runeSecondsWarning;
            CreepSecondsWarning = creepSecondsWarning;
            RuneStopWarningAfterMinutes = runeStopWarningAfterMinutes;
            CreepStopWarningAfterMinutes = creepStopWarningAfterMinutes;
        }

        public bool RuneToggle { get; }
        public bool CreepToggle { get; }
        public int RuneSecondsWarning { get; }
        public int CreepSecondsWarning { get; }
        public int RuneStopWarningAfterMinutes { get; }
        public int CreepStopWarningAfterMinutes { get; }
    }
}