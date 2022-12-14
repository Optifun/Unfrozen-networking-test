using System;
using System.Collections.Generic;
using System.Linq;
using Chat.API.DataAccess;
using Chat.API.Hubs;
using Chat.API.Mapping;
using Chat.API.Services;
using Mapster;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatContext>();
// builder.Services.AddDbContext<ChatContext>((provider, optionsBuilder) =>
// {
//     string connectionString = builder.Configuration.GetConnectionString("chat-db");
//     Console.WriteLine(connectionString);
//     optionsBuilder.UseNpgsql(connectionString, contextOptionsBuilder => { contextOptionsBuilder.EnableRetryOnFailure(3); });
// });
builder.Services.AddSignalR(options => { options.EnableDetailedErrors = true; });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromDays(1);
        options.SlidingExpiration = true;
        options.AccessDeniedPath = "/Auth/user-profile";
    });

builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddSingleton<IChatUsers, InMemoryChatUsers>();

var mappingConfig = TypeAdapterConfig.GlobalSettings.Default.Config;
IList<IRegister> registers = mappingConfig.Scan(typeof(MessageConfig).Assembly);

foreach (IRegister register in registers)
    mappingConfig.Apply(register);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/ws/chat");


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ChatContext>();
    if (!context.Database.CanConnect())
        Console.WriteLine("Can't connect to DB");

    try
    {
        bool created = context.Database.EnsureCreated();
        Console.WriteLine($"DB bootstrapped={created}");
    }
    catch (Exception e)
    {
        Console.WriteLine("Migration error");
        Console.WriteLine(e);
        throw;
    }
}

app.Run();