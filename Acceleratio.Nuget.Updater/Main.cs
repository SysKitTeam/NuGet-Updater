using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Acceleratio.Nuget.Updater.Exceptions;
using Acceleratio.Nuget.Updater.Model;
using Microsoft.Build.Construction;

namespace Acceleratio.Nuget.Updater
{
    public partial class Main : Form
    {
        private readonly List<NuGetPackage> _packages;

        private string NuGetRepositoryUrl => repositoryTextBox.Text;
        private string LastValidRepositoryUrl;

        private List<NuGetPackage> PackagesToInstall
        {
            get
            {
                List<NuGetPackage> packages = new List<NuGetPackage>();

                foreach (string name in packagesComboBoxList.CheckedItems)
                {
                    var version = versionsDropDown.SelectedItem.ToString();
                    var package = _packages.FirstOrDefault(x => x.PackageName == name && x.PackageVersion == version);
                    if (package != null)
                    {
                        packages.Add(package);
                    }
                    else
                    {
                        WriteStatus($"Warning: Unable to find package {name} with version {version}.");
                    }
                }

                return packages;
            }
        }

        public Main()
        {
            _packages = new List<NuGetPackage>();
            InitializeComponent();
            versionLabel.Text = Constants.AppVersion;
            sourceControlComboBox.SelectedIndex = 1;
            SetControlsEnabled(false);
        }

        private async void Main_Shown(object sender, EventArgs e)
        {
            repositoryTextBox.Text = "Loading most recently used repository...";

            var url = await RegistryManager.GetRepositoryUrlFromRegistry();

            repositoryTextBox.Enabled = true;
            repositoryTextBox.Text = url;
            repositoryTextBox.BackColor = String.IsNullOrEmpty(url) ? Color.MistyRose : Color.White;
        }

        private async void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!String.IsNullOrEmpty(LastValidRepositoryUrl))
            {
                await RegistryManager.SetRepositoryUrlToRegistry(LastValidRepositoryUrl);
            }
            
