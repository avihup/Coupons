using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Reflection;
using System.Text;
using TestCase.Interfaces.Auth;
using TestCase.Interfaces.DataInitializer;
using TestCase.Interfaces.Repositories;
using TestCase.Interfaces.Services;
using TestCase.Mapping;
using TestCase.Models.Auth;
using TestCase.Repositories;
using TestCase.Services;
using TestCase.Services.Auth;
using TestCase.Services.BackgroundServices;
using TestCase.Services.DataInitializer;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Program.cs

// Add services to the container
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

// MongoDB configuration
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register repositories
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IUsersRespository, UsersRespository>();
builder.Services.AddScoped<IClientsRepository, ClientsRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IDataInitializer, DataInitializer>();

// Register services
builder.Services.AddSingleton<CouponUnitGenerationService>();
builder.Services.AddSingleton<ICouponUnitGenerationService>(sp =>
    sp.GetRequiredService<CouponUnitGenerationService>());
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddSingleton<CouponUnitCancelService>();
builder.Services.AddSingleton<ICouponUnitCancelService>(sp =>
    sp.GetRequiredService<CouponUnitCancelService>());
// Add hosted service
builder.Services.AddHostedService(sp =>
    sp.GetRequiredService<CouponUnitGenerationService>());
builder.Services.AddHostedService(sp =>
    sp.GetRequiredService<CouponUnitCancelService>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<IDataInitializer>();
    await initializer.InitializeAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.EnablePersistAuthorization();
        c.DefaultModelsExpandDepth(-1); // Hide schemas section
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();