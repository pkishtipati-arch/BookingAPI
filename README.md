# Booking API Tests

Automated tests for the Restful-Booker API using Reqnroll, RestSharp, and NUnit.

## Requirements

- .NET 8.0 SDK
- Docker (optional)

## Getting Started

```bash
dotnet restore
dotnet build
dotnet test
```

## Running Tests

All tests:
```bash
dotnet test
```

By category:
```bash
dotnet test --filter "Category=Smoke"
dotnet test --filter "Category=Regression"
```

Specific feature:
```bash
dotnet test --filter "FullyQualifiedName~CreateBooking"
```

## Docker

Run in container:
```bash
docker-compose up --build --abort-on-container-exit
```

Results save to `./TestResults` locally. Container stops when tests finish.

```bash
docker-compose down
```

## What's Covered

- CRUD operations (create, read, update, delete)
- Partial updates (PATCH)
- Auth with valid/invalid tokens
- Error handling (bad IDs, missing fields, invalid data)
- Query filtering

## Project Layout

```
Features/           - Feature files
StepDefinitions/    - Test steps
Models/             - Request/response objects
Services/           - API clients
Helpers/            - Config and utils
```

## Config

Edit `appsettings.json` to change environments:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://restful-booker.herokuapp.com"
  },
  "Auth": {
    "Username": "admin",
    "Password": "password123"
  }
}
```

## Reports

Check `TestResults/TestReport.html` after running tests for detailed results.

## Notes

The Restful-Booker API sometimes returns 500 instead of 400 for invalid dates.

## CI/CD

GitLab CI runs tests and saves results as artifacts.
