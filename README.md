# WebApiApp

A small ASP.NET Core Web API that shows authentication and authorization in a single project, split the way a clean architecture would split them.

Passwords are hashed. Login returns a JWT. Protected routes check that token, and one route also checks the `Admin` role.

## Layout

Everything lives in `WebApiApp/`. Each folder has one job:

| Folder | Responsibility |
| --- | --- |
| `Controllers` | HTTP. Maps routes to the application service. |
| `Dto`, `Validators` | Input contracts and FluentValidation rules. |
| `IService`, `Services` | Application logic: register, login, issue a token. |
| `Entities` | Domain model (`User`, `Role`). |
| `Data` | EF Core, SQLite, and migrations. |
| `GenericResponse` | Shared success and failure envelope. |

`Program.cs` only wires those pieces together: SQLite, JWT bearer authentication, the auth service, validators, and the HTTP pipeline.

## Endpoints

| Method | Route | Who can call it |
| --- | --- | --- |
| `POST` | `/api/Auth/register` | Anyone |
| `POST` | `/api/Auth/Login` | Anyone |
| `GET` | `/api/Auth` | Any authenticated user |
| `GET` | `/api/Auth/admin` | Authenticated user with the `Admin` role |

Registration always stores `Role.User`. The admin route reads the role claim from the JWT, so a normal registration cannot call it.

## Run

Requires the .NET 10 SDK (`global.json`).

```bash
dotnet run --project WebApiApp
```

The app listens on `http://localhost:5055`.

## API documentation

API documentation uses [Scalar](https://github.com/scalar/scalar). In Development, `Program.cs` serves the OpenAPI document and the Scalar UI at [http://localhost:5055/scalar](http://localhost:5055/scalar).

The database is a local SQLite file, `WebApiAppDb`, created from the migrations in `WebApiApp/Data/Migrations`. Apply them once:

```bash
dotnet ef database update --project WebApiApp
```

## JWT signing key

`appsettings.json` only holds a placeholder. Set a real key with [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) (at least 32 characters) before you log in:

```bash
dotnet user-secrets set "AppSettings:Token" "your-long-random-signing-key" --project WebApiApp
```

Issuer and audience stay in `appsettings.json` (`MyWebApiApp`).

## Try it

Register:

```http
POST http://localhost:5055/api/Auth/register
Content-Type: application/json

{
  "name": "Ada Lovelace",
  "email": "ada@example.com",
  "password": "Password1"
}
```

Log in. The token comes back in `data`:

```http
POST http://localhost:5055/api/Auth/Login
Content-Type: application/json

{
  "email": "ada@example.com",
  "password": "Password1"
}
```

Call a protected route:

```http
GET http://localhost:5055/api/Auth
Authorization: Bearer {token}
```

The same request file is in `WebApiApp/WebApiApp.http`.
