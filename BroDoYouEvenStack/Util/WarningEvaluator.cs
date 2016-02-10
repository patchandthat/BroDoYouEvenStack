using System;

namespace BroDoYouEvenStack.Util
{
    public class WarningEvaluator
    {
        private readonly int _warningAt;

        private const int RUNE_SPAWN_TIME = 120;
        private const int NEUTRAL_SPAWN_TIME = 120;

        private readonly int spawnTime;

        public WarningEvaluator(TimeSpan warningAt, WarningType type)
        {
            _warningAt = (int) warningAt.TotalSeconds;

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
        }

        public bool IsTimeForWarning(int gametime)
        {
            int time = gametime % spawnTime;

            return time == _warningAt;
        }
    }
}