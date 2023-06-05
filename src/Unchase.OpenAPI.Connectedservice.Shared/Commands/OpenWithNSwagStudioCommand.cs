using System;
using System.ComponentModel.Design;
using System.IO;
using System.Windows;

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.Win32;
using Unchase.OpenAPI.ConnectedService.Common;
using Task = System.Threading.Tasks.Task;

namespace Unchase.OpenAPI.ConnectedService.Commands
{
    /// <summary>
    /// Command handler.
    /// </summary>
    internal sealed class OpenWithNSwagStudioCommand
    {
        #region Properties and fields

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int OpenInNSwagStudioCommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid OpenInNSwagStudioCommandSet = new Guid("6914aba2-4e20-4f5b-8f5e-3485d4091437");

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
        /// Initializes a new instance of the <see cref="OpenWithNSwagStudioCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        /// <param name="options">Options.</param>
        /// <param name="dte"><see cref="DTE2"/>.</param>
        private OpenWithNSwagStudioCommand(
            AsyncPackage package,
            OleMenuCommandService commandService,
            Options options,
            DTE2 dte)
        {
            _options = options;
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _dte = dte;
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));
            var menuCommandId = new CommandID(OpenInNSwagStudioCommandSet, OpenInNSwagStudioCommandId);
            var menuItem = new OleMenuCommand(OpenFolderInVs, menuCommandId);
            menuItem.BeforeQueryStatus += BeforeQueryStatusCallback;
            commandService.AddCommand(menuItem);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static OpenWithNSwagStudioCommand Instance { get; private set; }

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
            // Switch to the main thread - the call to AddCommand in OpenWithNSwagStudioCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);
            var commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            var dte = await package.GetServiceAsync(typeof(DTE)) as DTE2;
            Instance = new OpenWithNSwagStudioCommand(package, commandService, options, dte);
        }

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
                cmd.Visible = !string.IsNullOrWhiteSpace(path) && !Directory.Exists(path.Trim('"')) && (path.Trim('"').EndsWith(".nswag") || path.Trim('"').EndsWith(".nswag.json"));
                cmd.Enabled = cmd.Visible;
            });
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void OpenFolderInVs(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var path = ProjectHelper.GetSelectedPath(_dte);
                if (!string.IsNullOrEmpty(path))
                {
                    OpenNSwagStudio(path);
                }
                else
                {
                    MessageBox.Show("Couldn't resolve the file path.", Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                LoggerHelper.Log(ex);
            }
        }

        /// <summary>
        /// Open file with <paramref name="path"/> in NSwagStudio.
        /// </summary>
        /// <param name="path"></param>
        private void OpenNSwagStudio(string path)
        {
            EnsureNSwagStudioPathExist();
            var isDirectory = Directory.Exists(path.Trim('"'));
            if (isDirectory)
            {
                return;
            }

            var fileInfo = new FileInfo(path.Trim('"'));
            if (!fileInfo.Extension.Equals(".nswag") && !fileInfo.Extension.Equals(".json"))
            {
                MessageBox.Show("Only files with extensions of \".nswag\" or \".json\" can be openned by NSwagStudio.",
                    Constants.ExtensionName, MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var args = $"{path}";

            var start = new System.Diagnostics.ProcessStartInfo
            {
                FileName = $"\"{_options.PathToNSwagStudioExe}\"",
                Arguments = args,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
            };

            using (System.Diagnostics.Process.Start(start))
            {
            }
        }

        /// <summary>
        /// Ensure that NSwagStudio path is exists.
        /// </summary>
        private void EnsureNSwagStudioPathExist()
        {
            if (File.Exists(_options.PathToNSwagStudioExe))
            {
                return;
            }

            var box = MessageBox.Show("Can't find NSwagStudio (NSwagStudio.exe). Would you like to help me find it?", Constants.ExtensionName, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (box == MessageBoxResult.No)
            {
                return;
            }

            var dialog = new OpenFileDialog
            {
                DefaultExt = ".exe",
                FileName = "NSwagStudio.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                CheckFileExists = true
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                _options.PathToNSwagStudioExe = dialog.FileName;
                _options.SaveSettingsToStorage();
            }
        }

        #endregion
    }
}