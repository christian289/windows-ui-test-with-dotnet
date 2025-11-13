# 제3장. 개발 환경 구성

## 3.1 Visual Studio 설정

### 3.1.1 필수 워크로드 설치

**[Visual Studio](https://visualstudio.microsoft.com/ko/downloads/)** 2022 Community 이상 권장

#### 설치할 워크로드:

1. **.NET 데스크톱 개발** (.NET Desktop Development)
   - Windows Forms 및 WPF 애플리케이션
   - .NET Framework 및 .NET 8.0+

2. **.NET Core 크로스 플랫폼 개발** (.NET Core cross-platform development)
   - .NET 8.0 SDK
   - NuGet 패키지 관리자

#### 설치 방법:

```
Visual Studio Installer 실행
↓
수정 (Modify) 클릭
↓
워크로드 탭에서:
  ☑ .NET 데스크톱 개발
  ☑ .NET Core 크로스 플랫폼 개발
↓
설치 클릭
```

### 3.1.2 테스트 프로젝트 템플릿

Visual Studio에는 기본 테스트 템플릿이 포함되어 있습니다:

- **MSTest Test Project** (.NET Core)
- **NUnit Test Project** (.NET Core)
- **xUnit Test Project** (.NET Core)

---

## 3.2 .NET SDK 및 런타임 설정

### .NET 8.0 설치

**[.NET 8.0 SDK](https://dotnet.microsoft.com/ko-kr/download/dotnet/8.0)** 다운로드 및 설치

#### 설치 확인:

```bash
dotnet --version
# 출력: 8.0.x
```

#### .NET SDK 목록 확인:

```bash
dotnet --list-sdks
# 출력:
# 6.0.xxx [C:\Program Files\dotnet\sdk]
# 8.0.xxx [C:\Program Files\dotnet\sdk]
```

---

## 3.3 Windows SDK 설치

**[Windows SDK](https://developer.microsoft.com/ko-kr/windows/downloads/windows-sdk/)**는 UI Automation API 사용에 필요합니다.

Windows SDK는 Visual Studio와 함께 설치되지만, 별도 설치도 가능합니다.

#### 설치 확인:

```
C:\Program Files (x86)\Windows Kits\10\Include\{버전}\um\UIAutomationCore.h
```

---

## 3.4 테스트 프레임워크 선택

### 3.4.1 MSTest

Microsoft 공식 테스트 프레임워크

```xml
<ItemGroup>
  <PackageReference Include="MSTest.TestAdapter" Version="3.2.0" />
  <PackageReference Include="MSTest.TestFramework" Version="3.2.0" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>
```

```csharp
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class CalculatorTests
{
    [TestInitialize]
    public void Setup()
    {
        // 테스트 초기화
    }

    [TestMethod]
    public void TestAddition()
    {
        // 테스트 코드
        Assert.AreEqual(5, 2 + 3);
    }

    [TestCleanup]
    public void Cleanup()
    {
        // 테스트 정리
    }
}
```

### 3.4.2 NUnit

인기 있는 오픈소스 테스트 프레임워크

```xml
<ItemGroup>
  <PackageReference Include="NUnit" Version="4.0.1" />
  <PackageReference Include="NUnit3TestAdapter" Version="4.5.0" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>
```

```csharp
using NUnit.Framework;

[TestFixture]
public class CalculatorTests
{
    [SetUp]
    public void Setup()
    {
        // 테스트 초기화
    }

    [Test]
    public void TestAddition()
    {
        // 테스트 코드
        Assert.AreEqual(5, 2 + 3);
    }

    [TearDown]
    public void Cleanup()
    {
        // 테스트 정리
    }
}
```

### 3.4.3 xUnit

현대적인 테스트 프레임워크 (.NET Core 기본)

```xml
<ItemGroup>
  <PackageReference Include="xunit" Version="2.6.4" />
  <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
  <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
</ItemGroup>
```

```csharp
using Xunit;

public class CalculatorTests : IDisposable
{
    public CalculatorTests()
    {
        // 생성자: 테스트 초기화
    }

    [Fact]
    public void TestAddition()
    {
        // 테스트 코드
        Assert.Equal(5, 2 + 3);
    }

    [Theory]
    [InlineData(2, 3, 5)]
    [InlineData(10, 20, 30)]
    public void TestAdditionWithData(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }

    public void Dispose()
    {
        // 테스트 정리
    }
}
```

### 프레임워크 비교

| 특징 | MSTest | NUnit | xUnit |
|------|--------|-------|-------|
| **개발사** | Microsoft | Open Source | Open Source |
| **학습 곡선** | 낮음 | 낮음 | 중간 |
| **병렬 실행** | ✅ | ✅ | ✅ (기본) |
| **데이터 주도 테스트** | ✅ DataRow | ✅ TestCase | ✅ Theory |
| **VS 통합** | 완벽 | 우수 | 우수 |
| **.NET Core** | ✅ | ✅ | ✅ |
| **커뮤니티** | 중간 | 크다 | 크다 |

**권장:** xUnit (현대적 설계, .NET Core 친화적)

---

## 3.5 필수 NuGet 패키지 설치

### FlaUI 기반 프로젝트

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
    <Platforms>x64</Platforms>
  </PropertyGroup>

  <ItemGroup>
    <!-- 테스트 프레임워크 (xUnit 예제) -->
    <PackageReference Include="xunit" Version="2.6.4" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />

    <!-- FlaUI -->
    <PackageReference Include="FlaUI.Core" Version="4.0.0" />
    <PackageReference Include="FlaUI.UIA3" Version="4.0.0" />

    <!-- 유틸리티 -->
    <PackageReference Include="Serilog" Version="3.1.1" />
    <PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
  </ItemGroup>

</Project>
```

### WinAppDriver 기반 프로젝트

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsPackable>false</IsPackable>
    <IsTestProject>true</IsTestProject>
  </PropertyGroup>

  <ItemGroup>
    <!-- 테스트 프레임워크 -->
    <PackageReference Include="xunit" Version="2.6.4" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.6" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />

    <!-- WinAppDriver -->
    <PackageReference Include="Appium.WebDriver" Version="5.0.0-rc.1" />
    <PackageReference Include="Selenium.WebDriver" Version="4.16.2" />
  </ItemGroup>

</Project>
```

### 추가 유용한 패키지

```xml
<!-- 스크린샷 -->
<PackageReference Include="System.Drawing.Common" Version="8.0.1" />

<!-- 대기 및 재시도 -->
<PackageReference Include="Polly" Version="8.2.0" />

<!-- 데이터 주도 테스트 -->
<PackageReference Include="CsvHelper" Version="30.0.1" />

<!-- 로깅 -->
<PackageReference Include="Serilog" Version="3.1.1" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.1" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />

<!-- Assertion 확장 -->
<PackageReference Include="FluentAssertions" Version="6.12.0" />
```

---

## 개발자 모드 활성화 (WinAppDriver 필수)

WinAppDriver 사용 시 Windows 개발자 모드 필수

### 활성화 방법:

```
Windows 설정
↓
개인 정보 보호 및 보안
↓
개발자용
↓
개발자 모드 켜기
```

또는 PowerShell (관리자 권한):

```powershell
reg add "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock" /t REG_DWORD /f /v "AllowDevelopmentWithoutDevLicense" /d "1"
```

---

## 프로젝트 구조 권장 사항

```
MySolution/
├─ MyApp/                          # 테스트 대상 애플리케이션
│   ├─ MyApp.csproj
│   └─ ...
│
├─ MyApp.UITests/                  # UI 자동화 테스트
│   ├─ MyApp.UITests.csproj
│   ├─ Pages/                      # Page Objects
│   │   ├─ LoginPage.cs
│   │   └─ MainWindow.cs
│   ├─ Tests/                      # 테스트 클래스
│   │   ├─ LoginTests.cs
│   │   └─ CalculationTests.cs
│   ├─ Helpers/                    # 유틸리티
│   │   ├─ TestBase.cs
│   │   ├─ ScreenshotHelper.cs
│   │   └─ WaitHelper.cs
│   └─ TestData/                   # 테스트 데이터
│       ├─ users.csv
│       └─ config.json
│
└─ MyApp.sln                       # 솔루션 파일
```

---

## 요약

이 장에서는 UI 자동화 테스트를 위한 개발 환경 구성을 다루었습니다:

- Visual Studio 2022 + .NET 8.0 SDK 설치
- xUnit, NUnit, MSTest 중 선택 (xUnit 권장)
- FlaUI 또는 WinAppDriver NuGet 패키지 설치
- 개발자 모드 활성화 (WinAppDriver)
- 체계적인 프로젝트 구조

다음 장에서는 첫 번째 UI 자동화 테스트를 실제로 작성해보겠습니다.

---

## 참고 자료

- [Visual Studio Downloads](https://visualstudio.microsoft.com/ko/downloads/)
- [.NET Downloads](https://dotnet.microsoft.com/ko-kr/download)
- [xUnit Documentation](https://xunit.net/)
- [NUnit Documentation](https://nunit.org/)
- [MSTest Documentation](https://learn.microsoft.com/ko-kr/dotnet/core/testing/unit-testing-with-mstest)
- [FlaUI NuGet](https://www.nuget.org/packages/FlaUI.Core)

[◀ 이전: 제2장](chapter-02.md) | [다음: 제4장 ▶](../part2/chapter-04.md)
