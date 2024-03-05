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

    protected override BinaryPacket ConstructBody()
    {
        var packet = new BinaryPacket().WriteByte(0); // known const
        packet.Barrier(w =>
        {
            w.WriteUint((uint)AppInfo.AppId)
                .WriteUint(0x00000072) // const
                .WriteUshort(0) // const 0
                .WriteByte(0) // const 0
                .WriteUint((uint)DateTimeOffset.Now.ToUnixTimeSeconds()) // length actually starts here
                .WriteByte(0x02) // header for packet, counted into length of next barrier manually
                .Barrier(w => w
                    .WriteUshort(_qrCodeCommand)
                    .WriteUlong(0) // const 0
                    .WriteUint(0) // const 0
                    .WriteUlong(0) // const 0 
                    .WriteUshort(3) // const 3
                    .WriteUshort(0) // const 0
                    .WriteUshort(50) // unknown const
                    .WriteUlong(0)
                    .WriteUint(0)
                    .WriteUshort(0)
                    .WriteUint((uint)AppInfo.AppId)
                    .WritePacket(ConstructTransEmp()), Prefix.Uint16 | Prefix.WithPrefix, 1); // addition is the packet start counted in

        }, Prefix.Uint16 | Prefix.WithPrefix, -13); // -13 is the length of zeros, which could be found at TransEmp31 and TransEmp12.ConstructTransEmp()

        packet.Barrier(writer => 
        {
            writer.WriteUint(AppInfo.AppId)
                .WriteUint(0x00000072) // const
                .WriteUshort(0) // const 0
                .WriteByte(0) // const 0
                .WriteUint((uint)DateTimeOffset.Now.ToUnixTimeSeconds()); // length actually starts here

            writer.WriteByte(0x02); // header for packet, counted into length of next barrier manually

            writer.Barrier(w => w
                .WriteUshort(_qrCodeCommand)
                .WriteUlong(0) // const 0
                .WriteUint(0) // const 0
                .WriteUlong(0) // const 0 
                .WriteUshort(3) // const 3
                .WriteUshort(0) // const 0
                .WriteUshort(50) // unknown const
                .WriteUlong(0)
                .WriteUint(0)
                .WriteUshort(0)
                .WriteUint(AppInfo.AppId)
                .WritePacket(ConstructTransEmp()), Prefix.Uint16 | Prefix.WithPrefix, 2); // addition is the packet start counted in

            writer.WriteByte(0x03); // packet end
        }, Prefix.Uint16 | Prefix.WithPrefix, -13); // -13 is the length of zeros, which could be found at TransEmp31 and TransEmp12.ConstructTransEmp()

        return packet;
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

    protected abstract BinaryPacket ConstructTransEmp();


    internal enum State : byte
    {
        Confirmed = 0,
        CodeExpired = 17,
        WaitingForScan = 48,
        WaitingForConfirm = 53,
        Canceled = 54,
    }
}