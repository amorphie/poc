using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Jose;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509.Extension;
using B = Org.BouncyCastle.X509; //Bouncy certificates

// Step 1: Create a Key Pair for the CA
AsymmetricCipherKeyPair caKeyPair = GenerateKeyPair();


// Step 2: Create a CA Certificate
B.X509Certificate caCert = GenerateCACertificate(
    "CN=CA",                // Common Name for the CA
    caKeyPair,              // CA key pair
    "SHA256withRSA",        // Signature algorithm
    365                     // Validity period in days
);

ExportPfxFile(caCert, caKeyPair, "1234", "ca");
ExportCerFile(caCert, "ca");
// ExportCsrFile(caKeyPair, "ca");
ExportKeyPairFile(caKeyPair, "ca");
//----------------------------

// Step 3: Create a Self-Signed Certificate using the CA Key Pair

AsymmetricCipherKeyPair serverKeyPair = GenerateKeyPair();
B.X509Certificate serverCert = GenerateSelfSignedCertificate(
    "CN=test.com",            // Common Name for the entity
    serverKeyPair,          // Entity key pair
    caKeyPair,              // CA key pair
    caCert,                 // CA certificate
    "SHA256withRSA",        // Signature algorithm
    365                     // Validity period in days
);

ExportPfxFile(serverCert, serverKeyPair, "1234", "Server");
ExportCerFile(serverCert, "Server");
ExportKeyPairFile(serverKeyPair, "Server");

//----------------------------

// Step 3: Create a Self-Signed Certificate using the CA Key Pair
AsymmetricCipherKeyPair clientKeyPair = GenerateKeyPair();
B.X509Certificate clientCert = GenerateSelfSignedCertificate(
    "CN=Client",            // Common Name for the entity
    clientKeyPair,          // Entity key pair
    caKeyPair,              // CA key pair
    caCert,                 // CA certificate
    "SHA256withRSA",        // Signature algorithm
    365                     // Validity period in days
);


BigInteger serialNumber = clientCert.SerialNumber;
// clientCert.IssuerUniqueID;
// clientCert.IssuerDN;
string thumbprint = DotNetUtilities.ToX509Certificate(clientCert).GetCertHashString();

ExportPfxFile(clientCert, clientKeyPair, "1234", "Client");
ExportCerFile(clientCert, "Client");
ExportKeyPairFile(clientKeyPair, "Client");
//----------------------------

// Step 3: Load the key pair from files
AsymmetricCipherKeyPair loadedKeyPair = LoadKeyPairFromFile("ca");
// Step 4: Load the CA certificate from file
B.X509Certificate loadedCACert = LoadCACertificateFromFile("ca");


X509Certificate2 ca_certificate = new X509Certificate2("ca-certificate.pfx", "1234");
Console.WriteLine($"Ca - Has Private {ca_certificate.HasPrivateKey}");

X509Certificate2 entity_pfx_certificateSign = new X509Certificate2("server-certificate.pfx", "1234");
Console.WriteLine($"entity_pfx_certificateSign - Has Private {entity_pfx_certificateSign.HasPrivateKey}");

X509Certificate2 entity_crt_certificateVerify = new X509Certificate2("server-certificate.cer");
Console.WriteLine($"entity_crt_certificateVerify - Has Private {entity_crt_certificateVerify.HasPrivateKey}");
// SignSample(entity_pfx_certificateSign, entity_crt_certificateVerify);
Sign(entity_pfx_certificateSign, entity_crt_certificateVerify);

SignJWS(entity_pfx_certificateSign, entity_crt_certificateVerify);

EncryptDecrypt(entity_crt_certificateVerify, entity_pfx_certificateSign);

Console.ReadLine();


