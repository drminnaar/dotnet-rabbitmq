#!/bin/bash
set -euo pipefail

DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$DIR/../compose.yaml"

echo -e "\n"

docker compose --file "$COMPOSE_FILE" exec rabbit1 sh -c \
  'printf "export RABBITMQ_USER=$RABBITMQ_DEFAULT_USER\nexport RABBITMQ_PASSWORD=$RABBITMQ_DEFAULT_PASS\nalias ra=\"rabbitmqadmin -u admin -p password\"\n" > /tmp/radminrc && exec bash --rcfile /tmp/radminrc -i'
