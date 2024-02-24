# Inter.PlaneWrangler

## Summary
This microservice is in charge of taking in plane frame messages and, upon 
command (the tick message), it combines the given moment's set of plane info
and supplys that via web api and rabbit message.


# Overview
A brief description of each of the processes.

## Plane Ingest

This process handles data intake for the service, in the form of a [PlaneFrameMessage](Application/Models/PlaneFrameMessage.cs).


## Plane Compiler

This process handles the compilation of plane data, prompted by the [TickMessage](Application/Models/TickMessage.cs).  The final result of this compilation is stored and published as a rabbit message.

This task is timed and its duration is reported via Metrics.

## Access

This process is a web api to expose the compiled plane information to interested parties.

# How to run

Clone this repository and run with `dotnet run --project Application/Application.csproj`

## General information

This project requires dotnet 6 sdk to run (install link [here](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)).

When running locally, I have the rabbit password replaced using the dotnet user-secrets tool. 
Please follow Microsoft's [guide](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=linux) to set the value of rabbit_pass to your configured rabbit account's password for the Applications.csproj.

This project uses the MelbergFramework nuget package, please see [my github](https://github.com/Joseph-Melberg/) for more info.

## Required Infrastructure
|Product|Details|Database Install Link|
|-|-|-|
|InfluxDB| You will need a bucket called plane_data, change the InfluxDBContext value of the ConnectionStrings section of [appsettings.json](Application/appsettings.json).| Docker installation guide for influxdb [here](https://hub.docker.com/_/influxdb).|
|Redis| Update the PlaneCacheContext value  of the ConnectionStrings section of [appsettings.json](Application/appsettings.json).| Docker installation guide for redis [here](https://github.com/bitnami/containers/blob/main/bitnami/redis/README.md).|
|RabbitMQ| The code will create the exchanges, queues, and bindings for you, just update the Rabbit:ClientDeclarations:Connections:0 details in [appsettings.json](Application/appsettings.json). Note that this will not give you access to the incoming data stream, please reach out to me if interested.  To trigger the Plane Compiler, send a message to the Clock exchange following the binding as described by the appsettings.json file.| Docker installation guide for RabbitMQ [here](https://hub.docker.com/_/rabbitmq).|


