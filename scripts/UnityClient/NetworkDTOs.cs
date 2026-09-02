using System;

namespace Unity2D.Client
{
    [Serializable]
    public class PlayerMovementDto
    {
        public string PlayerId { get; set; } = string.Empty;
        public float PositionX { get; set; } = 0;
        public float PositionY { get; set; } = 0;
        public string Direction { get; set; } = "down";
    }

    [Serializable]
    public class ChatMessageDto
    {
        public string SenderId { get; set; } = string.Empty;
        public string SenderUsername { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public float SenderX { get; set; }
        public float SenderY { get; set; }
        public bool IsProximity { get; set; } = true;
    }
}
