# Stage 1: Build & Publish with .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Cache dependency restoration layer
COPY ["Hl7FhirWorkbench.csproj", "./"]
RUN dotnet restore "Hl7FhirWorkbench.csproj"

# Copy source and publish release binaries
COPY . .
RUN dotnet publish "Hl7FhirWorkbench.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 2: Minimal Runtime with .NET 10 ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 8080

ENTRYPOINT ["dotnet", "Hl7FhirWorkbench.dll"]
