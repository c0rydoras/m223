default:
    @just --list

start-db:
    @docker compose up --force-recreate --build mariadb -d

frontend-dev:
    cd frontend && npm i && npm run start

backend-dev:
    dotnet watch --environment Development  --project backend/Bank.Web/Bank.Web.csproj

start:
    docker compose up --force-recreate --build -d
