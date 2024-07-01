using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Packets;

internal class SsoPacket : IDisposable
{
    public uint PacketType { get; }

    public byte EncodeType { get; }

    public string Command { get; }

    public byte[] MsgCookie { get; }

    public int Sequence { get; }

    public BinaryPacket Payload { get; }

    public int RetCode { get; }

    public string? Extra { get; }

    public SsoPacket(uint packetType, byte encodeType, string command, byte[] msgCookie, int sequence, BinaryPacket payload)
    {
        PacketType = packetType;
        EncodeType = encodeType;
        Command = command;
        MsgCookie = msgCookie;
        Sequence = sequence;
        Payload = payload;
    }

    public SsoPacket(string command, byte[] msgCookie, int sequence, BinaryPacket payload, int retCode, string extra)
    {
        Command = command;
        MsgCookie = msgCookie;
        Sequence = sequence;
        Payload = payload;
        RetCode = retCode;
        Extra = extra;
    }

    public void Dispose() => Payload.Dispose();
}