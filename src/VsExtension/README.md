# VS Extension

This directory contains the Visual Studio extension component of WpfSmartDesigner.

## Project Structure

```
VsExtension/
├── WpfSmartDesignerPackage.cs          # Main package entry point
├── VSCommandTable.vsct                 # Command definitions
├── source.extension.vsixmanifest       # Extension manifest
├── Commands/
│   └── AnalyzeCommand.cs               # "Analyze with AI" command
└── Services/
    ├── ErrorListenerService.cs         # Listens to VS error list
    └── EngineCommunicator.cs           # Named Pipe communication with AI engine
```

## Features

### Current (v0.1)
- [x] Basic package structure
- [x] Error list listener (skeleton)
- [x] Named Pipe communication protocol
- [x] "Analyze with AI" command in Tools menu
- [x] Engine availability check

### Planned
- [ ] Real-time error detection from Error List
- [ ] XAML designer event hooks
- [ ] Code edit application (Roslyn integration)
- [ ] Suggestion preview tool window
- [ ] Keyboard shortcuts (Ctrl+Shift+A)
- [ ] Context menu integration

## Building

**Prerequisites:**
- Visual Studio 2022 with "Visual Studio extension development" workload
- .NET 8.0 SDK

**Build Steps:**
```bash
# Restore packages
dotnet restore

# Build (creates VSIX)
dotnet build -c Release

# Output: bin/Release/WpfSmartDesigner.VsExtension.vsix
```

## Debugging

1. Open `WpfSmartDesigner.VsExtension.csproj` in Visual Studio 2022
2. Press F5 to launch the Experimental Instance
3. The extension will be loaded in the experimental VS instance
4. Set breakpoints in the code to debug

## Installation

### From VSIX (built locally)
```powershell
# Navigate to output directory
cd bin/Release

# Install VSIX
./WpfSmartDesigner.VsExtension.vsix
```

### From source (development)
- Press F5 in Visual Studio to debug
- The extension auto-loads when a solution is opened

## Communication Protocol

The extension communicates with the standalone AI engine via Named Pipes.

**Pipe Name:** `WpfSmartDesigner_Pipe`

**Message Format:** JSON

See `EngineCommunicator.cs` for message type definitions.

## Key Classes

### WpfSmartDesignerPackage
- Main entry point
- Initializes services and commands
- Auto-loads when a solution is opened

### ErrorListenerService
- Subscribes to Visual Studio Error List
- Detects XAML and build errors
- Forwards errors to AI engine

### EngineCommunicator
- Manages Named Pipe connection
- Sends error notifications
- Requests fixes from AI engine
- Receives fix suggestions

### AnalyzeCommand
- Provides "Analyze with AI" command in Tools menu
- Gets current active document
- Triggers manual analysis request

## Known Limitations

- VS 2022 only (no older version support)
- Windows-only (VS SDK limitation)
- Named Pipe communication (no HTTP option yet)
- Manual engine startup required (no auto-launch)

## Future Enhancements

- Auto-start AI engine with VS
- HTTP communication option
- Real-time error highlighting in editor
- Diff preview before applying fixes
- Settings UI for customization
