using Lagrange.Core.Common;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;

namespace Lagrange.Core.Internal.Packets.Tlv;

[Tlv(0x544)]
internal class Tlv544 : TlvBody
{
    public Tlv544(BotAppInfo appInfo, BotDeviceInfo device, BotKeystore keystore, uint cmd, uint subCmd)
    {
        SignT544 = appInfo.SignProvider.Energy(appInfo, device, keystore, $"{cmd:x}_{subCmd:x}");
    }
    [BinaryProperty(Prefix.None)] public byte[] SignT544 { get; set; }
}