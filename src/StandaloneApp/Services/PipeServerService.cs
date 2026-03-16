using System.IO.Pipes;
using System.Text;
using Newtonsoft.Json;

namespace WpfSmartDesigner.Engine
{
    /// <summary>
    /// Named Pipe server that listens for requests from the VS Extension.
    /// </summary>
    public class PipeServerService
    {
        private const string PipeName = "WpfSmartDesigner_Pipe";
        private CancellationTokenSource? cancellationTokenSource;
        private Task? serverTask;

        public event EventHandler<MessageReceivedEventArgs>? MessageReceived;

        public void Start()
        {
            cancellationTokenSource = new CancellationTokenSource();
            serverTask = Task.Run(() => RunServerAsync(cancellationTokenSource.Token));
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            serverTask?.Wait();
        }

        private async Task RunServerAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    using var pipeServer = new NamedPipeServerStream(
                        PipeName,
                        PipeDirection.InOut,
                        NamedPipeServerStream.MaxAllowedServerInstances,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    Log("Waiting for VS Extension connection...");

                    // Wait for client connection
                    await pipeServer.WaitForConnectionAsync(cancellationToken);

                    Log("✅ VS Extension connected!");

                    // Read message
                    var buffer = new byte[4096];
                    var messageBuilder = new StringBuilder();

                    int bytesRead;
                    while ((bytesRead = await pipeServer.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
                    {
                        messageBuilder.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));

                        if (!pipeServer.IsConnected)
                            break;
                    }

                    var message = messageBuilder.ToString();
                    if (!string.IsNullOrEmpty(message))
                    {
                        Log($"📨 Received message: {message.Substring(0, Math.Min(100, message.Length))}...");
                        await ProcessMessageAsync(message, pipeServer);
                    }
                }
                catch (OperationCanceledException)
                {
                    Log("Server shutdown requested");
                    break;
                }
                catch (Exception ex)
                {
                    Log($"❌ Pipe error: {ex.Message}");
                    await Task.Delay(1000, cancellationToken); // Wait before retry
                }
            }
        }

        private async Task ProcessMessageAsync(string jsonMessage, NamedPipeServerStream pipeServer)
        {
            try
            {
                var message = JsonConvert.DeserializeObject<dynamic>(jsonMessage);
                string? messageType = message?.Type;

                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(jsonMessage));

                switch (messageType)
                {
                    case "error_detected":
                        await HandleErrorDetectedAsync(message);
                        break;

                    case "fix_request":
                        var response = await HandleFixRequestAsync(message);
                        await SendResponseAsync(pipeServer, response);
                        break;

                    default:
                        Log($"⚠️ Unknown message type: {messageType}");
                        break;
                }
            }
            catch (Exception ex)
            {
                Log($"❌ Message processing error: {ex.Message}");
            }
        }

        private async Task HandleErrorDetectedAsync(dynamic message)
        {
            await Task.Run(() =>
            {
                string? filePath = message?.FilePath;
                Log($"🔍 Error detected in: {filePath}");
                // TODO: Store error for analysis
            });
        }

        private async Task<object> HandleFixRequestAsync(dynamic message)
        {
            return await Task.Run(() =>
            {
                string? filePath = message?.FilePath;
                bool captureScreen = message?.CaptureScreen ?? false;

                Log($"🔧 Fix requested for: {filePath}");

                if (captureScreen)
                {
                    Log("📸 Capturing Visual Studio screen...");
                    var captureService = new ScreenCaptureService();
                    captureService.CaptureVisualStudioWindow();
                }

                // TODO: Implement actual AI analysis
                // For now, return a mock response
                return new
                {
                    Type = "fix_response",
                    FilePath = filePath,
                    Suggestions = new[]
                    {
                        new
                        {
                            Description = "Example suggestion (AI analysis not yet implemented)",
                            Edits = Array.Empty<object>(),
                            Confidence = 0.0
                        }
                    }
                };
            });
        }

        private async Task SendResponseAsync(NamedPipeServerStream pipeServer, object response)
        {
            var json = JsonConvert.SerializeObject(response);
            var bytes = Encoding.UTF8.GetBytes(json);

            await pipeServer.WriteAsync(bytes, 0, bytes.Length);
            await pipeServer.FlushAsync();

            Log($"📤 Response sent: {json.Substring(0, Math.Min(100, json.Length))}...");
        }

        private void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            Console.WriteLine($"[{timestamp}] {message}");
            System.Diagnostics.Debug.WriteLine($"[PipeServer] {message}");
        }
    }

    public class MessageReceivedEventArgs : EventArgs
    {
        public string Message { get; }

        public MessageReceivedEventArgs(string message)
        {
            Message = message;
        }
    }
}
