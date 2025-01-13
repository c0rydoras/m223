#!/usr/bin/env sh

set -eu

cp /$APPROOT/appsettings.template.json /$APPROOT/appsettings.json

sed -i \
  -e "s@JWT_PRIVATE_KEY@$JWT_PRIVATE_KEY@g" \
  -e "s@SERVICE_URL@$SERVICE_URL@g" \
  -e "s@MARIADB_HOST@$MARIADB_HOST@g" \
  -e "s@MARIADB_DATABASE@$MARIADB_DATABASE@g" \
  -e "s@MARIADB_USER@$MARIADB_USER@g" \
  -e "s@MARIADB_PASSWORD@$MARIADB_PASSWORD@g" \
  /$APPROOT/appsettings.json

exec "$@"