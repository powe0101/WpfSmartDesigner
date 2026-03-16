# Standalone AI Engine

This directory contains the standalone WPF application that serves as the AI analysis engine for WpfSmartDesigner.

## Project Structure

```
StandaloneApp/
├── WpfSmartDesigner.Engine.csproj     # Project file
├── App.xaml / App.xaml.cs             # Application entry point
├── MainWindow.xaml / .cs              # Main UI window
├── MainViewModel.cs                    # UI view model
└── Services/
    ├── PipeServerService.cs            # Named Pipe server
    ├── ScreenCaptureService.cs         # Screen capture (Win32 API)
    └── AnalysisService.cs              # AI analysis (placeholder)
```

## Features

### Current (v0.1)
- [x] WPF UI with real-time log display
- [x] Named Pipe server (listens on `WpfSmartDesigner_Pipe`)
- [x] Screen capture of Visual Studio window
- [x] Request/response handling from VS Extension
- [x] Statistics tracking (requests, analyses)

### Planned
- [ ] Roslyn-based XAML/C# parsing
- [ ] OpenClaw integration for AI vision
- [ ] Error pattern recognition
- [ ] Code suggestion generation
- [ ] Settings UI (API keys, preferences)
- [ ] System tray support

## Building

**Prerequisites:**
- .NET 8.0 SDK
- Windows 10/11

**Build Steps:**
```bash
# Restore packages
dotnet restore

# Build
dotnet build -c Release

# Run
dotnet run
```

## Running

### From Visual Studio
1. Open `WpfSmartDesigner.Engine.csproj`
2. Press F5 to run

### From Command Line
```bash
cd src/StandaloneApp
dotnet run
```

### Standalone Executable
```bash
cd bin/Release/net8.0-windows
./WpfSmartDesigner.Engine.exe
```

## UI Overview

### Main Window
- **Header**: Status indicator and engine state
- **Stats Panel**: Real-time metrics
  - Requests Received
  - Analyses Completed
  - Pipe Status (Listening/Connected)
- **Log Output**: Dark-themed console log
- **Action Buttons**:
  - 📸 Test Screen Capture
  - 🗑️ Clear Log

## Communication Protocol

### Named Pipe Server
- **Pipe Name**: `WpfSmartDesigner_Pipe`
- **Direction**: Bidirectional (InOut)
- **Format**: JSON messages

### Supported Message Types

**1. Error Detected** (VS → Engine)
```json
{
  "Type": "error_detected",
  "Timestamp": "2026-03-16T10:00:00Z",
  "FilePath": "C:\\...\\MainWindow.xaml",
  "Errors": [
    {
      "Line": 42,
      "Column": 15,
      "Severity": "error",
      "Code": "XLS0101",
      "Message": "Property 'Colr' not found"
    }
  ]
}
```

**2. Fix Request** (VS → Engine)
```json
{
  "Type": "fix_request",
  "FilePath": "C:\\...\\MainWindow.xaml",
  "CaptureScreen": true
}
```

**3. Fix Response** (Engine → VS)
```json
{
  "Type": "fix_response",
  "FilePath": "C:\\...\\MainWindow.xaml",
  "Suggestions": [
    {
      "Description": "Change 'Colr' to 'Color'",
      "Edits": [
        {
          "Line": 42,
          "Column": 15,
          "OldText": "Colr",
          "NewText": "Color"
        }
      ],
      "Confidence": 0.95
    }
  ]
}
```

## Screen Capture

### How It Works
1. Finds running Visual Studio process (`devenv.exe`)
2. Gets main window handle via Win32 API
3. Captures window content using `PrintWindow` API
4. Saves as PNG to `Documents\WpfSmartDesigner\Captures\`

### Output Location
```
%USERPROFILE%\Documents\WpfSmartDesigner\Captures\
└── capture_20260316_100530.png
```

## Services

### PipeServerService
- Manages Named Pipe server lifecycle
- Handles incoming requests asynchronously
- Dispatches messages to appropriate handlers
- Sends responses back to VS Extension

### ScreenCaptureService
- Uses Win32 API (`user32.dll`)
- Supports full window capture
- Supports region capture (future)
- High DPI aware

### AnalysisService (Placeholder)
- File analysis (XAML, C#)
- Image analysis (AI vision)
- Suggestion generation
- **TODO**: Integrate actual AI models

## Integration Points

### OpenClaw
Future integration will use:
- `exec` tool to call OpenClaw CLI
- `browser` tool for UI automation
- Custom prompts for code analysis

### AI Models
Options for AI analysis:
- OpenClaw (local/remote)
- OpenAI Vision API
- Claude API
- Local LLMs (Ollama, LM Studio)

## Known Limitations

- Windows-only (Win32 API dependency)
- Requires Visual Studio to be running for capture
- No actual AI analysis yet (placeholder)
- Single instance only (no multi-client support yet)

## Troubleshooting

### Pipe Connection Failed
- Ensure no other instance is running
- Check Windows Firewall settings
- Run as Administrator if needed

### Screen Capture Returns Black Image
- Visual Studio window must be visible (not minimized)
- Hardware acceleration in VS may interfere
- Try disabling GPU rendering in VS settings

## Future Enhancements

- [ ] OpenClaw CLI integration
- [ ] Roslyn code analysis
- [ ] XAML parser with error detection
- [ ] Multi-model AI support
- [ ] Background service mode (no UI)
- [ ] Settings persistence (JSON config)
