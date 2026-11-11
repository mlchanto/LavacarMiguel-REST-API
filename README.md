# Lavacar Miguel – REST API

## Project Description

This is an academic project developed for the Web Programming course at Universidad Hispanoamericana.

The project consists of a REST API built with ASP.NET Core .NET 8 to manage vehicle wash reservations, customer information, and vehicle records.

## Technologies Used

- C#
- ASP.NET Core Web API (.NET 8)
- Entity Framework Core
- SQL Server
- REST APIs
- Swagger / OpenAPI
- Postman
- Visual Studio 2026

## Main Features

- Create, retrieve, update, and cancel reservations.
- Manage vehicle and owner information.
- Store and retrieve data using SQL Server.
- Implement CRUD operations through REST endpoints.
- Validate reservation dates and available time slots.
- Prevent duplicate reservations for the same time slot.
- Automatically calculate service prices based on vehicle type.
- Test API endpoints using Swagger and Postman.

## Database Structure

The database consists of three main entities:

- **Propietario:** Stores vehicle owner information.
- **Vehiculo:** Stores vehicle information and its relationship with the owner.
- **Reservacion:** Stores reservation details, including date, time, price, and status.

Entity Framework Core is used with the Code First approach to manage the database structure and relationships.

## API Endpoints

| Method | Endpoint | Description |
|---|---|---|
| GET | /api/reservaciones | Retrieve all reservations |
| GET | /api/reservaciones/{id} | Retrieve a reservation by ID |
| POST | /api/reservaciones | Create a reservation |
| PUT | /api/reservaciones/{id} | Update a reservation |
| DELETE | /api/reservaciones/{id} | Cancel a reservation |
| GET | /api/reservaciones/placa/{placa} | Search reservations by license plate |
| GET | /api/reservaciones/fecha/{fecha} | Search reservations by date |
| GET | /api/reservaciones/disponibilidad | Check available time slots |

## Project Purpose

The purpose of this project is to apply backend development concepts, relational database design, REST API implementation, business validations, and API testing using Microsoft technologies.

## Project Type

Academic Project – Web Programming

Universidad Hispanoamericana, Costa Rica.
