# OpsPilot

IT operations management platform for tracking **assets**, **incidents** and **maintenance**, with SLA monitoring, role-based access and a full audit trail. Built with ASP.NET Core / Blazor Server on .NET 10.

## Features

- **Asset management** – register, search and filter hardware (laptops, servers, network gear, etc.) with status and condition tracking.
- **Incident management** – report incidents, link them to assets, assign technicians and move them through a controlled lifecycle:
  `Open → Assigned → In progress ⇄ Pending → Resolved → Closed` (with reopen).
- **SLA policies & breach monitoring** – per-priority response/resolution targets, checked by a background service (`SlaBreachMonitor`).
- **Maintenance scheduling** – plan and track maintenance records per asset.
- **Audit log** – every important change is recorded with the acting user.
- **Role-based access** – `SuperAdmin`, `ITManager`, `Technician`, `Employee`, enforced on pages and navigation.
- **Dashboard** – live counts of assets, open and critical incidents.

## Architecture

Clean/layered architecture with dependencies pointing inward:

```
OpsPilot.Web             Blazor Server UI, Razor Pages (auth), composition root
OpsPilot.Infrastructure  EF Core (SQL Server), Identity, repositories, background services
OpsPilot.Application     Use-case services, DTOs, repository interfaces
OpsPilot.Domain          Entities, enums, business rules (no dependencies)
OpsPilot.Domain.Tests    xUnit tests for domain rules
```

Business rules such as incident status transitions live in the domain entities, so they can't be bypassed from the UI.

## Tech stack

.NET 10 · ASP.NET Core Blazor Server · EF Core + SQL Server · ASP.NET Core Identity · xUnit

## Getting started

**Prerequisites:** [.NET 10 SDK](https://dotnet.microsoft.com/download) and SQL Server LocalDB (installed with Visual Studio) or any SQL Server instance.

```bash
git clone <your-repo-url>
cd OpsPilot

# optional: point at your own SQL Server instead of LocalDB
dotnet user-secrets set "ConnectionStrings:OpsPilotDb" "<connection string>" --project OpsPilot.Web

# create your first admin account (prompts for name, email, password)
dotnet run --project OpsPilot.Web -- --create-superadmin

# run the app
dotnet run --project OpsPilot.Web
```

In the `Development` environment the app applies migrations automatically and seeds demo assets, incidents and default SLA policies (all prefixed `DEMO-`). Open `http://localhost:5006` and sign in with the admin you created.

Passwords must be at least 12 characters with upper-case, lower-case and a digit. Accounts lock for 15 minutes after 5 failed attempts.

## Running tests

```bash
dotnet test
```

## Configuration

| Setting | Purpose | Default |
| --- | --- | --- |
| `ConnectionStrings:OpsPilotDb` | SQL Server connection string | LocalDB (Development only) |
| `SlaMonitoring:IntervalSeconds` | SLA breach check interval (min 10) | `60` |

For production, supply the connection string through environment variables or a secret store, and apply migrations with `dotnet ef database update`.

## Roadmap

- Reports and trend insights
- Email/notification hooks on SLA breach
- Integration tests for the application layer
