using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.System;

[ProtoContract]
internal class SsoLoginMerge
{
    [ProtoMember(1)] public BusiBuffItem[]? BuffList { get; set; }
    [ProtoMember(2)] public int? MaxRespSizeHint { get; set; }
}

[ProtoContract]
internal class BusiBuffItem
{
    [ProtoMember(1)] public int Seq { get; set; }
    [ProtoMember(2)] public string? Cmd { get; set; }
    [ProtoMember(3)] public uint Size { get; set; }
    [ProtoMember(4)] public byte[]? Data { get; set; }
    [ProtoMember(5)] public bool NeedResp { get; set; }
}