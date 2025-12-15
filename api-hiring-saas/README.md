#Mopp Hiring SAAS API

## Overview

This repository contains the SAAS API solution, a .NET-based microservices architecture composed of several services, including an API, Email Service, Worker Service, Notification Service, and supporting infrastructure components such as MariaDB, RabbitMQ, Elasticsearch, Kibana, and Redis.

## Solution Structure

The solution is organized as follows:

- **src**: Contains the source code for various services.
  - **Api**: Main API service.
  - **Application**: Contains application-specific logic and services.
  - **DTO**: Data Transfer Objects used across the solution.
  - **Domain**: Contains domain models and business logic.
  - **EmailService**: Service responsible for sending emails.
  - **Infrastructure**: Contains infrastructure-specific code, such as repositories.
  - **NotificationService**: Service responsible for managing notifications.
  - **Worker**: Background worker service for handling asynchronous tasks.
  
- **tests**: Contains unit and integration tests for the solution.

- **docker-compose.yml**: Docker Compose configuration file to orchestrate the deployment of the services.

- **docker-compose.override.yml**: Override configuration for Docker Compose, typically used in development.

- **SAAS.sln**: Solution file for Visual Studio.

## Prerequisites

Before deploying the solution, ensure the following prerequisites are met:

- **Docker**: Ensure Docker is installed on your machine.
- **Docker Compose**: Ensure Docker Compose is installed.
- **.NET SDK**: Required for building the solution (if not building via Docker).

## Environment Configuration

The environment variables required by the services are already set in the Docker Compose file. However, you can customize them according to your requirements:

- **MariaDB**:
  - `MARIADB_ROOT_PASSWORD`: Root password for MariaDB.
  - `MARIADB_USER`: Username for accessing the database.
  - `MARIADB_PASSWORD`: Password for the specified user.
  - `MARIADB_DATABASE`: Database name to be created.

- **RabbitMQ**:
  - `RABBITMQ_DEFAULT_USER`: Default RabbitMQ user.
  - `RABBITMQ_DEFAULT_PASS`: Password for the RabbitMQ user.
  - `RABBITMQ_NODE_PORT`: Port on which RabbitMQ will run.

- **Elasticsearch**:
  - `xpack.security.enabled`: Security configuration for Elasticsearch.
  - `discovery.type`: Discovery type for Elasticsearch (set to single-node).
  - `bootstrap.memory_lock`: Memory locking configuration.
  - `ES_JAVA_OPTS`: Java options for Elasticsearch.

- **ASP.NET Core**:
  - `ASPNETCORE_ENVIRONMENT`: Environment setting for the ASP.NET Core API.

## Build and Deployment

### Build the Solution

If you wish to build the solution locally, use the following command:

```
dotnet build SAAS.sln
```

## Build and Deploy Services

Use Docker Compose to build and deploy the services:

Clear the old containers and images
```
docker-compose down --rmi all

```

Build the new images and start the containers
```
docker-compose -p saas -f docker-compose.yml up -d --build
```

## Access the Services

- **API**: Accessible at [http://localhost:8082](http://localhost:8082).
- **Notification Service**: Accessible at [http://localhost:8083](http://localhost:8083).
- **Kibana**: Accessible at [http://localhost:5601](http://localhost:5601).
- **Elasticsearch**: Accessible at [http://localhost:9200](http://localhost:9200).
- **RabbitMQ Management**: Accessible at [http://localhost:15673](http://localhost:15673).

## Stopping the Services

To stop all running services, use the following command:

```
docker-compose down
```

## Troubleshooting

- **Port Conflicts**: Ensure that the ports defined in the `docker-compose.yml` file are not in use by other applications.
- **Logs**: Check logs for each service by using the following command:

```
docker-compose logs -f <service_name>
```

By Techup Team
