# 제2장. Windows UI 자동화 기술 스택 이해

## 2.1 Microsoft UI Automation (UIA) 프레임워크

### 2.1.1 UIA의 역사와 발전

**[Microsoft UI Automation](https://learn.microsoft.com/ko-kr/dotnet/framework/ui-automation/ui-automation-overview)**(UIA)은 Windows 애플리케이션의 접근성과 자동화를 위한 프레임워크입니다.

#### 발전 과정

| 시기 | 기술 | 특징 |
|------|------|------|
| **1990년대** | MSAA (Microsoft Active Accessibility) | 최초의 접근성 API, 제한적 기능 |
| **2005년** | UI Automation 1.0 (WinForms 지원) | .NET Framework 3.0과 함께 출시 |
| **2006년** | UI Automation 2.0 (WPF 지원) | WPF의 네이티브 지원, 향상된 성능 |
| **2012년** | UI Automation 3.0 (Windows 8) | UWP 지원, 터치 제스처, 현대적 컨트롤 |
| **2020년~** | UI Automation + Windows App SDK | WinUI 3 지원, 크로스 플랫폼 확장 |

#### MSAA vs UIA

```
MSAA (Legacy)                    UIA (Modern)
    │                                │
    ├─ 단순한 계층 구조               ├─ 풍부한 트리 구조
    ├─ 제한적인 속성                 ├─ 확장 가능한 속성
    ├─ 표준 역할만 지원               ├─ 패턴 기반 아키텍처
    └─ Win32/COM 기반                └─ .NET/COM 기반
```

### 2.1.2 UIA 아키텍처 개요

UIA는 **Provider-Client 아키텍처**를 사용합니다.

```
┌────────────────────────────────────────────────────┐
│              UI Automation Client                   │
│  (테스트 자동화 도구, 스크린 리더, 테스트 스크립트)      │
│                                                     │
│  - TreeWalker (트리 탐색)                           │
│  - CacheRequest (성능 최적화)                       │
│  - EventHandler (이벤트 리스닝)                     │
└──────────────────┬─────────────────────────────────┘
                   │
         UI Automation Core
              (UIAutomationCore.dll)
                   │
┌──────────────────┴─────────────────────────────────┐
│            UI Automation Provider                   │
│         (애플리케이션의 UI 요소)                      │
│                                                     │
│  WPF Provider  │  WinForms    │  UWP Provider      │
│  (네이티브)     │  Provider    │  (네이티브)         │
│                │  (래퍼)       │                    │
└────────────────────────────────────────────────────┘
```

#### 핵심 구성 요소

1. **AutomationElement**: UI 요소의 추상 표현
2. **Control Patterns**: 컨트롤의 상호작용 방식 정의
3. **Properties**: 요소의 속성 (이름, 타입, 상태 등)
4. **Events**: UI 변경 이벤트 통지

### 2.1.3 AutomationElement 이해하기

**[AutomationElement](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.automationelement)**는 UI Automation의 핵심 클래스입니다.

#### AutomationElement의 주요 속성

```csharp
using System.Windows.Automation;

// 루트 요소 (데스크톱)
AutomationElement desktop = AutomationElement.RootElement;

// 기본 속성
string name = element.Current.Name;                    // 요소 이름
string automationId = element.Current.AutomationId;    // 고유 ID
string className = element.Current.ClassName;          // 클래스 이름
ControlType controlType = element.Current.ControlType; // 컨트롤 타입
bool isEnabled = element.Current.IsEnabled;            // 활성화 상태
Rect boundingRect = element.Current.BoundingRectangle; // 화면 좌표
```

#### AutomationElement 트리 구조

```
Desktop (RootElement)
 ├─ Window 1
 │   ├─ MenuBar
 │   │   ├─ MenuItem "File"
 │   │   └─ MenuItem "Edit"
 │   ├─ ToolBar
 │   └─ ContentPane
 │       ├─ TextBox (AutomationId="txtUsername")
 │       └─ Button (AutomationId="btnLogin")
 └─ Window 2
     └─ ...
```

#### Control Patterns

Control Pattern은 컨트롤의 기능을 추상화한 인터페이스입니다.

**주요 패턴:**

| 패턴 | 인터페이스 | 지원 컨트롤 | 기능 |
|------|-----------|------------|------|
| **[Invoke](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.invokepattern)** | InvokePattern | Button, MenuItem | 클릭 동작 |
| **[Value](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.valuepattern)** | ValuePattern | TextBox, Slider | 값 가져오기/설정 |
| **[Text](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.textpattern)** | TextPattern | TextBox, Document | 텍스트 조작 |
| **[Selection](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.selectionpattern)** | SelectionPattern | ListBox, ComboBox | 항목 선택 |
| **[Toggle](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.togglepattern)** | TogglePattern | CheckBox | 상태 토글 |
| **[Window](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.windowpattern)** | WindowPattern | Window | 창 조작 (최소화, 닫기) |
| **[ScrollItem](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.scrollitempattern)** | ScrollItemPattern | ListItem | 스크롤하여 보이게 |

**사용 예제:**

```csharp
// Button 클릭
var button = FindElement(By.AutomationId("btnSubmit"));
var invokePattern = button.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
invokePattern?.Invoke();

// TextBox 값 설정
var textBox = FindElement(By.AutomationId("txtUsername"));
var valuePattern = textBox.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
valuePattern?.SetValue("user@example.com");

// CheckBox 토글
var checkBox = FindElement(By.AutomationId("chkAgree"));
var togglePattern = checkBox.GetCurrentPattern(TogglePattern.Pattern) as TogglePattern;
togglePattern?.Toggle();
```

---

## 2.2 접근성(Accessibility)과 UI 자동화의 관계

### 접근성이란?

**[접근성](https://learn.microsoft.com/ko-kr/windows/apps/design/accessibility/accessibility-overview)**(Accessibility)은 장애가 있는 사용자를 포함한 모든 사용자가 소프트웨어를 사용할 수 있도록 하는 것입니다.

### UI 자동화와 접근성의 관계

```
┌──────────────────────────────────────┐
│      UI Automation Framework          │
│                                       │
│   ┌─────────────┐   ┌──────────────┐ │
│   │ Accessibility│   │Test Automation│ │
│   │   Tools     │   │    Tools      │ │
│   │             │   │               │ │
│   │ - NVDA      │   │ - WinAppDriver│ │
│   │ - JAWS      │   │ - FlaUI       │ │
│   │ - Narrator  │   │ - White       │ │
│   └─────────────┘   └──────────────┘ │
│                                       │
│         Same API & Infrastructure     │
└──────────────────────────────────────┘
```

**핵심 원칙: 접근성이 좋은 앱은 자동화하기 쉽습니다!**

### 접근성 구현이 테스트 자동화에 미치는 영향

#### 좋은 접근성 구현 ✅

```xml
<!-- WPF XAML -->
<Button x:Name="SubmitButton"
        AutomationProperties.AutomationId="btnSubmit"
        AutomationProperties.Name="Submit Form"
        AutomationProperties.HelpText="Click to submit the form">
    Submit
</Button>
```

```csharp
// 쉬운 자동화
var submitButton = window.FindElement(By.AutomationId("btnSubmit"));
submitButton.Click();
```

#### 나쁜 접근성 구현 ❌

```xml
<!-- 접근성 속성 없음 -->
<Image Source="button.png" MouseDown="OnClick" />
```

```csharp
// 어려운 자동화 - 이미지 좌표로 클릭해야 함
var image = window.FindElement(By.ClassName("Image"));
// 정확한 식별 불가능, 클릭 위치 계산 필요
```

### WCAG와 UI 자동화

**[WCAG](https://www.w3.org/WAI/WCAG21/quickref/)** (Web Content Accessibility Guidelines)의 원칙은 데스크톱 애플리케이션에도 적용됩니다:

1. **인지 가능 (Perceivable)**: UI 요소에 명확한 이름과 역할 부여
2. **조작 가능 (Operable)**: 키보드로 모든 기능 접근 가능
3. **이해 가능 (Understandable)**: 일관된 네이밍과 구조
4. **견고성 (Robust)**: 표준 컨트롤 사용, 접근성 API 준수

---

## 2.3 주요 UI 자동화 도구 비교

### 2.3.1 Windows Application Driver (WinAppDriver)

**[WinAppDriver](https://github.com/Microsoft/WinAppDriver)**는 Microsoft에서 개발한 공식 UI 자동화 서버입니다.

#### 특징

- ✅ **Selenium/Appium 호환**: WebDriver 프로토콜 사용
- ✅ **공식 지원**: Microsoft에서 직접 유지보수
- ✅ **다양한 언어 지원**: C#, Python, Java, JavaScript 등
- ✅ **UWP/WPF/WinForms 지원**: 모든 Windows 앱 타입
- ❌ **서버 모드**: 별도 서버 프로세스 필요
- ❌ **성능**: 네트워크 오버헤드

#### 아키텍처

```
┌─────────────────┐      HTTP/JSON      ┌──────────────────┐
│  Test Script    │ ──────────────────► │  WinAppDriver    │
│  (C#, Python)   │   (WebDriver Wire   │  (Server)        │
│                 │      Protocol)       │                  │
└─────────────────┘                      └────────┬─────────┘
                                                  │
                                                  │ UI Automation API
                                                  ▼
                                         ┌──────────────────┐
                                         │  Windows App     │
                                         │  (UWP, WPF, etc) │
                                         └──────────────────┘
```

#### 설치 및 기본 사용

```bash
# WinAppDriver 다운로드
# https://github.com/Microsoft/WinAppDriver/releases

# 서버 시작
WinAppDriver.exe
```

```csharp
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

// 세션 생성
var options = new AppiumOptions();
options.AddAdditionalCapability("app", @"C:\Path\To\App.exe");
options.AddAdditionalCapability("deviceName", "WindowsPC");

var driver = new WindowsDriver<WindowsElement>(
    new Uri("http://127.0.0.1:4723"),
    options
);

// 요소 찾기 및 조작
var element = driver.FindElementByAccessibilityId("txtUsername");
element.SendKeys("user@example.com");

driver.FindElementByAccessibilityId("btnLogin").Click();
```

**장점:**
- Selenium 경험이 있다면 학습 곡선 낮음
- 크로스 플랫폼 테스트 프레임워크와 통합 용이
- 원격 테스트 가능

**단점:**
- 서버 관리 오버헤드
- 복잡한 시나리오에서 성능 이슈
- 고급 UIA 기능 일부 제한

### 2.3.2 FlaUI

**[FlaUI](https://github.com/FlaUI/FlaUI)**는 .NET 기반의 순수 UI Automation 라이브러리입니다.

#### 특징

- ✅ **순수 .NET**: 서버 없이 직접 UIA API 사용
- ✅ **높은 성능**: 네트워크 오버헤드 없음
- ✅ **UIA2/UIA3 지원**: 레거시와 최신 앱 모두 지원
- ✅ **유연성**: 저수준 API 직접 접근 가능
- ✅ **활발한 커뮤니티**: 지속적인 업데이트
- ❌ **.NET 전용**: C# 외 언어 사용 어려움
- ❌ **학습 곡선**: WebDriver보다 복잡한 API

#### 아키텍처

```
┌─────────────────┐
│  Test Script    │
│  (C# only)      │
└────────┬────────┘
         │
         │ Direct API Call
         ▼
┌──────────────────┐
│     FlaUI        │
│   (Wrapper)      │
└────────┬─────────┘
         │
         │ UI Automation API
         ▼
┌──────────────────┐
│  Windows App     │
└──────────────────┘
```

#### NuGet 패키지

```xml
<PackageReference Include="FlaUI.Core" Version="4.0.0" />
<PackageReference Include="FlaUI.UIA3" Version="4.0.0" />
```

#### 기본 사용

```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

// Application 시작
using var automation = new UIA3Automation();
using var app = Application.Launch("notepad.exe");

// 메인 윈도우 가져오기
var mainWindow = app.GetMainWindow(automation);

// 요소 찾기
var textBox = mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("15"))?.AsTextBox();

// 텍스트 입력
textBox?.Text = "Hello, FlaUI!";

// 저장 다이얼로그 열기
var fileMenu = mainWindow.FindFirstDescendant(cf =>
    cf.ByName("File"))?.AsMenuItem();
fileMenu?.Click();

var saveMenu = mainWindow.FindFirstDescendant(cf =>
    cf.ByName("Save"))?.AsMenuItem();
saveMenu?.Click();
```

**장점:**
- 매우 빠른 성능
- 고급 UIA 기능 완전 지원
- .NET 생태계와 완벽한 통합
- 유연한 대기 메커니즘
- 스크린샷, 캡처 기능 내장

**단점:**
- .NET(C#) 외 언어 사용 불가
- WebDriver에 익숙한 팀에겐 새로운 학습 필요

### 2.3.3 TestStack.White (Legacy)

**[TestStack.White](https://github.com/TestStack/White)**는 오래된 UI 자동화 라이브러리입니다.

#### 특징

- ⚠️ **레거시**: 2016년 이후 업데이트 중단
- ⚠️ **호환성 문제**: 최신 Windows/앱과 호환 이슈
- ✅ **간단한 API**: 초보자에게 친화적
- ❌ **비추천**: 새 프로젝트에 사용 금지

**현재는 FlaUI로 마이그레이션 권장**

### 2.3.4 Playwright for Windows (실험적)

**[Playwright](https://playwright.dev/)**는 주로 웹 자동화 도구이지만, Electron 앱 테스트 가능합니다.

#### 특징

- ✅ **Electron 앱**: Chromium 기반 데스크톱 앱 지원
- ✅ **크로스 플랫폼**: Windows, Mac, Linux
- ❌ **제한적**: 네이티브 Windows 앱 불가

```csharp
// Electron 앱 테스트 예제
using Microsoft.Playwright;

var playwright = await Playwright.CreateAsync();
var browser = await playwright.Chromium.LaunchAsync(new()
{
    ExecutablePath = @"C:\Path\To\ElectronApp.exe"
});

var page = await browser.NewPageAsync();
await page.ClickAsync("#submit-button");
```

---

## 2.4 도구 선택 가이드라인

### 의사결정 트리

```
프로젝트 요구사항 분석
    │
    ├─ .NET 전용 프로젝트인가?
    │   ├─ Yes → FlaUI (최고 성능, 완전한 기능)
    │   └─ No → 다음 단계로
    │
    ├─ 다른 언어(Python, Java) 사용 필요?
    │   ├─ Yes → WinAppDriver (Selenium 호환)
    │   └─ No → 다음 단계로
    │
    ├─ Electron 앱인가?
    │   ├─ Yes → Playwright
    │   └─ No → FlaUI 또는 WinAppDriver
    │
    └─ 기존 Selenium 인프라 활용?
        ├─ Yes → WinAppDriver
        └─ No → FlaUI 권장
```

### 비교표

| 기준 | WinAppDriver | FlaUI | TestStack.White | Playwright |
|------|-------------|-------|----------------|-----------|
| **성능** | 중간 | 높음 | 낮음 | 높음 |
| **학습 곡선** | 낮음 (Selenium 유사) | 중간 | 낮음 | 중간 |
| **언어 지원** | 다수 | C# only | C# only | 다수 |
| **유지보수** | 활발 | 활발 | 중단 | 활발 |
| **WPF 지원** | ✅ | ✅ | ✅ | ❌ |
| **WinForms 지원** | ✅ | ✅ | ✅ | ❌ |
| **UWP 지원** | ✅ | ✅ | ⚠️ | ❌ |
| **Electron 지원** | ❌ | ❌ | ❌ | ✅ |
| **서버 필요** | Yes | No | No | No |
| **고급 UIA 기능** | 제한적 | 완전 | 제한적 | N/A |
| **CI/CD 통합** | ✅ | ✅ | ✅ | ✅ |
| **커뮤니티** | 중간 | 활발 | 거의 없음 | 매우 활발 |

### 권장 사항

#### 🥇 **FlaUI - 최우선 추천**
**사용 시나리오:**
- .NET/C# 기반 프로젝트
- 높은 성능이 필요한 경우
- 복잡한 UI 자동화 요구사항
- 완전한 UIA 기능 활용 필요

```csharp
// FlaUI 예제
using FlaUI.Core;
using FlaUI.UIA3;

var app = Application.Launch("YourApp.exe");
var automation = new UIA3Automation();
var window = app.GetMainWindow(automation);
// ... 자동화 코드
```

#### 🥈 **WinAppDriver - 대안**
**사용 시나리오:**
- 다국어 팀 (Python, Java 개발자 포함)
- 기존 Selenium/Appium 인프라 활용
- 웹 + 데스크톱 통합 테스트
- 원격 테스트 필요

```python
# WinAppDriver with Python
from appium import webdriver

desired_caps = {
    "app": r"C:\Path\To\App.exe",
    "deviceName": "WindowsPC"
}

driver = webdriver.Remote("http://localhost:4723", desired_caps)
driver.find_element_by_accessibility_id("btnSubmit").click()
```

#### 🎯 **Playwright - 특수 목적**
**사용 시나리오:**
- Electron 기반 앱
- VS Code, Slack, Discord 같은 앱
- 크로스 플랫폼 요구사항

#### ❌ **TestStack.White - 사용 금지**
- 새 프로젝트에 절대 사용하지 마세요
- 기존 프로젝트는 FlaUI로 마이그레이션 권장

---

## 실전 예제: 동일한 작업을 각 도구로 구현

### 시나리오: 계산기 앱에서 2 + 3 계산하기

#### WinAppDriver

```csharp
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

var options = new AppiumOptions();
options.AddAdditionalCapability("app", "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");

var driver = new WindowsDriver<WindowsElement>(
    new Uri("http://127.0.0.1:4723"), options);

driver.FindElementByName("Two").Click();
driver.FindElementByName("Plus").Click();
driver.FindElementByName("Three").Click();
driver.FindElementByName("Equals").Click();

var result = driver.FindElementByAccessibilityId("CalculatorResults").Text;
Assert.Contains("5", result);

driver.Quit();
```

#### FlaUI

```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

var app = Application.LaunchStoreApp("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
var automation = new UIA3Automation();
var window = app.GetMainWindow(automation);

window.FindFirstDescendant(cf => cf.ByName("Two")).Click();
window.FindFirstDescendant(cf => cf.ByName("Plus")).Click();
window.FindFirstDescendant(cf => cf.ByName("Three")).Click();
window.FindFirstDescendant(cf => cf.ByName("Equals")).Click();

var result = window.FindFirstDescendant(cf =>
    cf.ByAutomationId("CalculatorResults")).AsLabel();
Assert.Contains("5", result.Text);

app.Close();
```

### 성능 비교

```
동일한 작업 100회 반복 실행 시간:
- FlaUI: ~15초
- WinAppDriver: ~35초
- TestStack.White: ~40초 (+ 몇 번 실패)
```

---

## 요약

이 장에서는 Windows UI 자동화 기술 스택을 살펴보았습니다:

- **Microsoft UI Automation (UIA)**는 Windows의 통합 접근성 및 자동화 프레임워크
- **AutomationElement**와 **Control Patterns**가 핵심 개념
- **접근성 구현이 좋은 앱**은 자동화하기 쉬움
- **FlaUI**는 .NET 프로젝트에 최우선 추천 (성능, 기능)
- **WinAppDriver**는 다국어 팀이나 Selenium 인프라 활용 시 선택
- **TestStack.White**는 더 이상 사용하지 말 것

다음 장에서는 실제 개발 환경을 구성하는 방법을 알아보겠습니다.

---

## 참고 자료

- [UI Automation Overview](https://learn.microsoft.com/ko-kr/dotnet/framework/ui-automation/ui-automation-overview)
- [FlaUI GitHub](https://github.com/FlaUI/FlaUI)
- [WinAppDriver GitHub](https://github.com/Microsoft/WinAppDriver)
- [Control Patterns](https://learn.microsoft.com/ko-kr/dotnet/framework/ui-automation/ui-automation-control-patterns-overview)
- [AutomationElement Class](https://learn.microsoft.com/ko-kr/dotnet/api/system.windows.automation.automationelement)
- [Windows Accessibility](https://learn.microsoft.com/ko-kr/windows/apps/design/accessibility/accessibility-overview)

[◀ 이전: 제1장](chapter-01.md) | [다음: 제3장 ▶](chapter-03.md)
