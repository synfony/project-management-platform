# Project and Task Management Platform

A .NET 8 Clean Architecture solution for managing projects and tasks with MVC web interface and REST API.

## Architecture

```
ProjectManagement.sln
src/
  ProjectManagement.Domain/          # Entities, Interfaces, Enums
  ProjectManagement.Application/     # Services, DTOs, Interfaces
  ProjectManagement.Infrastructure/  # EF Core, Repositories, Identity, JWT
  ProjectManagement.Web/             # ASP.NET Core MVC + API Controllers
tests/
  ProjectManagement.Tests/           # xUnit + Moq + FluentAssertions
```

## Quick Start

```bash
cd src/ProjectManagement.Web
dotnet run
```

## URLs

| URL | Description |
|-----|-------------|
| http://localhost:5000 | MVC Web Application |
| http://localhost:5000/swagger | Swagger UI (REST API) |

## Test Credentials

- **Email:** admin@test.com
- **Password:** Admin123

## Features

- Project CRUD with status workflow (Draft -> Active -> Completed)
- Task management with priority levels (Low, Medium, High)
- Task reordering (up/down)
- Business rules:
  - Projects can only be activated if they have at least one task
  - Projects can only be completed if all tasks are completed
  - Task order must be unique within a project
- JWT authentication for REST API
- Cookie authentication for MVC web app
- SQLite database (auto-created on first run)
- Swagger UI for API exploration

## API Endpoints

### Auth
- `POST /api/auth/register` - Register
- `POST /api/auth/login` - Login (returns JWT token)

### Projects (requires Bearer token)
- `GET /api/projects/search` - Search/filter projects
- `POST /api/projects` - Create project
- `PUT /api/projects/{id}` - Update project
- `GET /api/projects/{id}/summary` - Get summary with task counts
- `GET /api/projects/{id}/tasks` - Get project tasks
- `PATCH /api/projects/{id}/activate` - Activate project
- `PATCH /api/projects/{id}/complete` - Complete project
- `DELETE /api/projects/{id}` - Delete project

### Tasks (requires Bearer token)
- `POST /api/tasks/{projectId}` - Create task
- `PUT /api/tasks/{id}` - Update task
- `DELETE /api/tasks/{id}` - Delete task
- `PATCH /api/tasks/{id}/complete` - Mark task complete
- `PATCH /api/tasks/{id}/reorder` - Reorder task (up/down)

## Running Tests

```bash
dotnet test
```
