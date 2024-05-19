using backend.api.Models;
using backend.data.DataContext;
using backend.servicios.Config;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllersWithViews();
//Configuracion para error de Cors desde el front
builder.Services.AddCors(option =>
{
    option.AddPolicy("MyPolicy", builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IMapsService, MapsService>();
builder.Services.AddScoped<IOrganizacionService, OrganizacionService>();
builder.Services.AddHttpClient<IMapsService, MapsService>();
builder.Services.AddScoped<IOrganizacionInfoService, InfoOrganizacionService>();
builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UsuarioRequestModel>());

var groqApiConfig = builder.Configuration.GetSection("GroqApiConfig").Get<GroqApiConfig>();
builder.Services.AddSingleton(groqApiConfig);
builder.Services.AddSingleton<IGenerateIdeaApiService, GroqApiService>();
builder.Services.AddHttpClient();

builder.Services.AddPredictionEnginePool<FabricModelInput, FabricModelOutput>()
    .FromFile(modelName: "ClasificacionImagen.MLModels.FabricMLModel", filePath: "MLModel/FabricMLModel.mlnet", watchForChanges: true);

var app = builder.Build();

var googleMapsApiKey = app.Configuration["GoogleMapsApiKey"];

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("MyPolicy");
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
