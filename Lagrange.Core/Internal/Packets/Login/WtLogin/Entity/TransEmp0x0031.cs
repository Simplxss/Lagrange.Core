using Lagrange.Core.Common;
using Lagrange.Core.Internal.Packets.Tlv;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;

namespace Lagrange.Core.Internal.Packets.Login.WtLogin.Entity;

internal class TransEmp0x0031 : TransEmp
{
    private const ushort QrCodeCommand = 0x0031;

    public TransEmp0x0031(BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device)
        : base(QrCodeCommand, keystore, appInfo, device) { }

    protected override BinaryPacket ConstructInner()
    {
        var packet = new BinaryPacket()
            .WriteUshort(0)
            .WriteUint(AppInfo.AppId)
            .WriteUlong(0) // uin
            .WriteBytes(Array.Empty<byte>()) // TGT
            .WriteByte(0)
            .WriteBytes(Array.Empty<byte>(), Prefix.Uint16 | Prefix.LengthOnly);

        if (Keystore.Session.UnusualSign != null)
        {
            packet.WriteUshort(8)
                .WritePacket(new TlvPacket(0x011, new Tlv011(Keystore)));
        }
        else
        {
            packet.WriteUshort(7);
        }
        packet.WritePacket(new TlvPacket(0x016, new Tlv016(AppInfo, Device)))
            .WritePacket(new TlvPacket(0x01B, new Tlv01B()))
            .WritePacket(new TlvPacket(0x01D, new Tlv01D(AppInfo)))
            .WritePacket(new TlvPacket(0x033, new Tlv033(Device)))
            .WritePacket(new TlvPacket(0x035, new Tlv035(AppInfo)))
            .WritePacket(new TlvPacket(0x066, new Tlv066(AppInfo)))
            .WritePacket(new TlvPacket(0x0D1, new Tlv0D1(AppInfo, Device)));

        return packet;
    }

    public static Dictionary<ushort, TlvBody> Deserialize(BinaryPacket packet, out byte[] signature)
    {
        packet.ReadByte();
        signature = packet.ReadBytes(Prefix.Uint16 | Prefix.LengthOnly).ToArray();
        return TlvPacker.ReadTlvCollections(packet);
    }
}