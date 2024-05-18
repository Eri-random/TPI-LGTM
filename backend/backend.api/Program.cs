using backend.api.Models;
using backend.data.DataContext;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddJsonFile("appsettings.json");
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.IgnoreNullValues = true;
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


app.UseAuthorization();

app.MapControllers();

app.Run();
