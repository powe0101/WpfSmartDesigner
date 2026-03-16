# WpfSmartDesigner - 테스트 가이드

## 🎯 개요

이 문서는 WpfSmartDesigner를 로컬에서 빌드하고 테스트하는 방법을 안내합니다.

## ⚠️ 중요: 시스템 요구사항

### 필수 환경
- **OS**: Windows 10 (1809+) 또는 Windows 11
- **Visual Studio 2022**: Community 이상 (VS 2019는 지원 안 됨)
- **.NET SDK**: 8.0 이상

### Visual Studio 워크로드
VS 2022 설치 시 다음 워크로드가 필요합니다:

1. **Visual Studio extension development** (확장 개발용)
2. **.NET desktop development** (WPF 앱용)
3. **Windows 10/11 SDK**

---

## 📦 1단계: 리포지토리 클론

```powershell
# Windows PowerShell 또는 Git Bash
git clone https://github.com/powe0101/WpfSmartDesigner.git
cd WpfSmartDesigner
```

---

## 🛠️ 2단계: VS Extension 수정 (현재 필요)

VS Extension은 현재 빌드할 수 없습니다. 다음 파일들을 수정해야 합니다:

### A. VSCommandTable.vsct 컴파일 설정 추가

`src/VsExtension/WpfSmartDesigner.VsExtension.csproj` 파일을 열고 다음을 추가:

```xml
<ItemGroup>
  <VSCTCompile Include="VSCommandTable.vsct">
    <ResourceName>Menus.ctmenu</ResourceName>
  </VSCTCompile>
</ItemGroup>
```

### B. 리소스 파일 생성

다음 경로에 빈 파일 또는 임시 파일을 만듭니다:

```
src/VsExtension/Resources/
├── LICENSE.txt          (프로젝트 루트 LICENSE 복사)
├── Icon.png             (16x16 PNG, 임시로 아무 이미지)
├── Preview.png          (200x200 PNG, 임시로 아무 이미지)
└── AnalyzeCommand.png   (16x16 PNG, 임시로 아무 이미지)
```

### C. source.extension.vsixmanifest 단순화

리소스 참조 제거 (선택 사항):

```xml
<!-- 이 줄들을 주석 처리 또는 삭제 -->
<!--
<License>Resources\LICENSE.txt</License>
<Icon>Resources\Icon.png</Icon>
<PreviewImage>Resources\Preview.png</PreviewImage>
-->
```

---

## 🚀 3단계: Standalone Engine 빌드 및 실행

### 빌드

```powershell
cd src/StandaloneApp
dotnet restore
dotnet build -c Release
```

### 실행

```powershell
dotnet run
```

또는:

```powershell
cd bin/Release/net8.0-windows
./WpfSmartDesigner.Engine.exe
```

**예상 결과**:
- WPF 창이 열림
- "🟢 Engine Running - Waiting for Visual Studio requests..." 표시
- 로그에 "Named Pipe server started: WpfSmartDesigner_Pipe" 출력

### 화면 캡처 테스트

1. Visual Studio를 실행 (아무 프로젝트나 열기)
2. Engine 앱에서 **"📸 Test Screen Capture"** 버튼 클릭
3. 로그 확인:
   - ✅ 성공 시: `Screen capture successful!` + 저장 경로
   - ⚠️ 실패 시: `Visual Studio window not found`

**캡처 저장 위치**:
```
C:\Users\[YourName]\Documents\WpfSmartDesigner\Captures\
└── capture_20260316_101530.png
```

---

## 🔌 4단계: VS Extension 빌드 (수정 후)

### Visual Studio에서 빌드

1. Visual Studio 2022 열기
2. `src/VsExtension/WpfSmartDesigner.VsExtension.csproj` 열기
3. 솔루션 빌드 (Ctrl+Shift+B)

**예상 출력**:
```
bin/Release/WpfSmartDesigner.VsExtension.vsix
```

### 디버그 실행 (Experimental Instance)

