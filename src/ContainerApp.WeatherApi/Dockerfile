FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /api

# Copy everything else and build
COPY . .

# Run restore packages
RUN dotnet restore && dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /api
COPY --from=build /api/out .

# Set port
# ENV ASPNETCORE_URLS="http://0.0.0.0:5000"

ENTRYPOINT ["dotnet", "ContainerApp.WeatherApi.dll"]