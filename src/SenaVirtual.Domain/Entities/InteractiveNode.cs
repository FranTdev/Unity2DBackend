namespace SenaVirtual.Domain.Entities;

public class InteractiveNode 
{
    public string Id { get; set; } = string.Empty;
    public string RoomId { get; set; } = string.Empty;
    public float PositionX { get; set; } = 0; // Representa el valor por defecto
    public float PositionY { get; set; } = 0; 
    public string Type { get; set; } = "Link"; // PDF, Board, Quiz... etc

    // Contiene la configuracion (Ej. { "url": "https://example.com" })
    public string MetadataJSON { get; set; } = "{}"; // Representa un objeto JSON vacío por defecto
}