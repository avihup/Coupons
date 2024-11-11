using MongoDB.Driver;
using TestCase.Mapping;
using TestCase.Repositories;
using TestCase.Services;
using TestCase.Services.BackgroundServices;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MongoDB configuration
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration.GetConnectionString("MongoDB")));

// Register AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// Register repositories
builder.Services.AddScoped<ICouponRepository, CouponRepository>();

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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();