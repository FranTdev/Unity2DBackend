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

// Mapear el endpoint del Hub de SignalR (URL a la que se conecta el cliente Unity: /hubs/game)
app.MapHub<GameHub>("/hubs/game");

app.MapControllers();

app.Run();