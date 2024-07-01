using System.Collections.Concurrent;
using Lagrange.Core.Common;
using Lagrange.Core.Internal.Packets;
using Lagrange.Core.Utility.Binary;

#pragma warning disable CS4014

namespace Lagrange.Core.Internal.Context;

/// <summary>
/// <para>Translate the protocol event into SSOPacket and further ServiceMessage</para>
/// <para>And Dispatch the packet from <see cref="SocketContext"/> by managing the sequence from Tencent's server</para>
/// <para>Every Packet should be send and received from this context instead of being directly send to <see cref="SocketContext"/></para>
/// </summary>
internal class PacketContext : ContextBase
{
    private readonly ConcurrentDictionary<int, TaskCompletionSource<SsoPacket>> _pendingTasks;

    public PacketContext(ContextCollection collection, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device)
        : base(collection, keystore, appInfo, device)
        => _pendingTasks = new ConcurrentDictionary<int, TaskCompletionSource<SsoPacket>>();

    private byte[] BuildPacket(SsoPacket packet)
    {
        var sso = SsoPacker.Build(packet, AppInfo, DeviceInfo, Keystore, AppInfo.SignProvider);
        var encrypted = packet.EncodeType switch
        {
            0 => sso,
            1 => Keystore.TeaImpl.Encrypt(sso, Keystore.Session.D2Key),
            2 => Keystore.TeaImpl.Encrypt(sso, new byte[16]),
            _ => throw new Exception($"Unknown encode type: {packet.EncodeType}")
        };

        return new BinaryPacket()
            .Barrier(w => w
                .WriteUint(packet.PacketType)
                .WriteByte(packet.EncodeType)
                .WriteBytes(packet.EncodeType == 1 ? Keystore.Session.D2 : Array.Empty<byte>(), Prefix.Uint32 | Prefix.WithPrefix)
                .WriteByte(0) // unknown
                .WriteString(Keystore.Uin.ToString(), Prefix.Uint32 | Prefix.WithPrefix)
            .WriteBytes(encrypted, Prefix.None), Prefix.Uint32 | Prefix.WithPrefix)
            .ToArray();
    }

    /// <summary>
    /// Parse Universal Packet, every service should derive from this, protocol 12 and 13
    /// </summary>
    private SsoPacket ParsePacket(BinaryPacket packet)
    {
        uint length = packet.ReadUint();
        uint protocol = packet.ReadUint();
        byte encode = packet.ReadByte();
        byte flag = packet.ReadByte();
        string uin = packet.ReadString(Prefix.Uint32 | Prefix.WithPrefix);

        if (protocol != 10 && protocol != 11 && protocol != 12 && protocol != 13) throw new Exception($"Unrecognized protocol: {protocol}");
        if (uin != Keystore.Uin.ToString() && protocol == 12) throw new Exception($"Uin mismatch: {uin} != {Keystore.Uin}");

        var encrypted = packet.ReadBytes((int)packet.Remaining);
        var decrypted = encode switch
        {
            0 => encrypted,
            1 => Keystore.TeaImpl.Decrypt(encrypted, Keystore.Session.D2Key),
            2 => Keystore.TeaImpl.Decrypt(encrypted, new byte[16]),
            _ => throw new Exception($"Unknown encode type: {encode}")
        };
        var service = new BinaryPacket(decrypted);
        return SsoPacker.Parse(protocol, encode, service);
    }

    /// <summary>
    /// Send the packet and wait for the corresponding response by the packet's sequence number.
    /// </summary>
    public Task<SsoPacket> SendPacket(SsoPacket packet)
    {
        var task = new TaskCompletionSource<SsoPacket>();
        _pendingTasks.TryAdd(packet.Sequence, task);

        bool _ = Collection.Socket.Send(BuildPacket(packet)).GetAwaiter().GetResult();

        return task.Task;
    }

    /// <summary>
    /// Send the packet and don't wait for the corresponding response by the packet's sequence number.
    /// </summary>
    public async Task<bool> PostPacket(SsoPacket packet)
    {
        return await Collection.Socket.Send(BuildPacket(packet));
    }

    public void DispatchPacket(BinaryPacket packet)
    {
        var sso = ParsePacket(packet);
        Keystore.Session.MsgCookie = sso.MsgCookie;

        if (sso.Sequence > 0 && _pendingTasks.TryRemove(sso.Sequence, out var task))
        {
            if (sso is { RetCode: not 0, Extra: { } extra })
            {
                string msg = $"Packet '{sso.Command}' returns {sso.RetCode} with seq: {sso.Sequence}, extra: {extra}";
                task.SetException(new InvalidOperationException(msg));
            }
            else
            {
                task.SetResult(sso);
            }
        }
        else
        {
            Collection.Business.HandleServerPacket(sso);
        }
    }
}