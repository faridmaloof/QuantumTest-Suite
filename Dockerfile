# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Tests/QuantumTestSuite.Tests.csproj", "Tests/"]
RUN dotnet restore "Tests/QuantumTestSuite.Tests.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/Tests"
RUN dotnet build "QuantumTestSuite.Tests.csproj" -c Release -o /app/build

# Test stage
FROM build AS test
WORKDIR /src/Tests

# Install Node.js for Playwright
RUN apt-get update && \
    apt-get install -y curl && \
    curl -fsSL https://deb.nodesource.com/setup_20.x | bash - && \
    apt-get install -y nodejs

# Install Playwright browsers
RUN pwsh -c "npx playwright install --with-deps chromium firefox webkit"

# Set environment variables
ENV DOTNET_ENVIRONMENT=Production
ENV PLAYWRIGHT_HEADLESS=true
ENV RUN_UI_TESTS=true

# Entry point for running tests
ENTRYPOINT ["dotnet", "test", "--no-build", "--configuration", "Release"]
