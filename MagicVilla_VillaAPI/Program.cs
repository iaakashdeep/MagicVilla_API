using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options=>options.ReturnHttpNotAcceptable=true
    ).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();      //For adding PATCH functionality defined in https://jsonpatch.com/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//options=>options.ReturnHttpNotAcceptable=true -> this will give not acceptable status when the request format is not Json, because in Accept header
//if we send any other type of format the api will always send Json format
//AddXmlDataContractSerializerFormatters -> this will allow the api to support xml request format as well

#region Serilog Configuration
Log.Logger=new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(
    "log/VillaSerilog.txt",rollingInterval: RollingInterval.Day
    ).CreateLogger();
//WriteTo.File-> Says log the logging info in a file we can also do in Console and other things
builder.Host.UseSerilog();      //To tell that default logging system won't be used now

//To use Serilog we have to install Serilog.AspnetCore and Serilog.Files package you can also install Serilog.Console package if you want to log in console

#endregion

#region Custom Logging

builder.Services.AddSingleton<ILogging, Logging>();
#endregion

builder.Services.AddDbContext<ApiDBContext>(option => {
    option.UseSqlServer(builder.Configuration.GetConnectionString("defaultSQLConnection"));
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
