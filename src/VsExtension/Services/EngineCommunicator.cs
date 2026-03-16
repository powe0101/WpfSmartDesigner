using System;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WpfSmartDesigner.VsExtension
{
    /// <summary>
    /// Handles communication with the standalone AI engine via Named Pipes.
    /// </summary>
    public class EngineCommunicator
    {
        private const string PipeName = "WpfSmartDesigner_Pipe";
        private NamedPipeClientStream? pipeClient;

        /// <summary>
        /// Sends an error notification to the AI engine.
        /// </summary>
        public async Task<bool> SendErrorAsync(ErrorInfo error)
        {
            try
            {
                var message = new ErrorNotificationMessage
                {
                    Type = "error_detected",
                    Timestamp = DateTime.UtcNow,
                    FilePath = error.FilePath,
                    Errors = new[] { error }
                };

                return await SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WpfSmartDesigner: Failed to send error - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Requests a fix from the AI engine.
        /// </summary>
        public async Task<FixResponse?> RequestFixAsync(string filePath, bool captureScreen = true)
        {
            try
            {
                var message = new FixRequestMessage
                {
                    Type = "fix_request",
                    FilePath = filePath,
                    CaptureScreen = captureScreen
                };

                var success = await SendMessageAsync(message);
                if (!success)
                    return null;

                // TODO: Receive response from engine
                // For now, return null as placeholder
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WpfSmartDesigner: Failed to request fix - {ex.Message}");
                return null;
            }
        }

        private async Task<bool> SendMessageAsync(object message)
        {
            try
            {
                // Create or reuse pipe client
                if (pipeClient == null || !pipeClient.IsConnected)
                {
                    pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
                    await pipeClient.ConnectAsync(5000); // 5 second timeout
                }

                var json = JsonSerializer.Serialize(message);
                var bytes = Encoding.UTF8.GetBytes(json);

                await pipeClient.WriteAsync(bytes, 0, bytes.Length);
                await pipeClient.FlushAsync();

                return true;
            }
            catch (TimeoutException)
            {
                System.Diagnostics.Debug.WriteLine("WpfSmartDesigner: Engine not responding (timeout)");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"WpfSmartDesigner: Communication error - {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the AI engine is running and responsive.
        /// </summary>
        public async Task<bool> IsEngineAvailableAsync()
        {
            try
            {
                using var testPipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
                await testPipe.ConnectAsync(1000); // 1 second timeout
                return testPipe.IsConnected;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
            pipeClient?.Dispose();
        }
    }

    #region Message Types

    public class ErrorNotificationMessage
    {
        public string Type { get; set; } = "error_detected";
        public DateTime Timestamp { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public ErrorInfo[] Errors { get; set; } = Array.Empty<ErrorInfo>();
    }

    public class FixRequestMessage
    {
        public string Type { get; set; } = "fix_request";
        public string FilePath { get; set; } = string.Empty;
        public bool CaptureScreen { get; set; }
    }

    public class FixResponse
    {
        public string Type { get; set; } = "fix_response";
        public string FilePath { get; set; } = string.Empty;
        public Suggestion[] Suggestions { get; set; } = Array.Empty<Suggestion>();
    }

    public class Suggestion
    {
        public string Description { get; set; } = string.Empty;
        public CodeEdit[] Edits { get; set; } = Array.Empty<CodeEdit>();
        public double Confidence { get; set; }
    }

    public class CodeEdit
    {
        public int Line { get; set; }
        public int Column { get; set; }
        public string OldText { get; set; } = string.Empty;
        public string NewText { get; set; } = string.Empty;
    }

    #endregion
}
