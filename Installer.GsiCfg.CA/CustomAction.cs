using Microsoft.Deployment.WindowsInstaller;

namespace Installer.GsiCfg.CA
{
    public class CustomActions
    {
        [CustomAction]
        public static ActionResult InstallCfg(Session session)
        {
            session.Log("Begin CustomAction1");

            string dotaDir = session["DOTADIR"];

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult RemoveCfg(Session session)
        {
            session.Log("Begin CustomAction1");

            return ActionResult.Success;
        }
    }
}
