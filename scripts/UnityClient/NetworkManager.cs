using System;
using System.Threading.Tasks;
using UnityEngine;
using Microsoft.AspNetCore.SignalR.Client;

namespace Unity2D.Client
{
    /// <summary>
    /// Componente principal para gestionar la conexión WebSocket / SignalR entre Unity y el Backend .NET Core.
    /// Adjuntar este script a un GameObject persistente (ej. NetworkManager) en tu escena de Unity.
    /// </summary>
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager Instance { get; private set; }

        [Header("Configuración del Servidor")]
        [Tooltip("URL completa del Hub de SignalR en el backend .NET")]
        [SerializeField] private string serverUrl = "http://localhost:5240/hubs/game";

        [Tooltip("Conectar automáticamente al iniciar la escena")]
        [SerializeField] private bool autoConnectOnStart = true;

        private HubConnection _hubConnection;

        // Eventos públicos para suscribirse desde otros scripts de Unity
        public event Action<PlayerMovementDto> OnPlayerMovedReceived;
        public event Action<ChatMessageDto> OnChatMessageReceived;
        public event Action<string> OnPlayerLeftReceived;
        public event Action OnConnected;
        public event Action OnDisconnected;

        public bool IsConnected => _hubConnection != null && _hubConnection.State == HubConnectionState.Connected;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            if (autoConnectOnStart)
            {
                await ConnectAsync();
            }
        }

        /// <summary>
        /// Inicia la conexión con el servidor SignalR y registra los escuchadores de eventos.
        /// </summary>
        public async Task ConnectAsync()
        {
            if (IsConnected)
            {
                Debug.LogWarning("[NetworkManager] El cliente ya está conectado.");
                return;
            }

            Debug.Log($"[NetworkManager] Conectando a {serverUrl}...");

            _hubConnection = new HubConnectionBuilder()
                .WithUrl(serverUrl)
                .WithAutomaticReconnect(new TimeSpan[] { 
                    TimeSpan.FromSeconds(0), 
                    TimeSpan.FromSeconds(2), 
                    TimeSpan.FromSeconds(5), 
                    TimeSpan.FromSeconds(10) 
                })
                .Build();

            // 1. Suscribirse a eventos recibidos desde el servidor
            _hubConnection.On<PlayerMovementDto>("OnPlayerMoved", (movement) =>
            {
                Debug.Log($"[NetworkManager] Movimiento recibido del jugador {movement.PlayerId} (X:{movement.PositionX}, Y:{movement.PositionY})");
                OnPlayerMovedReceived?.Invoke(movement);
            });

            _hubConnection.On<ChatMessageDto>("OnChatMessageReceived", (chatMessage) =>
            {
                Debug.Log($"[NetworkManager] Chat recibido de {chatMessage.SenderUsername}: {chatMessage.Message}");
                OnChatMessageReceived?.Invoke(chatMessage);
            });

            _hubConnection.On<string>("OnPlayerLeft", (connectionId) =>
            {
                Debug.Log($"[NetworkManager] Jugador desconectado: {connectionId}");
                OnPlayerLeftReceived?.Invoke(connectionId);
            });

            // Manejo de reconexión y desconexión
            _hubConnection.Reconnecting += (error) =>
            {
                Debug.LogWarning($"[NetworkManager] Perdió conexión. Intentando reconectar... Error: {error?.Message}");
                return Task.CompletedTask;
            };

            _hubConnection.Reconnected += (connectionId) =>
            {
                Debug.Log($"[NetworkManager] Reconectado exitosamente. Nuevo ConnectionId: {connectionId}");
                OnConnected?.Invoke();
                return Task.CompletedTask;
            };

            _hubConnection.Closed += (error) =>
            {
                Debug.LogError($"[NetworkManager] Conexión cerrada. Error: {error?.Message}");
                OnDisconnected?.Invoke();
                return Task.CompletedTask;
            };

            // 2. Iniciar la conexión
            try
            {
                await _hubConnection.StartAsync();
                Debug.Log("[NetworkManager] ¡Conectado exitosamente al servidor SignalR! ConnectionId: " + _hubConnection.ConnectionId);
                OnConnected?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NetworkManager] Error al conectar con el servidor: {ex.Message}");
            }
        }

        /// <summary>
        /// Envía las coordenadas y dirección del jugador local al servidor.
        /// </summary>
        public async Task SendMovementAsync(string playerId, float posX, float posY, string direction = "down")
        {
            if (!IsConnected) return;

            var movement = new PlayerMovementDto
            {
                PlayerId = playerId,
                PositionX = posX,
                PositionY = posY,
                Direction = direction
            };

            try
            {
                await _hubConnection.SendAsync("SendMovement", movement);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NetworkManager] Error al enviar movimiento: {ex.Message}");
            }
        }

        /// <summary>
        /// Envía un mensaje de chat al servidor.
        /// </summary>
        public async Task SendChatMessageAsync(string senderId, string username, string roomId, string message, float posX = 0, float posY = 0, bool isProximity = true)
        {
            if (!IsConnected) return;

            var chat = new ChatMessageDto
            {
                SenderId = senderId,
                SenderUsername = username,
                RoomId = roomId,
                Message = message,
                SenderX = posX,
                SenderY = posY,
                IsProximity = isProximity
            };

            try
            {
                await _hubConnection.SendAsync("SendChatMessage", chat);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[NetworkManager] Error al enviar mensaje de chat: {ex.Message}");
            }
        }

        private async void OnDestroy()
        {
            if (_hubConnection != null)
            {
                await _hubConnection.StopAsync();
                await _hubConnection.DisposeAsync();
            }
        }
    }
}
