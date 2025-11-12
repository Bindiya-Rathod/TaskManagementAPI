using Microsoft.EntityFrameworkCore;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Factories;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.API.Middleware;
using TaskManagement.Shared.Configuration;
using Microsoft.Azure.Cosmos;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// AZURE KEY VAULT INTEGRATION
// ==========================================
var keyVaultUrl = builder.Configuration["KeyVault:VaultUrl"];

if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential());

    //builder.Logging.LogInformation("Key Vault integrated: {VaultUrl}", keyVaultUrl);
}

// ==========================================
// OPTIONS PATTERN - Configure Settings
// ==========================================
builder.Services.Configure<AzureStorageSettings>(
    builder.Configuration.GetSection(AzureStorageSettings.SectionName));

builder.Services.Configure<CosmosDbSettings>(
    builder.Configuration.GetSection(CosmosDbSettings.SectionName));

// ==========================================
// DEPENDENCY INJECTION
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQL Database - Connection string from Key Vault
builder.Services.AddDbContext<TaskManagementContext>(options =>
{
    // Key Vault secret name: SqlConnectionString
    var connectionString = builder.Configuration["SqlConnectionString"];
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(30), null);
    });
});

// Configure CosmosDB - Connection string from Key Vault
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    // Key Vault secret name: CosmosConnectionString
    var connectionString = builder.Configuration["CosmosConnectionString"];
    return new CosmosClient(connectionString);
});

// FACTORY PATTERN
builder.Services.AddSingleton<IStorageClientFactory, StorageClientFactory>();

// REPOSITORY PATTERN
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

// UNIT OF WORK PATTERN
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// SERVICE LAYER
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHealthChecks().AddDbContextCheck<TaskManagementContext>("Database");

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
    options.RoutePrefix = string.Empty;
});

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Logger.LogInformation("Task Management API starting...");
app.Run();