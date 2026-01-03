using Autofac.Core;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
	app.UseSwaggerUi(options =>
	{
		options.DocumentPath = "openapi/v1.json";
	});
}

var cs = builder.Configuration.GetConnectionString("Default");
builder.Services.AddDbContext<ApplicationDbContext>(opt => opt.UseNpgsql(cs));
builder.Services.AddMediatR(cfg =>
	cfg.RegisterServicesFromAssembly(typeof(Application.Users.Commands.CreateUser.CreateUserCommand).Assembly));

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
