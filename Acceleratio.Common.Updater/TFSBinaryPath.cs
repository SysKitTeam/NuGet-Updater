using System.IO;

namespace Acceleratio.Common.Updater
{
    public sealed class TFSBinaryPath
    {
        private TFSBinaryPath() { }

        private const string vs2015path = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\Common7\IDE\TF.exe";
        private const string vs2013path = @"C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\TF.exe";
        private const string vs2012path = @"C:\Program Files (x86)\Microsoft Visual Studio 11.0\Common7\IDE\TF.exe";

        public static string GetTFSBinaryPath()
        {
            if (File.Exists(vs2015path))
            {
                return vs2015path;
            }
            else if (File.Exists(vs2013path))
            {
                return vs2013path;
            }
            else if (File.Exists(vs2012path))
            {
                return vs2012path;
            }

            return null;
        }
    }
}
