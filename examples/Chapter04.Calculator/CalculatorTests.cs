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
