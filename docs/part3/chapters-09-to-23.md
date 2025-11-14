# 제9장~제23장 개요

## 제3부: 중급편 - 테스트 설계와 패턴

### 제9장. 테스트 데이터 관리

#### 9.1 데이터 주도 테스트 (Data-Driven Testing)

**[데이터 주도 테스트](https://learn.microsoft.com/ko-kr/dotnet/core/testing/unit-testing-best-practices#test-driven-development)**는 동일한 테스트를 다양한 데이터 세트로 실행하는 방법입니다.

```csharp
// xUnit Theory 예제
[Theory]
[InlineData("user1@test.com", "Pass123!", true)]
[InlineData("user2@test.com", "Pass456!", true)]
[InlineData("invalid", "wrong", false)]
public void Login_VariousCredentials_ReturnsExpectedResult(
    string username, string password, bool shouldSucceed)
{
    // Test implementation
}
```

#### CSV 파일 활용

```csharp
public class CsvDataAttribute : DataAttribute
{
    private readonly string _filePath;

    public CsvDataAttribute(string filePath)
    {
        _filePath = filePath;
    }

    public override IEnumerable<object[]> GetData(MethodInfo testMethod)
    {
        // CSV 파일 읽기 및 데이터 반환
    }
}
```

---

### 제10장. 동기화와 대기 전략

#### 10.1 암시적 대기 vs 명시적 대기

**암시적 대기** (Implicit Wait):
```csharp
_session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
```

**명시적 대기** (Explicit Wait):
```csharp
var wait = new WebDriverWait(_session, TimeSpan.FromSeconds(10));
var element = wait.Until(driver =>
    driver.FindElementByAccessibilityId("result"));
```

#### 10.2 FluentWait 구현

```csharp
using Polly;

var policy = Policy
    .Handle<NoSuchElementException>()
    .WaitAndRetry(5, retryAttempt =>
        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

var element = policy.Execute(() =>
    window.FindFirstDescendant(cf => cf.ByAutomationId("element")));
```

---

### 제11장. 예외 처리와 복구 전략

#### 11.1 일반적인 예외 시나리오

- `ElementNotAvailableException`: 요소가 사용 불가능
- `NoSuchElementException`: 요소를 찾을 수 없음
- `StaleElementReferenceException`: 요소 참조가 더 이상 유효하지 않음

#### 11.2 Retry 메커니즘

```csharp
public T RetryOnException<T>(Func<T> action, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return action();
        }
        catch (Exception ex) when (i < maxRetries - 1)
        {
            Thread.Sleep(TimeSpan.FromSeconds(Math.Pow(2, i)));
        }
    }
    throw new Exception("Max retries exceeded");
}
```

---

### 제12장. WPF 애플리케이션 테스트

#### 12.1 WPF 컨트롤 특성

**[WPF](https://learn.microsoft.com/ko-kr/dotnet/desktop/wpf/)** (Windows Presentation Foundation)는 XAML 기반 UI 프레임워크입니다.

- 데이터 바인딩
- MVVM 패턴
- 스타일과 템플릿
- 커스텀 컨트롤

#### 12.2 MVVM 패턴과 테스트 전략

```
View (XAML) ←→ ViewModel ←→ Model
     ↑
     UI Automation으로 테스트
```

---

### 제13장. WinForms 애플리케이션 테스트

#### 13.1 WinForms 컨트롤 계층 구조

**[WinForms](https://learn.microsoft.com/ko-kr/dotnet/desktop/winforms/)**는 .NET Framework의 전통적인 UI 프레임워크입니다.

- 간단한 이벤트 기반 프로그래밍
- 풍부한 서드파티 컨트롤
- 레거시 애플리케이션 대응

---

## 제4부: 고급편 - 엔터프라이즈 레벨

### 제14장. 테스트 프레임워크 아키텍처

#### 14.1 레이어드 아키텍처 설계

```
┌───────────────────────┐
│   Tests Layer         │ ← 테스트 케이스
├───────────────────────┤
│   Page Objects Layer  │ ← UI 추상화
├───────────────────────┤
│   Services Layer      │ ← 비즈니스 로직
├───────────────────────┤
│   Core/Utils Layer    │ ← 공통 유틸리티
└───────────────────────┘
```

#### 14.2 의존성 주입(DI) 활용

```csharp
using Microsoft.Extensions.DependencyInjection;

public class TestStartup
{
    public IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IAutomation, UIA3Automation>();
        services.AddScoped<IApplicationManager, ApplicationManager>();
        return services.BuildServiceProvider();
    }
}
```

---

### 제15장. 크로스 플랫폼 테스트

#### 15.1 .NET MAUI 애플리케이션 테스트

**[.NET MAUI](https://learn.microsoft.com/ko-kr/dotnet/maui/)** (Multi-platform App UI)는 크로스 플랫폼 UI 프레임워크입니다.

- Windows, macOS, iOS, Android 지원
- 단일 코드베이스

---

### 제16장. 성능 테스트와 최적화

#### 16.1 병렬 테스트 실행

**xUnit 병렬화**:
```csharp
[Collection("NonParallel")]
public class UITests
{
    // 병렬 실행 안 함
}
```

**NUnit 병렬화**:
```csharp
[Parallelizable(ParallelScope.All)]
public class UITests
{
    // 병렬 실행
}
```

#### 16.2 테스트 실행 시간 단축 전략

- 불필요한 대기 시간 제거
- 선택적 테스트 실행
- 테스트 데이터 재사용
- Setup/Teardown 최적화

---

### 제17장. CI/CD 파이프라인 통합

#### 17.1 Azure DevOps 통합

**[Azure DevOps](https://learn.microsoft.com/ko-kr/azure/devops/)** YAML 파이프라인:

```yaml
trigger:
  - main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    version: '8.x'

- script: |
    dotnet restore
    dotnet build --configuration Release
  displayName: 'Build'

- script: |
    start /B "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
    dotnet test --configuration Release --logger trx
  displayName: 'Run UI Tests'

- task: PublishTestResults@2
  inputs:
    testResultsFormat: 'VSTest'
    testResultsFiles: '**/*.trx'
```

#### 17.2 GitHub Actions 통합

```yaml
name: UI Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: windows-latest

    steps:
    - uses: actions/checkout@v3

    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'

    - name: Restore dependencies
      run: dotnet restore

    - name: Build
      run: dotnet build --no-restore

    - name: Start WinAppDriver
      run: |
        Start-Process "C:\Program Files (x86)\Windows Application Driver\WinAppDriver.exe"
        Start-Sleep -Seconds 5

    - name: Run tests
      run: dotnet test --no-build --verbosity normal

    - name: Upload screenshots
      if: failure()
      uses: actions/upload-artifact@v3
      with:
        name: screenshots
        path: ./screenshots/
```

---

### 제18장. 고급 시나리오

#### 18.1 멀티 윈도우 애플리케이션 테스트

```csharp
var windows = app.GetAllTopLevelWindows(automation);
var mainWindow = windows.First(w => w.Name == "Main Window");
var childWindow = windows.First(w => w.Name == "Child Window");
```

#### 18.2 모달 다이얼로그 처리

```csharp
// 다이얼로그가 나타날 때까지 대기
var dialog = Retry.WhileNull(() =>
    automation.GetDesktop().FindFirstChild(cf =>
        cf.ByName("Save As").AndByControlType(ControlType.Window)),
    TimeSpan.FromSeconds(10)
).Result;

// 다이얼로그 처리
var fileNameTextBox = dialog.FindFirstDescendant(cf =>
    cf.ByAutomationId("FileNameTextBox"));
fileNameTextBox.AsTextBox().Text = "test.txt";

dialog.FindFirstDescendant(cf =>
    cf.ByAutomationId("SaveButton")).AsButton().Invoke();
```

#### 18.3 파일 다이얼로그 자동화

```csharp
using System.Runtime.InteropServices;

// Win32 API를 통한 파일 다이얼로그 처리
[DllImport("user32.dll")]
static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

// AutoIt, SendKeys 등을 사용한 대안
```

---

### 제19장. 접근성 테스트 자동화

#### 19.1 WCAG 가이드라인

**[WCAG](https://www.w3.org/WAI/WCAG21/quickref/)** (Web Content Accessibility Guidelines) 2.1 레벨 AA 준수

#### 19.2 Axe-Windows 활용

**[Axe-Windows](https://github.com/microsoft/axe-windows)**는 Microsoft의 접근성 자동 테스트 도구입니다.

```bash
# Axe-Windows CLI 설치
dotnet tool install -g Microsoft.Axe.Windows.CLI

# 실행
axe-windows scan --app MyApp.exe --output-dir ./results
```

---

## 제5부: 실무편 - 베스트 프랙티스

### 제20장. 테스트 전략과 계획

#### 20.1 테스트 범위 결정

- **크리티컬 패스**: 비즈니스에 핵심적인 기능
- **회귀 테스트**: 자주 변경되는 영역
- **연기 테스트**: 빌드 검증

#### 20.2 Risk-based Testing

높은 위험도 영역에 테스트 리소스 집중:
- 복잡한 비즈니스 로직
- 자주 변경되는 코드
- 과거 버그 이력

---

### 제21장. 유지보수성 향상

#### 21.1 테스트 명명 규칙

```csharp
// Given_When_Then 패턴
[Fact]
public void Login_WithValidCredentials_NavigatesToDashboard()
{
    // ...
}

// MethodName_StateUnderTest_ExpectedBehavior 패턴
[Fact]
public void Calculate_TwoPositiveNumbers_ReturnsSum()
{
    // ...
}
```

#### 21.2 DRY 원칙 적용

- 중복 코드 제거
- Helper 메서드 추출
- Base 클래스 활용

---

### 제22장. 트러블슈팅 가이드

#### 22.1 일반적인 문제와 해결방법

**문제: Flaky Tests (불안정한 테스트)**

해결방법:
- 적절한 대기 시간 추가
- 동기화 메커니즘 개선
- 테스트 데이터 격리
- 환경 의존성 제거

**문제: 느린 테스트 실행**

해결방법:
- 불필요한 대기 제거
- 병렬 실행 활용
- Setup/Teardown 최적화

#### 22.2 로깅 전략

```csharp
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/uitest-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

Log.Information("Starting test: {TestName}", testName);
Log.Error(ex, "Test failed: {TestName}", testName);
```

---

### 제23장. 팀 협업과 커뮤니케이션

#### 23.1 개발팀과의 협업

- 접근성 속성 (AutomationId) 명명 규칙 합의
- UI 변경 시 사전 공지
- 테스트 가능한 설계 (Testability)

#### 23.2 테스트 결과 커뮤니케이션

- CI/CD 대시보드 활용
- 실패 시 스크린샷 및 로그 첨부
- 명확한 버그 리포트 작성

---

## 요약

**제3부 (9-13장)**: 테스트 데이터 관리, 동기화 전략, WPF/WinForms 테스트

**제4부 (14-19장)**: 엔터프라이즈 아키텍처, CI/CD 통합, 고급 시나리오, 접근성

**제5부 (20-23장)**: 테스트 전략, 유지보수성, 트러블슈팅, 팀 협업

---

## 참고 자료

- [Azure DevOps Documentation](https://learn.microsoft.com/ko-kr/azure/devops/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [Axe-Windows](https://github.com/microsoft/axe-windows)
- [.NET MAUI Documentation](https://learn.microsoft.com/ko-kr/dotnet/maui/)

[◀ 이전: 제8장](../part2/chapter-05-to-08.md) | [다음: 부록 ▶](../appendix/appendix.md)
