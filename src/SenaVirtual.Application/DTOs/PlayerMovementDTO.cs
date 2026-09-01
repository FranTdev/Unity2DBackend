namespace SenaVirtual.Application.DTOs;

public class PlayerMovementDTO
{
    public string PlayerId { get; set; } = string.Empty;
    public float PositionX { get; set; } = 0;
    public float PositionY { get; set; } = 0;
    public string Direction { get; set; } = "down";
}