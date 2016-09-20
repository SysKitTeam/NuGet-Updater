using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Acceleratio.Nuget.Updater.Model
{
    public class NuGetPackage
    {
        public readonly string FullPackageName;
        public readonly string PackageName;
        public readonly string PackageVersion;

        public NuGetPackage(string fullPackageName)
        {
            FullPackageName = fullPackageName.Replace(" ", ".");
            PackageName = fullPackageName.Split(' ').ElementAt(0).Trim();
            PackageVersion = fullPackageName.Split(' ').ElementAt(1).Trim();
        }
    }
}
