using Lagrange.Core.Utility.Crypto.Provider.Ecdh;

namespace Lagrange.Core.Utility.Crypto;

internal partial class EcdhImpl
{
    private readonly EcdhProvider _ecdhProvider;

    public CryptMethod Method { get; }

    public byte[] ShareKey { get; private set; }

    public EcdhImpl(CryptMethod method, bool isHash = true)
    {
        Method = method;

        var curve = CurveTable[method]; // Select the curve

        _ecdhProvider = new EcdhProvider(curve.Curve);

        ShareKey = GenerateShared(curve.PublicKey, isHash);
    }

    public byte[] GenerateShared(byte[] bobPublic, bool isHash = true)
    {
        var unpack = _ecdhProvider.UnpackPublic(bobPublic);
        ShareKey = _ecdhProvider.KeyExchange(unpack, isHash);

        return ShareKey;
    }

    public byte[] GetPublicKey(bool compress = true) => _ecdhProvider.PackPublic(compress);
}