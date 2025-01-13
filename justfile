default:
    @just --list

start-db:
    @docker compose up --force-recreate --build mariadb -d

frontend-dev:
    cd frontend && npm i && npm run start

backend-dev:
    dotnet watch --environment Development --project backend/Bank.Web/Bank.Web.csproj

cli-dev:
    dotnet watch --environment Development --project backend/Bank.Cli/Bank.Cli.csproj

setup-dotenv:
    @if [[ -e .env ]]; then exit; fi
    echo "MARIADB_ROOT_PASSWORD=$(head -c 24 /dev/urandom | base64 -w0)" >> .env
    echo "MARIADB_DATABASE=bank" >> .env
    echo "MARIADB_USER=$(head -c 12 /dev/urandom | base64 -w0)" >> .env
    echo "MARIADB_PASSWORD=$(head -c 24 /dev/urandom | base64 -w0)" >> .env
    echo "JWT_PRIVATE_KEY=$(head -c 513 /dev/urandom | base64 -w0)" >> .env

start:
    docker compose up --force-recreate --build -d
