FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["CatalogDotnet5.API.csproj", "./"]
RUN dotnet restore "CatalogDotnet5.API.csproj"
COPY . .
RUN dotnet publish "CatalogDotnet5.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CatalogDotnet5.API.dll"]
