## ⚙️ 실행 전 필수 설정 (환경 변수 및 DB 설정)

본 프로젝트는 보안을 위해 데이터베이스 연결 정보와 JWT 인증 키를 소스코드에 직접 포함하지 않고 환경 변수로 분리하여 관리하고 있습니다. 정상적인 프로젝트 실행을 위해 로컬 환경에 맞게 아래의 설정을 먼저 진행해 주시길 바랍니다.

### 1. 환경 설정 파일 세팅
프로젝트 백엔드 폴더(`TeamMatching.Web/TeamMatching.Web`)의 최상단에 있는 `appsettings.json` (또는 `.env`) 파일을 열고, 아래의 양식에 맞게 교수님의 로컬 PC(MySQL) 환경에 맞춰 값을 수정해 주세요.

<details>
<summary><b>📂 .env 파일을 사용하시는 경우 (클릭해서 열기)</b></summary>

TeamMatching.Web에 `.env` 파일을 생성하고 아래 내용을 복사하여 붙여넣어 주세요.
`DB_PASSWORD` 부분에 교수님의 MySQL root 비밀번호를 입력해 주시면 됩니다.

```env
# [ MySQL 데이터베이스 연결 정보 ]
DB_SERVER=localhost
DB_PORT=3306
DB_USER=root
DB_PASSWORD=여기에_교수님의_MySQL_비밀번호를_입력해주세요
DB_NAME=teammatching_db

# [ JWT 토큰 인증 키 ]
# (토큰 암호화에 사용되는 임의의 문자열입니다. 기본값을 그대로 사용하셔도 무방합니다.)
JWT_SECRET_KEY=TeamMatchingSuperSecretKey2026KwUniv!
JWT_ISSUER=TeamMatchingApp
JWT_AUDIENCE=TeamMatchingAppUsers


# 🚀 팀찾사 (팀원을 찾는 사람들)
**응용소프트웨어실습 3조 - 교내 팀·스터디 매칭 웹 플랫폼**

기존 교내 팀 매칭의 파편화된 정보와 소통 부재, 그리고 '프리라이더(무임승차)' 문제를 해결하기 위해 기획된 **올인원 팀 매칭 및 협업 플랫폼**입니다. 사용자는 태그 기반으로 원하는 프로젝트를 찾고, 팀 결성 후 전용 공간에서 협업하며, 프로젝트 종료 후 상호 평가를 통해 신뢰도를 쌓을 수 있습니다.

<br>

## 👥 팀원 소개 (3조)
* **문인호** (2022203002)
* **강은우** (2022203031)
* **김건우** (2022203050)

<br>

## 🛠 기술 스택
* **Frontend:** C# Blazor WebAssembly
* **Backend:** ASP.NET Core Web API
* **Database:** MySQL (Entity Framework Core)
* **Security:** JWT (Bearer Token), BCrypt Password Hashing
* **IDE:** Visual Studio 2022

<br>

## 🌟 주요 기능
1. **맞춤형 매칭 및 필터링:** 기술 스택(태그) 기반 검색으로 원하는 스터디/프로젝트 탐색 및 지원
2. **팀 메인 (협업 공간):** 팀 결성 시 제공되는 전용 게시판, 역할 배분 및 댓글 기능을 통한 협업
3. **상호 평가 시스템:** 프로젝트 종료 후 팀원 간 평가를 진행하여 '신뢰도, 기여도, 의사소통' 점수를 프로필에 반영 (프리라이더 방지)
4. **내 활동 관리:** 지원 내역, 참여 중인 프로젝트, 작성한 모집글 관리 및 프로필 수정

<br>
