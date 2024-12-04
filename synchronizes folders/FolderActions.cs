using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class FolderActions
{
    private FileStream logFile;

    public FolderActions(string logFile = "defaltLog.txt") //default log file
    {
        //this is in "Append mode" to when we reboot the program we not lose the previous logs.
        this.logFile = File.Open(logFile, FileMode.Append);
    }

    /* Copy or Update a file in replica
     */
    public void Copy_or_Update_File(string sourcePath ,string replicaPath, string fileName, bool update)
    {
        int bufferSise = 1024;
        byte[] buffer = new byte[bufferSise];
        int numberOfbytesRead = 1; //this is 1 so the cycle "while" dont end before it starts
        try
        {
            FileStream sourceFile = File.Open(Path.Combine(sourcePath, fileName), FileMode.Open);
            FileStream outputFile = File.Open(Path.Combine(replicaPath, fileName), FileMode.Create);


            while (numberOfbytesRead > 0)
            {
                numberOfbytesRead = sourceFile.Read(buffer, 0, bufferSise);
                outputFile.Write(buffer, 0, numberOfbytesRead);
            }

            //closes the Streams
            sourceFile.Close();
            outputFile.Close();
        }
        catch (Exception e)
        {
            writeLog("Exception: " + e.Message);
        }
        finally
        {
            string action;
            if (update)
                action = "Update File";
            else
                action = "Copy File";

            writeLog(action + ";" + Path.Combine(replicaPath, fileName) + ";" + DateTime.Now + "\n");
        }
    }

    public void DeleteFile(string filePath, string fileName)
    {
        try
        {
            File.Delete(Path.Combine(filePath, fileName));
        }
        catch (Exception e)
        {
            writeLog("Exception: " + e.Message);
        }
        finally
        {
            writeLog("Delete File" + ";" + Path.Combine(filePath, fileName) + ";" + DateTime.Now + "\n");
        }
    }

    public void UpdateAllFiles(string sourcefolder,string replicaFolder)
    {
        //source files and folders
        string[] sourceFolderfiles = Directory.GetFiles(sourcefolder);
        string[] sourceSubFolders = Directory.GetDirectories(sourcefolder);

        //replica files and subfolders that will be deleted
        HashSet<string> delete_From_replicafiles = new HashSet<string>( Directory.GetFiles(replicaFolder));
        HashSet<string> delete_From_replicaSubFolders = new HashSet<string>( Directory.GetDirectories(replicaFolder));

        //search and update all files in the folder.
        foreach (string sourcefilepath in sourceFolderfiles)
        {
            string sourcefilename = Path.GetFileName(sourcefilepath);

            //see if the file already exits in replica folder
            bool fileExists_inReplica = delete_From_replicafiles.Contains(Path.Combine(replicaFolder, sourcefilename));

            if (fileExists_inReplica)
            {
                //if it exits then we remove from the delete Set
                delete_From_replicafiles.Remove(Path.Combine(replicaFolder, sourcefilename));
                //if the file was not modified skip this foreach loop
                //the copy should be newer than the original
                if (File.GetLastWriteTime(sourcefilepath) < File.GetLastWriteTime(Path.Combine(replicaFolder, sourcefilename)))
                    continue;
            }

            Copy_or_Update_File(sourcefolder, replicaFolder, sourcefilename, fileExists_inReplica);
        }

        //Delete all the Files that no longer exists in "sourse"
        foreach (string replicafilepath in delete_From_replicafiles)
        {
            DeleteFile(Path.GetDirectoryName(replicafilepath), Path.GetFileName(replicafilepath) );
        }

        //search and update all the folders.
        foreach (string SubFolder in sourceSubFolders)
        {
            string subReplicaFolder = Path.Combine(replicaFolder, Path.GetFileName(SubFolder));
            bool subFolder_AlreadyExist = delete_From_replicaSubFolders.Contains(subReplicaFolder);

            if(!subFolder_AlreadyExist) //create the folder if it does not exist
                Directory.CreateDirectory(subReplicaFolder);
            else
                delete_From_replicaSubFolders.Remove(subReplicaFolder); //remove from the list of subfolder that will be deleted

            UpdateAllFiles(SubFolder, subReplicaFolder);
        }

        //Delete all the Folders that no longer exists in "sourse"
        foreach (string replicafolderpath in delete_From_replicaSubFolders)
        {
            try
            {
                Directory.Delete(replicafolderpath, true);
            }
            catch (Exception e)
            {
                writeLog("Exception: " + e.Message);
            }
            finally
            {
                writeLog("Delete Folder and its files" + ";" + replicafolderpath + ";" + DateTime.Now + "\n");
            }
        }

    }
    /* Write the logs and write in the console to see what is happening
     * */
    public void writeLog(string log)
    {
        Console.Write(log);
        byte[] bytes = Encoding.ASCII.GetBytes(log);
        logFile.Write(bytes, 0, bytes.Length);
    }

    public void flushLog()
    {
        logFile.Flush();
    }
}

