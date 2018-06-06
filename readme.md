# Serilog.Sinks.Vecc.RabbitMQ
This is customizable RabbitMQ sink for Serilog.

Pretty much everything can be changed as needed by implementing the interfaces IRabbitConnectionFactory and IRabbitMessageBuilder and putthing them into the RabbitMQSink. There's even an extension method to easily take those objects.

## Building from source
This is written using Dot Net Core 2.0, targeting netstandard2.0 which allows the .NET Framework to consume it as well.

## Functional Testing/Sandbox
To use the sandbox without modifying anything you will need to setup RabbitMQ locally. The easiest way is using Docker and the following command
```
docker run -d --name rabbit-logs -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```
This will start up a fresh rabbit mq server in a docker container. You need to have Docker configured for Linux Containers. At the time of this the native Linux Containers in Docker for Windows does not work with the RabbitMQ container.

You need to the configure a `logs` virtual host with a `logevents` exchange. The type is `topic`, durability is `Durable`, auto delete is `no`, internal is `no` and no arguments. Make sure the guest user has full access to the `logs` virtual host.

You can get more information on how to configure and install RabbitMQ at http://www.rabbitmq.com/.

## Documentation
The API documentation is generated using DocFX, you can get more info on that at https://dotnet.github.io/docfx/.

## SQL example
To use the SQL example, you will want to start with the Functional Testing/Sandbox instructions.

Once you get the sandbox running and pushing messages to the exchange you will need to set up some queues. They should have Durability set to `Durable` and Auto Delete set to `No`. You need the following queues:

* `allevents`
* `debugevents`
* `verboseevents`
* `informationevents`
* `warningevents`
* `errorevents`
* `fatalevents`

From there you need to setup some bindings from the `logevents` exchange to the log message queues.

| To | Routing Key | Arguments |
|-|-|-|
| allevents | `#` | |
| debugevents | `#.Debug` | |
| verboseevents | `#.Verbose` | |
| informationevents | `#.Information` | |
| warningevents | `#.Warning` | |
| errorevents | `#.Error` | |
| fatalevents | `#.Fatal` | |

Now that RabbitMQ is configured to push events into the correct queues you need to create the database. If you don't have a locally installed SQL you can use SQL in Docker. It's easy

* For the Linux container:
    ```
    docker run -d -p 1433:1433 --name sql-logs -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Abcd1234!' microsoft/mssql-server-linux
    ```

* For the Windows Container:
    ```
    docker run -d -p 1433:1433 --name sql-logs -e sa_password=Abcd1234! -e ACCEPT_EULA=Y microsoft/mssql-server-windows-developer
    ```