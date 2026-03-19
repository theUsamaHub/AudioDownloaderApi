# Audio Downloader API

A .NET 8 Web API for downloading audio from video URLs and converting them to MP3 format. This API uses yt-dlp for video/audio extraction and FFmpeg for audio conversion.

## 📋 Table of Contents

- [Project Overview](#project-overview)
- [Features](#features)
- [Project Structure](#project-structure)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Usage Examples](#usage-examples)
- [Services Overview](#services-overview)
- [File Management](#file-management)
- [Rate Limiting & Concurrency](#rate-limiting--concurrency)
- [Error Handling](#error-handling)
- [Development](#development)

## 🎯 Project Overview

The Audio Downloader API is a RESTful web service that allows users to download audio content from various video platforms (YouTube, etc.) and convert them to MP3 format. The API includes built-in rate limiting, file size restrictions, duration limits, and automatic cleanup of temporary files.

## ✨ Features

- **Audio Download**: Extract audio from video URLs using yt-dlp
- **Format Conversion**: Convert audio to MP3 format using FFmpeg
- **Queue Management**: Background processing with configurable concurrency limits
- **File Validation**: Size and duration restrictions for security and resource management
- **Automatic Cleanup**: Scheduled removal of temporary files after 15 minutes
- **Rate Limiting**: Concurrent download limit to prevent server overload
- **Swagger Documentation**: Interactive API documentation
- **Error Handling**: Comprehensive error responses and validation

## 📁 Project Structure

```
AudioDownloaderApi/
├── Controllers/
│   ├── AudioController.cs          # Main API endpoints for audio operations
│   ├── TestController.cs          # Testing endpoints
│   └── WeatherForecastController.cs # Sample weather API
├── Models/
│   ├── DownloadAudioRequest.cs     # Request model for audio downloads
│   └── DownloadJob.cs             # Job model for queue processing
├── Services/
│   ├── ProcessService.cs          # Core download and conversion logic
│   ├── DownloadQueueService.cs    # Background queue processing service
│   ├── DownloadLimiterService.cs  # Rate limiting service
│   └── FileCleanupService.cs      # Automatic file cleanup service
├── Helpers/                       # Utility functions (empty folder, prepared for future use)
├── Storage/                       # Temporary file storage directory
├── Properties/
│   └── launchSettings.json        # Development configuration
├── wwwroot/                       # Static files (if needed)
├── appsettings.json              # Application configuration
├── appsettings.Development.json  # Development-specific settings
├── Program.cs                     # Application entry point and service configuration
├── AudioDownloaderApi.csproj      # Project file and dependencies
└── AudioDownloaderApi.http        # HTTP testing file
```

## 🛠 Technology Stack

### Core Framework
- **.NET 8.0** - Latest .NET framework with improved performance and features
- **ASP.NET Core Web API** - RESTful API framework

### External Dependencies
- **Swashbuckle.AspNetCore v6.6.2** - Swagger/OpenAPI documentation generation

### External Tools (Required)
- **yt-dlp** - Video/audio downloader (must be installed system-wide)
- **FFmpeg** - Multimedia conversion tool (must be installed system-wide)

### Development Tools
- **Visual Studio 2022** or **VS Code** with .NET SDK
- **.NET 8.0 SDK** - Development and runtime environment

## 📦 Prerequisites

### System Requirements
- **Operating System**: Windows, Linux, or macOS
- **.NET 8.0 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Git** - For version control (optional)

### External Tools Installation

#### 1. yt-dlp Installation
```bash
# Using pip (recommended)
pip install yt-dlp

# Or download directly
# Visit: https://github.com/yt-dlp/yt-dlp/releases
```

#### 2. FFmpeg Installation
```bash
# Windows (using Chocolatey)
choco install ffmpeg

# Windows (using Scoop)
scoop install ffmpeg

# macOS (using Homebrew)
brew install ffmpeg

# Ubuntu/Debian
sudo apt update && sudo apt install ffmpeg

# CentOS/RHEL
sudo yum install ffmpeg
```

### Verification
After installation, verify tools are available:
```bash
yt-dlp --version
ffmpeg -version
dotnet --version
```

## 🚀 Installation

1. **Clone the Repository**
   ```bash
   git clone <repository-url>
   cd AudioDownloaderApi
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build the Project**
   ```bash
   dotnet build
   ```

4. **Run the Application**
   ```bash
   dotnet run
   ```

The API will be available at `https://localhost:7000` or `http://localhost:5052` (development).

## ⚙️ Configuration

### Application Settings
The application uses standard ASP.NET Core configuration:

**appsettings.json**
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Default Limits
- **Maximum File Size**: 50MB
- **Maximum Duration**: 15 minutes (900 seconds)
- **Concurrent Downloads**: 3
- **File Cleanup**: Files deleted after 15 minutes
- **Cleanup Interval**: Every 5 minutes

## 🔌 API Endpoints

### Audio Download
**POST** `/api/audio/download`

Downloads audio from a video URL and converts it to MP3 format.

#### Request Body
```json
{
  "url": "https://www.youtube.com/watch?v=VIDEO_ID",
  "maxFileSizeMB": 50,        // Optional: Default 50
  "maxDurationMinutes": 15     // Optional: Default 15
}
```

#### Response
- **Success**: Returns MP3 file as binary data
- **Error**: Returns error message with status code 400

#### Validation Rules
- URL must be a valid, accessible video URL
- File size cannot exceed 50MB
- Video duration cannot exceed 15 minutes

### Weather Forecast (Sample)
**GET** `/weatherforecast/`

Returns sample weather data (included as template).

## 📖 Usage Examples

### cURL Example
```bash
curl -X POST "https://localhost:7000/api/audio/download" \
     -H "Content-Type: application/json" \
     -d '{
       "url": "https://www.youtube.com/watch?v=dQw4w9WgXcQ",
       "maxFileSizeMB": 50,
       "maxDurationMinutes": 15
     }' \
     --output downloaded_audio.mp3
```

### JavaScript Example
```javascript
const response = await fetch('/api/audio/download', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    url: 'https://www.youtube.com/watch?v=VIDEO_ID',
    maxFileSizeMB: 50,
    maxDurationMinutes: 15
  })
});

if (response.ok) {
  const blob = await response.blob();
  const url = window.URL.createObjectURL(blob);
  const a = document.createElement('a');
  a.href = url;
  a.download = 'audio.mp3';
  a.click();
}
```

### PowerShell Example
```powershell
$body = @{
    url = "https://www.youtube.com/watch?v=VIDEO_ID"
    maxFileSizeMB = 50
    maxDurationMinutes = 15
} | ConvertTo-Json

$response = Invoke-RestMethod -Uri "https://localhost:7000/api/audio/download" `
                            -Method Post `
                            -ContentType "application/json" `
                            -Body $body

# Save the file
$response | Out-File -FilePath "audio.mp3" -Encoding Byte
```

## 🏗️ Services Overview

### ProcessService
Core service handling:
- Video information extraction using yt-dlp
- Audio download with format selection
- MP3 conversion using FFmpeg
- File validation (size, duration)
- File name sanitization

### DownloadQueueService
Background service managing:
- Concurrent download processing
- Queue-based job handling
- Configurable concurrency limits (default: 3)
- Asynchronous job completion

### DownloadLimiterService
Rate limiting service:
- Semaphore-based concurrency control
- Maximum 3 concurrent operations
- Thread-safe access management

### FileCleanupService
Background cleanup service:
- Automatic file removal after 15 minutes
- Safe file deletion (checks for locks)
- Runs every 5 minutes
- Error handling for locked/in-use files

## 📁 File Management

### Storage Directory
- **Location**: `./Storage/` (relative to application root)
- **Temporary Files**: WebM and MP3 files during processing
- **Naming**: GUID-based filenames to avoid conflicts
- **Cleanup**: Automatic removal after 15 minutes

### File Processing Flow
1. Generate unique GUID for the download
2. Download audio as WebM format to `Storage/{guid}.webm`
3. Convert WebM to MP3 at `Storage/{guid}.mp3`
4. Delete temporary WebM file
5. Return MP3 file to client
6. Schedule MP3 file for cleanup (15 minutes)

## 🚦 Rate Limiting & Concurrency

### Concurrency Limits
- **Maximum Concurrent Downloads**: 3
- **Queue Processing**: Background service with configurable limits
- **Rate Limiting**: SemaphoreSlim-based implementation

### Queue Management
- **Unbounded Channel**: Accepts unlimited job requests
- **Background Processing**: 3 workers by default
- **Async Completion**: TaskCompletionSource for job results
- **Error Handling**: Exception capture and reporting

## ⚠️ Error Handling

### Common Error Scenarios
1. **Invalid URL**: Returns validation error
2. **Video Too Long**: "Video exceeds 15 minute limit."
3. **File Too Large**: "File exceeds size limit (50MB)."
4. **Download Failed**: "Download failed."
5. **Conversion Failed**: "Conversion failed."
6. **Conversion Timeout**: "Conversion timed out."
7. **yt-dlp Error**: Detailed error message from tool

### Error Response Format
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Url": ["The Url field is required."]
  }
}
```

## 🛠️ Development

### Running in Development Mode
```bash
# Run with hot reload
dotnet watch

# Or standard run
dotnet run
```

### Swagger Documentation
- **URL**: `https://localhost:7000/swagger`
- **Interactive API testing**: Available in development mode
- **OpenAPI Specification**: `/swagger/v1/swagger.json`

### Testing with .http File
The project includes an `AudioDownloaderApi.http` file for API testing in Visual Studio.

### Environment Variables
- `ASPNETCORE_ENVIRONMENT`: Development/Production
- `ASPNETCORE_URLS`: Custom binding URLs

### Debugging
- **Logs**: Configurable logging levels in appsettings.json
- **Exception Handling**: Comprehensive error logging
- **Background Services**: Separate logging for queue and cleanup services

## 🔒 Security Considerations

- **Input Validation**: URL validation and sanitization
- **File Size Limits**: Prevents disk space exhaustion
- **Duration Limits**: Prevents long-running downloads
- **Temporary Files**: Automatic cleanup prevents accumulation
- **Rate Limiting**: Prevents server overload
- **Error Messages**: Sanitized error responses

## 📝 License

[Add your license information here]

## 🤝 Contributing

[Add contribution guidelines here]

## 📞 Support

[Add contact information or support links here]
