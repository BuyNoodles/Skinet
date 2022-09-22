using API;
using API.Extensions;
using API.Helpers;
using API.Middleware;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container. 
services.AddControllers();
services.AddAutoMapper(typeof(MappingProfiles));
services.AddDbContext<StoreContext>(x => x.UseSqlite(
        config.GetConnectionString("DefaultConnection")));
services.AddApplicationServices();
services.AddSwaggerDocumentation();

var app = builder.Build();

// Try to apply migrations
await Migration.Migrate(app);

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();

app.UseStatusCodePagesWithReExecute("errors/{0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseAuthorization();

app.UseSwaggerDocumentation();

app.MapControllers();

app.Run();

