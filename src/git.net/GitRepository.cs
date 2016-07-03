namespace dr.git.net
{
    public class GitRepository
    {
        public string RootPath {get;} 
        public GitRepository(string rootPath)
        {
            RootPath = rootPath;
        }
    }
}