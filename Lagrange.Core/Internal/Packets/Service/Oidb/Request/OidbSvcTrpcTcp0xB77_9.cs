using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.Service.Oidb.Request;

#pragma warning disable CS8618
// ReSharper disable InconsistentNaming

/// <summary>
/// Group File Upload
/// </summary>
[ProtoContract]
internal class OidbSvcTrpcTcp0xB77_9
{
    [ProtoMember(1)] public OidbSvcTrpcTcp0x6D6Upload? File { get; set; }
}
