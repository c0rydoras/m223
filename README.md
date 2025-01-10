# m223

Repository containing our Application for [Modul 223](https://www.modulbaukasten.ch/module/223/3/de-DE?title=Multi-User-Applikationen-objektorientiert-realisieren).

## Setup

Either copy `.env.sample` and modify it or generate a `.env` file with

```bash
just setup-dotenv
```

Then you can start the application with

```bash
just start # this just runs `docker compose up --force-recreate --build -d`
```

## Development

First of all ensure `.env` exists and has valid values.

### Starting the database

```bash
just start-db # or just run `docker compose up --force-recreate --build mariadb -d`
```

### Developing the frontend

```bash
just frontend-dev # or just run `cd frontend && npm i && npm run start`
```

The frontend can then be accessed at http://localhost:4200

### Developing the backend

```bash
just backend-dev # or just run `dotnet watch --environment Development  --project backend/Bank.Web/Bank.Web.csproj`
```

The backend can then be accessed at http://localhost:5000

## License

Code released under the [MIT License](LICENSE).
