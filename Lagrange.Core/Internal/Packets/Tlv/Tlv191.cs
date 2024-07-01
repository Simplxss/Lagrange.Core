using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;

namespace Lagrange.Core.Internal.Packets.Tlv;

[Tlv(0x191)]
internal class Tlv191 : TlvBody
{
    public Tlv191(byte codeType) => CodeType = codeType;

    [BinaryProperty] public byte CodeType { get; set; } // CodeType: picture: 0x01, slider captcha: 0x82
}