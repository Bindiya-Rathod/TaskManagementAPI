using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using TaskManagement.Core.Interfaces;
using TaskManagement.Infrastructure.Data;
using TaskManagement.Infrastructure.Factories;
using TaskManagement.Infrastructure.Repositories;
using TaskManagement.Infrastructure.Services;
using TaskManagement.API.Middleware;
using TaskManagement.Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// STEP 1.  Load configuration files and environment vars
// =======================================================
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

// =======================================================
// STEP 2.  Try to add Azure Key Vault (uses Managed Identity or local login)
// =======================================================
try
{
    var keyVaultName = "kv-taskmanagement-gate";          // <-- 🔁 change to your vault name
    var keyVaultUri = new Uri($"https://{keyVaultName}.vault.azure.net/");

    // DefaultAzureCredential checks: 1) env vars  2) VS login  3) Azure CLI  4) Managed Identity
    builder.Configuration.AddAzureKeyVault(keyVaultUri, new DefaultAzureCredential());
    Console.WriteLine("✅ Key Vault configuration loaded successfully.");
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("⚠️  Could not load Key Vault configuration. Using local appsettings instead.");
    Console.WriteLine($"   Reason: {ex.Message}");
    Console.ResetColor();
}

// =======================================================
// STEP 3.  Bind strongly-typed settings (Options Pattern)
// =======================================================
builder.Services.Configure<AzureStorageSettings>(
    builder.Configuration.GetSection(AzureStorageSettings.SectionName));

builder.Services.Configure<CosmosDbSettings>(
    builder.Configuration.GetSection(CosmosDbSettings.SectionName));

// =======================================================
// STEP 4.  Register EF Core, Repositories, Services
// =======================================================

// ---- SQL Server DbContext ----
var sqlConn = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<TaskManagementContext>(opts =>
{
    opts.UseSqlServer(sqlConn, sql =>
    {
        sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);

    });
});

// ---- Cosmos DB Client ----
builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var cfg = sp.GetRequiredService<
        Microsoft.Extensions.Options.IOptions<CosmosDbSettings>>().Value;
    return new CosmosClient(cfg.ConnectionString, new CosmosClientOptions
    {
        ApplicationName = "TaskManagementAPI",
        ConnectionMode = ConnectionMode.Direct
    });
});

// ---- Storage Factory ----
builder.Services.AddSingleton<IStorageClientFactory, StorageClientFactory>();

// ---- Repositories & UoW ----
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// ---- Services ----
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IStorageService, StorageService>();
builder.Services.AddScoped<IQueueService, QueueService>();

// =======================================================
// STEP 5.  Misc service registrations
// =======================================================
builder.Services.AddApplicationInsightsTelemetry();
builder.Services.AddHealthChecks().AddDbContextCheck<TaskManagementContext>("Database");

builder.Services.AddCors(p =>
{
    p.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Task Management API",
        Version = "v1",
        Description = "Azure-based Task Management System"
    });
});

// =======================================================
// STEP 6.  Build the app
// =======================================================
var app = builder.Build();

// =======================================================
// STEP 7.  Middleware Pipeline
// =======================================================
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Task Management API v1");
    c.RoutePrefix = string.Empty; // Swagger at root
});

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// =======================================================
// STEP 8.  Startup Logs
// =======================================================
app.Logger.LogInformation("🚀 Task Management API starting in {env}", app.Environment.EnvironmentName);
app.Logger.LogInformation("SQL Connection Loaded: {hasConn}", !string.IsNullOrEmpty(sqlConn));


// Immediately after retrieving the SQL connection string
var sqlConne = builder.Configuration.GetConnectionString("SqlConnection");
var cosmosConn = builder.Configuration["CosmosDb:ConnectionString"];
var storageConn = builder.Configuration["AzureStorage:ConnectionString"];

Console.WriteLine("-------------------------------------------------------");
Console.WriteLine("🔍 Configuration Check:");
Console.WriteLine($"SQL Connection Found: {!string.IsNullOrEmpty(sqlConne)}");
Console.WriteLine($"Cosmos Connection Found: {!string.IsNullOrEmpty(cosmosConn)}");
Console.WriteLine($"Storage Connection Found: {!string.IsNullOrEmpty(storageConn)}");
Console.WriteLine("-------------------------------------------------------");

app.Run();
