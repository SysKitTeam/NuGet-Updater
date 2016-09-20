using Acceleratio.Nuget.Updater.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Acceleratio.Nuget.Updater
{
    public sealed class LogWriter
    {
        public static async Task<string> WriteAllToLog(string statusText, List<string> solutionsList, List<NuGetPackage> PackagesToInstall)
        {
            var now = DateTime.Now;
            var filename = now.ToString("s").Replace(":", "-") + ".txt";
            List<string> solutions = solutionsList;
            List<string> packages = PackagesToInstall.Select(x => x.FullPackageName).ToList();

            await Task.Run(() =>
            {
                string text = "";
                text += $"Acceleratio.Nuget.Updater Update Log, {now.ToString("g")}" + Environment.NewLine;

                text += Environment.NewLine;
                text += "------------------------------------------------------------------------------------------------";
                text += Environment.NewLine + Environment.NewLine;

                text += "Solutions:" + Environment.NewLine;
                foreach (string solution in solutions)
                {
                    text += solution + Environment.NewLine;
                }
                text += Environment.NewLine;

                text += "Packages:" + Environment.NewLine;
                foreach (string package in packages)
                {
                    text += package + Environment.NewLine;
                }

                text += Environment.NewLine;
                text += "------------------------------------------------------------------------------------------------";
                text += Environment.NewLine + Environment.NewLine;

                text += statusText;

                File.WriteAllText(filename, text);
            });

            return filename;
        }

    }
}