B.X509Certificate GenerateCACertificate(string dn, AsymmetricCipherKeyPair keyPair, string signatureAlgorithm, int validityDays)
{
    var certificateGenerator = new B.X509V3CertificateGenerator();

    var serialNumber = BigInteger.ProbablePrime(120, new Random());

    certificateGenerator.SetSerialNumber(serialNumber);
    certificateGenerator.SetSubjectDN(new X509Name(dn));
    certificateGenerator.SetIssuerDN(new X509Name(dn));
    certificateGenerator.SetNotBefore(DateTime.Now);
    certificateGenerator.SetNotAfter(DateTime.Now.AddDays(validityDays));

    certificateGenerator.SetPublicKey(keyPair.Public);

    // Extension to mark the certificate as a CA certificate
    certificateGenerator.AddExtension(X509Extensions.BasicConstraints, true, new BasicConstraints(true));

    var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, keyPair.Private);
    var certificate = certificateGenerator.Generate(signatureFactory);

    return certificate;
}

B.X509Certificate GenerateSelfSignedCertificate(string dn, AsymmetricCipherKeyPair entityKeyPair, AsymmetricCipherKeyPair caKeyPair, B.X509Certificate caCert, string signatureAlgorithm, int validityDays)
{
    var certificateGenerator = new B.X509V3CertificateGenerator();

    var serialNumber = BigInteger.ProbablePrime(120, new Random());

    certificateGenerator.SetSerialNumber(serialNumber);
    certificateGenerator.SetSubjectDN(new X509Name(dn));
    certificateGenerator.SetIssuerDN(caCert.SubjectDN);
    certificateGenerator.SetNotBefore(DateTime.Now);
    certificateGenerator.SetNotAfter(DateTime.Now.AddDays(validityDays));

    certificateGenerator.SetPublicKey(entityKeyPair.Public);

    // Authority Key Identifier extension to link the entity cert to the CA cert
    certificateGenerator.AddExtension(X509Extensions.AuthorityKeyIdentifier, false, new AuthorityKeyIdentifierStructure(caCert.GetPublicKey()));

    var signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, caKeyPair.Private);
    var certificate = certificateGenerator.Generate(signatureFactory);

    return certificate;
}

AsymmetricCipherKeyPair GenerateKeyPair()
{
    var keyGenerationParameters = new KeyGenerationParameters(new SecureRandom(), 2048);
    var keyPairGenerator = GeneratorUtilities.GetKeyPairGenerator("RSA");
    keyPairGenerator.Init(keyGenerationParameters);
    return keyPairGenerator.GenerateKeyPair();
}


void ExportPfxFile(B.X509Certificate certificate, AsymmetricCipherKeyPair keyPair, string password, string fileNamePrefix)
{
    var store = new Pkcs12StoreBuilder().Build();
    var certEntry = new X509CertificateEntry(certificate);
    store.SetKeyEntry($"{fileNamePrefix}-key", new AsymmetricKeyEntry(keyPair.Private), new[] { certEntry });

    using (var fileStream = new FileStream($"{fileNamePrefix}-certificate.pfx", FileMode.Create, FileAccess.ReadWrite))
    {
        store.Save(fileStream, password.ToCharArray(), new SecureRandom());
    }
}

void ExportKeyPairFile(AsymmetricCipherKeyPair keyPair, string fileNamePrefix)
{
    // Save private key to file
    using (var writer = new StreamWriter($"{fileNamePrefix}-certificate-private-key.key")) // or .pem
    {
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(keyPair.Private);
    }

    // Save public key to file
    using (var writer = new StreamWriter($"{fileNamePrefix}-certificate-public-key.key")) // or .pem
    {
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(keyPair.Public);
    }
}

void ExportCerFile(B.X509Certificate certificate, string fileNamePrefix)
{
    using (var writer = new StreamWriter($"{fileNamePrefix}-certificate.cer")) // or .pem
    {
        var pemWriter = new PemWriter(writer);
        pemWriter.WriteObject(certificate);
    }
}


AsymmetricCipherKeyPair LoadKeyPairFromFile(string fileNamePrefix)
{
    // Load private key from file
    using (var reader = new StreamReader($"{fileNamePrefix}-certificate-private-key.key"))
    {
        var pemReader = new PemReader(reader);
        var keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();
        return keyPair;
    }
}

