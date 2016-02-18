namespace Bootstrapper.Interface.Util
{
    public class DirectorySearchResult
    {
        public DirectorySearchResult(SearchOutcome outcome)
        {
            Outcome = outcome;
            Path = "";
        }

        public DirectorySearchResult(SearchOutcome outcome, string path)
        {
            Outcome = outcome;
            Path = path;
        }

        public SearchOutcome Outcome { get; }
        public string Path { get; }
    }
}