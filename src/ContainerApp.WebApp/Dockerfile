FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy everything else and build
COPY . .

# Run restore packages
RUN dotnet restore && dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Set port
# ENV ASPNETCORE_URLS="http://0.0.0.0:6000"

ENTRYPOINT ["dotnet", "ContainerApp.WebApp.dll"]