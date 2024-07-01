using Lagrange.Core.Common;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;
using Lagrange.Core.Utility.Binary.Tlv.Attributes;

namespace Lagrange.Core.Internal.Packets.Tlv;

[Tlv(0x193)]
internal class Tlv193 : TlvBody
{
    public Tlv193(BotKeystore keystore)
    {
        var (ticket, _, _) = keystore.Session.Captcha!.Value;
        Ticket = ticket;
    }

    [BinaryProperty(Prefix.None)] public string Ticket { get; set; }
}