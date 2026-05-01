# StarterApp Rental Marketplace

This is my SET09102 coursework project: a .NET MAUI peer-to-peer rental marketplace based on the provided StarterApp. The idea is a "Library of Things" app where users can list items they own, find items nearby, request rentals, manage the rental process, and leave reviews once a rental is complete.

## Features

- Login and registration using the coursework API and JWT authentication
- Browse all rental items
- View item details and reviews
- Create item listings with category, daily rate, and location
- Update own item listings
- Search for nearby items by latitude, longitude, radius, and category
- Submit rental requests
- View incoming and outgoing rental requests
- Manage rental workflow:
  - Requested
  - Approved
  - Rejected
  - Out for Rent
  - Overdue
  - Returned
  - Completed
- Submit reviews for completed rentals
- View reviews for listed items

## Technology Stack

- .NET 10
- .NET MAUI
- C#
- PostgreSQL / Entity Framework Core from the StarterApp foundation
- Shared SET09102 coursework API
- xUnit for automated testing
- GitHub Actions for CI/CD

## API

The app uses the SET09102 shared API for authentication, items, rentals, categories, nearby search, and reviews:

```text
https://set09102-api.b-davison.workers.dev
```

API documentation is available at:

```text
https://set09102-api.b-davison.workers.dev/
```

Authenticated requests use a JWT bearer token from:

```text
POST /auth/token
```

The token is stored using MAUI `SecureStorage` and sent with authenticated API requests.

## Project Structure

```text
StarterApp/
├── StarterApp/              # Main .NET MAUI application
│   ├── Views/               # XAML pages
│   ├── ViewModels/          # MVVM ViewModels
│   ├── Services/            # Business logic and API services
│   └── Repositories/        # Repository abstractions and API-backed repositories
├── StarterApp.Database/     # Shared database library from StarterApp
├── StarterApp.Migrations/   # EF Core migration project
├── StarterApp.Test/         # xUnit test project
└── .github/workflows/       # GitHub Actions workflow
```

## Architecture

The app follows an MVVM structure with repository and service layers. The main aim was to keep the UI, business rules, and API access separate so the code is easier to test and explain.

```text
View -> ViewModel -> Service/Repository -> ApiService -> Coursework API
```

Main patterns used:

- **MVVM**: XAML views bind to ViewModels using observable properties and relay commands.
- **Repository Pattern**: item, rental, and review data access is hidden behind repository interfaces.
- **Service Layer**: rental and review rules are handled in services instead of being placed directly in the ViewModels.

## Running the App

The command below builds a signed Android APK for the emulator:

```bash
dotnet build StarterApp/StarterApp.csproj -f net10.0-android -c Debug --no-restore --disable-build-servers -m:1 -p:RuntimeIdentifier=android-x64 -t:SignAndroidPackage
```

Install the APK on an Android emulator:

```bash
adb uninstall com.companyname.starterapp
adb install -r StarterApp/bin/Debug/net10.0-android/android-x64/com.companyname.starterapp-Signed.apk
```

Then launch the app from the emulator.

## Running Tests

Run the xUnit test suite:

```bash
dotnet test StarterApp.Test/StarterApp.Test.csproj
```

Run tests with coverage collection:

```bash
dotnet test StarterApp.Test/StarterApp.Test.csproj --collect:"XPlat Code Coverage"
```

The current automated tests cover:

- RentalService status transition and price calculation tests
- ReviewService validation tests
- CreateItemViewModel validation tests
- NearbyItemsViewModel validation tests

## CI/CD

GitHub Actions is configured in:

```text
.github/workflows/build.yml
```

The workflow runs on pushes and pull requests to `main`. It restores the test project, builds it, and runs the automated tests.

## Notes

This project was built from the provided StarterApp foundation and extended into a Library of Things rental marketplace for SET09102 coursework.
