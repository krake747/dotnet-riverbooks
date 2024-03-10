# RiverBooks

## EntityFramework

```bash
cd RiverBooks.Api
dotnet tool install -g dotnet-ef
dotnet tool update -g dotnet-ef
dotnet ef migrations add Initial -c BookDbContext -p ..\RiverBooks.Books\RiverBooks.Books.csproj -s .\RiverBooks.Api.csproj -o Data/Migrations
dotnet ef database update
```

## Testing Database

Create a new copy `appsettings.Testing.json` from the `appsettings.Development.json` and name the test database
*RiverBooks.Testing*.

```bash
dotnet ef database update -- --environment Testing
```
