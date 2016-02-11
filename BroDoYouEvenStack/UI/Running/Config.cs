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

    class MutableConfig
    {
        public bool RuneToggle { get; set; }
        public bool CreepToggle { get; set; }
        public int RuneSecondsWarning { get; set; }
        public int CreepSecondsWarning { get; set; }
        public int RuneStopWarningAfterMinutes { get; set; }
        public int CreepStopWarningAfterMinutes { get; set; }

        public Config ToImmutable()
        {
            return new Config(
                RuneToggle,
                CreepToggle,
                RuneSecondsWarning,
                CreepSecondsWarning,
                RuneStopWarningAfterMinutes,
                CreepStopWarningAfterMinutes);
        }
    }
}