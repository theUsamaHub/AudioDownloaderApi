# ----------- BUILD STAGE -----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["AudioDownloaderApi.csproj", "./"]
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish

# ----------- RUNTIME STAGE -----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install dependencies: ffmpeg + python + yt-dlp
RUN apt-get update && \
    apt-get install -y ffmpeg python3 python3-pip && \
    pip3 install yt-dlp && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

# Verify installations (fail build if missing)
RUN ffmpeg -version && yt-dlp --version

# Copy published app
COPY --from=build /app/publish .

# Create storage directory
RUN mkdir -p /app/Storage

# Expose port
EXPOSE 8080

# Environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Run app
ENTRYPOINT ["dotnet", "AudioDownloaderApi.dll"]