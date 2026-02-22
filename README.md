![dotnet-rabbitmq-github](https://user-images.githubusercontent.com/33935506/114295409-344e1a00-9af9-11eb-87ef-96131e46fdcf.png)

# RabbitMQ for .NET Developers

A composition of RabbitMQ examples for C# .NET developers.

This project has a specific focus on demonstrating how to use _RabbitMQ_ with _C#.NET 5_. This is achieved by providing a collection of practical examples (written in C#.NET 5) that highlight the following _messaging patterns_.

- [Example 1 - One-Way Messaging](/Example1)
  
  This example demonstrates the simplest messaging pattern: sending a single message from a producer to a consumer using RabbitMQ. The producer sends a message to a queue, and the consumer receives messages from that queue. This pattern is useful for basic task distribution and decoupling of application components.

  - **Producer**: Sends a message to a named queue.
  - **Consumer**: Listens to the queue and processes incoming messages.

  The code in the [`Example1`](/Example1) folder provides a minimal implementation in C#, showing how to connect to RabbitMQ, declare a queue, publish a message, and consume messages.

  **Key Concepts:**

  - Point-to-point messaging
  - Queue declaration and usage
  - Basic message publishing and consumption
  
- [Example 2 - Competing Consumers (Worker Queues)](/Example2)
  
  This example demonstrates the _Competing Consumers_ (also known as _Worker Queues_) messaging pattern. In this pattern, multiple consumers (workers) pull messages from the same queue, allowing for parallel processing and load balancing of tasks. RabbitMQ ensures that each message is delivered to only one consumer, distributing the workload among all available workers.

  - **Producer**: Sends multiple messages (tasks) to a shared queue.
  - **Consumers (Workers)**: Multiple consumers listen to the same queue and process messages independently.

  This approach is useful for distributing time-consuming tasks among several workers, improving throughput and scalability.

  The code in the [`Example2`](/Example2) folder provides a practical implementation in C#, showing how to set up a producer that enqueues tasks and multiple consumers that process those tasks concurrently.

  **Key Concepts:**

  - Work queue pattern for task distribution
  - Multiple consumers competing for messages
  - Message acknowledgment to ensure reliability
  - Fair dispatch to prevent one worker from being overloaded
  
- [Example 3 - Publish/Subscribe](/Example3)
  
  This example demonstrates the _Publish/Subscribe_ messaging pattern using RabbitMQ's **fanout exchange**. In this pattern, a producer publishes messages to an exchange, and the exchange broadcasts those messages to all queues bound to it. Each consumer receives a copy of every message, enabling broadcast communication.

  - **Producer**: Publishes messages to a fanout exchange.
  - **Consumers**: Each consumer creates a temporary queue and binds it to the exchange, receiving all messages published.

  This approach is useful for scenarios where the same message needs to be delivered to multiple consumers, such as event broadcasting, notifications, or logging.

  The code in the [`Example3`](/Example3) folder provides a practical C# implementation, showing how to declare a fanout exchange, publish messages, and set up multiple consumers that each receive all published messages.

  **Key Concepts:**

  - Fanout exchange for broadcasting messages
  - Temporary queues for each consumer
  - Multiple consumers receiving all messages
  - Decoupling producers from consumers

- [Example 4 - Routing](/Example4)
  
  This example demonstrates the _Routing_ messaging pattern using RabbitMQ's **direct exchange**. In this pattern, a producer sends messages to a direct exchange, and the exchange routes each message to the queues whose binding key exactly matches the message's routing key. This allows for selective message delivery based on routing keys.

  - **Producer**: Publishes messages to a direct exchange with specific routing keys (e.g., "info", "warning", "error").
  - **Consumers**: Each consumer binds its queue to the exchange with one or more routing keys, receiving only messages that match those keys.

  This approach is useful for scenarios where messages need to be selectively delivered to different consumers based on message type or category, such as logging systems that separate logs by severity.

  The code in the [`Example4`](/Example4) folder provides a practical C# implementation, showing how to declare a direct exchange, publish messages with routing keys, and set up consumers that receive only the messages relevant to their binding.

  **Key Concepts:**

  - Direct exchange for selective message routing
  - Routing keys and queue bindings
  - Multiple consumers receiving messages based on routing key
  - Decoupling producers from consumers with flexible routing

- [Example 5 - Topics](/Example5)
  
  This example demonstrates the _Topics_ messaging pattern using RabbitMQ's **topic exchange**. In this pattern, messages are published to a topic exchange with a routing key, and queues can bind to the exchange using patterns with wildcards (`*` for a single word, `#` for zero or more words). This allows for flexible and powerful routing based on message topics.

  - **Producer**: Publishes messages to a topic exchange with various routing keys (e.g., "kern.critical", "user.info").
  - **Consumers**: Bind their queues to the exchange with specific patterns (e.g., "kern.*", "*.critical") to receive only messages matching those patterns.

  This approach is useful for scenarios such as logging systems, where consumers may be interested in messages from certain categories or severities.

  The code in the [`Example5`](/Example5) folder provides a practical C# implementation, showing how to declare a topic exchange, publish messages with different routing keys, and set up consumers that subscribe to messages based on topic patterns.

  **Key Concepts:**

  - Topic exchange for pattern-based routing
  - Routing keys with wildcards
  - Flexible message delivery to multiple consumers
  - Decoupling producers and consumers with advanced routing logic

- [Example 6 - Headers](/Example6)
  
  This example demonstrates the _Headers_ messaging pattern using RabbitMQ's **headers exchange**. Unlike direct or topic exchanges that use routing keys, a headers exchange routes messages based on message header attributes. Producers attach headers to messages, and consumers bind their queues to the exchange with specific header matching rules.

  - **Producer**: Publishes messages to a headers exchange, specifying custom headers (e.g., `format=pdf`, `type=report`).
  - **Consumers**: Bind their queues to the exchange with header arguments, receiving only messages whose headers match the binding criteria.

  This pattern is useful when routing decisions are better expressed as a combination of multiple attributes rather than a single routing key.

  The code in the [`Example6`](/Example6) folder provides a practical C# implementation, showing how to declare a headers exchange, publish messages with custom headers, and set up consumers that receive messages based on header matches.

  **Key Concepts:**

  - Headers exchange for attribute-based routing
  - Message headers and binding arguments
  - Flexible and complex routing logic
  - Decoupling producers and consumers with header-based rules

---

## ✅ Pre-Requisites

One should have a basic understanding of _RabbitMQ_ and its underlying protocol _AMQP_. I therefore provide a curated list of _RabbitMQ_ resources to help you on your _RabbitMQ_ journey.

### Official RabbitMQ Resources

#### Documentation

- [RabbitMQ Github](https://github.com/rabbitmq/rabbitmq-server)
- [Official Documentation](https://www.rabbitmq.com/documentation.html)
- [AMQP 0-9-1 Model Explained](https://www.rabbitmq.com/tutorials/amqp-concepts.html)

#### Tutorials

- [Getting Started](https://www.rabbitmq.com/getstarted.html)
- [Hello World](https://www.rabbitmq.com/tutorials/tutorial-one-dotnet.html)
- [Work Queues](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html)
- [Publish/Subscribe](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html)
- [Routing](https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html)
- [Topics](https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html)
- [RPC](https://www.rabbitmq.com/tutorials/tutorial-six-dotnet.html)
- [Publisher Confirms](https://www.rabbitmq.com/tutorials/tutorial-seven-dotnet.html)

### Erlang Solutions

#### Blogs

- [Blog](https://www.erlang-solutions.com/blog)
- [Understanding Topic Exchanges](https://www.erlang-solutions.com/blog/rabbit-s-anatomy-understanding-topic-exchanges.html)
- [What is RabbitMQ](https://www.erlang-solutions.com/blog/an-introduction-to-rabbitmq-what-is-rabbitmq.html)

### AMQP Resources

[_CloudAMQP_](https://www.cloudamqp.com/blog/index.html) offers an abundance of GREAT _RabbitMQ_ resources.

#### Miscallaneous

- [Free ebook, "Optimal RabbitMQ Guide"](https://www.cloudamqp.com/rabbitmq_ebook.html)
- [Blog](https://www.cloudamqp.com/blog/index.html)

#### Fundamentals Tutorials

- [Part 1: RabbitMQ for beginners - What is RabbitMQ?](https://www.cloudamqp.com/blog/2015-05-18-part1-rabbitmq-for-beginners-what-is-rabbitmq.html)
- [Part 2: RabbitMQ for beginners - Sample code](https://www.cloudamqp.com/blog/part2-rabbitmq-for-beginners_example-and-sample-code.html)
- [Part 3: The RabbitMQ Management Interface](https://www.cloudamqp.com/blog/2015-05-27-part3-rabbitmq-for-beginners_the-management-interface.html)
- [Part 4: RabbitMQ Exchanges, routing keys and bindings](https://www.cloudamqp.com/blog/2015-09-03-part4-rabbitmq-for-beginners-exchanges-routing-keys-bindings.html)

#### Best Practice Tutorials

- [Part 1: RabbitMQ Best Practices](https://www.cloudamqp.com/blog/2017-12-29-part1-rabbitmq-best-practice.html)
- [Part 2: RabbitMQ Best Practice for High Performance (High Throughput)](https://www.cloudamqp.com/blog/2018-01-08-part2-rabbitmq-best-practice-for-high-performance.html)
- [Part 3: RabbitMQ Best Practice for High Availability](https://www.cloudamqp.com/blog/2018-01-09-part3-rabbitmq-best-practice-for-high-availability.html)

---

## 🚀 Getting Started

From *Docker* to *Cloud*, there are a number of options available to start using **RabbitMQ**. Try out any of the following methods to get started:

- [Docker](#docker)
- [Docker Compose](#docker-compose)
- [Local](#local)
- [CloudAMQP](#cloudamqp)

Before we start looking at RabbitMQ hosting options, I suggest installing the *RabbitMQ CLI* as it allows you to easily connect to any *RabbitMQ* server. Therefore, please view the [RabbitMQ Client](#rabbitmq-client) section to learn more

---

## 🐰🖥️ RabbitMQ Client

In the following sections, a number of examples will use the `rabbitmqadmin` CLI tool to manage RabbitMQ. Therefore, before getting into the "server side" of RabbitMQ, please follow the setup instructions in this section to install the `rabbitmqadmin` CLI tool.

According to the official documention ([Management Command Line Tool](https://www.rabbitmq.com/management-cli.html)), the `rabbitmqadmin` tool allows for the following management tasks:

- list exchanges, queues, bindings, vhosts, users, permissions, connections and channels
- show overview information
- declare and delete exchanges, queues, bindings, vhosts, users and permissions
- publish and get messages
- close connections and purge queues
- import and export configuration

The listing above is a summarized list of what can be done with the `rabbitmqadmin` CLI tool. For more information, see the [official documentation](https://www.rabbitmq.com/management-cli.html). Also, once `rabbitmqadmin` is installed, be sure to try the following commands to get more information:

```bash
# get general help
rabbitmqadmin --help

# get help on subcommands
rabbitmqadmin help subcommands
```

> [!IMPORTANT]
> &nbsp;  
> See [**_rabbitmqadmin_** latest release here](https://github.com/rabbitmq/rabbitmqadmin-ng/releases)  
> &nbsp;  

### Linux Installation and Setup

The following setup instructions should work for most _Debian_ based Linux distributions (Including those running on [Windows Subsystem for Linux 2 (WSL2)]).

```bash
# ensure that Python version 3 is installed
python3 --version

# 1. Get latest version tag
RABBITMQADMIN_VERSION=$(curl -s "https://api.github.com/repos/rabbitmq/rabbitmqadmin-ng/releases/latest" | grep -Po '"tag_name": "\Kv[0-9.]+' | sed 's/^v//')

# 2. Download the binary (adjust architecture if needed: x86_64-unknown-linux-gnu or aarch64-unknown-linux-gnu)
sudo wget -qO /usr/local/bin/rabbitmqadmin \
  "https://github.com/rabbitmq/rabbitmqadmin-ng/releases/latest/download/rabbitmqadmin-${RABBITMQADMIN_VERSION}-x86_64-unknown-linux-gnu"

# 3. Make it executable
sudo chmod +x /usr/local/bin/rabbitmqadmin

# 4. Verify
rabbitmqadmin -u admin -p password show overview

# 5. Get help on 'rabbitmqadmin'
rabbitmqadmin --help
rabbitmqadmin help subcommands
```

### Windows Setup

```pwsh
# 1. Go to → https://github.com/rabbitmq/rabbitmqadmin-ng/releases/latest
# 2. Download the file that ends with -x86_64-pc-windows-msvc.exe (or similar naming)
# 3. Rename it to rabbitmqadmin.exe (optional but convenient)
# 4. Place it in a folder that's already on your PATH, for example:
#    - C:\Windows\System32\ (needs admin rights)
#    - or a user folder like C:\Users\YourName\bin\ and add that folder to PATH

#powershell
$newPath = "C:\Users\YourName\bin"
[Environment]::SetEnvironmentVariable("Path", "$env:Path;$newPath", "User")

# 5. Open a new Command Prompt or PowerShell and verify:
rabbitmqadmin -u admin -p password show overview

```

---

## 🐰🖧 RabbitMQ Server

> [!IMPORTANT]
> &nbsp;  
> See [RabbitMQ Server Releases](https://github.com/rabbitmq/rabbitmq-server/releases) to find the latest release.
> &nbsp;  

### Docker

Use the _Docker CLI_ to start a new RabbitMQ container instance.

> [!NOTE]
> &nbsp;  
> Find the [latest RabbitMQ Docker images here](https://hub.docker.com/_/rabbitmq).  
> &nbsp;  

```bash
# create docker network
docker network create rabbit-net

# run RabbitMQ container
docker container run \
  --detach \
  --name rabbit1 \
  --hostname rabbit1 \
  --publish 15672:15672 \
  --publish 5672:5672 \
  --env RABBITMQ_ERLANG_COOKIE='cookie_for_clustering' \
  --env RABBITMQ_DEFAULT_USER=admin \
  --env RABBITMQ_DEFAULT_PASS=password \
  --network rabbit-net \
  rabbitmq:4-management-alpine
```

#### Connect Using rabbitmqadmin

```bash
# connect to rabbitmq server
rabbitmqadmin -u admin -p password list vhosts
rabbitmqadmin -u admin -p password list exchanges
```

#### Connect Using Web RabbitMQ Manager

```powershell
# open the following url in your browser
http://localhost:15672

# enter credentials
username: admin
password: password
```

### Docker Compose

> [!NOTE]
> &nbsp;  
> Find the [latest RabbitMQ Docker images here](https://hub.docker.com/_/rabbitmq).  
> &nbsp;  

#### Definition

Define a RabbitMQ stack using a `docker-compose.yml` file.

```yaml
services:
  rabbit1:
    image: rabbitmq:4-management-alpine
    container_name: rabbit1
    hostname: rabbit1
    restart: unless-stopped
    ports:
      - 5672:5672
      - 15672:15672
    environment:
      RABBITMQ_ERLANG_COOKIE: cookie_for_clustering
      RABBITMQ_DEFAULT_USER: admin
      RABBITMQ_DEFAULT_PASS: password
    volumes:
      - rabbit1-etc:/etc/rabbitmq/
      - rabbit1-data:/var/lib/rabbitmq/
      - rabbit1-logs:/var/log/rabbitmq/
volumes:
  rabbit1-etc:
  rabbit1-data:
  rabbit1-logs:
networks:
  default:
    name: rabbit-net
```

#### Execution

Manage the RabbitMQ stack using `docker-compose` CLI tool.

```bash
# start RabbitMQ stack
docker-compose up --detach

# stop RabbitMQ stack
docker-compose down --volumes --remove-orphans

# list containers
docker-compose ps

# connect to rabbitmq server
rabbitmqadmin -u admin -p password show overview
```

#### Connect Using rabbitmqadmin

```bash
# connect to rabbitmq server
rabbitmqadmin -u admin -p password list vhosts
rabbitmqadmin -u admin -p password list exchanges
```

#### Connect Using Web RabbitMQ Manager

```powershell
# open the following url in your browser
http://localhost:15672

# enter credentials
username: admin
password: password
```

### Local

Please find the install for your platform (OS) of choice.

- [RabbitMQ Releases](https://github.com/rabbitmq/rabbitmqadmin-ng/releases)

### CloudAMQP

This is the easiest way to get up and running with RabbitMQ. No installation or setup is required. Run a free instance in the cloud using [CloudAMQP](https://www.cloudamqp.com). See the following links:

- [CloudAMQP Plans]
- [CloudAMQP Free Plan]
- [CloudAMQP Signup]

#### Connect Using CLI

> [!NOTE]
> &nbsp;
> You can find username, password, cluster (host), and vhost information via the cloudamqp web ui.  
> &nbsp;  

Access your RabbitMQ instance using `rabbitmqadmin` as follows:

```bash
# list exchanges
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME list exchanges

# create queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME declare queue name=example.messages auto_delete=false durable=false

# list queues
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME list queues

# publish message to queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME publish routing_key=example.messages payload="Hello World"

# list messages in queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME get queue=messages count=1

# purge queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME purge queue name=example.messages

# delete queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --use-tls --username=YOUR_USERNAME --password=YOUR_PASSWORD -V YOUR_USERNAME delete queue name=example.messages
```

---

## ▶️ Run Examples

### Manage RabbitMQ Server

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

### Connect to RabbitMQ Server

#### RabbitMQ Web App

Open RabbitMQ Admin app: [`http://localhost:15672`](http://localhost:15672)

#### Using the CLI
  
> [!IMPORTANT]
> - Run these commands from a Git Bash / POSIX shell (on Windows use Git Bash or WSL).
>
> - Ensure that all scripts have execute (`x`) permissions. Run `chmod +x my-script.sh` to add execute permissions.
  
```bash
# Open remote session to RabbitMQ Server container
./scripts/rabbitmqadmin.sh

rabbit1:/# rabbitmqadmin --username admin --password password show overview
```

### Run .NET Examples

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

[CloudAMQP]: https://customer.cloudamqp.com
[CloudAMQP Signup]: https://customer.cloudamqp.com/signup
[CloudAMQP Plans]: https://www.cloudamqp.com/plans.html
[CloudAMQP Free Plan]: https://customer.cloudamqp.com/instance/create?plan=lemur
[Windows Subsystem for Linux 2 (WSL2)]: https://docs.microsoft.com/en-us/windows/wsl/
[Add To Windows 10 Path Environment Variable]: https://www.architectryan.com/2018/03/17/add-to-the-path-on-windows-10/