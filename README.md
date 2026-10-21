ProgramowanieZaawansowane.Api

A REST API application built with C# and ASP.NET Core Web API.

Technologies

C#

ASP.NET Core Web API

Entity Framework Core

SQL Server

DTOs

Database migrations

Features

REST API endpoints implemented using controllers

Data models and DTOs

Database integration using Entity Framework Core

Database schema management through migrations

Project Structure

The application is organized into controllers, models, DTOs, and database configuration files.

Getting Started

Requirements

.NET SDK compatible with the project

SQL Server or SQL Server LocalDB, if required by the configured connection string

Run locally

Clone the repository.

Configure the database connection in the application settings.

Open a terminal in the project directory.

Restore dependencies:

dotnet restore

Apply database migrations if required:

dotnet ef database update

Start the application:

dotnet run

Check the terminal output for the local URL and open it in your browser.

Purpose

This project demonstrates backend development with ASP.NET Core Web API, structured application code, data transfer objects, and database integration.