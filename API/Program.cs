using API.Extensions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
using Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container. 
services.AddControllers();
services.AddAutoMapper(typeof(MappingProfiles));

services.AddDbContext<StoreContext>(x => x.UseNpgsql(
        config.GetConnectionString("DefaultConnection")));

services.AddDbContext<AppIdentityDbContext>(x => x.UseNpgsql(
        config.GetConnectionString("IdentityConnection")));

services.AddSingleton<IConnectionMultiplexer>(c =>
{
    var configuration = ConfigurationOptions.Parse(
        config.GetConnectionString("Redis"), true);
    return ConnectionMultiplexer.Connect(configuration);
});

services.AddApplicationServices();
services.AddIdentityServices(config);
services.AddSwaggerDocumentation();
services.AddCors(opt =>
{
    opt.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200");
    });
});

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// Try to apply migrations
await Migration.Migrate(app);

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("/errors/{0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "Content/")),
    RequestPath = "/content"
});

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.UseSwaggerDocumentation();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapFallbackToController("Index", "Fallback");
});

app.Run();

