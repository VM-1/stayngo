# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY Directory.Packages.props ./
COPY StayNGo.slnx ./
COPY global.json ./
COPY src/ ./src/
COPY tests/ ./tests/
RUN dotnet restore StayNGo.slnx
RUN dotnet publish src/StayNGo.Api/StayNGo.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StayNGo.Api.dll"]
