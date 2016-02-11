using BroDoYouEvenStack.UI.Running;

namespace BroDoYouEvenStack.Messages
{
    class ConfigChanged
    {
        public ConfigChanged(Config newConfig)
        {
            NewConfig = newConfig;
        }

        public Config NewConfig { get; }
    }
}
