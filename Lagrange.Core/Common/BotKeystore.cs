using System.Text;
using Lagrange.Core.Utility.Binary;
using Lagrange.Core.Utility.Crypto;
using Lagrange.Core.Utility.Extension;
using Lagrange.Core.Utility.Generator;
using Lagrange.Core.Utility.Tencent;
using Lagrange.Core.Utils.Tencent;


namespace Lagrange.Core.Common;

[Serializable]
public class BotKeystore
{
    /// <summary>
    /// Create the Bot keystore for QrCode login
    /// </summary>
    public BotKeystore()
    {
        TeaImpl = new TeaImpl();

        Stub = new KeyCollection();
        Session = new WtLoginSession();
    }

    /// <summary>
    /// Create the Bot keystore
    /// </summary>
    /// <param name="uin">QQ</param>
    /// <param name="password">Password Raw</param>
    internal BotKeystore(uint uin, string password) : this()
    {
        InitPassword(uin, password);
    }

    internal void InitPassword(uint uin, string password)
    {
        Uin = uin;
        PasswordMd5 = Encoding.UTF8.GetBytes(password).Md5().UnHex();

        byte[] tmp = new BinaryPacket().WriteBytes(PasswordMd5, Prefix.None)
            .WriteUint(0)
            .WriteUint(uin)
            .ToArray();
        PasswordWithSalt = tmp.Md5().UnHex();
    }
    public uint Uin { get; set; }

    public string? Uid { get; set; }

    public byte[]? PasswordMd5 { get; set; }
    public byte[]? PasswordWithSalt { get; set; }

    internal TeaImpl TeaImpl { get; set; }

    internal KeyCollection Stub { get; }

    public WtLoginSession Session { get; set; }

    public BotInfo? Info { get; set; }

    public class KeyCollection
    {
        public byte[]? RandomKey { get; set; }
        public byte[] Tgtgt { get; set; } = ByteGen.GenRandomBytes(16); // from tlv01E, size: 16
    }

    public class WtLoginSession
    {
        public DateTime SessionDate { get; set; }
        public byte[]? NoPicSig { get; set; } // android: from tlv16A, size: 80; windows from tlv019, size: 176 size: 88
        public byte[] StWebSig { get; set; } = Array.Empty<byte>(); // from tlv103, size: 64
        public byte[]? A2 { get; set; } // android: from tlv106, size: 160; windows from tlv018, size: 176
        public byte[] Tgt { get; set; } = Array.Empty<byte>(); // from tlv10A, size: 72
        public byte[] Tgtgt { get; set; } = Array.Empty<byte>(); // from tlv10C, size: 16
        public byte[] TgtKey { get; set; } = Array.Empty<byte>(); // from tlv10D, size: 16
        public byte[] StKey { get; set; } = Array.Empty<byte>(); // from tlv10E, size: 16
        public byte[] WtSessionTicket { get; set; } = Array.Empty<byte>(); // from tlv133, size: 48
        public byte[] WtSessionTicketKey { get; set; } = Array.Empty<byte>(); // from tlv134, size: 16
        public byte[] D2 { get; set; } = Array.Empty<byte>(); // from tlv143, size: 88
        public byte[] D2Key { get; set; } = Array.Empty<byte>(); // from tlv305, size: 16
        public byte[] DeviceToken { get; set; } = Array.Empty<byte>(); // from tlv322, size: 16

        internal string Skey { get; set; } = ""; // from tlv120, size: 10
        internal string SuperKey { get; set; } = ""; // from tlv16D, size: 44
        internal Dictionary<string, string> PSkey { get; set; } = new(); // from tlv512

        internal string? T104 { get; set; } // from tlv104, size: 44
        internal byte[]? T105 { get; set; } // from tlv105, size: 36
        internal byte[]? T174 { get; set; } // from tlv174, size: 64
        internal byte[]? T402 { get; set; } // from tlv402, size: 8
        internal byte[]? T403 { get; set; } // from tlv403, size: 8
        internal PowValue? PowValue { get; set; } // from tlv546

        public byte[] MsgCookie { get; set; } = ByteGen.GenRandomBytes(4);
        public byte[] Ksid { get; set; } = ByteGen.GenRandomBytes(16);

        public QImei? QImei { get; set; }

        internal string? PhoneNumber { get; set; } // with area code
        internal string? SmsCode { get; set; }

        internal byte[]? QrSign { get; set; } // size: 24
        internal string? QrSig { get; set; }
        internal string? QrUrl { get; set; }

        internal byte[]? ExchangeKey { get; set; }
        internal byte[]? KeySign { get; set; }
        internal byte[]? UnusualSign { get; set; }
        internal string? UnusualCookies { get; set; }
        internal string? CaptchaUrl { get; set; }
        internal string? NewDeviceVerifyUrl { get; set; }
        internal (string, string, string)? Captcha { get; set; } // (ticket, randStr, aid)

        private ushort _sequence;
        internal ushort Sequence
        {
            get => _sequence++;
            set => _sequence = value;
        }
    }

    public class BotInfo
    {
        public byte Age { get; private set; }

        public byte Gender { get; private set; }

        public string Name { get; private set; }

        public BotInfo(byte age, byte gender, string name)
        {
            Age = age;
            Gender = gender;
            Name = name;
        }

        public override string ToString() => $"Bot name: {Name} | Gender: {Gender} | Age: {Age}";
    }

    internal void ClearSession()
    {
        Session.D2 = Array.Empty<byte>();
        Session.Tgt = Array.Empty<byte>(); 
        Session.D2Key = new byte[16];
    }
}