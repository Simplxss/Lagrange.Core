namespace Lagrange.Core.Event.EventArg;

public class BotNewDeviceVerifyEvent : EventBase
{
    public string? PhoneNumber { get; }

    public string? Url { get; }
    public byte[]? QrCode { get; }
    
    public BotNewDeviceVerifyEvent(string phoneNumber) 
    {
        PhoneNumber = phoneNumber;

        EventMessage = $"[{nameof(BotNewDeviceVerifyEvent)}]: PhoneNumber: {phoneNumber}";
    }
    
    public BotNewDeviceVerifyEvent(string url, byte[] qrCode) 
    {
        Url = url;
        QrCode = qrCode;

        EventMessage = $"[{nameof(BotNewDeviceVerifyEvent)}]: Url: {url}";
    }
}