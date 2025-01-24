# m223

Repository containing our Application for [Modul 223](https://www.modulbaukasten.ch/module/223/3/de-DE?title=Multi-User-Applikationen-objektorientiert-realisieren).
Verlangte Dokumentation [Hier](tasks.md)

## Setup

```bash
make
```

## Development

### Starting the database

```bash
make mariadb
```

### Developing the frontend

```bash
make frontend-dev
```

The frontend can then be accessed at http://localhost:4200

### Developing the backend

```bash
make backend-dev
```

The backend can then be accessed at http://localhost:5000

## License

Code released under the [MIT License](LICENSE).
