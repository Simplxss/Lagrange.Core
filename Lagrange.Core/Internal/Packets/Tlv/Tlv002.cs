using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;

namespace Lagrange.Core.Internal.Packets.Tlv;

[Tlv(0x002)]
internal class Tlv002 : TlvBody
{
    [BinaryProperty] public ulong u1 { get; set; }
}