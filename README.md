# Windows UI 자동화 테스트 완전 정복 가이드

## 📚 목차 (Table of Contents)

---

## 제1부: 기초편 - UI 자동화 테스트 입문

### [📖 1장. UI 자동화 테스트 개요](docs/part1/chapter-01.md)
- 1.1 UI 자동화 테스트란?
- 1.2 수동 테스트 vs 자동화 테스트
- 1.3 UI 자동화 테스트의 필요성과 ROI
- 1.4 Windows 플랫폼의 UI 자동화 특징
- 1.5 테스트 피라미드와 UI 테스트의 위치

### [📖 2장. Windows UI 자동화 기술 스택 이해](docs/part1/chapter-02.md)
- 2.1 Microsoft UI Automation (UIA) 프레임워크
  - 2.1.1 UIA의 역사와 발전
  - 2.1.2 UIA 아키텍처 개요
  - 2.1.3 AutomationElement 이해하기
- 2.2 접근성(Accessibility)과 UI 자동화의 관계
- 2.3 주요 UI 자동화 도구 비교
  - 2.3.1 Windows Application Driver (WinAppDriver)
  - 2.3.2 FlaUI
  - 2.3.3 TestStack.White (Legacy)
  - 2.3.4 Playwright for Windows
- 2.4 도구 선택 가이드라인

### [📖 3장. 개발 환경 구성](docs/part1/chapter-03.md)
- 3.1 Visual Studio 설정
  - 3.1.1 필수 워크로드 설치
  - 3.1.2 테스트 프로젝트 템플릿
- 3.2 .NET SDK 및 런타임 설정
- 3.3 Windows SDK 설치
- 3.4 테스트 프레임워크 선택
  - 3.4.1 MSTest
  - 3.4.2 NUnit
  - 3.4.3 xUnit
- 3.5 필수 NuGet 패키지 설치

---

## 제2부: 실전편 - 기본 구현

### [📖 4장. 첫 번째 UI 자동화 테스트 작성](docs/part2/chapter-04.md)
- 4.1 WinAppDriver 시작하기
  - 4.1.1 WinAppDriver 설치 및 설정
  - 4.1.2 개발자 모드 활성화
  - 4.1.3 첫 테스트 프로젝트 생성
- 4.2 계산기 앱 자동화 예제 📂 [코드](examples/Chapter04.Calculator/)
- 4.3 메모장 자동화 예제 📂 [코드](examples/Chapter04.Notepad/)
- 4.4 테스트 실행 및 디버깅

### [📖 5-8장. 실전 핵심 기술](docs/part2/chapter-05-to-08.md)

#### 5장. UI 요소 탐색과 식별
- 5.1 Inspect.exe 도구 활용법
- 5.2 요소 로케이터 전략 (AutomationId, Name, ClassName, XPath)
- 5.3 동적 요소 처리 방법

#### 6장. FlaUI를 활용한 네이티브 자동화
- 6.1 FlaUI 아키텍처 이해
- 6.2 Application 시작과 연결
- 6.3 Window와 Control 다루기
- 6.4 이벤트 처리와 대기 전략 📂 [코드](examples/Chapter06.FlaUI/)

#### 7장. 기본 상호작용 구현
- 7.1 마우스 동작 (클릭, 드래그, 호버)
- 7.2 키보드 입력 (텍스트, 단축키, 특수 키)
- 7.3 컨트롤별 상호작용 (Button, TextBox, ComboBox, ListView 등)

---

## 제3부: 중급편 - 테스트 설계와 패턴

### [📖 8장. Page Object Model (POM) 패턴](docs/part2/chapter-05-to-08.md#제8장-page-object-model-pom-패턴)
- 8.1 POM 패턴의 이해
- 8.2 Page 클래스 설계
- 8.3 Fluent Interface 적용 📂 [코드](examples/Chapter08.PageObjectModel/)

### [📖 9-13장. 중급 테스트 기법](docs/part3/chapters-09-to-23.md#제3부-중급편---테스트-설계와-패턴)

#### 9장. 테스트 데이터 관리
- 9.1 데이터 주도 테스트 (CSV, JSON, Database)
- 9.2 Configuration 관리

#### 10장. 동기화와 대기 전략
- 10.1 암시적 대기 vs 명시적 대기
- 10.2 FluentWait 구현
- 10.3 Polling 메커니즘

#### 11장. 예외 처리와 복구 전략
- 11.1 일반적인 예외 시나리오
- 11.2 Retry 메커니즘 구현
- 11.3 스크린샷과 로그 수집

