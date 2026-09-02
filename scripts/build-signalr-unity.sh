#!/bin/bash

set -e

# Cambiar al directorio donde se encuentra el script
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$SCRIPT_DIR"

echo "===================================================================="
echo "📦 Extractor de Librerías Cliente de SignalR para Unity (.NET Standard 2.1)"
echo "===================================================================="

# 1. Eliminar residuos anteriores si existen
rm -rf SignalRUnityBuild SignalR_Unity_Libs

# 2. Crear proyecto fuente temporal netstandard2.1
echo "[1/4] Generando proyecto fuente temporal netstandard2.1..."
dotnet new classlib -n SignalRUnityBuild -f netstandard2.1 > /dev/null

# 3. Instalar paquete oficial de SignalR Client
cd SignalRUnityBuild
echo "[2/4] Instalando Microsoft.AspNetCore.SignalR.Client v8.0.11..."
dotnet add package Microsoft.AspNetCore.SignalR.Client --version 8.0.11 > /dev/null

# 4. Compilar y publicar DLLs consolidadas en scripts/SignalR_Unity_Libs
echo "[3/4] Compilando y publicando DLLs consolidadas en modo Release..."
dotnet publish -c Release -o ../SignalR_Unity_Libs > /dev/null

# 5. Limpieza de código fuente temporal
cd ..
echo "[4/4] Limpiando archivos temporales..."
rm -rf SignalRUnityBuild

echo "===================================================================="
echo "✅ PROCESO COMPLETADO CON ÉXITO"
echo "📂 Carpeta generada: scripts/SignalR_Unity_Libs"
echo "🎯 Copia o arrastra la carpeta 'SignalR_Unity_Libs' a 'Assets/Plugins/' en tu proyecto de Unity."
echo "===================================================================="
