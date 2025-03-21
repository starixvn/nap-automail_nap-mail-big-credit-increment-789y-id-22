FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder
WORKDIR /src

COPY ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Business ./Fanex.BigCreditIncrement789Y.Business
COPY ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service.csproj ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service.csproj

WORKDIR /src/Fanex.BigCreditIncrement789Y.Service
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n -r linux-x64

COPY ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service .

RUN dotnet publish -r linux-x64 --self-contained false -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-nap-mail-core

# Don’t run as root
RUN adduser --disabled-password \
  --home /app \
  --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

ENV TZ="America/La_Paz"
ENV COMPlus_EnableDiagnostics=0

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.BigCreditIncrement789Y.Service.dll"]
