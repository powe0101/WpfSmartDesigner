# WpfSmartDesigner 🎨🤖

AI-powered WPF designer assistant for Visual Studio: auto-debugging, XAML analysis, and intelligent code editing.

## 🎯 Project Goals

1. **Auto-Debugging & Screen Analysis**: Antigravity-style automatic detection and correction of WPF designer and XAML errors
2. **Intelligent Code Editing**: AI-driven modification of WPF designer views and C# code in Visual Studio

## 🏗️ Architecture

### Hybrid Approach

```
[VS Extension (Lightweight)]  ←→  [Standalone AI Engine]
         ↓                              ↓
    - Error detection              - Screen capture & analysis
    - File path relay              - Code suggestion generation
    - Edit application             - Context management
    - Shortcuts/commands           - AI model integration
```

**Components:**

- **VS Extension** (`src/VsExtension/`): Minimal Visual Studio integration layer
  - XAML/build error detection
  - Communication bridge to AI engine
  - Code editor integration (Roslyn)

- **Standalone AI Engine** (`src/StandaloneApp/`): Core AI logic
  - Screen capture and visual analysis
  - Independent Roslyn-based code analysis
  - AI model integration (OpenClaw/LLM)

- **Core Libraries** (`src/Core/`): Shared components
  - Code analysis utilities
  - Communication protocols
  - Data models

## 🛠️ Technology Stack

### VS Extension
- Visual Studio SDK
- Roslyn (Microsoft.CodeAnalysis)
- IVsErrorList, IVsSolution APIs

### Standalone Application
- .NET 8.0+
- WPF (UI)
- System.Drawing (screen capture)
- Microsoft.CodeAnalysis (Roslyn)
- OpenClaw CLI/API integration

### Communication
- Named Pipes or HTTP (localhost)
- JSON-based message protocol

## 📋 Development Roadmap

### Phase 1: Prototype (Current)
- [x] Repository setup
- [ ] Core architecture design
- [ ] Screen capture POC
- [ ] AI analysis pipeline

### Phase 2: Standalone Engine
- [ ] Screen capture & analysis
- [ ] XAML error detection
- [ ] Code suggestion generation
- [ ] OpenClaw integration

### Phase 3: VS Extension
- [ ] Error listener
- [ ] File system bridge
- [ ] Edit application layer
- [ ] Keyboard shortcuts

### Phase 4: Integration & Testing
- [ ] End-to-end pipeline
- [ ] Performance optimization
- [ ] User testing

## 🚀 Getting Started

_(Coming soon)_

## 📚 Documentation

- [Architecture](docs/architecture.md)
- [Requirements](docs/requirements.md)

## 📄 License

MIT

## 👤 Author

**powe0101**
- GitHub: [@powe0101](https://github.com/powe0101)
