![dotnet-rabbitmq-github](https://user-images.githubusercontent.com/33935506/114295409-344e1a00-9af9-11eb-87ef-96131e46fdcf.png)

# RabbitMQ for .NET Developers

A composition of RabbitMQ examples for C# .NET developers.

This project has a specific focus on demonstrating how to use _RabbitMQ_ with _C#.NET 5_. This is achieved by providing a collection of practical examples (written in C#.NET 5) that highlight the following _messaging patterns_.

- [One-Way Messaging](https://github.com/drminnaar/dotnet-rabbitmq/tree/master/Example1)
- [Competing Consumers (Worker Queues)](https://github.com/drminnaar/dotnet-rabbitmq/tree/master/Example2)
- [Publish/Subscribe](https://github.com/drminnaar/dotnet-rabbitmq/tree/master/Example3)
- [Routing](https://github.com/drminnaar/dotnet-rabbitmq/tree/master/Example4)
- [Topics](https://github.com/drminnaar/dotnet-rabbitmq/tree/master/Example5)
- [Headers](https://github.com/drminnaar/dotnet-rabbitmq/tree/master/Example6)

---

## Pre-Requisites

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

## Getting Started

From *Docker* to *Cloud*, there are a number of options that can be used to start playing with **RabbitMQ**. Try out any of the following methods to get started:

- [Docker](#docker)
- [Docker Compose](#docker-compose)
- [Local](#local)
- [CloudAMQP](#cloudamqp)

Before we start looking at RabbitMQ hosting options, I suggest installing the *RabbitMQ CLI* as it allows you to easily connect to any *RabbitMQ* server. Therefore, please view the [RabbitMQ Client](#rabbitmq-client) section to learn more

---

## RabbitMQ Client

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

### Linux Installation and Setup

The following setup instructions should work for most _Debian_ based Linux distributions (Including those running on [Windows Subsystem for Linux 2 (WSL2)]).

```bash
# ensure that Python version 3 is installed
python3 --version

# get 'rabbitmqadmin' CLI tool
sudo wget https://raw.githubusercontent.com/rabbitmq/rabbitmq-server/v3.8.14/deps/rabbitmq_management/bin/rabbitmqadmin -O /usr/local/bin/rabbitmqadmin
sudo chmod +x /usr/local/bin/rabbitmqadmin

# verify 'rabbitmqadmin' setup
rabbitmqadmin --version

# get help on 'rabbitmqadmin'
rabbitmqadmin --help
rabbitmqadmin help subcommands
```

### Windows Setup

The following setup instructions will work in Windows 10 using Powershell. The preference is to use _Powershell Version 7_. It will also be required to update the Windows 10 _Path_ environment variable. Please see [Add To Windows 10 Path Environment Variable] for more information.

```powershell
# ensure that Python version 3 is installed
python.exe --version

# get 'rabbitmqadmin' CLI tool
mkdir ~/rabbitmq-tools
Invoke-WebRequest -Uri https://raw.githubusercontent.com/rabbitmq/rabbitmq-server/v3.8.14/deps/rabbitmq_management/bin/rabbitmqadmin -OutFile ~/rabbitmq-tools/rabbitmqadmin -UseBasicParsing

# add '~/rabbitmq-tools' to your PATH environment variable. See https://www.architectryan.com/2018/03/17/add-to-the-path-on-windows-10
# try the following command to verify that '~/rabbitmq-tools' is included in the PATH environment variable
$env:Path -split ';' | findstr -i rabbitmq-tools

# verify 'rabbitmqadmin' setup
python.exe rabbitmqadmin --version

# get help on 'rabbitmqadmin'
python.exe rabbitmqadmin --help
python.exe rabbitmqadmin help subcommands
```

---

## RabbitMQ Server

### Docker

Use the _Docker CLI_ to start a new RabbitMQ container instance.

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
  rabbitmq:3.8-management-alpine

# connect to rabbitmq server
rabbitmqadmin -u admin -p password list vhosts
rabbitmqadmin -u admin -p password list exchanges
```

### Docker Compose

#### Definition

Define a RabbitMQ stack using a `docker-compose.yml` file.

```yaml
version: "3.7"
services:
  rabbit1:
    image: rabbitmq:3.8-management-alpine
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
docker-compose up -d

# stop RabbitMQ stack
docker-compose down -v

# list containers
docker-compose ps

# connect to rabbitmq server
rabbitmqadmin -u admin -p password list vhosts
rabbitmqadmin -u admin -p password list exchanges
```

### Local

Please find the install for your platform (OS) of choice.

- [RabbitMQ Releases](https://github.com/rabbitmq/rabbitmq-server/releases/tag/v3.8.14)

### CloudAMQP

This is the easiest way to get up and rinning with RabbitMQ. No installation or setup is required. All one needs to do is head over to [CloudAMQP] CloudAMQP is a cloud provider of various "products as a service" such as RabbitMQ, Apache Kafka, and ElephantSQL. To get started using 'RabbitMQ as a Service', see the following links:

- [CloudAMQP Plans]
- [CloudAMQP Free Plan]
- [CloudAMQP Signup]

### Connect Using Web Admin

TODO: Connect to CloudAMQP using Web Admin

### Connect Using CLI

```bash
# list exchanges
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm list exchanges

# create queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm declare queue name=example.messages auto_delete=false durable=false

# list queues
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm list queues

# publish message to queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm publish routing_key=example.messages payload="Hello World"

# list messages in queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm get queue=messages count=1

# purge queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm purge queue name=example.messages

# delete queue
rabbitmqadmin --host=your-custom-host.rmq.cloudamqp.com --port=443 --ssl --username=rdpnqczm --password=jUUDLyS_A1c0De-qQyigfTZDyaoRuUo6 -V rdpnqczm delete queue name=example.messages
```

---

[CloudAMQP]: https://customer.cloudamqp.com
[CloudAMQP Signup]: https://customer.cloudamqp.com/signup
[CloudAMQP Plans]: https://www.cloudamqp.com/plans.html
[CloudAMQP Free Plan]: https://customer.cloudamqp.com/instance/create?plan=lemur
[Windows Subsystem for Linux 2 (WSL2)]: https://docs.microsoft.com/en-us/windows/wsl/
[Add To Windows 10 Path Environment Variable]: https://www.architectryan.com/2018/03/17/add-to-the-path-on-windows-10/