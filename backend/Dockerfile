FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

COPY backend/*.csproj ./
RUN dotnet restore

COPY backend/. ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 5000
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PolygonApp.dll"]
