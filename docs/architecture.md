# Architecture Design

## Overview

WpfSmartDesigner uses a **hybrid architecture** that separates Visual Studio integration concerns from AI processing logic.

## System Components

### 1. Visual Studio Extension (Thin Client)

**Responsibilities:**
- Listen to XAML designer and build errors
- Capture file paths and error details
- Relay information to AI engine
- Apply code edits received from engine
- Provide UI commands (shortcuts, context menu)

**Technologies:**
- Visual Studio SDK (DTE, MEF)
- Roslyn Workspace API
- Named Pipe Client

**Key APIs:**
```csharp
// Error detection
IVsErrorList errorList
IVsBuildManagerAccessor buildManager

// Code editing
VisualStudioWorkspace workspace
Document.WithText(sourceText)

// Communication
NamedPipeClientStream pipeClient
```

### 2. Standalone AI Engine (Heavy Lifting)

**Responsibilities:**
- Screen capture from Visual Studio designer
- Visual analysis using AI vision models
- XAML parsing and error detection
- Code analysis with Roslyn
- Generate fix suggestions
- Manage conversation context

**Technologies:**
- .NET 8.0 WPF application
- System.Drawing / Win32 API (screen capture)
- Microsoft.CodeAnalysis (Roslyn)
- OpenClaw CLI/API or direct LLM integration

**Pipeline:**
```
Screen Capture → Image Analysis → Error Identification
                                          ↓
    Code Edit ← Suggestion Generation ← Context Building
```

### 3. Core Shared Library

**Responsibilities:**
- Data models (Error, Fix, FileContext)
- Communication protocol (JSON messages)
- Roslyn utilities
- XAML parsing helpers

## Communication Protocol

### Message Format (JSON over Named Pipe or HTTP)

**Error Notification (VS → Engine):**
```json
{
  "type": "error_detected",
  "timestamp": "2026-03-16T09:59:00Z",
  "file_path": "C:\\Projects\\App\\MainWindow.xaml",
  "errors": [
    {
      "line": 42,
      "column": 15,
      "severity": "error",
      "code": "XLS0101",
      "message": "Property 'Colr' not found on 'Button'"
    }
  ]
}
```

**Fix Request (VS → Engine):**
```json
{
  "type": "fix_request",
  "file_path": "C:\\Projects\\App\\MainWindow.xaml",
  "capture_screen": true
}
```

**Fix Response (Engine → VS):**
```json
{
  "type": "fix_response",
  "file_path": "C:\\Projects\\App\\MainWindow.xaml",
  "suggestions": [
    {
      "description": "Change 'Colr' to 'Color'",
      "edits": [
        {
          "line": 42,
          "column": 15,
          "old_text": "Colr",
          "new_text": "Color"
        }
      ],
      "confidence": 0.95
    }
  ]
}
```

## Data Flow

```
┌─────────────────────┐
│  Visual Studio      │
│  (Developer works)  │
└──────────┬──────────┘
           │ XAML error occurs
           ↓
┌─────────────────────┐
│  VS Extension       │
│  - Detect error     │
│  - Send to engine   │
└──────────┬──────────┘
           │ Named Pipe / HTTP
           ↓
┌─────────────────────┐
│  AI Engine          │
│  - Capture screen   │
│  - Analyze context  │
│  - Generate fix     │
└──────────┬──────────┘
           │ Fix response
           ↓
┌─────────────────────┐
│  VS Extension       │
│  - Apply edit       │
│  - Show to user     │
└──────────┬──────────┘
           │
           ↓
┌─────────────────────┐
│  Visual Studio      │
│  (Error resolved)   │
└─────────────────────┘
```

## Deployment Model

### Development Phase
- VS Extension: Debug mode (F5 in VS Experimental Instance)
- AI Engine: Console app with WPF UI

### Production
- VS Extension: VSIX package (Visual Studio Marketplace)
- AI Engine: Standalone executable or Windows Service
- Auto-start: Engine launches with VS or on-demand

## Security Considerations

- Communication: localhost-only (Named Pipe or 127.0.0.1)
- File access: Read/write permissions verified
- AI prompts: Sanitize file paths and sensitive data
- User consent: Explicit approval for code changes (initially)

## Scalability

- **Multi-instance**: Support multiple VS instances
- **Queue management**: Handle concurrent fix requests
- **Model swapping**: Hot-swap AI models without restart
- **Logging**: Structured logs for debugging

## Future Enhancements

- Remote AI engine (cloud-based)
- Collaborative editing (team suggestions)
- Learning from user feedback
- Support for other IDEs (Rider, VS Code)
