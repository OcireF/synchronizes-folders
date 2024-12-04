
class Program
{
    /* args[0] = source folder path
     * args[1] = replica folder path
     * args[2] = synchronization interval in miliseconds
     * args[3] = log file path
     */
    static int Main(string[] args)
    {
        if (args.Length < 4)
        {
            System.Console.WriteLine("missing 1 or more arguments\n" +
                "<Source Folder Path> <Replica Folder Path> <Synchronization Interval> <Log File Path>");
            return 1;
        }
        string sourceFolderPath = args[0];
        string replicaFolderPath = args[1];
        int synchronizationInterval = int.Parse(args[2]);
        string logFolderPath = args[3];
        
        FolderActions forlderActions = new FolderActions(logFolderPath);

        //loop to update the files
        while (true)
        {
            forlderActions.UpdateAllFiles(sourceFolderPath, replicaFolderPath);
            forlderActions.flushLog(); // we need to do this to update the file log
            System.Threading.Thread.Sleep(synchronizationInterval);
        }
    }
}