using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lagrange.Core.Internal.Packets.System;
using Lagrange.Core.Utility.Extension;
using Lagrange.Core.Utility.Network;
using Lagrange.Core.Common;
using Lagrange.Core.Utility.Generator;
using ProtoBuf;

namespace Lagrange.Core.Utility.Sign;

internal class AndroidSigner : SignProvider
{
    private const string Url = "https://kritor.support/android/v9.0.20";
    private const string SignUrl = $"{Url}/sign";
    private const string EnergyUrl = $"{Url}/energy";
    private const string GetXwDebugIdUrl = $"{Url}/get_xw_debug_id";

    private readonly HttpClient _client = new();

    public override byte[] Sign(BotAppInfo appInfo, BotDeviceInfo device, BotKeystore keystore, string cmd, int seq, byte[] body)
    {
        var signature = new ReserveFields
        {
            Flag = 1,
            LocaleId = 2052,
            Qimei = keystore.Session.QImei?.Q36,
            NewconnFlag = 0,
            TraceParent = StringGen.GenerateTrace(),
            Uid = keystore.Uid,
            Imsi = 0,
            NetworkType = 1,
            IpStackType = 1,
            MsgType = 0,
            TransInfo = new Dictionary<string, string>{
                { "client_conn_seq", DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() }
            },
            NtCoreVersion = 100,
            SsoIpOrigin = 3
        };
        if (WhiteListCommand.Contains(cmd) && !string.IsNullOrEmpty(Url))
        {
            try
            {
                var payload = new JsonObject
                {
                    { "uin", keystore.Uin },
                    { "cmd", cmd },
                    { "seq", seq },
                    { "buffer", body.Hex() },
                    { "android_id", device.System.AndroidId },
                    { "guid", device.System.Guid.ToByteArray().Hex() }
                };
                var message = _client.PostAsJsonAsync(SignUrl, payload).Result;
                string response = message.Content.ReadAsStringAsync().Result;
                var json = JsonSerializer.Deserialize<JsonObject>(response);

                var secSig = json?["data"]?["sign"]?.ToString().UnHex();
                var secDeviceToken = json?["data"]?["token"]?.ToString().UnHex();
                var secExtra = json?["data"]?["extra"]?.ToString().UnHex();

                signature.SecInfo = new SsoSecureInfo
                {
                    SecSig = secSig,
                    SecDeviceToken = secDeviceToken is null ? null : Encoding.UTF8.GetString(secDeviceToken),
                    SecExtra = secExtra,
                };
            }
            catch (Exception) { }
        }
        var stream = new MemoryStream();
        Serializer.Serialize(stream, signature);
        return stream.ToArray();
    }

    public override byte[] Energy(BotAppInfo appInfo, BotDeviceInfo device, BotKeystore keystore, string data)
    {
        try
        {
            var payload = new JsonObject
            {
                { "uin", keystore.Uin },
                { "data", data },
                { "version", appInfo.BaseVersion },
                { "guid", device.System.Guid.ToByteArray().Hex() }
            };
            var message = _client.PostAsJsonAsync(EnergyUrl, payload).Result;
            string response = message.Content.ReadAsStringAsync().Result;
            var json = JsonSerializer.Deserialize<JsonObject>(response);

            return json?["data"]?.ToString().UnHex() ?? Array.Empty<byte>();
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
    }

    public override byte[] GetXwDebugId(uint uin, string cmd, string subCmd)
    {
        try
        {
            var payload = new Dictionary<string, string>
            {
                { "uin", uin.ToString() },
                { "cmd", cmd },
                { "subCmd", subCmd },
            };
            string response = Http.GetAsync(GetXwDebugIdUrl, payload).GetAwaiter().GetResult();
            var json = JsonSerializer.Deserialize<JsonObject>(response);

            return json?["data"]?.ToString().UnHex() ?? Array.Empty<byte>();
        }
        catch (Exception)
        {
            return Array.Empty<byte>();
        }
    }
}