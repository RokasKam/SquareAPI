# SquareAPI

A .NET 9 Web API for managing points and finding squares, with PostgreSQL database, Docker support, and OpenAPI (Swagger) documentation.

## Prerequisites
- [Docker](https://www.docker.com/get-started)
- [Git](https://git-scm.com/)
- (Optional for local dev) [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

## Getting Started

### 1. Clone the Repository
```sh
git clone https://github.com/RokasKam/SquareAPI
cd SquareAPI
```

### 2. Start with Docker
This will build the API and start a PostgreSQL database automatically.
```sh
docker compose up --build
```
- API will be available at: `http://localhost:8080`
- PostgreSQL will be available at: `localhost:5432` (user: `postgres`, password: `postgres`, db: `SquaresDatabase`)

### 3. Access Swagger (API Docs)
- Open [http://localhost:8080/swagger](http://localhost:8080/swagger) in your browser.
- All endpoints are documented and testable via Swagger UI.

## API Endpoints

All endpoints are prefixed with `/api/`.

### Points
- `POST   /api/Point` — Add a new point. Body: `{ "x": int, "y": int }`
- `POST   /api/Point/import` — Bulk import points. Body: `[ { "x": int, "y": int }, ... ]`
- `DELETE /api/Point/X/{x}/Y/{y}` — Delete a point by coordinates.

### Squares
- `GET    /api/Square` — Get all squares that can be formed from the current points.

## Error Handling
- Errors are returned as JSON:
  ```json
  {
    "statusCode": 400,
    "message": "Error message here."
  }
  ```
- Common status codes: 400 (Bad Request), 404 (Not Found), 500 (Internal Server Error)

## Running Tests

You can run unit tests using the .NET CLI:
```sh
dotnet test SquareAPITest/SquareAPITest.csproj
```
Or run all tests in the solution:
```sh
dotnet test
```

## Database & Migrations
- Database is auto-created and migrations are applied on startup (see `SquareInfrastructure/Migrations`).
- No manual migration steps are needed when using Docker.

## Project Structure
- `SquareAPI/` — Main Web API project
- `SquareCore/` — Core logic, interfaces, DTOs
- `SquareDomain/` — Domain entities and exceptions
- `SquareInfrastructure/` — Data access, EF Core, migrations
- `SquareAPITest/` — Unit tests

## Notes
- Default environment is `Development`.
- Configuration files: `SquareAPI/appsettings.json`, `SquareAPI/appsettings.Development.json`
- For local dev without Docker, ensure PostgreSQL is running and matches the connection string in `appsettings.json`. 
