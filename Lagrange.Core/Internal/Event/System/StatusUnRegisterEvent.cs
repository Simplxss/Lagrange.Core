#pragma warning disable CS8618

namespace Lagrange.Core.Internal.Event.System;

internal class StatusUnRegisterEvent : ProtocolEvent
{
    public string Message { get; set; }

    private StatusUnRegisterEvent() : base(true) { }

    private StatusUnRegisterEvent(string result) : base(0) => Message = result;

    public static StatusUnRegisterEvent Create() => new();

    public static StatusUnRegisterEvent Result(string result) => new(result);
}