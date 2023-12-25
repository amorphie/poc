

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDaprClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

MinioDaprModule.AddRoutes(app);
app.UseCloudEvents();
app.UseHttpsRedirection();
app.MapSubscribeHandler();


app.Run();
