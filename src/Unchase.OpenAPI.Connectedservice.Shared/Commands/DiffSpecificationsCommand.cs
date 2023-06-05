using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Windows;

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using Unchase.OpenAPI.ConnectedService.Common;
using MessageBox = System.Windows.MessageBox;
using OpenFileDialog = System.Windows.Forms.OpenFileDialog;
using Task = System.Threading.Tasks.Task;

namespace Unchase.OpenAPI.ConnectedService.Commands
{
    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class DiffSpecificationsCommand
    {
        #region Properties and fields

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int DiffSpecificationsCommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid DiffSpecificationsCommandSet = new Guid("b637f8e5-4b21-41e4-9039-62fc114e1646");

        /// <summary>
        /// Options.
        /// </summary>
        private readonly Options _options;

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage _package;

        /// <summary>
        /// <see cref="DTE2"/>.
        /// </summary>
        private readonly DTE2 _dte;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DiffSpecificationsCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="options">Options.</param>
        /// <param name="dte"><see cref="DTE2"/>.</param>
        private DiffSpecificationsCommand(
            AsyncPackage package,
            OleMenuCommandService commandService,
            Options options,
            DTE2 dte)
        {
            _options = options;
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _dte = dte;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandId = new CommandID(DiffSpecificationsCommandSet, DiffSpecificationsCommandId);
            var menuItem = new OleMenuCommand(DiffSpecificationWithSource, menuCommandId);
            menuItem.BeforeQueryStatus += BeforeQueryStatusCallback;
            commandService.AddCommand(menuItem);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This function is the callback used for <see cref="OleMenuCommand.BeforeQueryStatus"/>.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void BeforeQueryStatusCallback(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(_package.DisposalToken);
                var cmd = (OleMenuCommand)sender;
                var path = ProjectHelper.GetSelectedPath(_dte);
                cmd.Visible = !string.IsNullOrWhiteSpace(path) && !Directory.Exists(path.Trim('"')) && path.Trim('"').EndsWith(".nswag.json");
                cmd.Enabled = cmd.Visible;
            });
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static DiffSpecificationsCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IAsyncServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="options">Options.</param>
        public static async Task InitializeAsync(AsyncPackage package, Options options)
        {
            // Switch to the main thread - the call to AddCommand in DiffSpecificationsCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Instance = new DiffSpecificationsCommand(package, commandService, options, dte);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void DiffSpecificationWithSource(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (CanSpecificationFileAndSourceBeCompared(out var file1, out var file2))
                {
                    if (!DiffFileUsingCustomTool(file1, file2))
                    {
                        DiffFilesUsingDefaultTool(file1, file2);
                    }

                    if (File.Exists(file2) && file2.EndsWith(".tmp"))
                    {
                        File.Delete(file2);
                    }
                }
            }
            catch (Exception exception)
            {
                LoggerHelper.Log(exception);
            }
        }

        private bool CanSpecificationFileAndSourceBeCompared(out string file1, out string file2)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var items = GetSelectedFiles().ToList();
            file1 = items.ElementAtOrDefault(0);
            file2 = items.ElementAtOrDefault(1) ?? GetSpecificationFromSource();

