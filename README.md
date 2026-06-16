## ⚙️ 실행 전 필수 설정 (환경 변수 및 DB 설정)

본 프로젝트는 보안을 위해 데이터베이스 연결 정보와 JWT 인증 키를 소스코드에 직접 포함하지 않고 환경 변수로 분리하여 관리하고 있습니다. 정상적인 프로젝트 실행을 위해 로컬 환경에 맞게 아래의 설정을 먼저 진행해 주시길 바랍니다.

### 1. 환경 설정 파일 세팅
프로젝트 백엔드 폴더(`TeamMatching.Web/TeamMatching.Web`)의 최상단에 있는 `appsettings.json` (또는 `.env`) 파일을 열고, 아래의 양식에 맞게 교수님의 로컬 PC(MySQL) 환경에 맞춰 값을 수정해 주세요.

<details>
<summary><b>📂 .env 파일을 사용하시는 경우 (클릭해서 열기)</b></summary>

프로젝트 루트 디렉토리에 `.env` 파일을 생성하고 아래 내용을 복사하여 붙여넣어 주세요.
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
