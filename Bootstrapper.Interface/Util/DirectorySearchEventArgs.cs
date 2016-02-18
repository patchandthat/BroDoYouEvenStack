namespace Bootstrapper.Interface.Util
{
    internal class DirectorySearchEventArgs
    {
        public DirectorySearchEventArgs(string location)
        {
            Location = location;
        }

        public string Location { get; }
    }
}