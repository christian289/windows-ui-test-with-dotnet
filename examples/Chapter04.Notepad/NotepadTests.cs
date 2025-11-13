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
