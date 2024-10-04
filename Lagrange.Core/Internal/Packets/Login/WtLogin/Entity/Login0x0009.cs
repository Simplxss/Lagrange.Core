﻿using Lagrange.Core.Common;
using Lagrange.Core.Internal.Packets.Tlv;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Binary.Tlv;

namespace Lagrange.Core.Internal.Packets.Login.WtLogin.Entity;

internal class Login0x0009 : Login
{
    private const ushort LoginCommand = 0x0009;

    private int SsoSeq;

    public Login0x0009(BotKeystore keystore, BotAppInfo appInfo, BotDeviceInfo device, int ssoSeq)
        : base(LoginCommand, keystore, appInfo, device) => SsoSeq = ssoSeq;

    // Todo: Improve this method
    protected override BinaryPacket ConstructTlv() => new BinaryPacket()
        .WriteShort(0x001B)
        .WritePacket(new TlvPacket(0x018, new Tlv018(Keystore, AppInfo)))
        .WritePacket(new TlvPacket(0x001, new Tlv001(Keystore, false)))
        .WritePacket(new TlvPacket(0x106, new Tlv106(Keystore, AppInfo, Device), (Keystore.TeaImpl, Keystore.PasswordWithSalt!)))
        .WritePacket(new TlvPacket(0x116, new Tlv116(AppInfo)))
        .WritePacket(new TlvPacket(0x100, new Tlv100(AppInfo)))
        .WritePacket(new TlvPacket(0x107, new Tlv107()))
        .WritePacket(new TlvPacket(0x108, new Tlv108(Keystore)))
        .WritePacket(new TlvPacket(0x142, new Tlv142(AppInfo)))
        .WritePacket(new TlvPacket(0x144, new Tlv144(Device), (Keystore.TeaImpl, Keystore.Stub.Tgtgt)))
        .WritePacket(new TlvPacket(0x145, new Tlv145(Device)))
        .WritePacket(new TlvPacket(0x147, new Tlv147(AppInfo)))
        .WritePacket(new TlvPacket(0x154, new Tlv154(SsoSeq)))
        .WritePacket(new TlvPacket(0x141, new Tlv141(Device)))
        .WritePacket(new TlvPacket(0x008, new Tlv008()))
        .WritePacket(new TlvPacket(0x511, new Tlv511(new string[] { "office.qq.com", "qun.qq.com", "gamecenter.qq.com", "docs.qq.com", "mail.qq.com", "tim.qq.com", "ti.qq.com", "vip.qq.com", "tenpay.com", "qqweb.qq.com", "qzone.qq.com", "mma.qq.com", "game.qq.com", "openmobile.qq.com", "connect.qq.com" })))
        .WritePacket(new TlvPacket(0x187, new Tlv187()))
        .WritePacket(new TlvPacket(0x188, new Tlv188(Device)))
        .WritePacket(new TlvPacket(0x191, new Tlv191(0x82)))
        .WritePacket(new TlvPacket(0x177, new Tlv177(AppInfo)))
        .WritePacket(new TlvPacket(0x516, new Tlv516()))
        .WritePacket(new TlvPacket(0x521, new Tlv521(AppInfo)))
        .WritePacket(new TlvPacket(0x525, new Tlv525()))
        .WritePacket(new TlvPacket(0x544, new Tlv544(AppInfo, Device, Keystore, Cmd, LoginCommand)))
        .WritePacket(new TlvPacket(0x545, new Tlv545(Keystore, AppInfo, Device)))
        .WritePacket(new TlvPacket(0x548, new Tlv548()))
        .WritePacket(new TlvPacket(0x553, new Tlv553(AppInfo, Keystore)))
        .WritePacket(new TlvPacket(0x542, new Tlv542()));
}