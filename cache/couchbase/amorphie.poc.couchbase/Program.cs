using Dapr.Client;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDaprClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/", async([FromBody]Model model,[FromServices]DaprClient daprClient) =>
{
    await daprClient.SaveStateAsync<Model>("state.couchbase", Guid.NewGuid().ToString(), model);
})
.WithName("AddNewRecord")
.WithOpenApi();

app.MapGet("/", async([FromServices]DaprClient daprClient) =>
{
    var query = "{" +
                "\"filter\": {" +
                    "\"EQ\": { \"value.Id\": \"1\" }" +
                "}," +
                "\"sort\": [" +
                    "{" +
                        "\"key\": \"value.Balance\"," +
                        "\"order\": \"DESC\"" +
                    "}" +
                "]" +
            "}";

    return await daprClient.QueryStateAsync<Model>("state.couchbase", query);
})
.WithName("GetAllRecord")
.WithOpenApi();


app.Run();

record Model(string key, string value);
