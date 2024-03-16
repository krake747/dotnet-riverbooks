# RiverBooks

## EntityFramework

```bash
cd RiverBooks.Api
dotnet tool install -g dotnet-ef
dotnet tool update -g dotnet-ef
```

### Books Module

Sql connection string provided via `appsettings.Development.json`

```bash
dotnet ef migrations add Initial -c BookDbContext -p ..\RiverBooks.Books\RiverBooks.Books.csproj -s .\RiverBooks.Api.csproj -o Data/Migrations
dotnet ef database update
```

### Users Module

```bash
dotnet ef migrations add InitialUsers -c UsersDbContext -p ..\RiverBooks.Users\RiverBooks.Users.csproj -s .\RiverBooks.Api.csproj -o Data/Migrations
dotnet ef database update -c UsersDbContext
```

### OrderProcessing Module

```bash
dotnet ef migrations add InitialOrderProcessing -c OrderProcessingDbContext -p ../RiverBooks.OrderProcessing/RiverBooks.OrderProcessing.csproj -s .\RiverBooks.Api.csproj -o Data/Migrations 
dotnet ef database update -c OrderProcessingDbContext
```

## Testing Database

Create a new copy `appsettings.Testing.json` from the `appsettings.Development.json` and name the test database
*RiverBooks.Testing*.

```bash
dotnet ef database update -- --environment Testing
```
