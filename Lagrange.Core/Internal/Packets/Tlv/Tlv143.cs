using Lagrange.Core.Common;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;
#pragma warning disable CS8618

namespace Lagrange.Core.Internal.Packets.Tlv;

[Tlv(0x143)]
internal class Tlv143 : TlvBody
{
    public Tlv143(BotKeystore keystore) => D2 = keystore.Session.D2;

    [BinaryProperty(Prefix.None)] public byte[] D2 { get; set; }
}

[Tlv(0x143, true)]
internal class Tlv143Response : TlvBody
{
    [BinaryProperty(Prefix.None)] public byte[] D2 { get; set; }
}