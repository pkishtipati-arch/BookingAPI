# Use .NET SDK image for building and testing
FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

# Copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all source files
COPY . ./

# Build the project
RUN dotnet build --configuration Release --no-restore

# Create TestResults directory in container
RUN mkdir -p /app/TestResults

# Run tests and exit (container will stop after tests complete)
# --results-directory tells dotnet test where to save TRX files
# ExtentReports (from Hooks.cs) also saves to /app/TestResults
CMD ["dotnet", "test", "--configuration", "Release", "--no-build", "--results-directory", "/app/TestResults", "--logger", "trx;LogFileName=test-results.trx", "--logger", "console;verbosity=normal"]