
# LinkForge

An anonymous URL shortening platform built with ASP.NET Core Web API, C#, EF Core, SQL Server, React, and Vite.

## Features

- Create short URLs
- Generate random short codes or choose custom aliases
- Validate destination URLs and aliases
- Redirect short URLs using HTTP 302
- Expire links after seven days
- Track click counts
- Handle invalid, missing, and expired links
- Unit tests using xUnit and Moq

## Architecture

- `Linkforge-backend/LinkForge.Api` — HTTP API and controllers
- `Linkforge-backend/LinkForge.Application` — business logic and interfaces
- `Linkforge-backend/LinkForge.Domain` — domain entities
- `Linkforge-backend/LinkForge.Infrastructure` — EF Core and SQL Server
- `Linkforge-backend/LinkForge.Tests` — unit tests
- `LinkForge-React-Frontend-Refactored` — React frontend

## Backend tests

From the backend directory:

```powershell
dotnet test LinkForge.Tests/LinkForge.Tests.csproj
```

## Frontend

From the frontend directory:

```powershell
npm install
npm run dev
```

## Configuration

Configure the database connection and API URLs for your local environment. Do not commit passwords, connection-string credentials, or local `.env` files.
