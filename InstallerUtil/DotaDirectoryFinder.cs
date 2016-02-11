namespace InstallerUtil
{
    public class DotaDirectoryFinder
    {
        /*
            Try to identify the dota2 directory.

            Obvious choice is %programfiles%\Steam\Steamapps\common
            Also check the root of each drive for a "SteamLibrary" dir
            Else fallback to letting the user pick via dialog in bootstrapper ui

            Ultimately looking for the "dota 2 beta\game\dota\cfg" directory.

            Once found, should write this to registry to make uninstallation & updates easier
        */
    }
}
