FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["fsharp/SeaBooks.Api/SeaBooks.Api.fsproj", "fsharp/SeaBooks.Api/"]
RUN dotnet restore "fsharp/SeaBooks.Api/SeaBooks.Api.fsproj"
COPY . .
WORKDIR "/src/fsharp/SeaBooks.Api"
RUN dotnet build "SeaBooks.Api.fsproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SeaBooks.Api.fsproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SeaBooks.Api.dll"]
