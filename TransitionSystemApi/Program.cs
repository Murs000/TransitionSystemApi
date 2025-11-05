using Neo4j.Driver;
using TransitionSystemApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Neo4j driver
var neo4jUri = builder.Configuration.GetValue<string>("Neo4j:Uri") ?? "bolt://localhost:7687";
var neo4jUser = builder.Configuration.GetValue<string>("Neo4j:User") ?? "neo4j";
var neo4jPassword = builder.Configuration.GetValue<string>("Neo4j:Password") ?? "test1234";

builder.Services.AddSingleton<IDriver>(_ => GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUser, neo4jPassword)));
builder.Services.AddScoped<Neo4jService>(); 
builder.Services.AddScoped<TransitionService>();

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