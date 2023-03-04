using ImageGram.DB;
using ImageGram.DB.Repository;
using ImageGram.Extension;
using ImageGram.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

CosmosDBOptions cdbOptions = builder.Configuration.GetSection("CosmosDB").Get<CosmosDBOptions>()
    ?? throw new ArgumentException("Can`t get CosmosDB connection options"); 
builder.Services.AddCosmosDB(cdbOptions);
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IGramService, ImageGramService>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStaticFiles();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
