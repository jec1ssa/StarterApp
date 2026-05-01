# Testing Notes

The project uses xUnit for automated testing.

## Running Tests

```bash
dotnet test StarterApp.Test/StarterApp.Test.csproj
```

## Running Tests With Coverage

```bash
dotnet test StarterApp.Test/StarterApp.Test.csproj --collect:"XPlat Code Coverage"
```

## Current Test Areas

The current automated tests cover:

- `RentalService` status transitions and total price calculation
- `ReviewService` rating and item review validation
- `CreateItemViewModel` item creation validation
- `NearbyItemsViewModel` nearby search validation

## Latest Local Test Run

Latest local test run result:

```text
Total tests: 15
Failed: 0
Succeeded: 15
Skipped: 0
```

Coverage output is generated under:

```text
StarterApp.Test/TestResults/
```
