using Auth.Core.Application.Settings;
using Auth.Infraestructure.Identity;
using Auth.Infraestructure.Identity.Entities;
using Auth.Infraestructure.Identity.Seeds;
using Microsoft.AspNetCore.Identity;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication(); //Add this line
builder.Services.AddIdentityInfrastructure(builder.Configuration); //Add this line

var app = builder.Build();

// Configure the HTTP request pipeline.

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DefaultRoles.Seed(userManager, roleManager);
        await DefaultOwner.Seed(userManager, roleManager);
        await DefaultUser.Seed(userManager, roleManager);
    }
    catch (Exception ex)
    {
    }
} // Add this block

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
