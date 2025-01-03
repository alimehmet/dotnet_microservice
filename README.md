# DornetProject1

## Overview

DornetProject1 is a .NET-based solution that includes three main components: BusinessAPI, CurrencyWebSite, and DataAPI. This project is designed to provide a comprehensive web application for managing and displaying currency exchange rates.

## Project Structure

- **BusinessAPI**: Handles business logic and operations.
- **CurrencyWebSite**: The front-end web application for users to interact with.
- **DataAPI**: Manages data operations and interactions with the database.

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker](https://www.docker.com/get-started)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Redis](https://redis.io/download)

## Getting Started

### Clone the Repository

git clone https://github.com/alimehmet/dotnet_microservice.git
cd dotnet_project

### Build and Run with Docker
### Build and run the Docker containers:

docker-compose up --build

### The services will be available at the following ports:

CurrencyWebSite: http://localhost:5000
BusinessAPI: http://localhost:5002
DataAPI: http://localhost:5001
PostgreSQL: localhost:5432
Redis: localhost:6379

### The services will be available at the following ports to sync currency data from TCMB:

http://localhost:5001/api/exchangerate/sync

### Configuration
### Configuration files are located in the respective project directories:
appsettings.json
appsettings.Development.json

### Development
### Open the Solution in Visual Studio
Open the DornetProject1.sln solution file in Visual Studio.

### Run the Projects
### You can run the projects individually using Visual Studio or the .NET CLI:

dotnet run --project BusinessAPI/BusinessAPI.csproj
dotnet run --project CurrencyWebSite/CurrencyWebSite.csproj
dotnet run --project DataAPI/DataAPI.csproj

### Database Migrations
### To apply database migrations, use the following commands:

Navigate to the DataAPI project directory:

Apply the migrations:

