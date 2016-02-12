using System;

namespace BroDoYouEvenStack.Util
{
    public class WarningEvaluator
    {
        private readonly int _warningAt;

        private const int RUNE_SPAWN_TIME = 120;
        private const int NEUTRAL_SPAWN_TIME = 60;

        private readonly int spawnTime;

        public WarningEvaluator(TimeSpan warningBefore, WarningType type)
        {
            switch (type)
            {
                case WarningType.RuneSpawn:
                    spawnTime = RUNE_SPAWN_TIME;
                    break;
                case WarningType.NeutralCreepSpawn:
                    spawnTime = NEUTRAL_SPAWN_TIME;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            _warningAt = spawnTime - (int)warningBefore.TotalSeconds;
        }

        public bool IsTimeForWarning(int gametime)
        {
            int time = gametime % spawnTime;

            return time == _warningAt;
        }

        public int ProgressPercent(int clocktime)
        {
            int offset = spawnTime - _warningAt;
            int time = clocktime % spawnTime;

            double percent = (time + offset) / (double)spawnTime;

            var progress = Math.Round(100 * percent, 0, MidpointRounding.AwayFromZero);
            return (int)progress % 100;
        }
    }
}