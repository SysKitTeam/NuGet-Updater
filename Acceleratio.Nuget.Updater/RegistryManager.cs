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
        private static string key = "Acceleratio.Nuget.Updater\\Acceleratio\\" + Constants.AppVersion;
        public static async Task<string> GetRepositoryUrlFromRegistry()
        {
            string result = "";

            await Task.Run(() =>
            {
                using (RegistryKey nugetUrlRegKey = Registry.CurrentUser.CreateSubKey(key))
                {
                    if (nugetUrlRegKey != null)
                    {
                        string lastEnteredRepository = nugetUrlRegKey.GetValue(key, "").ToString();
                        if (!String.IsNullOrEmpty(lastEnteredRepository))
                        {
                            result = lastEnteredRepository;
                        }
                    }
                }
            });

            return result;
        }

        public static async Task SetRepositoryUrlToRegistry(string url)
        {
            await Task.Run(() =>
            {
                using (RegistryKey nugetUrlRegKey = Registry.CurrentUser.CreateSubKey(key))
                {
                    if (nugetUrlRegKey != null)
                    {
                        nugetUrlRegKey.SetValue(key, url);
                    }
                }
            });
        }
    }
}
