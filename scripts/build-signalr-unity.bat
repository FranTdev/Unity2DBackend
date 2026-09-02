@echo off
setlocal enabledelayedexpansion

echo ====================================================================
echo Extractor de Librerias Cliente de SignalR para Unity (.NET Standard 2.1)
echo ====================================================================

:: 1. Eliminar residuos anteriores si existen
if exist "SignalRUnityBuild" rmdir /s /q "SignalRUnityBuild"
if exist "SignalR_Unity_Libs" rmdir /s /q "SignalR_Unity_Libs"

:: 2. Crear proyecto fuente temporal netstandard2.1
echo [1/4] Generando proyecto fuente temporal netstandard2.1...
dotnet new classlib -n SignalRUnityBuild -f netstandard2.1 > nul
if %errorlevel% neq 0 (
    echo [ERROR] Error al crear la libreria de clases netstandard2.1.
    pause
    exit /b %errorlevel%
)

:: 3. Instalar paquete oficial de SignalR Client
cd SignalRUnityBuild
echo [2/4] Instalando Microsoft.AspNetCore.SignalR.Client v8.0.11...
dotnet add package Microsoft.AspNetCore.SignalR.Client --version 8.0.11 > nul
if %errorlevel% neq 0 (
    echo [ERROR] Error al instalar paquete Microsoft.AspNetCore.SignalR.Client.
    cd ..
    rmdir /s /q "SignalRUnityBuild"
    pause
    exit /b %errorlevel%
)

:: 4. Compilar y publicar DLLs consolidadas
echo [3/4] Compilando y publicando DLLs consolidadas en modo Release...
dotnet publish -c Release -o ..\SignalR_Unity_Libs > nul
if %errorlevel% neq 0 (
    echo [ERROR] Error al compilar las DLLs.
    cd ..
    rmdir /s /q "SignalRUnityBuild"
    pause
    exit /b %errorlevel%
)

:: 5. Limpieza de codigo fuente temporal
cd ..
echo [4/4] Limpiando archivos temporales...
rmdir /s /q "SignalRUnityBuild"

echo ====================================================================
echo [EXITO] PROCESO COMPLETADO SATISFACTORIAMENTE
echo Carpeta generada: SignalR_Unity_Libs
echo Copia o arrastra la carpeta 'SignalR_Unity_Libs' a 'Assets/Plugins/' en Unity.
echo ====================================================================
