# 1️⃣ .NET 8 SDK를 사용하여 빌드
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# 2️⃣ 프로젝트 파일 복사 및 복원
COPY ["surabot.csproj", "./"]
RUN dotnet restore "./surabot.csproj"

# 3️⃣ 전체 프로젝트 파일 복사 및 빌드222222222
COPY . ./
RUN dotnet publish "./surabot.csproj" -c Release -o /out /p:UseAppHost=false

# 4️⃣ 런타임 이미지를 사용하여 최적화된 컨테이너 생성
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build-env /out .

# 5️⃣ 기본 환경 변수 설정 (필요시 변경 가능)
ENV DISCORD_TOKEN="MTM0ODc1MjE4NDI3OTU2NDMwOA.GQsJj7.s9fNlMt-sza3Uh8renicLZ-zaAfozzZp2Q80RY"
ENV DISCORD_GUILD_ID="1153967439772659712"
ENV DB_CONNECTION_STRING="Server=58.229.105.96;Database=sql_chzbot;User=sql_chzbot;Password=cb3df373bfba7"

# 6️⃣ 실행 명령어 (프로그램 실행)
ENTRYPOINT ["dotnet", "surabot.dll"]
