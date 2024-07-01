namespace Lagrange.Core.Internal.Event.Login;

internal class LoginCommon
{
    // 140022017: 安全提醒: 登录失败，请前往QQ官网im.qq.com下载最新版QQ后重试，或通过问题反馈与我们联系。
    public enum Error : uint
    {
        preLoginDeviceVerify = 140022007,
        CaptchaVerify = 140022008,
        NewDeviceVerify = 140022010,
        UnusualVerify = 140022011,
        TokenExpired = 140022015,
        Success = 0,
        Unknown = 1,
    }

    public enum State : uint
    {
        Success = 0,
        PasswordError = 1,
        CaptchaVerify = 2,
        Recycle = 32,
        Freeze = 40,
        DeviceLock = 160,
        SmsSendFail = 162,
        SmsVerifyError = 163,
        Rollback = 180,
        DeviceLockVerify = 204,
        VersionLow = 235,
        NetworkAbnormal = 237,
        DeviceLock2 = 239,
        Unknown = 240,
        IllegalSource = 243,
    }
}