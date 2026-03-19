using AudioDownloaderApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProcessService>();
builder.Services.AddHostedService<FileCleanupService>();
builder.Services.AddSingleton<ProcessService>();
builder.Services.AddSingleton<DownloadQueueService>();
builder.Services.AddHostedService(provider => provider.GetRequiredService<DownloadQueueService>());
builder.Services.AddSingleton<DownloadLimiterService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
