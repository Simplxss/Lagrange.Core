using Lagrange.Core.Common;
using Lagrange.Core.Internal.Event;
using Lagrange.Core.Internal.Event.System;
using Lagrange.Core.Internal.Packets.System;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Extension;
using ProtoBuf;

namespace Lagrange.Core.Internal.Service.System;

[EventSubscribe(typeof(StatusUnRegisterEvent))]
[Service("trpc.qq_new_tech.status_svc.StatusService.UnRegister")]
internal class StatusRegisterService : BaseService<StatusUnRegisterEvent>
{
    protected override bool Build(StatusUnRegisterEvent input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out BinaryPacket output, out List<BinaryPacket>? extraPackets)
    {
        var packet = new StatusUnRegisterRequest
        {
            U1 = 0,
            DeviceInfo = new DeviceInfo
            {
                DevName = device.Model.DeviceName,
                DevType = appInfo.Kernel,
                OsVer = device.System.OsVersion,
                Brand = device.System.OsType,
                VendorOsName = appInfo.VendorOs,
            },
            U3 = 1
        };

        output = packet.Serialize();
        extraPackets = null;
        return true;
    }

    protected override bool Parse(Span<byte> input, BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device,
        out StatusUnRegisterEvent output, out List<ProtocolEvent>? extraEvents)
    {
        var response = Serializer.Deserialize<RegisterInfoResponse>(input);

        output = StatusUnRegisterEvent.Result(response.Message!);
        extraEvents = null;
        return true;
    }
}