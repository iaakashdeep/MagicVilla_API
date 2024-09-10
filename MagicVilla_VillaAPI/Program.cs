var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(options=>options.ReturnHttpNotAcceptable=true
    ).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();      //For adding PATCH functionality defined in https://jsonpatch.com/
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//options=>options.ReturnHttpNotAcceptable=true -> this will give not acceptable status when the request format is not Json, because in Accept header
//if we send any other type of format the api will always send Json format
//AddXmlDataContractSerializerFormatters -> this will allow the api to support xml request format as well
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
