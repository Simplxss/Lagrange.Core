using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Lagrange.Core.Common;
using Lagrange.Core.Internal.Packets.System;
using Lagrange.Core.Utility.Extension;
using Lagrange.Core.Utility.Generator;
using ProtoBuf;

namespace Lagrange.Core.Utility.Sign;

internal class LinuxSigner : SignProvider
{
    private const string Url = "https://sign.lagrangecore.org/api/sign/30366";
    private const string SignUrl = $"{Url}/sign";

    private readonly HttpClient _client = new();

    public override byte[] Sign(BotAppInfo appInfo, BotDeviceInfo device, BotKeystore keystore, string cmd, int seq, byte[] body)
    {
        var signature = new ReserveFields
        {
            TraceParent = StringGen.GenerateTrace(),
            Uid = keystore.Uid
        };
        var stream = new MemoryStream();
        Serializer.Serialize(stream, signature);
        if (!WhiteListCommand.Contains(cmd)) return stream.ToArray();
        if (string.IsNullOrEmpty(Url)) return stream.ToArray();

        try
        {
            var payload = new JsonObject
            {
                { "cmd", cmd },
                { "seq", seq },
                { "src", body.Hex() },
            };
            
            var message = _client.PostAsJsonAsync(SignUrl, payload).Result;
            string response = message.Content.ReadAsStringAsync().Result;
            var json = JsonSerializer.Deserialize<JsonObject>(response);

            var secSig = json?["value"]?["sign"]?.ToString().UnHex();
            var secDeviceToken = json?["value"]?["token"]?.ToString().UnHex();
            var secExtra = json?["value"]?["extra"]?.ToString().UnHex();

            signature.SecInfo = new()
            {
                SecSig = secSig,
                SecDeviceToken = secDeviceToken == null ? null : Encoding.UTF8.GetString(secDeviceToken),
                SecExtra = secExtra,
            };
            stream = new MemoryStream();
            Serializer.Serialize(stream, signature);
            return stream.ToArray();
        }
        catch (Exception)
        {
            return stream.ToArray(); // Dummy signature
        }
    }
}