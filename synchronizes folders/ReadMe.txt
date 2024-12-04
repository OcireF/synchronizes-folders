to run the program open a cmd in the folder "executable" and run the code replacing what is between <> with the respective values
"synchronizes folders.exe" <source folder path> <replica folder path> <synchronization interval in miliseconds> <log file path>

Exemple:
"synchronizes folders.exe" ..\source ..\replica 3000 log.txt
every 3 seconds it will copy/modified/delete files from "replica" in order to have the same files and folders of "source"





Possible problems:
-when you move/change name of a file or folder in "source", it will delete the original copy and create a new one.

-the block of bytes when we do a copy/update files is 1024 bytes, it should be adjusted to better optimize the speed of "copy/update"

-technically the time between each "Synchronization" is the time of the from previous updates + <synchronization interval in miliseconds>