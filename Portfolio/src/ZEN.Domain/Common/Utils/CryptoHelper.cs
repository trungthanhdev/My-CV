
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace ZEN.Domain.Common.Utils;

public class CryptoHelper 
{
    private readonly byte[] _encryptionKey;
    private readonly byte[] _initializationVector;

    public CryptoHelper(string encryptionKey, string initializationVector = "ffffffffffffffffffffffffffffffff")
    {
        _encryptionKey = Encoding.UTF8.GetBytes(encryptionKey);
        _initializationVector = Convert.FromHexString(initializationVector);
    }

    public string EncryptAes(string message)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _initializationVector;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream();
        using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
        using (var writer = new StreamWriter(cs))
        {
            writer.Write(message);
        }
        return Convert.ToBase64String(ms.ToArray());
    }

    public string DecryptAes(string encryptedMessage)
    {
        var buffer = Convert.FromBase64String(encryptedMessage);

        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.IV = _initializationVector;

        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var ms = new MemoryStream(buffer);
        using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
        using var reader = new StreamReader(cs);

        return reader.ReadToEnd();
    }

    public byte[] EncodeECDSASignature(string dataToSign, string privateKeyPath)
    {
        using var reader = File.OpenText(privateKeyPath);
        var pemReader = new PemReader(reader);
        var privateKey = (AsymmetricCipherKeyPair)pemReader.ReadObject();
        var signer = SignerUtilities.GetSigner("SHA256withECDSA");
        signer.Init(true, privateKey.Private);
        signer.BlockUpdate(Encoding.UTF8.GetBytes(dataToSign), 0, dataToSign.Length);
        return signer.GenerateSignature();
    }

    public bool VerifyECDSASignature(string message, byte[] signature, string publicKeyPath)
    {
        using var reader = File.OpenText(publicKeyPath);
        var pemReader = new PemReader(reader);
        var publicKey = (ECPublicKeyParameters)pemReader.ReadObject();
        var signer = SignerUtilities.GetSigner("SHA256withECDSA");
        signer.Init(false, publicKey);
        signer.BlockUpdate(Encoding.UTF8.GetBytes(message), 0, message.Length);
        return signer.VerifySignature(signature);
    }


}