            var isDirectory = Directory.Exists(file1?.Trim('"'));
            if (isDirectory)
            {
                MessageBox.Show("Only files can be diff with source.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            var fileInfo = new FileInfo(file1?.Trim('"') ?? throw new InvalidOperationException());
            if (!fileInfo.Extension.Equals(".json"))
            {
                MessageBox.Show("Only files with extensions of \".json\" can be diff with source.",
                    Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (items.Count == 1 && string.IsNullOrWhiteSpace(file2))
            {
                var dialog = new OpenFileDialog
                {
                    InitialDirectory = Path.GetDirectoryName(file1),
                    Title = "Choose specification-file instead of source Uri",
                    Filter = "Json-files (*.json)|*.json"
                };
                dialog.ShowDialog();
                file2 = dialog.FileName;
            }
            return !string.IsNullOrEmpty(file1) && !string.IsNullOrEmpty(file2);
        }

        public IEnumerable<string> GetSelectedFiles()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var items = (Array)_dte.ToolWindows.SolutionExplorer?.SelectedItems;

            return from item in items?.Cast<UIHierarchyItem>()
                   let pi = item.Object as ProjectItem
                   select pi.FileNames[1];
        }

        private string GetSpecificationFromSource()
        {
            var endpoint = _options.OpenApiSpecificationEndpoint;
            if (string.IsNullOrWhiteSpace(endpoint))
            {
                MessageBox.Show("The OpenAPI (Swagger) specification endpoint is not set in \"Tools -> Options -> Web -> Unchase OpenAPI (Swagger) Connected Service\".", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return string.Empty;
            }
            var workFile = Path.GetTempFileName();
            try
            {
                //ToDo: add credentials and proxy (with credentials)
                // from url
                if (endpoint.StartsWith("http://", StringComparison.Ordinal) || endpoint.StartsWith("https://", StringComparison.Ordinal))
                {
                    using (var httpClient = new HttpClient())
                    {
                        //TODO: make the whole method async?
                        var specificationJson = ThreadHelper.JoinableTaskFactory.Run(async () => await httpClient.GetStringAsync(endpoint));

                        if (string.IsNullOrWhiteSpace(specificationJson))
                        {
                            throw new InvalidOperationException("The json-file is an empty file.");
                        }

                        if (!ProjectHelper.IsJson(specificationJson))
                        {
                            throw new InvalidOperationException("Service endpoint is not an json-file.");
                        }

                        specificationJson = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(specificationJson), new JsonSerializerOptions { WriteIndented = true });
                        File.WriteAllText(workFile, specificationJson, Encoding.UTF8);
                    }
                }
                else // from local path
                {
                    if (!File.Exists(endpoint))
                    {
                        throw new ArgumentException("Please input the service endpoint with exists file path.", "OpenAPI Service Endpoint");
                    }

                    var specificationJson = File.ReadAllText(endpoint);

                    if (string.IsNullOrWhiteSpace(specificationJson))
                    {
                        throw new InvalidOperationException("The json-file is an empty file.");
                    }

                    if (!ProjectHelper.IsJson(specificationJson))
                    {
                        throw new InvalidOperationException("Service endpoint is not an json-file.");
                    }

                    specificationJson = JsonSerializer.Serialize(JsonSerializer.Deserialize<object>(specificationJson), new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(workFile, specificationJson, Encoding.UTF8);
                }
                return workFile;
            }
            catch (WebException e)
            {
                MessageBox.Show($"Cannot access \"{endpoint}\". {e.Message}",
                    Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return string.Empty;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Cannot access \"{endpoint}\". {e.Message}",
                    Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return string.Empty;
            }
        }

        private void DiffFilesUsingDefaultTool(string file1, string file2)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // This is the guid and id for the Tools.DiffFiles command
            const string diffFilesCmd = "{5D4C0442-C0A2-4BE8-9B4D-AB1C28450942}";
            const int diffFilesId = 256;
            object args = $"\"{file1}\" \"{file2}\"";
            _dte.Commands.Raise(diffFilesCmd, diffFilesId, ref args, ref args);
        }

        //Visual Studio allows replacing the default diff tool with a custom one.
        //See, for example:
        //Using WinMerge: https://blog.paulbouwer.com/2010/01/31/replace-diffmerge-tool-in-visual-studio-team-system-with-winmerge/
        //Using BeyondCompare: http://stackoverflow.com/questions/4466238/how-to-configure-visual-studio-to-use-beyond-compare
        private bool DiffFileUsingCustomTool(string file1, string file2)
        {
            try
            {
                //Checking the registry to see if a custom tool is configured
                //Relevant information: https://social.msdn.microsoft.com/Forums/vstudio/en-US/37a26013-2f78-4519-85e5-d896ac27f31e/see-what-default-visual-studio-tfexe-compare-tool-is-set-to-using-visual-studio-api?forum=vsx
                var registryFolder = $"{_dte.RegistryRoot}\\TeamFoundation\\SourceControl\\DiffTools\\.*\\Compare";

                using (var key = Registry.CurrentUser.OpenSubKey(registryFolder))
                {
                    var command = key?.GetValue("Command") as string;
                    if (string.IsNullOrEmpty(command))
                    {
                        return false;
                    }

                    var args = key.GetValue("Arguments") as string;
                    if (string.IsNullOrEmpty(args))
                    {
                        return false;
                    }

                    //Understanding the arguments: https://msdn.microsoft.com/en-us/library/ms181446(v=vs.100).aspx
                    args =
                        args.Replace("%1", $"\"{file1}\"")
                            .Replace("%2", $"\"{file2}\"")
                            .Replace("%5", string.Empty)
                            .Replace("%6", $"\"{file1}\"")
                            .Replace("%7", $"\"{file2}\"");
                    System.Diagnostics.Process.Start(command, args);
                }
                return true;
            }
            catch (Exception ex)
            {
                LoggerHelper.Log(ex);
                return false;
            }
        }

        #endregion
    }
}