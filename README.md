# RiverBooks

## EntityFramework

```bash
cd RiverBooks.Api
dotnet tool install -g dotnet-ef
dotnet tool update -g dotnet-ef
dotnet ef migrations add Initial -c BookDbContext -p ..\RiverBooks.Books\RiverBooks.Books.csproj -s .\RiverBooks.Api.csproj -o Data/Migrations
dotnet ef database update
```

