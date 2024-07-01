using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;
#pragma warning disable CS8618

namespace Lagrange.Core.Internal.Packets.Tlv;

[Tlv(0x17E, true)]
internal class Tlv17E : TlvBody
{
    [BinaryProperty] public string tip { get; set; }
}