using System.Text;
using GlobeLife.Api.Application;
using GlobeLife.Api.Domain;
using GlobeLife.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();
builder.Services.AddHealthChecks();
var jwtSigningKey = builder.Configuration["Jwt:SigningKey"] ?? throw new InvalidOperationException("Jwt:SigningKey is required.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters { ValidateIssuerSigningKey = true, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey)), ValidateIssuer = false, ValidateAudience = false, ValidateLifetime = true, RoleClaimType = System.Security.Claims.ClaimTypes.Role });
builder.Services.AddAuthorization(options => options.AddPolicy("Customer", policy => policy.RequireRole("Customer")));
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => RedisConnectionStringNormalizer.Connect(builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379"));
builder.Services.AddSingleton<IPolicyRepository, PolicyRepository>();
builder.Services.AddSingleton<IPolicyQueryService, PolicyQueryService>();
builder.Services.AddSingleton<ObjectStoragePort, LocalObjectStoragePort>();
builder.Services.AddSingleton<WorkflowCommandBus, LocalWorkflowCommandBus>();
builder.Services.AddSingleton<IDocumentWorkflowOrchestrator, DocumentWorkflowOrchestrator>();
builder.Services.AddSingleton<ICredentialRepository, DemoCredentialRepository>();
builder.Services.AddSingleton<JwtAccessTokenIssuer>();
var app = builder.Build();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseDefaultFiles(); app.UseStaticFiles(); app.UseAuthentication(); app.UseAuthorization();
app.MapHealthChecks("/api/health");
app.MapPost("/api/auth/login", (LoginCommand command, ICredentialRepository credentials, JwtAccessTokenIssuer issuer) => { var user = credentials.Verify(command); return user is null ? Results.Unauthorized() : Results.Ok(issuer.Issue(user)); });
app.MapGet("/api/policies", (IPolicyQueryService query, HttpContext context, CancellationToken token) => query.GetCustomerPoliciesAsync(context.User.Identity?.Name ?? "unknown", token)).RequireAuthorization("Customer");
app.MapGet("/api/support/queue", () => Results.Ok(new[] { new { id = "SUP-1001", priority = "High", subject = "Payment question", status = "Open" }, new { id = "SUP-1002", priority = "Normal", subject = "Document request", status = "Open" } })).RequireAuthorization(new Microsoft.AspNetCore.Authorization.AuthorizeAttribute { Roles = "Support" });
app.MapPost("/api/support-requests", (SupportRequest request) => Results.Accepted("/api/support-requests", new { request.Category, request.Message, status = "received" })).RequireAuthorization("Customer");
app.MapPost("/api/documents", async (DocumentSubmission submission, IDocumentWorkflowOrchestrator workflow, CancellationToken token) => Results.Accepted($"/api/documents/{submission.DocumentId}", await workflow.SubmitAsync(submission, token))).RequireAuthorization("Customer");
app.MapPost("/api/documents/upload-url", async (string objectKey, string contentType, ObjectStoragePort storage, CancellationToken token) => Results.Ok(new { uploadUrl = await storage.CreateUploadUrlAsync(objectKey, contentType, token) })).RequireAuthorization("Customer");
app.MapFallbackToFile("index.html");
app.Run();

public static class RedisConnectionStringNormalizer
{
    public static IConnectionMultiplexer Connect(string connectionString)
    {
        if (!connectionString.StartsWith("redis://", StringComparison.OrdinalIgnoreCase) && !connectionString.StartsWith("rediss://", StringComparison.OrdinalIgnoreCase))
            return ConnectionMultiplexer.Connect(connectionString + (connectionString.Contains("abortConnect", StringComparison.OrdinalIgnoreCase) ? "" : ",abortConnect=false"));

        var redisUri = new Uri(connectionString);
        var hostAndPort = $"{redisUri.Host}:{redisUri.Port}";
        var options = ConfigurationOptions.Parse(hostAndPort);
        options.AbortOnConnectFail = false;
        options.Ssl = redisUri.Scheme.Equals("rediss", StringComparison.OrdinalIgnoreCase) || redisUri.Host.Contains("upstash.io", StringComparison.OrdinalIgnoreCase);
        options.SslHost = redisUri.Host;
        options.ConnectTimeout = 15000;
        options.SyncTimeout = 10000;
        if (!string.IsNullOrEmpty(redisUri.UserInfo))
        {
            var credentials = redisUri.UserInfo.Split(':', 2);
            options.Password = Uri.UnescapeDataString(credentials.Length == 2 ? credentials[1] : credentials[0]);
        }
        return ConnectionMultiplexer.Connect(options);
    }
}

public partial class Program { }
