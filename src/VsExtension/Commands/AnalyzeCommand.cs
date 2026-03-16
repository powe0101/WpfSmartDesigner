using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Task = System.Threading.Tasks.Task;

namespace WpfSmartDesigner.VsExtension.Commands
{
    /// <summary>
    /// Command handler for triggering AI analysis on the current file.
    /// </summary>
    internal sealed class AnalyzeCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("f1e2d3c4-b5a6-7890-cdef-1234567890ab");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly AsyncPackage package;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzeCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        /// <param name="commandService">Command service to add command to, not null.</param>
        private AnalyzeCommand(AsyncPackage package, OleMenuCommandService commandService)
        {
            this.package = package ?? throw new ArgumentNullException(nameof(package));
            commandService = commandService ?? throw new ArgumentNullException(nameof(commandService));

            var menuCommandID = new CommandID(CommandSet, CommandId);
            var menuItem = new OleMenuCommand(this.Execute, menuCommandID);
            commandService.AddCommand(menuItem);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static AnalyzeCommand? Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private Microsoft.VisualStudio.Shell.IAsyncServiceProvider ServiceProvider => this.package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            System.Diagnostics.Debug.WriteLine("WpfSmartDesigner: AnalyzeCommand.InitializeAsync called");

            // Switch to the main thread - the call to AddCommand in AnalyzeCommand's constructor requires
            // the UI thread.
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            OleMenuCommandService commandService = await package.GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService
                ?? throw new InvalidOperationException("Cannot get OleMenuCommandService");

            Instance = new AnalyzeCommand(package, commandService);

            System.Diagnostics.Debug.WriteLine("WpfSmartDesigner: AnalyzeCommand initialized successfully");
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void Execute(object? sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            // Get the current active document
            var dte = Package.GetGlobalService(typeof(SDTE)) as EnvDTE.DTE;
            if (dte?.ActiveDocument == null)
            {
                ShowMessage("No active document", "Please open a file to analyze.");
                return;
            }

            var filePath = dte.ActiveDocument.FullName;

            // TODO: Check if it's a WPF-related file (XAML or C#)

            // Send analysis request to AI engine
            _ = Task.Run(async () =>
            {
                try
                {
                    var communicator = new EngineCommunicator();
                    var isAvailable = await communicator.IsEngineAvailableAsync();

                    if (!isAvailable)
                    {
                        await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                        ShowMessage("Engine Not Available", 
                            "The WpfSmartDesigner AI engine is not running.\n\nPlease start the engine application first.");
                        return;
                    }

                    var response = await communicator.RequestFixAsync(filePath, captureScreen: true);

                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    if (response?.Suggestions != null && response.Suggestions.Length > 0)
                    {
                        ShowMessage("Analysis Complete", 
                            $"Found {response.Suggestions.Length} suggestion(s) for {System.IO.Path.GetFileName(filePath)}");
                        // TODO: Display suggestions in a tool window
                    }
                    else
                    {
                        ShowMessage("Analysis Complete", "No issues detected.");
                    }
                }
                catch (Exception ex)
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    ShowMessage("Error", $"Analysis failed: {ex.Message}");
                }
            });

            ShowMessage("Analyzing...", $"Analyzing {System.IO.Path.GetFileName(filePath)}...");
        }

        private void ShowMessage(string title, string message)
        {
            VsShellUtilities.ShowMessageBox(
                this.package,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
