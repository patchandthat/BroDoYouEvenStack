namespace Bootstrapper.Interface.Messages
{
    public class DirectorySearchMessages
    {
        public class SearchForDotaDirectory { }

        public class DotaDirectoryNotFound { }

        public class DotaDirectoryFound
        {
            public DotaDirectoryFound(string path)
            {
                Path = path;
            }

            public string Path { get; }
        }
    }
}
