using Microsoft.AspNetCore.SignalR;
using Unity2D.Application.DTOs;

namespace Unity2D.WebApi.Hubs;

public class GameHub : Hub
{
	// 1. Gestión de Conexión: Se ejecuta cuando Unity se conecta
	public override async Task OnConnectedAsync() 
	{
		// Asignamos al cliente a la sala "Lobby"
		await Groups.AddToGroupAsync(Context.ConnectionId, "Lobby");
		await base.OnConnectedAsync();
	}

	// 2. Gestión de Desconexión: Cuando el jugador cierra o pierde la conexión
	public override async Task OnDisconnectedAsync(Exception? exception)
	{
		// Se le notifica a los demás que el jugador se ha desconectado
		await Clients.Others.SendAsync("OnPlayerLeft", Context.ConnectionId);
		await base.OnDisconnectedAsync(exception);
	}

	// 3. Recepción de Movimiento (2D)
	public async Task SendMovement(PlayerMovementDto movement)
	{
		// Re-transmitimos el movimiento a todos los demás jugadores en la sala "Lobby"
		// Usamos OthersInGroup para que el jugador que envió el movimiento no reciba su propio movimiento
		await Clients.OthersInGroup("Lobby").SendAsync("OnPlayerMoved", movement);
	}

	// 4. Recepción y Distribución de Mensajes de Chat
	public async Task SendChatMessage(ChatMessageDto message)
	{
		// Re-transmitimos el mensaje a todos los demás jugadores en la sala "Lobby"
		await Clients.Group(message.RoomId).SendAsync("OnChatMessageReceived", message);
	}
}