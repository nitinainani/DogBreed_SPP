using DogBreedAPI_SPP.Repos;
using DogBreedAPI_SPP.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGetDogBreedImgService, GetDogBreedImgService>();
builder.Services.AddScoped<IDogBreederProcessService, DogBreederProcessService>();
builder.Services.AddScoped<IDogBreedRepo, DogBreedRepo>();

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
