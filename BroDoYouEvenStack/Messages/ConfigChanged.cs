using BroDoYouEvenStack.UI.Running;
using BroDoYouEvenStack.UI.Running.Configuration;

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
