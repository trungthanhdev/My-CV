
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

public class ECDSAUtil
{
    private readonly byte[] _encryptionKey;
    private readonly byte[] _initializationVector;

    public ECDSAUtil(string encryptionKey, string initializationVector)
    {
        _encryptionKey = Encoding.UTF8.GetBytes(encryptionKey);
        _initializationVector = Convert.FromHexString(initializationVector);
    }

    public static string StartEncode(string template)
    {
        string encryptionKey = "12af3ad2ddc21d23faff23492baeeef2"; // 32 bytes for AES-256
        string initializationVector = "ffffffffffffffffffffffffffffffff"; // 16 bytes for AES

        var helper = new ECDSAUtil(encryptionKey, initializationVector);

        // Example data
        // string template = "644|57|2|55|56";
        long exp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // Encrypt the data
        string cardInfoStringEncryptAES = helper.EncryptAes(template);

        // Prepare plaintext for signing
        string plaintext = $"{cardInfoStringEncryptAES}{{&&}}{exp}";

        // Sign the data
        byte[] signature = helper.EncodeECDSASignature(plaintext, "private.pem");

        // Format the final output
        string signBase64 = Convert.ToBase64String(signature);
        string format = $"{cardInfoStringEncryptAES}|{signBase64}|{exp}";

        return format;
    }
    public static (string data, bool isSuccess, int type) StartDecode(string template)
    {
        string encryptionKey = "12af3ad2ddc21d23faff23492baeeef2"; // 32 bytes for AES-256
        string initializationVector = "ffffffffffffffffffffffffffffffff"; // 16 bytes for AES

        var splitData = template.Split('|');

        if (splitData.Length != 3)
        {
            return (string.Empty, false, -1);
        }

        var aesData = splitData[0];
        var sign = splitData[1];
        var exp = splitData[2];

        var helper = new ECDSAUtil(encryptionKey, initializationVector);

        var decryptedMessage = helper.DecryptAes(aesData);


        if (string.IsNullOrEmpty(decryptedMessage) || string.IsNullOrWhiteSpace(decryptedMessage))
        {
            return (string.Empty, false, -1);
        }
        var payload = $"{aesData}{{&&}}{exp}";

        var isvalid = helper.VerifyECDSASignature(payload, Convert.FromBase64String(sign), "/public.pem");

        if (isvalid)
        {
            return (decryptedMessage, true, 0);
        }

        return (string.Empty, false, -2);
    }
    public string EncryptAes(string message)
    {
        using (var aes = Aes.Create())
        {
            aes.Key = _encryptionKey;
            aes.IV = _initializationVector;

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (var writer = new StreamWriter(cs))
                    {
                        writer.Write(message);
                    }
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
    }

    public string DecryptAes(string encryptedMessage)
    {
        var buffer = Convert.FromBase64String(encryptedMessage);

        using (var aes = Aes.Create())
        {
            aes.Key = _encryptionKey;
            aes.IV = _initializationVector;

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(buffer))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public byte[] EncodeECDSASignature(string dataToSign, string privateKeyPath)
    {
        using (var reader = File.OpenText(privateKeyPath))
        {
            var pemReader = new PemReader(reader);
            var privateKey = (AsymmetricCipherKeyPair)pemReader.ReadObject();
            var signer = SignerUtilities.GetSigner("SHA256withECDSA");
            signer.Init(true, privateKey.Private);
            signer.BlockUpdate(Encoding.UTF8.GetBytes(dataToSign), 0, dataToSign.Length);
            return signer.GenerateSignature();
        }
    }

    public bool VerifyECDSASignature(string message, byte[] signature, string publicKeyPath)
    {
        var path = AppDomain.CurrentDomain.BaseDirectory;
        using var reader = File.OpenText(path + publicKeyPath);
        var pemReader = new PemReader(reader);
        var publicKey = (ECPublicKeyParameters)pemReader.ReadObject();
        var signer = SignerUtilities.GetSigner("SHA256withECDSA");
        signer.Init(false, publicKey);
        signer.BlockUpdate(Encoding.UTF8.GetBytes(message), 0, message.Length);
        return signer.VerifySignature(signature);
    }
}
