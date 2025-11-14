# 제5장~제8장 핵심 요약

## 제5장. UI 요소 탐색과 식별

### 5.1 Inspect.exe 도구 활용법

**[Inspect.exe](https://learn.microsoft.com/ko-kr/windows/win32/winauto/inspect-objects)**는 Windows SDK에 포함된 UI Automation 요소 검사 도구입니다.

**위치**: `C:\Program Files (x86)\Windows Kits\10\bin\{version}\x64\inspect.exe`

**주요 기능**:
- UI 요소의 AutomationId, Name, ClassName 확인
- Control Type 및 지원하는 Pattern 확인
- UI 트리 구조 탐색

### 5.2 요소 로케이터 전략

#### AutomationId (최우선 권장)

```csharp
// WinAppDriver
driver.FindElementByAccessibilityId("btnSubmit");

// FlaUI
window.FindFirstDescendant(cf => cf.ByAutomationId("btnSubmit"));
```

#### Name

```csharp
// WinAppDriver
driver.FindElementByName("Submit");

// FlaUI
window.FindFirstDescendant(cf => cf.ByName("Submit"));
```

#### ClassName

```csharp
// WinAppDriver
driver.FindElementByClassName("Button");

// FlaUI
window.FindFirstDescendant(cf => cf.ByClassName("Button"));
```

#### XPath

```csharp
// WinAppDriver
driver.FindElementByXPath("//Button[@Name='Submit']");
```

---

## 제6장. FlaUI를 활용한 네이티브 자동화

### 6.1 FlaUI 기본 사용법

#### Application 시작

```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;

// 실행 파일로 시작
var app = Application.Launch("MyApp.exe");

// 또는 프로세스 ID로 연결
var app = Application.Attach(processId);

// UIA3 Automation 사용 (최신)
var automation = new UIA3Automation();
```

#### Window 가져오기

```csharp
// 메인 윈도우
var mainWindow = app.GetMainWindow(automation);

// 또는 조건으로 찾기
var window = automation.GetDesktop().FindFirstChild(cf =>
    cf.ByName("My Application").AndByControlType(ControlType.Window));
```

#### 요소 찾기

```csharp
// 단일 요소
var button = mainWindow.FindFirstDescendant(cf =>
    cf.ByAutomationId("btnSubmit"));

// 여러 요소
var buttons = mainWindow.FindAllDescendants(cf =>
    cf.ByControlType(ControlType.Button));

// 직계 자식만
var child = mainWindow.FindFirstChild(cf =>
    cf.ByName("Panel"));
```

### 6.2 이벤트 처리와 대기 전략

```csharp
using FlaUI.Core.Conditions;
using FlaUI.Core.Tools;

// Retry 패턴
var element = Retry.WhileNull(() =>
    window.FindFirstDescendant(cf => cf.ByAutomationId("result")),
    TimeSpan.FromSeconds(10),
    TimeSpan.FromMilliseconds(500)
).Result;

// Wait.UntilResponsive
Wait.UntilResponsive(window, TimeSpan.FromSeconds(10));

// 사용자 정의 대기
Wait.Until(() => element.IsEnabled, TimeSpan.FromSeconds(5));
```

### 6.3 FlaUI 예제 프로젝트

예제 코드: [examples/Chapter06.FlaUI](../../examples/Chapter06.FlaUI/)

```csharp
using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.UIA3;
using Xunit;

namespace FlaUI.Examples;

public class CalculatorTestsFlaUI : IDisposable
{
    private Application _app;
    private UIA3Automation _automation;
    private Window _mainWindow;

    public CalculatorTestsFlaUI()
    {
        _app = Application.LaunchStoreApp("Microsoft.WindowsCalculator_8wekyb3d8bbwe!App");
        _automation = new UIA3Automation();
        _mainWindow = _app.GetMainWindow(_automation);
    }

    [Fact]
    public void Addition_TwoPlusThree_ReturnsFive()
    {
        // Act
        _mainWindow.FindFirstDescendant(cf => cf.ByName("Two")).Click();
        _mainWindow.FindFirstDescendant(cf => cf.ByName("Plus")).Click();
        _mainWindow.FindFirstDescendant(cf => cf.ByName("Three")).Click();
        _mainWindow.FindFirstDescendant(cf => cf.ByName("Equals")).Click();

        // Assert
        var result = _mainWindow.FindFirstDescendant(cf =>
            cf.ByAutomationId("CalculatorResults")).AsLabel();
        Assert.Contains("5", result.Text);
    }

    public void Dispose()
    {
        _app?.Close();
        _automation?.Dispose();
    }
}
```

---

## 제7장. 기본 상호작용 구현

### 7.1 마우스 동작

```csharp
using FlaUI.Core.Input;

// 클릭
element.Click();

// 더블클릭
element.DoubleClick();

// 우클릭
element.RightClick();

// 드래그 앤 드롭
Mouse.DragTo(targetElement.BoundingRectangle.Center);

// 호버링
Mouse.MoveTo(element.BoundingRectangle.Center);
```

### 7.2 키보드 입력

```csharp
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;

// 텍스트 입력
textBox.Enter("Hello World");

// 단축키 조합
Keyboard.Press(VirtualKeyShort.CONTROL, VirtualKeyShort.KEY_A); // Ctrl+A
Keyboard.Type(VirtualKeyShort.RETURN); // Enter

// 특수 키
Keyboard.Type(VirtualKeyShort.TAB);
Keyboard.Type(VirtualKeyShort.ESCAPE);
```

### 7.3 컨트롤별 상호작용

#### Button

```csharp
var button = element.AsButton();
button.Invoke();
```

#### TextBox

```csharp
var textBox = element.AsTextBox();
textBox.Text = "New Value";
string currentText = textBox.Text;
```

#### CheckBox

```csharp
var checkBox = element.AsCheckBox();
checkBox.IsChecked = true;
bool isChecked = checkBox.IsChecked;
```

#### ComboBox

```csharp
var comboBox = element.AsComboBox();
comboBox.Select("Option 1");
var selectedItem = comboBox.SelectedItem;
```

#### ListView

```csharp
var listView = element.AsListBox();
var items = listView.Items;
items[0].Select();
```

---

## 제8장. Page Object Model (POM) 패턴

### 8.1 POM 패턴의 이해

Page Object Model은 UI 요소와 동작을 클래스로 캡슐화하는 디자인 패턴입니다.

**장점**:
- 코드 재사용성 향상
- 유지보수 용이
- 테스트 코드 가독성 향상
- UI 변경 시 한 곳만 수정

### 8.2 Page 클래스 설계

#### LoginPage.cs

```csharp
using FlaUI.Core.AutomationElements;
using FlaUI.Core;

namespace MyApp.UITests.Pages;

public class LoginPage
{
    private readonly Window _window;

    public LoginPage(Window window)
    {
        _window = window;
    }

    private TextBox UsernameTextBox =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("txtUsername"))
            .AsTextBox();

    private TextBox PasswordTextBox =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("txtPassword"))
            .AsTextBox();

    private Button LoginButton =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("btnLogin"))
            .AsButton();

    private Label ErrorMessage =>
        _window.FindFirstDescendant(cf => cf.ByAutomationId("lblError"))
            .AsLabel();

    public void EnterUsername(string username)
    {
        UsernameTextBox.Text = username;
    }

    public void EnterPassword(string password)
    {
        PasswordTextBox.Text = password;
    }

    public void ClickLogin()
    {
        LoginButton.Invoke();
    }

    public string GetErrorMessage()
    {
        return ErrorMessage.Text;
    }

    // Fluent Interface
    public LoginPage WithUsername(string username)
    {
        EnterUsername(username);
        return this;
    }

    public LoginPage WithPassword(string password)
    {
        EnterPassword(password);
        return this;
    }

    public void Submit()
    {
        ClickLogin();
    }
}
```

### 8.3 Fluent Interface 적용

```csharp
[Fact]
public void Login_ValidCredentials_Success()
{
    // Arrange
    var loginPage = new LoginPage(_mainWindow);

    // Act
    loginPage
        .WithUsername("user@example.com")
        .WithPassword("Password123!")
        .Submit();

    // Assert
    var mainPage = new MainPage(_mainWindow);
    Assert.True(mainPage.IsDisplayed());
}
```

### 8.4 완전한 예제

예제 코드: [examples/Chapter08.PageObjectModel](../../examples/Chapter08.PageObjectModel/)

#### 프로젝트 구조

```
Chapter08.PageObjectModel/
├─ Pages/
│   ├─ LoginPage.cs
│   ├─ MainPage.cs
│   └─ BasePage.cs
├─ Tests/
│   ├─ LoginTests.cs
│   └─ TestBase.cs
└─ PageObjectModel.Tests.csproj
```

#### BasePage.cs

```csharp
using FlaUI.Core.AutomationElements;
using FlaUI.Core;
using FlaUI.Core.Conditions;

namespace MyApp.UITests.Pages;

public abstract class BasePage
{
    protected readonly Window Window;
    protected readonly ConditionFactory Cf;

    protected BasePage(Window window)
    {
        Window = window;
        Cf = new ConditionFactory(new UIA3.UIA3PropertyLibrary());
    }

    protected AutomationElement FindElement(ConditionBase condition)
    {
        return Window.FindFirstDescendant(condition);
    }

    protected AutomationElement[] FindElements(ConditionBase condition)
    {
        return Window.FindAllDescendants(condition);
    }

    public void TakeScreenshot(string filename)
    {
        var screenshot = FlaUI.Core.Capturing.Capture.Screen();
        screenshot.ToFile($"{filename}.png");
    }
}
```

#### TestBase.cs

```csharp
using FlaUI.Core;
using FlaUI.UIA3;
using Xunit;

namespace MyApp.UITests;

public abstract class TestBase : IDisposable
{
    protected Application App { get; private set; }
    protected UIA3Automation Automation { get; private set; }
    protected Window MainWindow { get; private set; }

    protected TestBase()
    {
        App = Application.Launch("MyApp.exe");
        Automation = new UIA3Automation();
        MainWindow = App.GetMainWindow(Automation);
    }

    public virtual void Dispose()
    {
        try
        {
            App?.Close();
        }
        catch
        {
            App?.Kill();
        }
        finally
        {
            Automation?.Dispose();
        }
    }
}
```

#### LoginTests.cs

```csharp
using MyApp.UITests.Pages;
using Xunit;

namespace MyApp.UITests;

public class LoginTests : TestBase
{
    [Fact]
    public void Login_WithValidCredentials_NavigatesToMainPage()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow);

        // Act
        loginPage
            .WithUsername("admin@test.com")
            .WithPassword("Pass123!")
            .Submit();

        // Assert
        var mainPage = new MainPage(MainWindow);
        Assert.True(mainPage.IsDisplayed());
    }

    [Fact]
    public void Login_WithInvalidCredentials_ShowsError()
    {
        // Arrange
        var loginPage = new LoginPage(MainWindow);

        // Act
        loginPage
            .WithUsername("invalid@test.com")
            .WithPassword("wrong")
            .Submit();

        // Assert
        var errorMessage = loginPage.GetErrorMessage();
        Assert.Contains("Invalid credentials", errorMessage);
    }
}
```

---

## 요약

**제5장**: Inspect.exe 도구와 다양한 로케이터 전략 (AutomationId 권장)

**제6장**: FlaUI의 강력한 기능 - 빠른 성능, 유연한 대기 메커니즘

**제7장**: 마우스, 키보드, 다양한 컨트롤 상호작용 방법

**제8장**: Page Object Model 패턴으로 유지보수성 향상

---

## 참고 자료

- [Inspect.exe Documentation](https://learn.microsoft.com/ko-kr/windows/win32/winauto/inspect-objects)
- [FlaUI Documentation](https://github.com/FlaUI/FlaUI)
- [Page Object Pattern](https://martinfowler.com/bliki/PageObject.html)

[◀ 이전: 제4장](chapter-04.md) | [다음: 제9장~ ▶](../part3/chapters-09-to-23.md)
