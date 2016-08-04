# NuGet-Updater
NuGet update tool designed to update NuGet packages outside of Visual Studio.

Due to some issues regarding the use of NuGet manager in Visual Studio 2013,
having a tool that would deal with them would be convenient, hence the
NuGet-Updater.

Given a solution file and a NuGet repository containing NuGet packages, this 
tool updates every project in this solution using a modified binary NuGet.exe. 
This binary can be found in this repository. 

Furthermore, there are two advanced options:
    1) uploading files to packages folder on source control
       (NuGet restore can sometimes function improperly)
    2) deleting older files from packages folder on source control