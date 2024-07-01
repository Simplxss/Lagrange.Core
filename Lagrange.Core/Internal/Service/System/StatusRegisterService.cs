using Lagrange.Core.Common;
using Lagrange.Core.Internal.Event;
using Lagrange.Core.Internal.Event.System;
using Lagrange.Core.Internal.Packets.System;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Extension;
using ProtoBuf;

namespace Lagrange.Core.Internal.Service.System;

[EventSubscribe(typeof(InfoSyncEvent))]
[Service("trpc.msg.register_proxy.RegisterProxy.SsoInfoSync")]
internal class StatusRegisterService : BaseService<InfoSyncEvent>
{
    protected override bool Build(InfoSyncEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packet = new SsoInfoSyncRequest
        {
            SyncFlag = 735,
            ReqRandom = (uint)Random.Shared.Next(),
            CurActiveStatus = 2,
            GroupLastMsgTime = 0,
            C2CInfoSync = new SsoC2CInfoSync
            {
                C2CMsgCookie = new SsoC2CMsgCookie
                {
                    C2CLastMsgTime = 0
                },
                C2CLastMsgTime = 0,
                LastC2CMsgCookie = new SsoC2CMsgCookie
                {
                    C2CLastMsgTime = 0
                }
            },
            NormalConfig = new NormalConfig
            {
                IntCfg = new Dictionary<uint, int> { { 46, 0 }, { 283, 0 } }
            },
            RegisterInfo = new RegisterInfo
            {
                Guid = device.System.Guid.ToByteArray().Hex(true),
                KickPC = 0,
                CurrentVersion = appInfo.CurrentVersion,
                IsFirstRegisterProxyOnline = 1,
                LocaleId = 2052,
                DeviceInfo = new OnlineDeviceInfo
                {
                    DevName = device.Model.DeviceName,
                    DevType = appInfo.Kernel,
                    OsVer = device.System.OsType,
                    Brand = "",
                    VendorOsName = appInfo.VendorOs,
                },
                SetMute = 0,
                RegisterVendorType = 6,
                RegType = 0,
                BusinessInfo = new OnlineBusinessInfo
                {
                    NotifySwitch = 1,
                    BindUinNotifySwitch = 1
                },
                BatteryStatus = 100
            },
            UnknownStructure = new UnknownStructure
            {
                GroupCode = 0,
                Flag2 = 2
            },
            AppState = new CurAppState
            {
                IsDelayRequest = 0,
                AppStatus = 1,
                SilenceStatus = 1
            }
        };

        output = packet.Serialize();
        extraPackets = null;
        return true;
    }

    protected override bool Parse(Span<byte> input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out InfoSyncEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var response = Serializer.Deserialize<SsoInfoSyncResponse>(input);

        output = InfoSyncEvent.Result(response?.RegisterInfoResponse?.Message ?? "IDK");
        extraEvents = null;
        return true;
    }
}