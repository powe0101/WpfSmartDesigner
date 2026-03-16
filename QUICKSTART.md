# Quick Start Guide

## 🎯 최소 테스트 절차 (Windows 전용)

### 준비물
- Windows 10/11
- Visual Studio 2022 (Community 이상)
- .NET 8.0 SDK

---

## Step 1: Engine 실행하기

```powershell
cd src/StandaloneApp
dotnet run
```

**확인사항**:
- WPF 창이 열리나요? ✅
- "Named Pipe server started" 로그가 보이나요? ✅

---

## Step 2: 화면 캡처 테스트

1. Visual Studio 2022 실행 (아무 프로젝트나 열기)
2. Engine 앱에서 **"📸 Test Screen Capture"** 클릭
3. 로그에서 성공 메시지 확인

**저장 위치 확인**:
```powershell
explorer $env:USERPROFILE\Documents\WpfSmartDesigner\Captures
```

---

## Step 3: VS Extension 테스트 (선택)

### A. VS 2022에서 열기
```
src/VsExtension/WpfSmartDesigner.VsExtension.csproj
```

### B. F5 눌러서 실험적 인스턴스 실행

### C. 실험적 VS 인스턴스에서
1. WPF 프로젝트 열기
2. XAML 파일 열기
3. **Tools → Analyze with WpfSmartDesigner** 클릭
4. Engine 앱 로그 확인

---

## ✅ 성공 기준

최소한 다음이 작동하면 성공:
- [x] Engine 실행됨
- [x] Visual Studio 화면 캡처 성공
- [x] Named Pipe 통신 (VS Extension에서 요청 시)

---

## 🚨 Linux/macOS 사용자

**불가능합니다.** WPF와 VS Extension은 Windows 전용입니다.

대안:
- Windows VM 사용
- Windows 개발 머신에 코드 전송

---

자세한 내용은 `TESTING.md` 참조.
