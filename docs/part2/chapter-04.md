# 제4장. 첫 번째 UI 자동화 테스트 작성

## 4.1 WinAppDriver 시작하기

### 4.1.1 WinAppDriver 설치 및 설정

**[WinAppDriver](https://github.com/Microsoft/WinAppDriver)** 다운로드 및 설치

1. **다운로드**: https://github.com/Microsoft/WinAppDriver/releases
2. **설치**: `WindowsApplicationDriver.msi` 실행
3. **설치 위치**: `C:\Program Files (x86)\Windows Application Driver\`

### 4.1.2 개발자 모드 활성화

```
Windows 설정 → 개인 정보 보호 및 보안 → 개발자용 → 개발자 모드 켜기
```

### 4.1.3 WinAppDriver 서버 시작

```powershell
# 기본 포트 (4723)로 시작
cd "C:\Program Files (x86)\Windows Application Driver\"
.\WinAppDriver.exe

# 출력:
# Windows Application Driver listening for requests at: http://127.0.0.1:4723/
```

---

## 4.2 계산기 앱 자동화 예제

### 프로젝트 생성

예제 코드: [examples/Chapter04.Calculator](../../examples/Chapter04.Calculator/)

### 프로젝트 파일 (Calculator.UITests.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="xunit" Version="2.6.4" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Appium.WebDriver" Version="5.0.0-rc.1" />
  </ItemGroup>
</Project>
```

### 계산기 테스트 코드

```csharp
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using Xunit;

namespace Calculator.UITests;

public class CalculatorTests : IDisposable
{
    private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
    private const string CalculatorAppId = "Microsoft.WindowsCalculator_8wekyb3d8bbwe!App";

    private WindowsDriver<WindowsElement> _session;

    public CalculatorTests()
    {
        var options = new AppiumOptions();
        options.AddAdditionalCapability("app", CalculatorAppId);
        options.AddAdditionalCapability("deviceName", "WindowsPC");

        _session = new WindowsDriver<WindowsElement>(
            new Uri(WindowsApplicationDriverUrl),
            options
        );

        _session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
    }

    [Fact]
    public void Addition_TwoPlusThree_ReturnsFive()
    {
        // Arrange & Act
        _session.FindElementByName("Two").Click();
        _session.FindElementByName("Plus").Click();
        _session.FindElementByName("Three").Click();
        _session.FindElementByName("Equals").Click();

        // Assert
        var result = _session.FindElementByAccessibilityId("CalculatorResults").Text;
        Assert.Contains("5", result);
    }

    [Theory]
    [InlineData("One", "Plus", "One", "2")]
    [InlineData("Five", "Minus", "Three", "2")]
    [InlineData("Four", "Multiply by", "Two", "8")]
    [InlineData("Nine", "Divide by", "Three", "3")]
    public void BasicOperations_VariousInputs_ReturnsCorrectResults(
        string num1, string operation, string num2, string expected)
    {
        // Act
        _session.FindElementByName(num1).Click();
        _session.FindElementByName(operation).Click();
        _session.FindElementByName(num2).Click();
        _session.FindElementByName("Equals").Click();

        // Assert
        var result = _session.FindElementByAccessibilityId("CalculatorResults").Text;
        Assert.Contains(expected, result);
    }

    [Fact]
    public void Clear_AfterCalculation_ResetsToZero()
    {
        // Arrange
        _session.FindElementByName("Five").Click();
        _session.FindElementByName("Plus").Click();
        _session.FindElementByName("Three").Click();
        _session.FindElementByName("Equals").Click();

        // Act
        _session.FindElementByName("Clear").Click();

        // Assert
        var result = _session.FindElementByAccessibilityId("CalculatorResults").Text;
        Assert.Contains("0", result);
    }

    public void Dispose()
    {
        _session?.Quit();
        _session?.Dispose();
    }
}
```

---

## 4.3 메모장 자동화 예제

예제 코드: [examples/Chapter04.Notepad](../../examples/Chapter04.Notepad/)

### 메모장 테스트 코드

```csharp
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using Xunit;

namespace Notepad.UITests;

public class NotepadTests : IDisposable
{
    private const string WindowsApplicationDriverUrl = "http://127.0.0.1:4723";
    private const string NotepadAppId = @"C:\Windows\System32\notepad.exe";

    private WindowsDriver<WindowsElement> _session;

    public NotepadTests()
    {
        var options = new AppiumOptions();
        options.AddAdditionalCapability("app", NotepadAppId);
        options.AddAdditionalCapability("deviceName", "WindowsPC");

        _session = new WindowsDriver<WindowsElement>(
            new Uri(WindowsApplicationDriverUrl),
            options
        );

        _session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
    }

    [Fact]
    public void TypeText_SimpleString_DisplaysInTextArea()
    {
        // Arrange
        const string testText = "Hello, Windows UI Automation!";

        // Act
        var textEditor = _session.FindElementByClassName("Edit");
        textEditor.SendKeys(testText);

        // Assert
        Assert.Equal(testText, textEditor.Text);
    }

    [Fact]
    public void FileMenu_Open_DisplaysFileDialog()
    {
        // Act
        _session.FindElementByName("File").Click();
        _session.FindElementByName("Open...").Click();

        // Assert - 파일 열기 다이얼로그 표시 확인
        var openDialog = _session.FindElementByName("Open");
        Assert.NotNull(openDialog);

        // Cleanup - ESC로 다이얼로그 닫기
        openDialog.SendKeys(OpenQA.Selenium.Keys.Escape);
    }

    [Fact]
    public void EditMenu_FindAndReplace_OpensDialog()
    {
        // Arrange - 텍스트 입력
        var textEditor = _session.FindElementByClassName("Edit");
        textEditor.SendKeys("This is a test. This is only a test.");

        // Act - Ctrl+H (찾기 및 바꾸기)
        textEditor.SendKeys(OpenQA.Selenium.Keys.Control + "h");

        // Assert
        var replaceDialog = _session.FindElementByName("Replace");
        Assert.NotNull(replaceDialog);

        // Cleanup
        replaceDialog.SendKeys(OpenQA.Selenium.Keys.Escape);
    }

    public void Dispose()
    {
        // 저장하지 않고 종료
        try
        {
            _session?.Close();
        }
        catch
        {
            // 이미 닫힌 경우 무시
        }
        finally
        {
            // 저장 확인 다이얼로그 처리
            try
            {
                var desktopSession = new WindowsDriver<WindowsElement>(
                    new Uri(WindowsApplicationDriverUrl),
                    new AppiumOptions()
                );

                var saveDialog = desktopSession.FindElementByName("Notepad");
                if (saveDialog != null)
                {
                    // "저장하지 않음" 버튼 클릭
                    desktopSession.FindElementByName("Don't Save").Click();
                }

                desktopSession.Quit();
            }
            catch
            {
                // 다이얼로그가 없으면 무시
            }

            _session?.Dispose();
        }
    }
}
```

---

## 4.4 테스트 실행 및 디버깅

### Visual Studio에서 실행

```
테스트 탐색기 열기 (Ctrl+E, T)
↓
모든 테스트 실행 (Ctrl+R, A)
↓
결과 확인
```

### 명령줄에서 실행

```bash
dotnet test
```

### 디버깅 팁

1. **암시적 대기 시간 증가**
   ```csharp
   _session.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
   ```

2. **명시적 대기 사용**
   ```csharp
   using OpenQA.Selenium.Support.UI;

   var wait = new WebDriverWait(_session, TimeSpan.FromSeconds(10));
   var element = wait.Until(driver =>
       driver.FindElementByAccessibilityId("btnSubmit"));
   ```

3. **스크린샷 캡처**
   ```csharp
   var screenshot = _session.GetScreenshot();
   screenshot.SaveAsFile("error.png");
   ```

4. **요소 정보 출력**
   ```csharp
   var element = _session.FindElementByName("Button");
   Console.WriteLine($"Name: {element.Text}");
   Console.WriteLine($"Enabled: {element.Enabled}");
   Console.WriteLine($"Location: {element.Location}");
   ```

---

## 일반적인 문제 및 해결방법

### 문제 1: WinAppDriver 연결 실패

```
OpenQA.Selenium.WebDriverException: Cannot start the driver service on http://127.0.0.1:4723/
```

**해결방법:**
- WinAppDriver.exe가 실행 중인지 확인
- 방화벽에서 차단되지 않았는지 확인
- 포트 4723이 이미 사용 중인지 확인

### 문제 2: 요소를 찾을 수 없음

```
OpenQA.Selenium.NoSuchElementException: An element could not be located
```

**해결방법:**
- Inspect.exe로 요소의 실제 이름 확인
- 암시적 대기 시간 증가
- 명시적 대기 사용

### 문제 3: 개발자 모드 비활성화

**해결방법:**
```
Windows 설정 → 개인 정보 보호 및 보안 → 개발자용 → 개발자 모드 켜기
```

---

## 요약

이 장에서는 첫 번째 UI 자동화 테스트를 작성했습니다:

- WinAppDriver 설치 및 설정
- 계산기 앱 자동화 예제
- 메모장 앱 자동화 예제
- 테스트 실행 및 디버깅 방법

다음 장에서는 UI 요소를 더 정확하게 탐색하고 식별하는 방법을 배웁니다.

---

## 참고 자료

- [WinAppDriver GitHub](https://github.com/Microsoft/WinAppDriver)
- [Appium Documentation](http://appium.io/docs/en/about-appium/intro/)
- [Selenium WebDriver](https://www.selenium.dev/documentation/webdriver/)

[◀ 이전: 제3장](../part1/chapter-03.md) | [다음: 제5장 ▶](chapter-05.md)
