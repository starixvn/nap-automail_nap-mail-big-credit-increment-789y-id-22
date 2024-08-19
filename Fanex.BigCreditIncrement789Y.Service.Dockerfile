FROM hub.nexdev.net/base/dotnet/core/sdk:5.0.102-ca-patch-buster-slim AS builder
WORKDIR /src

COPY ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Business ./Fanex.BigCreditIncrement789Y.Business
COPY ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service.csproj ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service.csproj

RUN ls /src -l
WORKDIR /src/Fanex.BigCreditIncrement789Y.Service
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN dotnet restore -v n

COPY ./Fanex.BigCreditIncrement789Y.Service/Fanex.BigCreditIncrement789Y.Service .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:5.0-nap-mail-core


ENV TZ="America/La_Paz" 

WORKDIR /app
COPY --from=builder /app .

ENTRYPOINT ["dotnet", "Fanex.BigCreditIncrement789Y.Service.dll"]
