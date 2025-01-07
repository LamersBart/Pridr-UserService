using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using UserService.AsyncDataServices;
using UserService.Data;
using UserService.Data.Encryption;
using UserService.EventProcessing;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment;
if (environment.IsDevelopment())
{
    Console.WriteLine("Loading .env file in Development environment...");
    Env.Load();
}
builder.Configuration.AddEnvironmentVariables();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.RequireHttpsMetadata = false;
        o.Audience = Environment.GetEnvironmentVariable("AUTH_AUDIENCE");
        o.MetadataAddress = Environment.GetEnvironmentVariable("AUTH_METADATA")!;
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = Environment.GetEnvironmentVariable("AUTH_ISSUER"),
        };
    });

builder.Services.AddAuthorization();

var contact = new OpenApiContact
{
    Name = "Bart Lamers",
    Email = "mail@bartlamers.nl",
    Url = new Uri("https://bartlamers.nl")
};
var license = new OpenApiLicense
{
    Name = "My License",
    Url = new Uri("https://bartlamers.nl")
};
var info = new OpenApiInfo
{
    Version = "v1",
    Title = "Swagger API",
    Description = "This is the API Swagger page",
    TermsOfService = new Uri("https://bartlamers.nl"),
    Contact = contact,
    License = license
};
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.CustomSchemaIds(id => id.FullName!.Replace("+", "-"));
    o.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            Implicit = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri(Environment.GetEnvironmentVariable("KEYCLOAK_URL")!),
                Scopes = new Dictionary<string, string>
                {
                    {"openid", "openid"},
                    {"profile", "profile"}
                }
            }
        }
    });
    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Keycloak",
                    Type = ReferenceType.SecurityScheme
                },
                In = ParameterLocation.Header,
                Name = "Bearer",
                Scheme = "Bearer",
            },
            []
        }
    };
    o.SwaggerDoc("v1", info);
    o.AddSecurityRequirement(securityRequirement);
});
var connectionStringBuilder = new NpgsqlConnectionStringBuilder
{
    Host = Environment.GetEnvironmentVariable("PGHOST"),
    Port = int.Parse(Environment.GetEnvironmentVariable("PGPORT") ?? "5432"),
    Database = Environment.GetEnvironmentVariable("PGDB"),
    Username = Environment.GetEnvironmentVariable("PGUSER"),
    Password = Environment.GetEnvironmentVariable("PGPASS")
};
var connectionString = connectionStringBuilder.ConnectionString;
builder.Services.AddDbContext<AppDbContext>(options =>
{
    Console.WriteLine("Using in Postgres DB");
    options.UseNpgsql(connectionString);
});
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IProfileRepo, ProfileRepo>();
builder.Services.AddSingleton<IEventProcessor, EventProcessor>();
builder.Services.AddHostedService<MessageBusSubscriber>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddControllers();
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("reactApp", p =>
    {
        p.WithOrigins("https://demo.pridr.app")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
EncryptionHelper.Initialize();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o => o.EnableTryItOutByDefault());
}
await PrepDb.PrepPopulation(app, environment.IsProduction());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseCors("reactApp");
app.Run();