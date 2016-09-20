using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Acceleratio.Nuget.Updater
{
    public class RegistryManager
    {
        private const string key = "Acceleratio.Nuget.Updater\\Acceleratio\\1.1";
        public static string GetRepositoryUrlFromRegistry()
        {
            using (RegistryKey nugetUrlRegKey = Registry.CurrentUser.CreateSubKey(key))
            {
                if (nugetUrlRegKey != null)
                {
                    string lastEnteredRepository = nugetUrlRegKey.GetValue(key, "").ToString();
                    if (!String.IsNullOrEmpty(lastEnteredRepository))
                    {
                        return lastEnteredRepository;
                    }
                }
            }

            return "";
        }

        public static void SetRepositoryUrlToRegistry(string url)
        {
            using (RegistryKey nugetUrlRegKey = Registry.CurrentUser.CreateSubKey(key))
            {
                if (nugetUrlRegKey != null)
                {
                    nugetUrlRegKey.SetValue(key, url);
                }
            }
        }
    }
}
