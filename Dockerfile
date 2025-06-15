# Use the official .NET SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0.301 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["MarketPro/MarketPro.WebAPI.csproj", "MarketPro/"]
COPY ["MarketPro.Infrastructure/MarketPro.Infrastructure.csproj", "MarketPro.Infrastructure/"]
COPY ["MarketPro.Domain/MarketPro.Domain.csproj", "MarketPro.Domain/"]
COPY ["Application/MarketPro.Application.csproj", "Application/"]
COPY ["MarketPro.Persistence/MarketPro.Persistence.csproj", "MarketPro.Persistence/"]
COPY ["MarketPro.Shared/MarketPro.Shared.csproj", "MarketPro.Shared/"]
RUN dotnet restore "MarketPro/MarketPro.WebAPI.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
WORKDIR "/src/MarketPro"
RUN dotnet build "MarketPro.WebAPI.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "MarketPro.WebAPI.csproj" -c Release -o /app/publish

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0.6 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 80
EXPOSE 443

# Set the entry point
ENTRYPOINT ["dotnet", "MarketPro.WebAPI.dll"] 