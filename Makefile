.DEFAULT_GOAL := help

.PHONY: help
help:
	@grep -hE '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) | sort -k 1,1 | awk 'BEGIN {FS = ":.*?## "}; {printf "\033[36m%-30s\033[0m %s\n", $$1, $$2}'

.PHONY: backend
backend: .env ## Run backend in container
	@docker compose up --build --force-recreate -d backend

.PHONY: backend-dev
backend-dev: ./backend/Bank.Web/appsettings.json ensure-mariadb-up ## Run backend for developement
	dotnet watch --environment Development --project backend/Bank.Web/Bank.Web.csproj

.PHONY: cli
cli: ./backend/Bank.Cli/appsettings.json ensure-mariadb-up ## Run the Cli locally
	dotnet run --project backend/Bank.Cli/Bank.Cli.csproj

.PHONY: ensure-mariadb-up
ensure-mariadb-up: .env ## Ensure mariadb container is running
	@docker compose up mariadb

.PHONY: frontend
frontend: ## Run frontend container
	@docker compose up --build --force-recreate -d frontend

.PHONY: frontend-dev
frontend-dev: ## Run frontend developement server
	@cd frontend && npm ci && npm run start

.PHONY: mariadb
mariadb: .env ## Start mariadb container
	@docker compose up --build --force-recreate -d mariadb

.PHONY: start
start: .env ## Start the application (with docker compose)
	@docker compose up --build --force-recreate -d

.env: ## Generate .env file
	@echo "Generating .env file"
	@printf '' > .env
	@echo "MARIADB_ROOT_PASSWORD=$$(head -c 24 /dev/urandom | base64 -w0)" >> .env
	@echo "MARIADB_DATABASE=bank" >> .env
	@echo "MARIADB_USER=$$(head -c 12 /dev/urandom | base64 -w0)" >> .env
	@echo "MARIADB_PASSWORD=$$(head -c 24 /dev/urandom | base64 -w0)" >> .env
	@echo "JWT_PRIVATE_KEY=$$(head -c 513 /dev/urandom | base64 -w0)" >> .env
	@echo "SERVICE_URL=http://localhost:5000" >> .env
	@if [ "$$(docker compose ps | wc -l)" -gt 1 ]; then \
		echo "Stopping previous containers"; \
		docker compose down -v -t0; \
	fi

-include .env
export

./backend/Bank.Web/appsettings.json: .env ## Build Bank.Web appsettings for local usage
	@MARIADB_HOST=localhost APPROOT=$(realpath ./backend/Bank.Web) ./backend/entrypoint.sh

./backend/Bank.Cli/appsettings.json: .env ## Build Bank.Cli appsettings for local usage
	@MARIADB_HOST=localhost APPROOT=$(realpath ./backend/Bank.Cli) ./backend/entrypoint.sh
