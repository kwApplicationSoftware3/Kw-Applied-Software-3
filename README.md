## ⚙️ 실행 전 필수 설정 (환경 변수 및 DB 설정)

본 프로젝트는 보안을 위해 데이터베이스 연결 정보와 JWT 인증 키를 소스코드에 직접 포함하지 않고 환경 변수로 분리하여 관리하고 있습니다. 정상적인 프로젝트 실행을 위해 로컬 환경에 맞게 아래의 설정을 먼저 진행해 주시길 바랍니다.

### 1. 환경 설정 파일 세팅
TeamMatching.Web에 `.env` 파일을 생성하고 아래 내용을 복사하여 붙여넣어 주세요.
`DB_PASSWORD` 부분에 교수님의 MySQL root 비밀번호를 입력해 주시면 됩니다.

```env
# [ MySQL 데이터베이스 연결 정보 ]


DB_CONNECTION="Server=localhost;Port=3306;Database=teammatching_db;User=root;Password=`DB_PASSWORD`;"
ACCESS_TOKEN_EXP_MIN="1440"
JWT_SECRET="YourSuperSecretKeyForJWTAuth2026!TeamMatching"
JWT_ISSUER="TeamMatchingWeb"
JWT_AUDIENCE="TeamMatchingUsers"
