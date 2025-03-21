# Surabot 프로젝트 서버 및 개발 환경 요약

---

## 🖥️ 서버 사양

- **운영체제 (OS)**: Rocky Linux 8.10
- **CPU**: Intel i9-14900T
- **RAM**: Samsung DDR5 44800U 128GB (32GB × 4)
- **스토리지**:
  - Samsung 970 Pro M.2 SSD 1TB × 2
  - HDD 12TB × 1

---

## 🐳 Docker 실행 환경

- Surabot 인스턴스는 **Docker 컨테이너 단위로 실행**
- 컨테이너 실행 시 환경 변수 전달:
  - `DISCORD_TOKEN`
  - `DISCORD_GUILD_ID`
  - `DB_CONNECTION_STRING`
- **컨테이너 이름 규칙**: `surabot_{guild_id}`
- 컨테이너 실행 시 **할당된 포트를 DB에 자동 저장**

---

## ⚙️ 개발 환경

- **개발 언어**: C#
- **플랫폼**: .NET 8
- **IDE**: Visual Studio 2022
- **빌드 및 배포 흐름**:
  1. `dotnet publish`
  2. `docker build`
  3. `docker run`

---

## 🗄️ 데이터베이스 / 상태 관리

- **DBMS**: MariaDB
- **기능별 테이블 예시**:
  - `chzzk_alerts`
  - `bot_settings`
  - `welcome_message_settings`
- **로그 테이블 구조**:
  - 메인: `bot_logs`
  - 아카이빙: `bot_logs_YYYY_MM` (월별 분리)
- **상태 관리**: Redis 사용 예정 (구현 여부 미정)

---

## 🤖 Surabot 구조

- 하나의 Surabot 컨테이너 = 하나의 Discord 길드 전용
- `bot_settings` 테이블에서 기능 사용 여부 판단
- 주요 서비스 예시:
  - `ChzzkNotificationBotService`
  - (추가 예정: 환영 메시지, YouTube, DM 등)

---

## 🌐 웹 연동 계획

- 웹 서버에서 특정 길드 요청 시 컨테이너 자동 실행
- Surabot 실행 완료 후, **DB 또는 웹에 상태 전달 예정**
