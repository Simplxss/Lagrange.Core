using ProtoBuf;

namespace Lagrange.Core.Internal.Packets.System;

// ReSharper disable once InconsistentNaming

/// <summary>
/// trpc.qq_new_tech.status_svc.StatusService.SsoHeartBeat
/// </summary>
[ProtoContract]
internal class SsoHeartBeatRequest
{
    [ProtoMember(1)] public uint Type { get; set; }
    
    [ProtoMember(2)] public SilenceState? LocalSilence { get; set; }

    [ProtoMember(3)] public uint? BatteryStatus { get; set; }

    [ProtoMember(4)] public ulong? Time { get; set; }
}

[ProtoContract]
internal class SilenceState
{
    [ProtoMember(1)] public List<uint>? SilenceStatus { get; set; }
}

[ProtoContract]
internal class SsoHeartBeatResponse
{
    [ProtoMember(3)] public int Interval { get; set; }
}