# NuGet-Updater
NuGet update is tool developed by [Acceleratio Ltd.](https://acceleratio.net/) and is designed to update NuGet packages outside of Visual Studio. 

This project is licensed under the terms of the MIT license.

# Download

Download the latest binary here: [Acceleratio.Nuget.Updater.zip](https://github.com/Acceleratio/NuGet-Updater/raw/master/dist/Acceleratio.Nuget.Updater.zip)

# Motivation
There is a common project in our company called "Acceleratio.Common" made for
code-sharing purposes. Using NuGet package manager this project has been 
referenced throughout other projects.

However, NuGet update wasn't performing properly: faulty project referencing
and a faulty automatic restore (restore caused by canceling some changes) on 
other computers.

Due to these issues, having a tool that would deal with them would be convenient, 
hence the NuGet-Updater.

# How this tool works
Given a solution file and a NuGet repository containing NuGet packages, this 
tool updates every project in this solution using a modified binary NuGet.exe. 

The reason why a modified binary is used is that there were cases when updating
NuGet package to specific Acceleratio.Common NuGet package versions was a necessity.
These modifications were implemented in NuGet source code. The new compiled 
NuGet.exe is the aforementioned modified binary.

This binary can be found in this repository. 

Furthermore, there are two advanced options:

1. uploading files to packages folder on source control (NuGet restore can sometimes function improperly)
2. deleting older files from packages folder on source control

# Tool - interface and usage description
Here's what it looks like:

![alt tag](https://www.dropbox.com/s/4j0qgjn204f69yd/updater.png?raw=1)

#####Interface elements:

1. NuGet repository - repository containing NuGet packages.
2. Browse for solution directory - root folder selection, all the .sln 
files from it's subfolders are loaded into Solutions list (in this interface)
3. Root directory - chosen root directory
4. Package(s) - after the root folder has been selected or the "Load" button has been clicked (next to NuGet Repository), a packages checkbox list will be loaded here. All the packages that are supposed to be updated should be checked/selected (in this checkbox list).
5. Package version - version of the updating package. Should there be more than one package selected (for update), common version of all the selected packages is chosen as the update version.
6. Latest: checked - updating all the packages to the latest version, unchecked - manually choose the version
7. Advanced options: Upload files to packages folder on source control(useful/necessary option - check Motivation paragraph - restore issue)
8. Advanced options: Delete older files from packages folder on source control (this could take a while) - useful option that prevents data piling up

# Simple use-case

Updating Acceleratio.Common to the latest version:

1. Choose Browse for solution directory and select a root folder. C:\Projects for instance.
2. Check all the solution files in solutions list where you would like to update Acceleratio.Common. Looking at the screenshot, you can see that chosen solutions are Common.UserControls.sln and TSL.sln.
3. Click Update selected solutions button and wait for the update to finish.
4. In case everything went fine, just commit the changes using Team Explorer in Visual Studio.
