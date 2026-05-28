# BlogPlatform

> Production-grade Blog REST API built with ASP.NET Core 9, following Clean Architecture and Domain-Driven Design principles.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-9.0-512BD4)](https://learn.microsoft.com/en-us/ef/core/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)
[![Architecture](https://img.shields.io/badge/Architecture-Clean-2ea44f)](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)

## 📖 Overview

**BlogPlatform** is a fully-featured backend REST API for a blogging system. It is designed as a reference implementation of **Clean Architecture** in the .NET ecosystem, with strict separation of concerns, dependency inversion, and a testable codebase.

The project demonstrates how to build a maintainable, scalable web API by combining:
- **Clean Architecture** for layer isolation
- **Repository & Unit of Work** patterns for data access abstraction
- **Result pattern** for explicit, exception-free error handling in the service layer
- **FluentValidation** for declarative request validation
- **AutoMapper** for object-to-object mapping
- **xUnit** for both unit and integration testing

## 🏗️ Architecture

The solution follows the dependency rule: **dependencies point inward**. Inner layers know nothing about outer layers.

```
┌─────────────────────────────────────────────────────────────┐
│                         Blog.Api                            │
│   Controllers · Middleware · DI configuration · Swagger     │
└──────────────────────────┬──────────────────────────────────┘
                           │ depends on
┌──────────────────────────▼──────────────────────────────────┐
│                    Blog.Infrastructure                      │
│   EF Core DbContext · Repositories · Migrations · Seeder    │
└──────────────────────────┬──────────────────────────────────┘
                           │ depends on
┌──────────────────────────▼──────────────────────────────────┐
│                    Blog.Application                         │
│   Services · DTOs · Validators · AutoMapper profiles        │
└──────────────────────────┬──────────────────────────────────┘
                           │ depends on
┌──────────────────────────▼──────────────────────────────────┐
│                      Blog.Domain                            │
│   Entities · Domain Exceptions · Repository Interfaces      │
│                  (no external dependencies)                 │
└─────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

| Layer | Responsibility | Key Components |
|---|---|---|
| **Domain** | Core business entities and contracts. Pure C#, no framework references. | `Post`, `Author`, `Comment`, `Tag`, `PostTag`, `IRepository<T>`, `IUnitOfWork`, `DomainException` |
| **Application** | Use-case orchestration, business workflows, validation, mapping. | `PostService`, `AuthorService`, `CommentService`, DTOs, `Result<T>`, FluentValidation validators, AutoMapper profile, `SlugHelper` |
| **Infrastructure** | External concerns: persistence, migrations, third-party integrations. | `BlogDbContext`, EF Core configurations, `PostRepository`, `AuthorRepository`, generic `Repository<T>`, `BlogDbSeeder` |
| **Api** | HTTP entry point. Translates HTTP ↔ application use-cases. | `PostsController`, `AuthorsController`, `ExceptionMiddleware`, DI wiring, Swagger setup |

## ✨ Features

### Core functionality
- ✅ Full **CRUD** for Posts, Authors, Comments
- ✅ Tag management with many-to-many relationship (`PostTag`)
- ✅ Lookup posts by **ID** or **SEO-friendly slug**
- ✅ Automatic **slug generation** from titles, with Turkish character support (`ç → c`, `ğ → g`, `ı → i`, `ö → o`, `ş → s`, `ü → u`)
- ✅ Add **comments** to posts via nested endpoint
- ✅ **Search & pagination** with `PagedResult<T>` and `SearchCriteria`
- ✅ Publish workflow with `IsPublished` flag and `PublishedAt` timestamp

### Cross-cutting concerns
- 🛡️ **Global exception handling** via custom `ExceptionMiddleware` — uncaught exceptions are converted to structured JSON error responses
- 📋 **Request validation** with FluentValidation (title length, content length, positive author IDs, tag constraints)
- 🎯 **Result pattern** (`Result<T>`) — services return success/error states explicitly, avoiding exception-driven control flow
- 🌐 **CORS** configured for cross-origin requests
- 📘 **Swagger / OpenAPI** documentation auto-generated and available in development
- 🌱 **Database seeding** with sample data on first run
- 🔁 **CancellationToken** plumbed through every async call for graceful request cancellation

## 🛠️ Tech Stack

| Category | Technology |
|---|---|
| Runtime | .NET 9 / ASP.NET Core |
| ORM | Entity Framework Core 9 |
| Database | SQL Server (LocalDB / Express / full) |
| Validation | FluentValidation 11 |
| Mapping | AutoMapper 12 |
| API Docs | Swashbuckle (Swagger) 10 |
| Testing | xUnit + `Microsoft.AspNetCore.Mvc.Testing` |
| Patterns | Clean Architecture, Repository, Unit of Work, Result, DTO |

## 📡 API Endpoints

Base URL: `/api/v1`

### Posts

| Method | Endpoint | Description | Response |
|---|---|---|---|
| `GET` | `/posts` | List all posts | `200 OK` — `PostListDto[]` |
| `GET` | `/posts/{id}` | Get a post by id | `200 OK` / `404 Not Found` |
| `GET` | `/posts/slug/{slug}` | Get a post by slug | `200 OK` / `404 Not Found` |
| `GET` | `/posts/search?...` | Search & paginate posts | `200 OK` — `PagedResult<PostListDto>` |
| `POST` | `/posts` | Create a new post | `201 Created` / `400 Bad Request` |
| `PUT` | `/posts/{id}` | Update an existing post | `204 No Content` / `400` / `404` |
| `DELETE` | `/posts/{id}` | Delete a post | `204 No Content` / `404 Not Found` |
| `POST` | `/posts/{postId}/comments` | Add a comment to a post | `201 Created` / `400` / `404` |

### Authors

| Method | Endpoint | Description | Response |
|---|---|---|---|
| `GET` | `/authors` | List all authors | `200 OK` — `AuthorDto[]` |
| `GET` | `/authors/{id}` | Get an author by id | `200 OK` / `404 Not Found` |
| `POST` | `/authors` | Create a new author | `201 Created` / `400 Bad Request` |
| `PUT` | `/authors/{id}` | Update an existing author | `204 No Content` / `400` / `404` |
| `DELETE` | `/authors/{id}` | Delete an author | `204 No Content` / `404 Not Found` |

### Example: Create a Post

```http
POST /api/v1/posts
Content-Type: application/json

{
  "title": "Getting Started with Clean Architecture",
  "content": "Clean Architecture provides a way to organize code so that...",
  "authorId": 1,
  "tags": ["architecture", "dotnet", "clean-code"],
  "isPublished": true
}
```

Response:
```http
HTTP/1.1 201 Created
Location: /api/v1/posts/42

{
  "id": 42,
  "title": "Getting Started with Clean Architecture",
  "slug": "getting-started-with-clean-architecture",
  "content": "...",
  "isPublished": true,
  "publishedAt": "2026-05-28T10:30:00Z",
  "author": { "id": 1, "name": "Furkan Bozkurt" },
  "tags": ["architecture", "dotnet", "clean-code"],
  "comments": []
}
```

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or full edition)
- A REST client (Swagger UI is included; [Postman](https://www.postman.com/) or [Insomnia](https://insomnia.rest/) optional)

### Installation

```bash
# 1) Clone the repository
git clone https://github.com/Furkan0010/BlogPlatform.git
cd BlogPlatform

# 2) Restore dependencies and build
dotnet restore
dotnet build

# 3) Apply database migrations
dotnet ef database update \
    --project src/Blog.Infrastructure \
    --startup-project src/Blog.Api

# 4) Run the API
dotnet run --project src/Blog.Api
```

The API will start on `https://localhost:{port}`. Swagger UI is available at:

```
https://localhost:{port}/swagger
```

> 💡 The port is shown in the console after `dotnet run` and is also configured in `src/Blog.Api/Properties/launchSettings.json`.

### Configuration

Connection string lives in `src/Blog.Api/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=LocalHost;Database=BlogPlatformDb;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

Adjust the `Server` value to match your local SQL Server instance (e.g. `(localdb)\\MSSQLLocalDB`, `.\\SQLEXPRESS`, etc.).

### Database Seeding

On first launch (outside the `Testing` environment), `BlogDbSeeder.SeedAsync` automatically populates the database with sample authors, posts, and tags so you can hit endpoints immediately.

## 🧪 Testing

The solution includes both unit tests and integration tests under `tests/Blog.Tests`.

```bash
# Run the entire test suite
dotnet test

# Run only unit tests
dotnet test --filter "FullyQualifiedName!~Integration"

# Run only integration tests
dotnet test --filter "FullyQualifiedName~Integration"
```

### Test Layout

| Test type | Location | What it covers |
|---|---|---|
| Unit | `tests/Blog.Tests/Services/PostServiceTests.cs` | Service-level business logic in isolation, with mocked repositories |
| Unit | `tests/Blog.Tests/Helpers/SlugHelperTests.cs` | Slug generation rules (Turkish chars, whitespace, special chars) |
| Integration | `tests/Blog.Tests/Integration/PostsApiTests.cs` | End-to-end HTTP tests using `CustomWebApplicationFactory` and an in-memory test server |

Integration tests spin up the full ASP.NET Core pipeline via `WebApplicationFactory<Program>` so behavior matches the deployed app as closely as possible.

## 📁 Project Structure

```
BlogPlatform/
├── src/
│   ├── Blog.Domain/                    # Entities and contracts (no dependencies)
│   │   ├── Entities/                   # Post, Author, Comment, Tag, PostTag, BaseEntity
│   │   ├── Interfaces/                 # IRepository<T>, IUnitOfWork, IPostRepository, ...
│   │   └── Exceptions/                 # DomainException
│   │
│   ├── Blog.Application/               # Use cases and business rules
│   │   ├── DTOs/                       # PostDto, AuthorDto, Result<T>, PagedResult<T>
│   │   ├── Services/                   # PostService, AuthorService, CommentService
│   │   ├── Validators/                 # FluentValidation rules
│   │   ├── Mappings/                   # AutoMapper profile
│   │   └── Helpers/                    # SlugHelper
│   │
│   ├── Blog.Infrastructure/            # Persistence and external concerns
│   │   ├── Persistence/
│   │   │   ├── BlogDbContext.cs
│   │   │   ├── BlogDbSeeder.cs
│   │   │   ├── Configurations/         # IEntityTypeConfiguration<T> implementations
│   │   │   └── Migrations/             # EF Core migrations
│   │   └── Repositories/               # PostRepository, AuthorRepository, Repository<T>
│   │
│   └── Blog.Api/                       # HTTP host
│       ├── Controllers/                # PostsController, AuthorsController
│       ├── Middleware/                 # ExceptionMiddleware
│       ├── Program.cs                  # DI registration & request pipeline
│       └── appsettings*.json
│
├── tests/
│   └── Blog.Tests/
│       ├── Services/                   # Unit tests
│       ├── Helpers/                    # Unit tests
│       └── Integration/                # Integration tests + WebApplicationFactory
│
├── BlogPlatform.sln
├── .gitignore
└── LICENSE
```

## 🎯 Design Decisions

A few notable choices and the reasoning behind them:

- **Result pattern over exceptions for business errors.** Services return `Result<T>` containing either a value or a list of error messages. Exceptions are reserved for truly exceptional, unrecoverable situations. This makes the happy-path and failure-path equally explicit and avoids using exceptions for control flow.
- **Repository + Unit of Work.** Even with EF Core providing a `DbContext` (which already behaves as a UoW), the abstraction is kept so the domain layer remains persistence-agnostic and easy to mock in tests.
- **Generic `Repository<T>` + specialized repositories.** Common CRUD lives in `Repository<T>`; query-specific behavior (e.g. `GetBySlug`, search filters) lives in `PostRepository` / `AuthorRepository`.
- **Slug as a first-class field.** Slugs are generated and stored at write-time rather than computed on read, giving stable, indexable URLs.
- **`CancellationToken` everywhere.** All async methods accept a token so requests cancelled by clients don't waste server resources.
- **Sensitive data logging only in development.** `EnableSensitiveDataLogging` is gated by `IsDevelopment()` to avoid leaking parameter values in production logs.

## 🗺️ Roadmap

Possible next steps for further development:
- 🔐 Authentication & authorization (JWT, role-based access)
- 📝 Soft delete with audit trail
- 🏷️ Tag-specific endpoints and filtering
- 📊 Structured logging (Serilog) and observability
- 🐳 Docker support and `docker-compose` for local SQL Server
- ⚡ Caching layer (Redis) for popular posts
- 🔄 Outbox pattern for reliable event publishing

## 📄 License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

## 👤 Author

**Furkan Bozkurt**
- GitHub: [@Furkan0010](https://github.com/Furkan0010)

---

⭐ If you found this project useful as a reference or learning resource, consider giving it a star!
