using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace WpfSmartDesigner.VsExtension
{
    /// <summary>
    /// Service that listens to build and XAML errors in the Error List.
    /// </summary>
    public class ErrorListenerService
    {
        private static ErrorListenerService? instance;
        private IVsErrorList? errorList;

        /// <summary>
        /// Gets the singleton instance of the ErrorListenerService.
        /// </summary>
        public static ErrorListenerService Instance => instance ?? throw new InvalidOperationException("Service not initialized");

        /// <summary>
        /// Initializes the ErrorListenerService.
        /// </summary>
        public static async Task InitializeAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            instance = new ErrorListenerService();
            await instance.InitializeInternalAsync(package);
        }

        private async Task InitializeInternalAsync(AsyncPackage package)
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync(package.DisposalToken);

            // Get the Error List service
            errorList = await package.GetServiceAsync(typeof(SVsErrorList)) as IVsErrorList;

            System.Diagnostics.Debug.WriteLine("WpfSmartDesigner: Error Listener initialized");
            
            // TODO: Implement actual error list monitoring
            // For now, this is a placeholder that will be expanded later
        }

        /// <summary>
        /// Gets current errors from the Error List.
        /// </summary>
        /// <returns>List of error information.</returns>
        public async Task<List<ErrorInfo>> GetCurrentErrorsAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();

            var errors = new List<ErrorInfo>();

            // TODO: Query the error list and extract error details
            // This is a placeholder implementation

            return errors;
        }

        /// <summary>
        /// Handles a detected error by sending it to the AI engine.
        /// </summary>
        public async Task HandleErrorAsync(ErrorInfo error)
        {
            await Task.Run(() =>
            {
                // TODO: Send error to standalone AI engine via Named Pipe or HTTP
                System.Diagnostics.Debug.WriteLine($"WpfSmartDesigner: Detected error - {error.Message} at {error.FilePath}:{error.Line}");
            });
        }
    }

    /// <summary>
    /// Represents an error detected in Visual Studio.
    /// </summary>
    public class ErrorInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public int Line { get; set; }
        public int Column { get; set; }
        public string Severity { get; set; } = "error";
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DetectedAt { get; set; } = DateTime.UtcNow;
    }
}
