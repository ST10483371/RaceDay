# RaceDay API

RaceDay is an ASP.NET Core Web API for managing races. Organizers can create races and categories, register participants, and record results. Participants can sign up, browse races, and register for them. Authentication uses JWT bearer tokens, and data is stored in SQL Server through Entity Framework Core.

## Tech stack

- .NET 9 (ASP.NET Core Web API)
- Entity Framework Core 9 with SQL Server
- JWT authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- ASP.NET Core Identity's `PasswordHasher<T>` for password hashing
- Swagger / OpenAPI for interactive API documentation

## Project structure

```
RaceDay.sln
RaceDay.API/
└── RaceDay.API/
    ├── Controllers/     API endpoints for each resource
    ├── Data/            AppDbContext and entity mappings
    ├── DTOs/            Request bodies for register and login
    ├── Models/          Entities: User, Organizer, Participant, Race, Category, RaceCategory, Registration, Result
    ├── Services/        JwtService, which issues access tokens
    ├── Properties/      launchSettings.json
    ├── Program.cs       Startup: auth, EF Core, Swagger, controllers
    ├── appsettings.json
    └── RaceDay.API.http Sample requests
```

## Getting started

### Prerequisites

- .NET 9 SDK
- SQL Server (LocalDB, SQL Server Express, or a full instance)
- Visual Studio 2022 (17.14 or later) or the `dotnet` CLI

### Configuration

The API reads three settings:

| Setting | Where it comes from | Notes |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | SQL Server connection string. Point it at your local instance. |
| `Jwt:Key` | User secrets or environment variable | Symmetric signing key for JWTs. Required, and not stored in source control. |
| `AllowedHosts` | `appsettings.json` | Defaults to `*`. |

To set the JWT key with user secrets (the project already has a `UserSecretsId`):

```bash
cd RaceDay.API/RaceDay.API
dotnet user-secrets set "Jwt:Key" "<a long random string, at least 32 characters>"
```

### Running the API

```bash
cd RaceDay.API/RaceDay.API
dotnet restore
dotnet run
```

Swagger UI is available at `/swagger` in the Development environment. Use the **Authorize** button and enter `Bearer <token>` to call protected endpoints.

## Authentication

Tokens are issued by `POST /api/Auth/login` and are valid for two hours. Each token carries the user ID, email, and role (`Organizer` or `Participant`). Tokens are validated against the issuer `RaceDay.API` and audience `RaceDay.Client`.

Passwords must be at least 8 characters.

## API endpoints

Routes follow `api/[controller]`, where the controller name is the resource name without the "Controller" suffix.

| Resource | Method | Route | Access |
| --- | --- | --- | --- |
| Auth | POST | `/api/Auth/register` | Public |
| Auth | POST | `/api/Auth/login` | Public |
| Races | GET | `/api/Races` | Public |
| Races | GET | `/api/Races/{id}` | Public |
| Races | POST | `/api/Races` | Organizer |
| Races | PUT | `/api/Races/{id}` | Organizer |
| Races | DELETE | `/api/Races/{id}` | Organizer |
| Categories | GET | `/api/Categories` | Public |
| Categories | GET | `/api/Categories/{id}` | Public |
| Categories | POST / PUT / DELETE | `/api/Categories` and `/api/Categories/{id}` | Organizer |
| RaceCategories | GET | `/api/RaceCategories` | Any signed-in user |
| RaceCategories | GET | `/api/RaceCategories/{raceId}/{categoryId}` | Any signed-in user |
| RaceCategories | POST | `/api/RaceCategories` | Organizer |
| RaceCategories | DELETE | `/api/RaceCategories/{raceId}/{categoryId}` | Organizer |
| Organizers | GET | `/api/Organizers` and `/api/Organizers/{id}` | Any signed-in user |
| Organizers | POST / PUT / DELETE | `/api/Organizers` and `/api/Organizers/{id}` | Organizer |
| Participants | GET | `/api/Participants` and `/api/Participants/{id}` | Any signed-in user |
| Participants | POST / PUT / DELETE | `/api/Participants` and `/api/Participants/{id}` | Organizer |
| Registrations | GET | `/api/Registrations` and `/api/Registrations/{id}` | Any signed-in user |
| Registrations | POST | `/api/Registrations` | Any signed-in user |
| Registrations | PUT / DELETE | `/api/Registrations/{id}` | Organizer |
| Results | GET | `/api/Results` and `/api/Results/{id}` | Any signed-in user |
| Results | POST / PUT / DELETE | `/api/Results` and `/api/Results/{id}` | Organizer |

## Data model

- **User**: login account with a hashed password, contact info (`ContactInfo` column), and user type (`UserType` column).
- **Organizer** and **Participant**: profile records linked to a user.
- **Race**: an event that participants can register for.
- **Category**: a race category, such as an age or distance group.
- **RaceCategory**: joins a race to a category. The primary key is the pair of IDs.
- **Registration**: a participant's entry into a race.
- **Result**: a recorded finishing result for a registration.

## Development notes

- The solution file references a `RaceDay.API.Tests` project. It is not in this repository yet, and unit tests will be added here.
- Keep `Jwt:Key` out of `appsettings.json`. The `.gitignore` excludes `bin/`, `obj/`, `.vs/`, and `*.user` files.
