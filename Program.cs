using ImageGram.Storage;
using ImageGram.Repository;
using ImageGram.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Options>(builder.Configuration.GetSection("CosmosDB"));
builder.Services.AddSingleton<IStorage, CosmosStorage>();
builder.Services.AddScoped<IRepository, CosmosRepository>();
builder.Services.AddScoped<ICDBService, CosmosService>();

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
