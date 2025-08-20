using acordemus;
using acordemus.Configurations;
using acordemus.Endpoints;
using acordemus.Middleware;
using acordemus.Services;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddMemoryCache();
builder.Services.AddAntiforgery();
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings")
);
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<DevKeys>();

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<ICondoService, CondoService>();
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IExcerptService, ExcerptService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddScoped<ILoginService, LoginService>();


builder.Services.AddAuthentication().AddJwtBearer(x =>
{
    x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        IssuerSigningKey = new DevKeys(builder.Environment).RsaSecurityKey,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"]
    };
    //x.Events = new JwtBearerEvents()
    //{
    //    OnTokenValidated = async context =>
    //    {
    //        var roleService = context.HttpContext.RequestServices.GetRequiredService<IMemberService>();
    //        var condoId = context.HttpContext.Request.Headers["X-Condos-Id"].ToString();
    //        var userRoles = roleService.GetPersonRoles(context.Principal!.Identity.Name, condoId);
    //        var userClaims = userRoles.Result.Select(role => new Claim(ClaimTypes.Role, role.Name));
    //        ((ClaimsIdentity)context.Principal!.Identity).AddClaims(userClaims);
    //    },
    //};
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();
app.UseMiddleware<ExternalTokenValidatorMiddleware>();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/healthcheck", () => Results.Ok("Papers please!"));
app.MapPeopleEndpoints();
app.MapCondoEndpoints();
app.MapDocumentEndpoints();
app.MapExcerptEndpoints();
app.MapLoginEndpoints();
app.MapUnitEndpoints();
app.MapMemberEndpoints();
app.Run();