            File.Delete(Constants.NuGetBinary);
        }

        private async void browseButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == DialogResult.OK)
            {
                SetControlsEnabled(false);
                WriteStatus("Loading solution files...");

                rootPathTextBox.Text = fbd.SelectedPath;
                List<string> solutionFiles = null;

                await Task.Run(() =>
                {
                    solutionFiles = Directory.GetFiles(fbd.SelectedPath, "*.sln", SearchOption.AllDirectories).ToList();
                });

                solutionsList.Items.Clear();
                foreach (var solution in solutionFiles)
                {
                    solutionsList.Items.Add(solution);
                }

                WriteStatus("done.", true);

                await LoadControls();

                SetControlsEnabled(true);
            }
        }

        private async void latestCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            await LoadVersionsDropdown();
            SetUpdateButtonEnabled(true);
        }

        private async void packagesComboBoxList_DropDownClosed(object sender, EventArgs e)
        {
            await LoadVersionsDropdown();
            SetUpdateButtonEnabled(true);
        }

        private void solutionsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetUpdateButtonEnabled(true);
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            await LoadControls();
        }

        private async Task LoadControls()
        {
            SetControlsEnabled(false);

            try
            {
                await EnsureLoadPackagesDropdown();
                await LoadVersionsDropdown();

                repositoryTextBox.Enabled = connectButton.Enabled = false;
            }
            catch (RepositoryNotFoundException)
            {
                repositoryTextBox.BackColor = Color.MistyRose;
                WriteStatus("repository not found!", true);
                ShowError("The specified NuGet repository was not found.");

                repositoryTextBox.Enabled = connectButton.Enabled = true;
            }
            catch (RepositoryContainsNoPackagesException)
            {
                packagesComboBoxList.BackColor = Color.MistyRose;
                WriteStatus("respository contains no packages!", true);
                ShowError("The specified NuGet repository contains no packages.");

                repositoryTextBox.Enabled = connectButton.Enabled = true;
            }
            catch (NuGetBinaryDownloadErrorException)
            {
                WriteStatus("download error.", true);
                ShowError($"Error downloading {Constants.NuGetBinary}, cannot continue.");
                repositoryTextBox.Enabled = connectButton.Enabled = true;
            }

            SetControlsEnabled(true);
        }

        private List<string> GetCommonVersionsForSelectedPackages()
        {
            var versions = _packages.Select(x => x.PackageVersion).Distinct().ToList();
            foreach (string name in packagesComboBoxList.CheckedItems)
            {
                var packageVersions = _packages.Where(x => x.PackageName == name).Select(x => x.PackageVersion).ToList();
                versions = versions.Intersect(packageVersions).ToList();
            }

            versions = versions.Distinct().OrderBy(x => new Version(Regex.Replace(x, "[^0-9.]", ""))).ToList();
            return versions;
        }

        private async Task LoadVersionsDropdown()
        {
            await EnsurePackagesLoaded();

            var versions = GetCommonVersionsForSelectedPackages();

            versionsDropDown.Items.Clear();
            foreach (var version in versions)
            {
                versionsDropDown.Items.Add(version);
            }

            if (!versions.Any())
            {
                WriteStatus("Warning: unable to find common versions for the selected packages.");
            }

            // select default
            versionsDropDown.SelectedItem = versions.Any() ? versions.Last() : null;
            versionsDropDown.Enabled = versions.Any() && !latestCheckBox.Checked;
        }

        private async Task EnsureLoadPackagesDropdown()
        {
            await EnsurePackagesLoaded();

            if (packagesComboBoxList.Items.Count == 0)
            {
                var distinctPackages = _packages.Select(x => x.PackageName).Distinct().OrderBy(x => x).ToList();

                foreach (var package in distinctPackages)
                {
                    packagesComboBoxList.Items.Add(package);
                }

                // select default
                for (var i = 0; i < packagesComboBoxList.Items.Count; i++)
                {
                    if (packagesComboBoxList.Items[i].ToString().Trim() == "Acceleratio.Common")
                    {
                        packagesComboBoxList.SetItemChecked(i, true);
                    }
                }
            }
        }

        private async Task EnsurePackagesLoaded()
        {
            if (!_packages.Any())
            {
                await EnsureNuGetBinary();

                WriteStatus("Downloading package metadata from the server...");

                repositoryTextBox.BackColor = Color.White;
                packagesComboBoxList.BackColor = Color.White;
                _packages.Clear();

                string result = await Task.Run(() => ExecuteBinary(Constants.NuGetBinary, $"list -source \"{NuGetRepositoryUrl}\" -allversions -prerelease"));

                if (String.IsNullOrEmpty(result))
                {
                    throw new RepositoryNotFoundException();
                }
                else
                {
                    LastValidRepositoryUrl = NuGetRepositoryUrl;

                    var packageStrings = result.Split('\n').Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x)).ToList();

                    if (packageStrings.Any())
                    {
                        foreach (var packageString in packageStrings)
                        {
                            _packages.Add(new NuGetPackage(packageString));
                        }

                        WriteStatus("done.", true);
                    }
                    else
                    {
                        throw new RepositoryContainsNoPackagesException();
                    }
                }
            }
        }

        private string ExecuteBinary(string binary, string arguments, bool outputToStatusWindow = false, string workingDirectory=null)
        {
            string output = null;

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = binary;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            if (!string.IsNullOrEmpty(workingDirectory))
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }
            if (outputToStatusWindow)
            {
                process.OutputDataReceived += (sender, args) =>
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        statusTextBox.Focus();
                        statusTextBox.AppendText(args.Data + Environment.NewLine);
                    }));
                };

                process.ErrorDataReceived += (sender, args) =>
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        statusTextBox.Focus();
                        statusTextBox.AppendText(args.Data + Environment.NewLine);
                    }));
                };
            }

            process.Start();

            if (outputToStatusWindow)
            {
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
            }
            else
            {
                output = process.StandardOutput.ReadToEnd();
            }

            process.WaitForExit();
            return output;
        }

        private async Task EnsureNuGetBinary()
        {
            if (!File.Exists(Constants.NuGetBinary))
            {
                WriteStatus("Downloading latest nuget.exe...");

                await Task.Run(() =>
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(Constants.NuGetWebBinary, Constants.NuGetBinary);
                        }
                    }
                    catch
                    {
                        throw new NuGetBinaryDownloadErrorException();
                    }
                });

                WriteStatus("done.", true);
            }

            WriteStatus("Using nuget.exe version " + FileVersionInfo.GetVersionInfo(Path.GetFullPath(Constants.NuGetBinary)).ProductVersion);
        }

        private string GetPackageDirectoryPath(string solution, NuGetPackage package)
        {
            return Path.GetDirectoryName(solution).TrimEnd('\\') + "\\packages\\" + package.PackageName + "." + package.PackageVersion;
        }

        private List<string> GetOldPackageDirectoriesPaths(string solution, NuGetPackage package)
        {
            return Directory.GetDirectories(Path.GetDirectoryName(solution).TrimEnd('\\') + "\\packages\\", package.PackageName + ".*").ToList();
        }

        public async Task DoUpdate()
        {
            bool isError = false;
            await EnsurePackagesLoaded();

            foreach (var package in PackagesToInstall)
            {
                WriteStatus("Starting package update..." + Environment.NewLine + Environment.NewLine);

                foreach (string solution in solutionsList.SelectedItems)
                {
                    FixProjectFiles(solution);
                    var workingDirectory = Path.GetDirectoryName(solution);
                    if (Directory.Exists(Path.Combine(workingDirectory, ".nuget")))
                    {
                        workingDirectory = Path.Combine(workingDirectory, ".nuget");
                    }
                    await Task.Run(() => ExecuteBinary(Constants.NuGetBinary, $"update \"{solution}\" -id \"{package.PackageName}\" -version \"{package.PackageVersion}\" -source \"{NuGetRepositoryUrl}\" -noninteractive -prerelease -verbose -verbosity detailed -fileconflictaction overwrite", true, workingDirectory));
                }

                WriteStatus("Update finished.");

                if (sourceControlComboBox.SelectedIndex == 1 && (deleteOldCheckBox.Checked || uploadCheckBox.Checked))
                {
                    WriteStatus("Starting TFS source control operations..." + Environment.NewLine + Environment.NewLine);

                    var tfsBinary = TFSManager.GetTFSBinaryPath();
                    if (tfsBinary != null)
                    {
                        foreach (string solution in solutionsList.SelectedItems)
                        {
                            var packageDirectory = GetPackageDirectoryPath(solution, package);

                            // remove old
                            if (deleteOldCheckBox.Checked)
                            {
                                var oldDirectories = GetOldPackageDirectoriesPaths(solution, package).Where(x => x.Trim() != packageDirectory.Trim()).ToList();
                                foreach (var oldDirectory in oldDirectories)
                                {
                                    WriteStatus($"Removing old package directory {oldDirectory}" + Environment.NewLine + Environment.NewLine);
                                    await Task.Run(() => ExecuteBinary(tfsBinary, $"del {oldDirectory} /recursive", true));
                                }
                            }

                            // add
                            if (uploadCheckBox.Checked)
                            {
                                if (Directory.Exists(packageDirectory))
                                {
                                    WriteStatus($"Adding {packageDirectory}");
                                    await Task.Run(() => ExecuteBinary(tfsBinary, $"add {packageDirectory} /recursive /noignore /noprompt", true));
                                }
                                else
                                {
                                    isError = true;
                                    WriteStatus("Error: Unable to include NuGet packages files for the solution: " +
                                                Environment.NewLine + Environment.NewLine + solution + Environment.NewLine + Environment.NewLine +
                                                "because the folder of the specified NuGet package could not be found: " +
                                                Environment.NewLine + Environment.NewLine + packageDirectory + Environment.NewLine + Environment.NewLine +
                                                "This could mean that the specified NuGet package may not have been installed to begin with and therefore was not updated. " +
                                                "Please revert all changes and manually check the solution in question.");
                                }
                            }
                        }
                    }
                    else
                    {
                        isError = true;
                        WriteStatus("Error: Unable to load TFS binary. Is Visual Studio installed?");
                    }
                }
            }

            if (isError)
            {
                WriteStatus("There were errors in the update process, please inspect the output." + Environment.NewLine);
            }
            else
            {
                WriteStatus("All done, with no apparent catastrophic errors. Inspect the changes in Visual Studio TFS window, and if all looks good, check them in to the source control." + Environment.NewLine);
            }

            var filename = await LogWriter.WriteAllToLog(statusTextBox.Text, solutionsList.SelectedItems.Cast<string>().ToList(), PackagesToInstall);
            WriteStatus($"All output has been written to log: {filename}");
        }

        public void FixProjectFiles(string solutionFilePath) // https://github.com/NuGet/Home/issues/2234
        {
            var solutionFile = SolutionFile.Parse(solutionFilePath);
            var projects = solutionFile.ProjectsInOrder.Where(x => x.ProjectType == SolutionProjectType.KnownToBeMSBuildFormat).ToList();

            foreach (var project in projects)
            {
                WriteStatus($"Updating project {project.ProjectName}...");

                var regex = Regex.Match(File.ReadAllText(project.AbsolutePath),
                                        "<Target Name=\"EnsureNuGetPackageBuildImports\" BeforeTargets=\"PrepareForBuild\">[\\s\\S]*</Target>",
                                        RegexOptions.IgnoreCase);

                if (!String.IsNullOrEmpty(regex.Value))
                {
                    var inners = Regex.Matches(regex.Value, "<Error Condition[\\s\\S]*?/>", RegexOptions.IgnoreCase);
                    foreach (Match inner in inners)
                    {
                        if (!String.IsNullOrEmpty(inner.Value))
                        {
                            File.WriteAllText(project.AbsolutePath, File.ReadAllText(project.AbsolutePath).Replace(inner.Value, ""), Encoding.UTF8);
                        }
                    }
                }

                WriteStatus("done.", true);
            }

            WriteStatus("");
        }

        private async void updateButton_Click(object sender, EventArgs e)
        {
            SetControlsEnabled(false);

            await DoUpdate();

            SetControlsEnabled(true);
        }

        private void SetControlsEnabled(bool enabled, bool setRepositoryTextBoxEnabled = true)
        {
            rootPathTextBox.Enabled = enabled;
            browseButton.Enabled = enabled;

            if (setRepositoryTextBoxEnabled)
            {
                repositoryTextBox.Enabled = repositoryTextBox.Enabled && enabled;
            }

            connectButton.Enabled = repositoryTextBox.Enabled && enabled;
            packagesComboBoxList.Enabled = enabled && _packages.Any();
            versionsDropDown.Enabled = enabled && versionsDropDown.SelectedItem != null && !latestCheckBox.Checked;
            latestCheckBox.Enabled = enabled && _packages.Any();
            solutionsList.Enabled = enabled;
            uploadCheckBox.Enabled = enabled && sourceControlComboBox.SelectedIndex == 1;
            deleteOldCheckBox.Enabled = enabled && sourceControlComboBox.SelectedIndex == 1;
            sourceControlComboBox.Enabled = enabled;

            SetUpdateButtonEnabled(enabled);
        }

        private void SetUpdateButtonEnabled(bool enabled)
        {
            updateButton.Enabled = enabled && solutionsList.SelectedItems.Count > 0 && versionsDropDown.SelectedItem != null && PackagesToInstall.Any();
        }

        private void ShowError(string text)
        {
            MessageBox.Show(text, @"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void WriteStatus(string status, bool append = false)
        {
            statusTextBox.Focus();
            statusTextBox.AppendText((append ? " " : String.IsNullOrEmpty(statusTextBox.Text) ? "" : Environment.NewLine) + (append ? "" : DateTime.Now.ToString("HH:mm:ss") + ": ") + status);
        }

        private void repositoryTextBox_TextChanged(object sender, EventArgs e)
        {
            Uri uriResult;
            bool isValidUrl = Uri.TryCreate(repositoryTextBox.Text, UriKind.Absolute, out uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            SetControlsEnabled(isValidUrl, false);
            repositoryTextBox.BackColor = isValidUrl ? Color.White : Color.MistyRose;
        }

        private void sourceControlComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            uploadCheckBox.Visible = sourceControlComboBox.SelectedIndex == 1;
            deleteOldCheckBox.Visible = sourceControlComboBox.SelectedIndex == 1;
            label7.Visible = sourceControlComboBox.SelectedIndex == 1;
        }
    }
}
