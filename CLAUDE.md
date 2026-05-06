# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build
dotnet build

# Run the API (from repo root or HealthApp.API/)
dotnet run --project HealthApp.API

# Run tests (if any exist)
dotnet test

# EF Core migrations
dotnet ef migrations add <MigrationName> --project HealthApp.DataAccess --startup-project HealthApp.API
dotnet ef database update --project HealthApp.DataAccess --startup-project HealthApp.API
```

## Architecture

Clean architecture split across four projects:

| Project | Role |
|---|---|
| `HealthApp.Domain` | Entities, enums, `BaseEntity` — no dependencies |
| `HealthApp.DataAccess` | EF Core `DbContext`, repositories, migrations |
| `HealthApp.Business` | Services, DTOs, AutoMapper `MappingProfile` |
| `HealthApp.API` | ASP.NET Core controllers, `Program.cs` wiring |

**Data flow**: Controller → Service → Repository → DbContext → SQL Server (LocalDB)

## Key Patterns

- **Generic repository**: `GenericRepository<T>` handles base CRUD; domain-specific repositories extend it.
- **DTO boundary**: Entities never leave the service layer — controllers receive/return DTOs only.
- **Two-token auth**: Access token (60 min) + Refresh token (7 days, stored in `User.RefreshToken`). `JwtService` generates both; `AuthService` validates and rotates the refresh token.
- **AutoMapper**: Single `MappingProfile` in `HealthApp.Business`. All entity↔DTO maps are defined there.
- **BaseEntity**: All entities inherit `Id` (int PK) and `UpdatedAt` (auto-set on save).

## Tech Stack

- **.NET 9.0**, ASP.NET Core Web API
- **EF Core 9.0.9** + SQL Server LocalDB (`(localdb)\mssqllocaldb;Database=HealthAppDb`)
- **JWT Bearer** (System.IdentityModel.Tokens.Jwt 8.0.1)
- **BCrypt.Net 4.1.0** for password hashing
- **AutoMapper 16.1.1**
- **Google Gemini API** via `GeminiHealthService` (HttpClient) — endpoint: `POST /api/AI/analyze`
- CORS policy `"AllowFlutter"` is unrestricted in dev; emulator IP `10.0.2.2` is the expected Flutter client

## Domain Model Highlights

- `User` (1:1) `Person` — registration creates both; `Person` holds health data (height, weight, gender, birth date).
- `Child` (1:N) `VaccineSchedule` (1:N) `Vaccine` — `VaccineScheduleGenerator` auto-creates schedules on child creation.
- `Medicine` (1:N) `Reminder` and (1:N) `MedicineReminderTime`.
- `Reminder` references `User`, optional `Medicine`, optional `Vaccine` — FK deletes use `NoAction` to avoid cycles.
- `VaccineStatus` enum: `Pending`, `Completed`, `Missed`, `Delayed`.

## Documentation

- `docs/API_DOCUMENTATION.md` — full endpoint reference with Flutter/Dart examples (Turkish).
- `docs/LEARNING_NOTES.md` — architecture walkthrough, JWT internals, migration guide.
- `docs/TEAM_TASK_PLAN.md` — team module assignments and git workflow.
