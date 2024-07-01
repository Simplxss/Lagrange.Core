using ProtoBuf;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8618

namespace Lagrange.Core.Internal.Packets.System;

[ProtoContract]
internal class OnlineDeviceInfo
{
    [ProtoMember(1)] public string? DevName { get; set; }
    
    [ProtoMember(2)] public string? DevType { get; set; }
    
    [ProtoMember(3)] public string? OsVer { get; set; }
    
    [ProtoMember(4)] public string? Brand { get; set; }
    
    [ProtoMember(5)] public string? VendorOsName { get; set; }
}