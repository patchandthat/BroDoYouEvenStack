using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BroDoYouEvenStack.UI.Running;
using BroDoYouEvenStack.UI.Running.Configuration;

namespace BroDoYouEvenStack.Messages
{
    class ConfigFirstLoaded
    {
        public ConfigFirstLoaded(Config config)
        {
            Config = config;
        }

        public Config Config { get; }
    }
}
