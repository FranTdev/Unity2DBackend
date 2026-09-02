namespace Unity2D.Application.DTOs;

public class PlayerMovementDto
{
    public string PlayerId { get; set; } = string.Empty;
    public float PositionX { get; set; } = 0;
    public float PositionY { get; set; } = 0;
    public string Direction { get; set; } = "down";
}

// Alias para compatibilidad
public class PlayerMovementDTO : PlayerMovementDto { }