using Lagrange.Core.Common;
using Lagrange.Core.Internal.Packets.System;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Compression;
using Lagrange.Core.Utility.Extension;
using Lagrange.Core.Utility.Sign;

namespace Lagrange.Core.Internal.Packets;

internal static class SsoPacker
{
    /// <summary>
    /// Build Protocol SSO packet
    /// </summary>
    public static byte[] Build(SsoPacket packet, BotAppInfo appInfo, BotDeviceInfo device, BotKeystore keystore, SignProvider signProvider)
    {
        var signature = signProvider.Sign(appInfo, device, keystore, packet.Command, packet.Sequence, packet.Payload.ToArray());

        var writer = packet.PacketType switch
        {
            10 => new BinaryPacket()
                .Barrier(w => w
                    .WriteInt(packet.Sequence) // sequence
                    .WriteUint(appInfo.SubAppId) // appId
                    .WriteUint(appInfo.SubAppId) // appId
                    .WriteBytes(new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00 }, Prefix.None)
                    .WriteBytes(keystore.Session.Tgt, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteString(packet.Command, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(keystore.Session.MsgCookie, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteString(device.System.AndroidId, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(keystore.Session.Ksid, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteString($"|{device.Model.Imsi}|{appInfo.CurrentVersion}", Prefix.Uint16 | Prefix.WithPrefix) // Actually at wtlogin.trans_emp, this string is empty and only prefix 00 02 is given, but we can just simply ignore that situation
                    .WriteBytes(signature, Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix),
            11 => new BinaryPacket()
                .Barrier(w => w
                    .WriteString(packet.Command, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(keystore.Session.MsgCookie, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(signature, Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix),
            12 => new BinaryPacket()
                .Barrier(w => w
                    .WriteInt(packet.Sequence) // sequence
                    .WriteUint(appInfo.SubAppId) // appId
                    .WriteUint(2052) // LocaleId
                    .WriteBytes(new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }, Prefix.None)
                    .WriteBytes(keystore.Session.Tgt, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteString(packet.Command, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(keystore.Session.MsgCookie, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteString(device.System.Guid.ToByteArray().Hex(true), Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(Array.Empty<byte>(), Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteString(appInfo.CurrentVersion, Prefix.Uint16 | Prefix.WithPrefix) // Actually at wtlogin.trans_emp, this string is empty and only prefix 00 02 is given, but we can just simply ignore that situation
                    .WriteBytes(signature, Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix),
            13 => new BinaryPacket()
                .Barrier(w => w
                    .WriteString(packet.Command, Prefix.Uint32 | Prefix.WithPrefix)
                    .WriteBytes(Array.Empty<byte>(), Prefix.Uint32 | Prefix.WithPrefix) // TODO: Unknown
                    .WritePacket(new NTPacketUid { Uid = keystore.Uid }.Serialize(), Prefix.Uint32 | Prefix.WithPrefix), Prefix.Uint32 | Prefix.WithPrefix),
            _ => throw new Exception($"Unknown packet type: {packet.PacketType}")
        };

        writer.WriteBytes(packet.Payload.ToArray(), Prefix.Uint32 | Prefix.WithPrefix);
        return writer.ToArray();
    }

    /// <summary>
    /// Parse Protocol 10, 11, 12 and 13 SSO packet
    /// </summary>
    public static SsoPacket Parse(uint protocol, byte encode, BinaryPacket packet)
    {
        var head = new BinaryPacket(packet.ReadBytes(Prefix.Uint32 | Prefix.WithPrefix));
        int sequence = head.ReadInt();
        int retCode = head.ReadInt();
        string extra = head.ReadString(Prefix.Uint32 | Prefix.WithPrefix);
        string command = head.ReadString(Prefix.Uint32 | Prefix.WithPrefix);
        byte[] msgCookie = head.ReadBytes(Prefix.Uint32 | Prefix.WithPrefix).ToArray();
        int isCompressed = head.ReadInt();
        // if (protocol == 10)
        //     head.ReadBytes(Prefix.Uint32 | Prefix.WithPrefix);
        // else
        //     head.ReadBytes(Prefix.Uint32 | Prefix.LengthOnly);

        var body = new BinaryPacket(packet.ReadBytes(Prefix.Uint32 | Prefix.WithPrefix));
        return new SsoPacket(command, msgCookie, sequence, isCompressed == 0 ? body : InflatePacket(body), retCode, extra);
    }

    private static BinaryPacket InflatePacket(BinaryPacket original)
    {
        var decompressed = ZCompression.ZDecompress(original.ToArray());
        return new BinaryPacket(decompressed);
    }
}