B.X509Certificate LoadCACertificateFromFile(string fileNamePrefix)
{
    // Load CA certificate from file
    using (var reader = new StreamReader($"{fileNamePrefix}-certificate.cer"))
    {
        var pemReader = new PemReader(reader);
        var certificate = (B.X509Certificate)pemReader.ReadObject();
        return certificate;
    }
}

void Sign(X509Certificate2 certificateSign, X509Certificate2 certificateVerify)
{
    // Sample data to sign
    string originalData = "This is the original data to be signed and verified.";
    byte[] dataBytes = Encoding.UTF8.GetBytes(originalData);

    // Sign the data using the entity's private key
    byte[] signature = SignData(originalData, certificateSign.GetRSAPrivateKey());

    // Verify the signature using the entity's public key
    bool isVerified = VerifyData(dataBytes, signature, certificateVerify.GetRSAPublicKey());

    Console.WriteLine("Original Data: " + originalData);
    Console.WriteLine("Signature Verified: " + isVerified);

    byte[] SignData(string originalData, RSA privateKey)
    {
        byte[] data = Encoding.UTF8.GetBytes(originalData);

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            byte[] signature = privateKey.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signature;
        }
    }

    bool VerifyData(byte[] data, byte[] signature, RSA publicKey)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hash = sha256.ComputeHash(data);
            return publicKey.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }
    }
}

void SignJWS(X509Certificate2 certificateSign, X509Certificate2 certificateVerify)
{

    string jws = signJWS();
    bool isValid = verifyJWS(jws);


    Console.WriteLine("JWS verification result: " + isValid);

    string signJWS()
    {
        // Create JWS header
        var jwsHeader = new Dictionary<string, object>()
            {
                { "alg", "RS256" },
                { "kid", Guid.NewGuid().ToString() }
            };

        // Create JWS payload
        var payload = new Dictionary<string, object>()
            {
                { "client_id", "my_client_id" },
                { "scope", "openid" }
    };

        var privateKey = certificateSign.GetRSAPrivateKey();
        return JWT.Encode(payload, privateKey, JwsAlgorithm.RS256, extraHeaders: jwsHeader);
    }

    bool verifyJWS(string jws)
    {
        var publicKey = certificateVerify.GetRSAPublicKey();

        try
        {
            JWT.Decode(jws, publicKey, JwsAlgorithm.RS256);
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}

void EncryptDecrypt(X509Certificate2 publicCertificate, X509Certificate2 privateCertificate)
{

    // Encrypt and decrypt a message using this a secret message!";
    string originalMessage = "Hello, this is a secret message!";
    byte[] encryptedMessage = EncryptWithCertificate(originalMessage, publicCertificate);
    string decryptedMessage = DecryptWithCertificate(encryptedMessage, privateCertificate);

    Console.WriteLine("Original Message: " + originalMessage);
    Console.WriteLine("Encrypted Message: " + Convert.ToBase64String(encryptedMessage));
    Console.WriteLine("Decrypted Message: " + decryptedMessage);

    byte[] EncryptWithCertificate(string message, X509Certificate2 certificate)
    {
        using (RSA rsa = (RSA)certificate.PublicKey.Key)
        {
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = rsa.Encrypt(messageBytes, RSAEncryptionPadding.OaepSHA256);
            return encryptedBytes;
        }
    }

    string DecryptWithCertificate(byte[] encryptedBytes, X509Certificate2 certificate)
    {
        using (RSA rsa = (RSA)certificate.PrivateKey)
        {
            byte[] decryptedBytes = rsa.Decrypt(encryptedBytes, RSAEncryptionPadding.OaepSHA256);
            string decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
            return decryptedMessage;
        }
    }

}

string GetThumbprint(B.X509Certificate certificate)
{
    if (certificate == null)
        throw new ArgumentNullException(nameof(certificate));

    return DotNetUtilities.ToX509Certificate(certificate).GetCertHashString();

}
