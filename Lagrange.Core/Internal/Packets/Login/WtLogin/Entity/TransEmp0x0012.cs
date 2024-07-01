using Lagrange.Core.Common;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;

namespace Lagrange.Core.Internal.Packets.Login.WtLogin.Entity;

internal class TransEmp0x0012 : TransEmp
{
    private const ushort QrCodeCommand = 0x0012;

    public TransEmp0x0012(BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device)
        : base(QrCodeCommand, keystore, appInfo, device) { }

    protected override BinaryPacket ConstructInner()
    {
        if (Keystore.Session.QrSign == null)
        {
            throw new Exception("QrSign is null");
        }

        return new BinaryPacket()
            .WriteUshort(0)
            .WriteUint(AppInfo.AppId)
            .WriteBytes(Keystore.Session.QrSign, Prefix.Uint16 | Prefix.LengthOnly)
            .WriteUlong(Keystore.Uin) // uin
            .WriteByte(0) // version
            .WriteBytes(Array.Empty<byte>(), Prefix.Uint16 | Prefix.LengthOnly)

            .WriteShort(0); // no tlvs
    }

    public static Dictionary<ushort, TlvBody> Deserialize(BinaryPacket packet, out State qrState, out ulong uin)
    {
        if ((qrState = (State)packet.ReadByte()) == State.Confirmed)
        {
            uin = packet.ReadUlong();
            uint time = packet.ReadUint();
            return TlvPacker.ReadTlvCollections(packet);
        }
        
        uin = 0;
        return new Dictionary<ushort, TlvBody>();
    }
}