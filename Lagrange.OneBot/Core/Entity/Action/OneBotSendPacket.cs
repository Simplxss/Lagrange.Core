using System.Text.Json.Serialization;

namespace Lagrange.OneBot.Core.Entity.Action;

[Serializable]
public class OneBotSendPacket
{
    [JsonPropertyName("data")] public string Data { get; set; } = string.Empty;
    
    [JsonPropertyName("command")] public string Command { get; set; } = string.Empty;
    
    [JsonPropertyName("sign")] public bool Sign { get; set; }

    [JsonPropertyName("packet_type")] public byte PacketType { get; set; } = 12;

    [JsonPropertyName("encode_type")] public byte EncodeType { get; set; } = 1;
}