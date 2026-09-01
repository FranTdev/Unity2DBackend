namespace Unity2D.Application.DTOs;

public class ChatMessageDto
{
    public string SenderId { get; set; } = string.Empty;
    public string SenderUsername { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public float SenderX { get; set; }
    public float SenderY { get; set; }
    public bool IsProximity { get; set; } = true; // Esto es para agregar proximidad a los chats, evitando que se saturen
}