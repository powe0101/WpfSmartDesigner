using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WpfSmartDesigner.Engine
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string statusText = "Initializing...";
        private string logText = "";
        private int requestCount = 0;
        private int analysisCount = 0;
        private string pipeStatus = "Connecting";

        public string StatusText
        {
            get => statusText;
            set { statusText = value; OnPropertyChanged(); }
        }

        public string LogText
        {
            get => logText;
            set { logText = value; OnPropertyChanged(); }
        }

        public int RequestCount
        {
            get => requestCount;
            set { requestCount = value; OnPropertyChanged(); }
        }

        public int AnalysisCount
        {
            get => analysisCount;
            set { analysisCount = value; OnPropertyChanged(); }
        }

        public string PipeStatus
        {
            get => pipeStatus;
            set { pipeStatus = value; OnPropertyChanged(); }
        }

        public MainViewModel()
        {
            StatusText = "🟢 Engine Running - Waiting for Visual Studio requests...";
            PipeStatus = "Listening";
            AddLog("AI Engine initialized successfully");
            AddLog("Named Pipe server started: WpfSmartDesigner_Pipe");
            AddLog("Ready to receive requests from Visual Studio Extension");
        }

        public void AddLog(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            LogText += $"[{timestamp}] {message}\n";
        }

        public void CaptureScreen()
        {
            AddLog("📸 Testing screen capture...");
            
            try
            {
                var captureService = new ScreenCaptureService();
                var success = captureService.CaptureVisualStudioWindow();
                
                if (success)
                {
                    AddLog("✅ Screen capture successful!");
                    AddLog($"   Saved to: {captureService.LastCapturePath}");
                }
                else
                {
                    AddLog("⚠️ Visual Studio window not found");
                }
            }
            catch (Exception ex)
            {
                AddLog($"❌ Screen capture failed: {ex.Message}");
            }
        }

        public void ClearLog()
        {
            LogText = "";
            AddLog("Log cleared");
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
