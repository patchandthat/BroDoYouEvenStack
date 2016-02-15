using Caliburn.Micro;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace Bootstrapper.Interface.UI
{
    class ShellViewModel : Screen
    {
        private readonly BootstrapperApplication _ba;

        public ShellViewModel(BootstrapperApplication ba)
        {
            _ba = ba;
        }

        public void PassconfigPathToMsi()
        {
            _ba.Engine.StringVariables["DotaConfigDir"] = "THE PATH";

            //Before
            //_ba.Engine.Plan(LaunchAction.Install);
        }
    }
}
