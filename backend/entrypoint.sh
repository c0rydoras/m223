#!/usr/bin/env sh

set -eu

sed -i \
  -e "s@JWT_PRIVATE_KEY@$JWT_PRIVATE_KEY@g" \
  -e "s@SERVICE_URL@$SERVICE_URL@g" \
  -e "s/MARIADB_DATABASE/$MARIADB_DATABASE/g" \
  -e "s/MARIADB_USER/$MARIADB_USER/g" \
  -e "s@MARIADB_PASSWORD@$MARIADB_PASSWORD@g" \
  /app/appsettings.json

exec "$@"