using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.Service.Oidb.Request;

// Resharper Disable InconsistentNaming

[ProtoContract]
[OidbSvcTrpcTcp(0xfe1, 8)]
internal class OidbSvcTrpcTcp0xFE1_8
{
    [ProtoMember(1)] public string? Uid { get; set; }
    
    [ProtoMember(2)] public uint Field2 { get; set; }
    
    [ProtoMember(3)] public OidbSvcTrpcTcp0xFE1_8Key? Keys { get; set; } // can be regarded as constants
}

[ProtoContract]
internal class OidbSvcTrpcTcp0xFE1_8Key
{
    [ProtoMember(1)] public List<uint>? Key { get; set; } // 傻逼

    [ProtoMember(3)] public uint Field3 { get; set; } // 傻逼
}