using Unity2D.WebApi.Hubs;

var builder = WebApplication.CreateBuilder(args);

// A. Registrar servicios en el contenedor de Inyección de Dependencias
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddSignalR(); // Habilita el motor de SignalR para WebSockets

// Configurar CORS para permitir que clientes Unity (incluyendo compilaciones WebGL o editor) se conecten
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowUnityClient", policy =>
    {
        policy.SetIsOriginAllowed(_ => true) // En desarrollo, permite cualquier origen
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Requisito clave para WebSockets / SignalR
    });
});

var app = builder.Build();

// B. Configurar el Pipeline de Middlewares
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowUnityClient");
app.UseRouting();

// C. Endpoints de Señal e Indicadores de Estado (Health Check / Server Signal)
app.MapGet("/", () => Results.Ok(new
{
    status = "Online",
    service = "Unity 2D Multiplayer .NET Backend",
    version = "1.0.0",
    signalRHub = "/hubs/game",
    environment = app.Environment.EnvironmentName,
    timestamp = DateTime.UtcNow
})).WithName("GetServerStatus");

app.MapGet("/health", () => Results.Ok(new { status = "Healthy", timestamp = DateTime.UtcNow }))
   .WithName("HealthCheck");

// D. Mapear el endpoint del Hub de SignalR (URL a la que se conecta el cliente Unity: /hubs/game)
app.MapHub<GameHub>("/hubs/game");

app.MapControllers();

// E. Notificación en consola al iniciar el servidor
app.Lifetime.ApplicationStarted.Register(() =>
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(@"==================================================================");
    Console.WriteLine(@"🚀 UNITY 2D MULTIPLAYER BACKEND SERVER IS ONLINE AND READY!");
    Console.WriteLine(@"🌐 Root Status Signal  : http://localhost:5240/");
    Console.WriteLine(@"❤️ Health Check        : http://localhost:5240/health");
    Console.WriteLine(@"⚡ SignalR Game Hub    : http://localhost:5240/hubs/game");
    Console.WriteLine(@"==================================================================");
    Console.ResetColor();
});

app.Run();