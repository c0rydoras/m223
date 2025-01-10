FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base

WORKDIR /app

EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY Bank.Web/Bank.Web.csproj Bank.Web/

RUN dotnet restore "Bank.Web/Bank.Web.csproj"

COPY . .

WORKDIR /src/Bank.Web

RUN dotnet build "Bank.Web.csproj" -c Release -o /app/build

FROM build AS publish

RUN dotnet publish "Bank.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final

WORKDIR /app

ENV ASPNETCORE_URLS="http://0.0.0.0:5000"

COPY --from=publish /app/publish .
COPY entrypoint.sh .

ENTRYPOINT ["./entrypoint.sh","dotnet", "Bank.Web.dll"]