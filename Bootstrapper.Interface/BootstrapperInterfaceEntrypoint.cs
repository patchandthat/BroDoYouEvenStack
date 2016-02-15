using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Tools.WindowsInstallerXml.Bootstrapper;

namespace Bootstrapper.Interface
{
    class BootstrapperInterfaceEntrypoint : BootstrapperApplication
    {
        private CaliburnMicroBootstrapper _bootstrapper;

        protected override void Run()
        {
#if DEBUG
            MessageBox.Show("Attach debugger and press OK.");
#endif

            Application app = new Application();
            _bootstrapper = new CaliburnMicroBootstrapper(this);
            app.Run();

            this.Engine.Quit(0);
        }
    }
}
