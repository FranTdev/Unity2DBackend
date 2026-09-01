namespace SenaVirtual.Domain.Entities;

public class Player
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "Player"; // Player, Admin, Moderator, etc.
    public float PositionX { get; set; } = 0; // Posicion por defecto, puede quitarse si da problemas
    public float PositionY { get; set; } = 0;
    public string Direction { get; set; } = "down";
    public string CurrentRoomId { get; set; } = "Lobby";
}

 