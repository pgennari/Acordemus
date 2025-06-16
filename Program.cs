using acordemus.Configurations;
using acordemus.Endpoints;
using acordemus.Middleware;
using acordemus.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddAntiforgery();
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddScoped<ICondoService, CondoService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IExcerptService, ExcerptService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISubscriptionService, SubscriptionService>();
var app = builder.Build();

app.UseAuthentication();
app.UseMiddleware<ExternalTokenValidatorMiddleware>();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/healthcheck", () => Results.Ok("Papers please!"));
app.MapCondoEndpoints();
app.MapDocumentEndpoints();
app.MapExcerptEndpoints();
app.MapUnitEndpoints();
app.MapNotificationEndpoints();
app.MapUserEndpoints();
app.Run();