1. F5 키 누르기
2. Visual Studio의 **실험적 인스턴스**가 새 창으로 열림
3. 실험적 인스턴스에서 WPF 프로젝트 열기
4. **Tools → Analyze with WpfSmartDesigner** 메뉴 확인

---

## 🧪 5단계: End-to-End 테스트

### 준비

1. **Engine 실행**: `WpfSmartDesigner.Engine.exe` 실행 (백그라운드)
2. **VS Extension 로드**: VS 2022 실험적 인스턴스 실행
3. **테스트 프로젝트**: WPF 프로젝트 생성 또는 열기

### 테스트 시나리오 1: 수동 분석

1. VS에서 XAML 파일 열기 (예: `MainWindow.xaml`)
2. **Tools → Analyze with WpfSmartDesigner** 클릭
3. Engine 앱 로그 확인:
   ```
   [10:15:30] ✅ VS Extension connected!
   [10:15:30] 📨 Received message: {"Type":"fix_request",...}
   [10:15:31] 📸 Capturing Visual Studio screen...
   [10:15:31] 📤 Response sent: {"Type":"fix_response",...}
   ```
4. VS에서 메시지 박스 확인: "Analysis Complete"

### 테스트 시나리오 2: 오류 감지 (향후)

현재는 자동 오류 감지가 구현되지 않았습니다. VS Extension의 `ErrorListenerService`가 스켈레톤 상태입니다.

---

## 🐛 문제 해결

### Engine이 VS를 찾지 못함

**증상**: "Visual Studio window not found"

**해결책**:
1. Visual Studio가 실제로 실행 중인지 확인 (`devenv.exe` 프로세스)
2. VS 창이 최소화되지 않았는지 확인
3. 관리자 권한으로 Engine 실행 시도

### Named Pipe 연결 실패

**증상**: VS에서 "Engine Not Available" 메시지

**해결책**:
1. Engine이 실행 중인지 확인
2. 다른 인스턴스가 Pipe를 점유하지 않았는지 확인
3. Windows Firewall 설정 확인
4. 관리자 권한으로 실행

### VS Extension이 메뉴에 나타나지 않음

**증상**: Tools 메뉴에 "Analyze with WpfSmartDesigner" 없음

**해결책**:
1. 실험적 인스턴스에서 실행했는지 확인 (F5)
2. VSIX가 올바르게 설치되었는지 확인:
   - Tools → Extensions and Updates
   - "WPF Smart Designer" 검색
3. VS 재시작

### 빌드 오류: "VSCTCompile task failed"

**증상**: `.vsct` 파일 컴파일 실패

**해결책**:
1. Visual Studio Extension Development 워크로드 설치 확인
2. 프로젝트 파일에 VSCTCompile 아이템 추가 (위 2단계 참조)

---

## 📝 현재 한계 (알려진 이슈)

### 구현되지 않은 기능

- [ ] 실제 AI 분석 (AnalysisService는 플레이스홀더)
- [ ] 자동 오류 감지 (ErrorListenerService 스켈레톤)
- [ ] 코드 편집 적용 (Roslyn 통합 필요)
- [ ] 설정 UI (API 키, 선호도)

### 테스트 가능한 기능

- [x] Named Pipe 통신 (VS ↔ Engine)
- [x] 화면 캡처 (Visual Studio 창)
- [x] 수동 분석 요청 (Tools 메뉴)
- [x] 로그 출력 (Engine UI)

---

## 🚀 다음 단계

빌드 및 기본 통신이 작동하면:

1. **OpenClaw 통합**: AI 분석 로직 구현
2. **Roslyn 통합**: C#/XAML 파싱
3. **자동 오류 감지**: Error List 리스너 완성
4. **코드 편집**: 제안 적용 기능

---

## 📞 도움이 필요하면

- GitHub Issues: https://github.com/powe0101/WpfSmartDesigner/issues
- 문서: `docs/architecture.md`, `docs/requirements.md`
