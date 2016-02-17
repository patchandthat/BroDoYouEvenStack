using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bootstrapper.Interface.Util
{
    /*
            Try to identify the dota2 directory.
            Obvious choice is %programfiles%\Steam\Steamapps\common
            Also check the root of each drive for a "SteamLibrary" dir
            Else fallback to letting the user pick via dialog in bootstrapper ui
            Ultimately looking for the "dota 2 beta\game\dota\cfg" directory.
            Once found, should write this to registry to make uninstallation & updates easier

            We could also automatically search the whole machine for the DOTA folder, 
            display a progress indicator, and have a button for the user to manually specify the directory.
            I much prefer it to be automated by default   
            
            Check the registry first to see if we're upgrading from an earlier version of BDYES         
    */

    class DotaDirectoryLocator
    {
        public DirectoryLocationResult SearchForDotaDirectory()
        {
            //Todo:
            //Async, support cancellation, indicate busy status, current directory, overall progress

            throw new NotImplementedException();
        }

        public void SpecifyDirectoryManually(string path)
        {
            //Cancels the async operation, and uses the path provided as the Dota2 directory.
            //Do some validation here, and possibly adjust the path so that we know we're in the right place
        }
    }

    public class DirectoryLocationResult
    {
        public enum SearchOutcome
        {
            Undefined,
            Found,
            NotFound
        }

        public DirectoryLocationResult(SearchOutcome outcome)
        {
            Outcome = outcome;
            Path = "";
        }

        public DirectoryLocationResult(SearchOutcome outcome, string path)
        {
            Outcome = outcome;
            Path = path;
        }

        public SearchOutcome Outcome { get; }
        public string Path { get; }
    }
}
