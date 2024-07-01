using Lagrange.Core.Common;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Internal.Packets.Login.WtLogin.Entity;

internal abstract class TransEmp : WtLoginBase
{
    private readonly ushort _qrCodeCommand;

    private const string PacketCommand = "wtlogin.trans_emp";
    private const ushort WtLoginCommand = 2066;
    private const byte WtLoginCmdVer = 135;
    private const byte WtLoginPubId = 19;

    protected TransEmp(ushort qrCmd, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device)
        : base(PacketCommand, WtLoginCommand, WtLoginCmdVer, WtLoginPubId, keystore, appInfo, device)
        => _qrCodeCommand = qrCmd;

    protected override BinaryPacket ConstructData()
    {
        var packet = new BinaryPacket()
            .WriteUint((uint)DateTimeOffset.Now.ToUnixTimeSeconds())
            .WriteByte(2)
            .Barrier(w => w
                .WriteUshort(_qrCodeCommand)
                .WriteBytes(new byte[21])
                .WriteByte(0x03)
                .WriteShort(0x00) // close
                .WriteShort(0x32) // Version Code: 50
                .WriteUint(0) // trans_emp sequence
                .WriteUlong(0) // dummy uin
                .WritePacket(ConstructTlv()), Prefix.Uint16 | Prefix.WithPrefix, 2)
            .WriteByte(3);

        return new BinaryPacket()
            .WriteByte(0x00) // encryptMethod == EncryptMethod.EM_ST || encryptMethod == EncryptMethod.EM_ECDH_ST
            .WriteUshort((ushort)packet.Length)
            .WriteUint(AppInfo.AppId)
            .WriteUint(0x72) // Role
            .WriteBytes(Array.Empty<byte>(), Prefix.Uint16 | Prefix.LengthOnly) // uSt
            .WriteBytes(Array.Empty<byte>(), Prefix.Uint8 | Prefix.LengthOnly) // rollback
            .WritePacket(packet);
    }

    public BinaryPacket DeserializeBody(BotKeystore keystore, BinaryPacket packet, out ushort command)
    {
        packet = DeserializePacket(keystore, packet);

        uint packetLength = packet.ReadUint();
        packet.ReadUshort();
        packet.ReadUshort();
        command = packet.ReadUshort();
        packet.ReadBytes(18);
        packet.ReadUint();
        packet.ReadUint();
        packet.Skip(14);
        uint appId = packet.ReadUint();

        return packet;
    }

    protected abstract BinaryPacket ConstructTlv();

    internal enum State : byte
    {
        Confirmed = 0,
        CodeExpired = 17,
        WaitingForScan = 48,
        WaitingForConfirm = 53,
        Canceled = 54,
    }
}