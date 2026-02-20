# Quickstart

## Manage RabbitMQ Server

For the examples, RabbitMQ is hosted within a _Docker_ container.

The example code repository includes a [_`docker compose`_](./compose.yaml) file that describes the RabbitMQ stack with a reasonable set of defaults. Use _`docker compose`_ to start, stop and display information about the RabbitMQ stack as follows:

```bash
# Verify that 'docker-compose' is installed
docker compose --version

# Start RabbitMQ stack in the background
docker compose up --detach

# Verify that RabbitMQ container is running
docker compose ps

# Display RabbitMQ logs
docker compose logs

# Display and follow RabbitMQ logs
docker compose logs --tail="all" --follow

# Tear down RabbitMQ stack
# Remove named volumes declared in the `volumes`
# section of the Compose file and anonymous volumes
# attached to container
docker compose down --volumes
```

---

## Connect to RabbitMQ Server

### RabbitMQ Web App

Open RabbitMQ Admin app: [`http://localhost:15672`](http://localhost:15672)

### Using the CLI
  
> [!IMPORTANT]
> - Run these commands from a Git Bash / POSIX shell (on Windows use Git Bash or WSL).
>
> - Ensure that all scripts have execute (`x`) permissions. Run `chmod +x my-script.sh` to add execute permissions.
  
```bash
# Open remote session to RabbitMQ Server container
./scripts/rabbitmqadmin.sh

rabbit1:/# rabbitmqadmin --username admin --password password show overview
```

---

## Run .NET Examples

```bash
# Example 1
dotnet watch run --project ./Example/Rabbit.Example1.Consumer
dotnet watch run --project ./Example/Rabbit.Example1.Producer

# Example 2
dotnet watch run --project ./Example/Rabbit.Example2.Consumer
dotnet watch run --project ./Example/Rabbit.Example2.Producer

# Example 3
dotnet watch run --project ./Example/Rabbit.Example3.Consumer
dotnet watch run --project ./Example/Rabbit.Example3.Producer

# Example 4
dotnet watch run --project ./Example/Rabbit.Example4.Consumer
dotnet watch run --project ./Example/Rabbit.Example4.Producer

# Example 5
dotnet watch run --project ./Example/Rabbit.Example5.Consumer
dotnet watch run --project ./Example/Rabbit.Example5.Producer

# Example 6
dotnet watch run --project ./Example/Rabbit.Example6.Consumer
dotnet watch run --project ./Example/Rabbit.Example6.Producer
```

---