#### 12장. WPF 애플리케이션 테스트
- 12.1 WPF 컨트롤 특성 이해
- 12.2 MVVM 패턴과 테스트 전략

#### 13장. WinForms 애플리케이션 테스트
- 13.1 WinForms 컨트롤 계층 구조
- 13.2 Legacy 애플리케이션 대응

---

## 제4부: 고급편 - 엔터프라이즈 레벨

### [📖 14-19장. 엔터프라이즈 고급 기법](docs/part3/chapters-09-to-23.md#제4부-고급편---엔터프라이즈-레벨)

#### 14장. 테스트 프레임워크 아키텍처
- 14.1 레이어드 아키텍처 설계
- 14.2 의존성 주입(DI) 활용

#### 15장. 크로스 플랫폼 테스트
- 15.1 .NET MAUI 애플리케이션 테스트
- 15.2 플랫폼별 추상화 전략

#### 16장. 성능 테스트와 최적화
- 16.1 병렬 테스트 실행
- 16.2 테스트 실행 시간 단축 전략

#### 17장. CI/CD 파이프라인 통합
- 17.1 Azure DevOps 통합
- 17.2 GitHub Actions 통합
- 17.3 테스트 리포트 생성과 공유

#### 18장. 고급 시나리오
- 18.1 멀티 윈도우 애플리케이션
- 18.2 모달 다이얼로그 처리
- 18.3 파일 다이얼로그 자동화

#### 19장. 접근성 테스트 자동화
- 19.1 WCAG 가이드라인 이해
- 19.2 Axe-Windows 활용

---

## 제5부: 실무편 - 베스트 프랙티스

### [📖 20-23장. 실무 베스트 프랙티스](docs/part3/chapters-09-to-23.md#제5부-실무편---베스트-프랙티스)

#### 20장. 테스트 전략과 계획
- 20.1 테스트 범위 결정
- 20.2 Risk-based Testing
- 20.3 Smoke, Sanity, Regression 테스트 구분

#### 21장. 유지보수성 향상
- 21.1 테스트 코드 리팩토링
- 21.2 공통 유틸리티 추출
- 21.3 테스트 명명 규칙

#### 22장. 트러블슈팅 가이드
- 22.1 일반적인 문제와 해결방법
- 22.2 디버깅 기법
- 22.3 로깅 전략

#### 23장. 팀 협업과 커뮤니케이션
- 23.1 개발팀과의 협업
- 23.2 테스트 결과 커뮤니케이션
- 23.3 버그 리포팅 베스트 프랙티스

---

## 제6부: 부록

### [📖 부록. 레퍼런스 및 리소스](docs/appendix/appendix.md)

#### 부록 A. 예제 코드 목록
- Calculator 테스트 예제
- Notepad 테스트 예제
- FlaUI 예제
- Page Object Model 예제

#### 부록 B. 핵심 키워드 및 참조 링크
- UI Automation 기초 용어 및 링크
- .NET 및 Windows 플랫폼 링크
- 테스트 프레임워크 링크
- UI 자동화 도구 링크
- CI/CD 및 접근성 링크

#### 부록 C. NuGet 패키지 레퍼런스
- UI 자동화 핵심 패키지
- 유틸리티 패키지 (로깅, Retry, Assertion 등)

#### 부록 D. 체크리스트
- 테스트 환경 구성 체크리스트
- 테스트 케이스 설계 체크리스트
- 코드 리뷰 체크리스트
- 배포 전 체크리스트

#### 부록 E. 코드 스니펫
- Base Test 클래스 템플릿
- Retry Helper
- Wait Helper

#### 부록 F. 트러블슈팅 FAQ

#### 부록 G. 참고 자료
- 추천 도서
- 온라인 리소스
- 커뮤니티

---

## 📝 독자 대상
- **입문자**: 1-7장
- **중급자**: 8-13장
- **고급자**: 14-19장
- **실무자**: 20-23장

## 🎯 학습 경로
1. **QA 엔지니어**: 1→2→3→4→5→7→8→9→10→20→21→22
2. **개발자**: 1→2→3→6→8→14→15→16→17
3. **테스트 아키텍트**: 전체 내용

## 📚 예제 코드
모든 장의 예제 코드는 GitHub 저장소에서 확인 가능합니다.
- Repository: `github.com/[your-repo]/windows-ui-automation-guide`
- 각 장별 폴더 구조로 정리
- .NET 8.0 기준 작성

## 🔄 업데이트 계획
- 분기별 최신 도구 및 프레임워크 업데이트 반영
- 독자 피드백 기반 내용 보완
- 새로운 Windows 버전 대응

---

**저자 소개** | **감사의 글** | **색인**
