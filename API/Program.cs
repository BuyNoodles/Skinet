using API;
using API.Helpers;
using Core.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// Add services to the container. 
services.AddControllers();
services.AddScoped<IProductRepository, ProductRepository>();
services.AddAutoMapper(typeof(MappingProfiles));
services.AddScoped(typeof(IGenericRepository<>), (typeof(GenericRepository<>)));
services.AddDbContext<StoreContext>(x => x.UseSqlite(
        config.GetConnectionString("DefaultConnection")));


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var app = builder.Build();

// Try to apply migrations
await Migration.Migrate(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();

