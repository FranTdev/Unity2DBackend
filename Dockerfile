# ===================================================================
# Multi-stage Dockerfile para Unity 2D Multiplayer Backend (.NET 9 Alpine)
# Imagen ultra ligera optimizada para producción
# ===================================================================

# 1. ETAPA DE RUNTIME (Alpine Linux)
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS base
WORKDIR /app
EXPOSE 5240

# Crear un usuario no-root por seguridad
RUN adduser -u 1000 -D appuser && chown -R appuser:appuser /app
USER appuser

ENV ASPNETCORE_URLS=http://+:5240
ENV ASPNETCORE_ENVIRONMENT=Production

# 2. ETAPA DE COMPILACIÓN (SDK Alpine)
FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src

# Copiar csproj para restaurar capas independientes y aprovechar la caché de Docker
COPY ["src/Unity2D.WebApi/Unity2D.WebApi.csproj", "src/Unity2D.WebApi/"]
COPY ["src/Unity2D.Infrastructure/Unity2D.Infrastructure.csproj", "src/Unity2D.Infrastructure/"]
COPY ["src/Unity2D.Application/Unity2D.Application.csproj", "src/Unity2D.Application/"]
COPY ["src/Unity2D.Domain/Unity2D.Domain.csproj", "src/Unity2D.Domain/"]

RUN dotnet restore "src/Unity2D.WebApi/Unity2D.WebApi.csproj"

# Copiar todo el código fuente
COPY src/ src/

WORKDIR "/src/src/Unity2D.WebApi"
RUN dotnet build "Unity2D.WebApi.csproj" -c Release -o /app/build

# 3. ETAPA DE PUBLICACIÓN
FROM build AS publish
RUN dotnet publish "Unity2D.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 4. ETAPA FINAL DE PRODUCCIÓN
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Unity2D.WebApi.dll"]
