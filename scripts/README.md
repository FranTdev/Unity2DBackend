# Guía de Extracción de Librerías e Integración de SignalR en Unity 🎮

Esta carpeta contiene los scripts de automatización y los componentes cliente de C# listos para conectar cualquier proyecto de **Unity 2D** con el backend en **.NET Core**.

---

## 📂 Estructura de la Carpeta `scripts/`

```text
scripts/
├── build-signalr-unity.bat   # Script automatizado de extracción para Windows
├── build-signalr-unity.sh    # Script automatizado de extracción para Linux / macOS
├── SignalR_Unity_Libs/       # Carpetas con las 30 DLLs compiladas en .NET Standard 2.1 (Generado por el script)
└── UnityClient/              # Scripts de C# listos para copiar a tu proyecto de Unity
    ├── NetworkManager.cs     # Componente MonoBehaviour principal para gestionar WebSockets / SignalR
    └── NetworkDTOs.cs        # DTOs de datos serializables (PlayerMovementDto, ChatMessageDto)
```

---

## 🚀 Guía de Integración Paso a Paso en Unity

### Paso 1: Generar/Actualizar las DLLs Oficiales de SignalR

Ejecuta el script correspondiente a tu sistema operativo dentro de esta carpeta:

* **En Windows (1 Clic):**
  Ejecuta `build-signalr-unity.bat` haciendo doble clic o desde la consola:
  ```cmd
  scripts\build-signalr-unity.bat
  ```

* **En Linux / macOS:**
  ```bash
  chmod +x scripts/build-signalr-unity.sh
  ./scripts/build-signalr-unity.sh
  ```

El script creará automáticamente la carpeta **`scripts/SignalR_Unity_Libs/`** con las 30 DLLs oficiales de Microsoft `Microsoft.AspNetCore.SignalR.Client` compiladas en **`.NET Standard 2.1`**.

---

### Paso 2: Importar en tu Proyecto de Unity

1. Copia la carpeta **`scripts/SignalR_Unity_Libs/`** a la ruta **`Assets/Plugins/`** en tu proyecto de Unity:
   ```text
   Assets/Plugins/SignalR_Unity_Libs/
   ```

2. Copia la carpeta **`scripts/UnityClient/`** a la ruta **`Assets/Scripts/`** en tu proyecto de Unity:
   ```text
   Assets/Scripts/UnityClient/
   ```

---

### Paso 3: Configurar el `NetworkManager` en la Escena de Unity

1. En Unity, crea un **Empty GameObject** en la jerarquía de tu escena y nómbralo `NetworkManager`.
2. Arrastra el script [`NetworkManager.cs`](file:///c:/Users/FranT/Desktop/Back-End/.NET/SenaVirtualBackend/scripts/UnityClient/NetworkManager.cs) hacia el objeto `NetworkManager`.
3. En el **Inspector**:
   * **Server Url:** `http://localhost:5240/hubs/game`
   * **Auto Connect On Start:** `true`

---

### 💻 Ejemplo de Uso desde tus Scripts de Juego en Unity

#### A. Mover a tu Jugador y Enviar Coordenadas $X,Y$ (`PlayerController.cs`)

```csharp
using UnityEngine;
using Unity2D.Client;

public class PlayerController : MonoBehaviour
{
    public string playerId = "player-1";
    public float speed = 5f;

    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        if (moveX != 0 || moveY != 0)
        {
            transform.Translate(new Vector3(moveX, moveY, 0) * speed * Time.deltaTime);

            // Transmitir posición 2D al servidor
            NetworkManager.Instance.SendMovementAsync(
                playerId, 
                transform.position.x, 
                transform.position.y, 
                moveY > 0 ? "up" : "down"
            );
        }
    }
}
```

#### B. Recibir Movimiento de Otros Jugadores en Tiempo Real

```csharp
using UnityEngine;
using Unity2D.Client;

public class RemotePlayerManager : MonoBehaviour
{
    void Start()
    {
        // Suscribirse al evento de movimiento
        NetworkManager.Instance.OnPlayerMovedReceived += OnOtherPlayerMoved;
    }

    private void OnOtherPlayerMoved(PlayerMovementDto movement)
    {
        Debug.Log($"Jugador {movement.PlayerId} se movió a ({movement.PositionX}, {movement.PositionY})");
        // Actualizar o interpolar posición de avatares remotos en Unity
    }

    void OnDestroy()
    {
        if (NetworkManager.Instance != null)
            NetworkManager.Instance.OnPlayerMovedReceived -= OnOtherPlayerMoved;
    }
}
```

---

### 💡 Resolución de Conflictos Comunes en Unity

* **Duplicidad de `System.Text.Json.dll`:**
  Si Unity muestra un aviso de DLL duplicada:
  1. Ve a `Assets/Plugins/SignalR_Unity_Libs/System.Text.Json.dll`.
  2. En el **Inspector**, desmarca la casilla **Any Platform**.
  3. Haz clic en **Apply**.
