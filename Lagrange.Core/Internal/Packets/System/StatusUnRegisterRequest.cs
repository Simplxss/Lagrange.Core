using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.System;

/// <summary>
/// trpc.qq_new_tech.status_svc.StatusService.UnRegister
/// </summary>
[ProtoContract]
internal class StatusUnRegisterRequest
{
    [ProtoMember(1)] public int U1 { get; set; }
    
    [ProtoMember(2)] public DeviceInfo? DeviceInfo { get; set; }
    
    [ProtoMember(3)] public int U3 { get; set; }
}