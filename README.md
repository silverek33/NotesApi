# Notes API

Notes API is a RESTful backend application built with ASP.NET Core, designed for managing notes with full CRUD functionality, authentication and automated testing.

The project focuses on backend fundamentals: API design, data persistence, authentication, validation and maintainable architecture.

---

## Features

- Full CRUD operations for notes
- JWT-based authentication and authorization
- Protected endpoints for authenticated users
- Entity Framework Core for data access
- Input validation and error handling
- Unit tests with xUnit
- Automated build and test pipeline with GitHub Actions

---

## Tech stack

- ASP.NET Core Web API (.NET 8)
- C#
- Entity Framework Core
- SQL Server / LocalDB
- JWT Authentication
- xUnit (testing)
- GitHub Actions (CI)

---

## Architecture

The project follows a layered structure to separate responsibilities:

- Controllers – handle HTTP requests and responses
- Services – contain business logic
- Data – EF Core DbContext and persistence layer
- Models / DTOs – represent data structures and API contracts

This separation keeps the code maintainable and scalable.

---

## Authentication

The API uses JWT (JSON Web Tokens) for authentication.

- Users can register and log in
- Authenticated users receive a token
- Protected endpoints require a valid token in the Authorization header

Example:
```
POST /api/auth/register
POST /api/auth/login

GET /api/notes
POST /api/notes
PUT /api/notes/{id}
DELETE /api/notes/{id}
```
## Running locally

Requirements:

- .NET 8 SDK
- SQL Server or LocalDB

Clone the repository:
```
git clone https://github.com/silverek33/NotesApi.git
cd NotesApi
```
Restore dependencies:
```
dotnet restore
```
Apply migrations:
```
dotnet ef database update
```
Run the API:
```
dotnet run
```
## Testing

The project includes unit tests using xUnit.

Run tests:
```
dotnet test
```
Tests cover:
- service logic
- basic validation scenarios
- API behavior (if implemented)

---

## CI / Automation

The project includes a GitHub Actions workflow that:

- builds the project
- runs tests automatically on push

This ensures code quality and prevents breaking changes.

---

## Technical decisions

- ASP.NET Core Web API was chosen for modern backend development practices
- Entity Framework Core simplifies database access and migrations
- JWT authentication provides stateless and scalable auth mechanism
- xUnit was selected for lightweight and fast testing
- GitHub Actions ensures automated verification of changes

---

## Limitations

This project is a portfolio prototype and does not include:

- refresh tokens
- advanced error logging
- rate limiting
- full production security configuration

---

## Possible improvements

- add refresh token flow
- add integration tests
- add logging (Serilog)
- add Docker support
- add OpenAPI / Swagger documentation improvements

