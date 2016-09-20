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
using Acceleratio.Common.Updater.Exceptions;
using Acceleratio.Common.Updater.Model;
using Microsoft.Build.Construction;
using Microsoft.Win32;

namespace Acceleratio.Common.Updater
{
    public partial class Form1 : Form
    {
        private string NuGetRepositoryURL => getRepositoryURL();
        private readonly List<NuGetPackage> _packages;

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


        public Form1()
        {
            _packages = new List<NuGetPackage>();
            InitializeComponent();
            InitRepositoryTextBox();
        }


        private string getRepositoryURL()
        {
            return String.IsNullOrEmpty(repositoryTextBox.Text) ? null : repositoryTextBox.Text;
        }


        private void InitRepositoryTextBox()
        {
            try
            {
                using (RegistryKey NuGetURLRegKey = Registry.CurrentUser.CreateSubKey(AppStrings.KEY_NAME))
                    if (NuGetURLRegKey != null)
                    {
                        string lastEnteredRepository = NuGetURLRegKey.GetValue(AppStrings.KEY_NAME, "").ToString();
                        if (!lastEnteredRepository.Equals(""))
                        {
                            repositoryTextBox.Text = lastEnteredRepository;
                        }
                    }
            }
            catch (Exception)
            {
                ShowError(AppStrings.CannotLoadLastRepoUrl);
            }
        }


        private void UpdateRegistry()
        {
            try
            {
                using (RegistryKey NuGetURLRegKey = Registry.CurrentUser.CreateSubKey(AppStrings.KEY_NAME))

                    if (NuGetURLRegKey != null)
                    {
                        NuGetURLRegKey.SetValue(AppStrings.KEY_NAME, repositoryTextBox.Text);
                    }
            }
            catch (Exception)
            {
                ShowError(AppStrings.CannotLoadLastRepoUrl);
            }
        }


        private async void browseButton_Click(object sender, EventArgs e)
        {
            if (!CheckNuGetURLNotNull())
            {
                return;
            }

            UpdateRegistry();

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
            if (!CheckNuGetURLNotNull())
            {
                return;
            }

            UpdateRegistry();

            await LoadControls();
        }


        private bool CheckNuGetURLNotNull()
        {
            if (NuGetRepositoryURL == null)
            {
                ShowWarning("NuGet repository URL must be provided");
                return false;
            }

            return true;
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
                ShowError($"Error downloading {AppStrings.NuGetBinary}, cannot continue.");
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
                WriteStatus(AppStrings.CommonVersionsNotFound);
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

                string result = await Task.Run(() => ExecuteBinary(AppStrings.NuGetBinary, $"list -source \"{NuGetRepositoryURL}\" -allversions -prerelease"));

                if (String.IsNullOrEmpty(result))
                {
                    throw new RepositoryNotFoundException();
                }
                else
                {
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


        private string ExecuteBinary(string binary, string arguments, bool outputToStatusWindow = false)
        {
            string output = null;

            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.FileName = binary;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;

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
            if (!File.Exists(AppStrings.NuGetBinary))
            {
                WriteStatus("Downloading latest nuget.exe...");

                await Task.Run(() =>
                {
                    try
                    {
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(AppStrings.NuGetWebBinary, AppStrings.NuGetBinary);
                        }
                    }
                    catch
                    {
                        throw new NuGetBinaryDownloadErrorException();
                    }
                });

                WriteStatus("done.", true);
            }

            WriteStatus("Using nuget.exe version " + FileVersionInfo.GetVersionInfo(Path.GetFullPath(AppStrings.NuGetBinary)).ProductVersion);
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
                    await Task.Run(() => ExecuteBinary(AppStrings.NuGetBinary,
                                                       $"update \"{solution}\" -id \"{package.PackageName}\" -version \"{package.PackageVersion}\" -source \"{NuGetRepositoryURL}\" -noninteractive -prerelease -verbose -verbosity detailed -fileconflictaction overwrite",
                                                       true));
                }

                WriteStatus("Update finished.");

                if (deleteOldCheckBox.Checked || uploadCheckBox.Checked)
                {
                    WriteStatus("Starting source control operations..." + Environment.NewLine + Environment.NewLine);

                    var tfsBinary = TFSBinaryPath.GetTFSBinaryPath();
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
                        WriteStatus(AppStrings.CannotLoadTfsBinary);
                    }
                }
            }

            if (isError)
            {
                WriteStatus(AppStrings.UpdateProcessErrors + Environment.NewLine);
            }
            else
            {
                WriteStatus(AppStrings.SuccessfulUpdate + Environment.NewLine);
            }

            var filename = await LogWriter.WriteToLog(statusTextBox.Text, solutionsList.SelectedItems.Cast<string>().ToList(), PackagesToInstall);
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

            if (NuGetRepositoryURL == null)
            {
                ShowWarning("NuGet repository URL must be provided");
                return;
            }

            UpdateRegistry();

            await DoUpdate();

            SetControlsEnabled(true);
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            File.Delete(AppStrings.NuGetBinary);
        }


        private void SetControlsEnabled(bool enabled)
        {
            rootPathTextBox.Enabled = enabled;
            browseButton.Enabled = enabled;
            repositoryTextBox.Enabled = repositoryTextBox.Enabled && enabled;
            connectButton.Enabled = repositoryTextBox.Enabled && enabled;
            packagesComboBoxList.Enabled = enabled && _packages.Any();
            versionsDropDown.Enabled = enabled && versionsDropDown.SelectedItem != null && !latestCheckBox.Checked;
            latestCheckBox.Enabled = enabled && _packages.Any();
            solutionsList.Enabled = enabled;
            uploadCheckBox.Enabled = enabled;
            deleteOldCheckBox.Enabled = enabled;

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


        private static void ShowWarning(string text)
        {
            MessageBox.Show(text, @"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }


        private void WriteStatus(string status, bool append = false)
        {
            statusTextBox.Focus();
            statusTextBox.AppendText((append ? " " : String.IsNullOrEmpty(statusTextBox.Text) ? "" : Environment.NewLine) + (append ? "" : DateTime.Now.ToString("HH:mm:ss") + ": ") + status);
        }
    }
}
