using backend.api;
using backend.api.Models;
using backend.data.DataContext;
using backend.repositories.implementations;
using backend.repositories.interfaces;
using backend.servicios.Config;
using backend.servicios.Interfaces;
using backend.servicios.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.ML;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: true);

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

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMapsService, MapsService>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IDonationService,DonationService>();
builder.Services.AddScoped<IOrganizationInfoService, InfoOrganizationService>();
builder.Services.AddScoped<IIdeaService, IdeaService>();
builder.Services.AddScoped<IHeadquartersService, HeadquartersService>();
builder.Services.AddScoped<INeedService, NeedService>();
//builder.Services.AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<UserRequestModel>());

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var groqApiConfig = builder.Configuration.GetSection("GroqApiConfig").Get<GroqApiConfig>();
builder.Services.AddSingleton(groqApiConfig);
builder.Services.AddSingleton<IGenerateIdeaApiService, GroqApiService>();
builder.Services.AddHttpClient();

var openAiApiConfig = builder.Configuration.GetSection("OpenAiApiConfig").Get<OpenAiApiConfig>();
builder.Services.AddSingleton(openAiApiConfig);
builder.Services.AddSingleton<IImageService, OpenAIImageService>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

var webSocketOptions = new WebSocketOptions()
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};


app.UseWebSockets(webSocketOptions);

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("MyPolicy");
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

var webSockets = new ConcurrentDictionary<string, WebSocket>();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var socketId = Guid.NewGuid().ToString();
        WebSocketHandler.AddSocket(socketId, webSocket);
        await WebSocketHandler.HandleWebSocketAsync(context, webSocket, socketId);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();


