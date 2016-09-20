namespace Acceleratio.Common.Updater
{
    public sealed class AppStrings
    {       
        public const string NuGetWebBinary = "https://github.com/Acceleratio/NuGet-Updater/raw/master/nuget.exe";
        public const string NuGetBinary = "nuget.exe";
        public const string KEY_NAME = "Acceleratio.Common.Updater\\Acceleratio\\1.1";
        public const string DefaultRepository = "http://nuget.acceleratio.hr/nuget/";   
        
        //some longer status messages used in Form1.cs            
        public const string CannotLoadTfsBinary = "Error: Unable to load TFS binary. Is Visual Studio installed?";        
        public const string CommonVersionsNotFound = "Warning: unable to find common versions for the selected packages.";        
        public const string UpdateProcessErrors = "There were errors in the update process, please inspect the output.";        
        public const string SuccessfulUpdate = "All done, with no apparent catastrophic errors. Inspect the changes in Visual Studio TFS window and, if all looks good, check-in them to the source control.";
        public const string CannotLoadLastRepoUrl = "Unable to retrieve your last entered NuGet repository URL";




    }
}
