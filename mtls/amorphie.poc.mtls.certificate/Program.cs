using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Jose;

string certPassword = "export_certificate_password";
string certSubject = "cert_subject";

// Generate a self-signed certificate
// X509Certificate2 root = GenerateRootCertificate();

X509Certificate2 certificate = GenerateSelfSignedCertificate();

// SignSample();

// SignJWS();

// EncryptSample();

// ExportPEMSample();

// SaveSample();

// WriteLoadCertificate();

Console.ReadLine();

void SignSample()
{
    // Sample data to sign
    string originalData = "This is the original data to be signed and verified.";
    byte[] dataBytes = Encoding.UTF8.GetBytes(originalData);

    // Sign the data
    byte[] signature = SignData(originalData, certificate.GetRSAPrivateKey());

    // Verify the signature
    bool isVerified = VerifyData(dataBytes, signature, certificate.GetRSAPublicKey());

    Console.WriteLine("Original Data: " + originalData);
    Console.WriteLine("Signature Verified: " + isVerified);

    byte[] SignData(string originalData, AsymmetricAlgorithm privateKey)
    {
        byte[] data = Encoding.UTF8.GetBytes(originalData);

        using (RSA rsa = privateKey as RSA)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                byte[] signature = rsa.SignHash(hash, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return signature;
            }
        }
    }

    bool VerifyData(byte[] data, byte[] signature, AsymmetricAlgorithm publicKey)
    {
        using (RSA rsa = publicKey as RSA)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hash = sha256.ComputeHash(data);
                return rsa.VerifyHash(hash, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }

}


void SignJWS()
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

    var privateKey = certificate.GetRSAPrivateKey();
    var publicKey = certificate.GetRSAPublicKey();



    // Create JWS object
    string jwsObject = JWT.Encode(payload, privateKey, JwsAlgorithm.RS256, extraHeaders: jwsHeader);

    // Print the serialized JWS
    Console.WriteLine("Serialized JWS: " + jwsObject);

    // Verify the JWS
    var isValid = JWT.Decode(jwsObject, publicKey, JwsAlgorithm.RS256);


    Console.WriteLine("JWS verification result: " + isValid);
}

void EncryptSample()
{

    // Encrypt and decrypt a message using this a secret message!";
    string originalMessage = "Hello, this is a secret message!";
    byte[] encryptedMessage = EncryptWithCertificate(originalMessage, certificate);
    string decryptedMessage = DecryptWithCertificate(encryptedMessage, certificate);

    Console.WriteLine("Original Message: " + originalMessage);
    Console.WriteLine("Encrypted Message: " + Convert.ToBase64String(encryptedMessage));
    Console.WriteLine("Decrypted Message: " + decryptedMessage);
}

void SaveSample()
{

    // Export and save the certificate to *.cer file
    SaveCertificateToCerFile(certificate, "certificate.cer");

    // Export and save the certificate with private key to *.pfx file
    SaveCertificateToPfxFile(certificate, "certificate.pfx", certPassword);


    Console.WriteLine("Certificate exported and saved.");
    Console.WriteLine("Public key saved to certificate.cer");
    Console.WriteLine("Private key saved to certificate.pfx");

    void SaveCertificateToCerFile(X509Certificate2 certificate, string fileName)
    {
        string certBase64 = ExportCertificateToPem(certificate);
        File.WriteAllText(fileName, certBase64);
    }

    void SaveCertificateToPfxFile(X509Certificate2 certificate, string fileName, string password)
    {
        string certBase64 = ExportPfxToPem(certificate, password);
        File.WriteAllText(fileName, certBase64);
    }

}

void ExportPEMSample()
{

    // Export and save the certificate in PEM format
    string certificatePem = ExportCertificateToPem(certificate);

    // Export and save the PFX with private key in PEM format
    string pfxPem = ExportPfxToPem(certificate, certPassword);

    Console.WriteLine("Certificate exported as PEM:");
    Console.WriteLine(certificatePem);

    Console.WriteLine("\nPFX with private key exported as PEM:");
    Console.WriteLine(pfxPem);
}

void WriteLoadCertificate()
{
    string pem = ExportCertificateToPem(certificate);
    string pfx = ExportPfxToPem(certificate, certPassword);

    // Encrypt and decrypt a message using this a secret message!";
    string originalMessage = "Hello, this is a secret message!";

    var pemCert = LoadSelfSignedCert(pem);
    var pfxCert = LoadSelfSignedKey(pfx, certPassword);


    byte[] encryptedMessage = EncryptWithCertificate(originalMessage, pemCert);
    string decryptedMessage = DecryptWithCertificate(encryptedMessage, pfxCert);

    Console.WriteLine("Original Message: " + originalMessage);
    Console.WriteLine("Encrypted Message: " + Convert.ToBase64String(encryptedMessage));
    Console.WriteLine("Decrypted Message: " + decryptedMessage);
}

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

string ExportCertificateToPem(X509Certificate2 certificate)
{
    // string certificatePem = certificate.ExportCertificatePem();

    byte[] certData = certificate.Export(X509ContentType.Cert);
    string certBase64 = Convert.ToBase64String(certData);

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("-----BEGIN CERTIFICATE-----");
    sb.AppendLine(certBase64);
    sb.AppendLine("-----END CERTIFICATE-----");

    return sb.ToString();
}

string ExportPfxToPem(X509Certificate2 certificate, string password)
{
    byte[] pfxData = certificate.Export(X509ContentType.Pfx, password);
    string pfxBase64 = Convert.ToBase64String(pfxData);

    StringBuilder sb = new StringBuilder();
    sb.AppendLine("-----BEGIN PFX-----");
    sb.AppendLine(pfxBase64);
    sb.AppendLine("-----END PFX-----");

    return sb.ToString();
}

X509Certificate2 LoadSelfSignedCert(string pemString)
{

    string base64Certificate = pemString
        .Replace("-----BEGIN CERTIFICATE-----", "")
        .Replace("-----END CERTIFICATE-----", "")
        .Replace("\n", "")
        .Replace("\r", ""); // Add this line to handle carriage return characters

    // Add padding if necessary
    while (base64Certificate.Length % 4 != 0)
    {
        base64Certificate += "=";
    }

    // Convert Base64 string to byte array
    byte[] certData = Convert.FromBase64String(base64Certificate);

    // Create X.509 certificate from byte array
    X509Certificate2 certificate = new X509Certificate2(certData);

    return certificate;
}

X509Certificate2 LoadSelfSignedKey(string pfxString, string password)
{
    // Remove the "-----BEGIN PFX-----" and "-----END PFX-----" lines, and any other unnecessary characters
    string cleanedPfxString = pfxString
        .Replace("-----BEGIN PFX-----", "")
        .Replace("-----END PFX-----", "")
        .Replace("\n", "")
        .Replace("\r", "");

    // Convert the Base64 string to a byte array
    byte[] pfxBytes = Convert.FromBase64String(cleanedPfxString);

    // Create the X509Certificate2 from the byte array and password
    return new X509Certificate2(pfxBytes, password, X509KeyStorageFlags.Exportable);
}

X509Certificate2 GenerateSelfSignedCertificate()
{
    using (RSA rsa = RSA.Create(2048))
    {
        var request = new CertificateRequest($"CN={certSubject}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DataEncipherment | X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, false));

        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1"), new Oid("1.3.6.1.5.5.7.3.2") }, false));
        var notBefore = DateTime.UtcNow;
        var notAfter = notBefore.AddYears(1);
        X509Certificate2 certificate = request.CreateSelfSigned(notBefore, notAfter);

        return certificate;
    }

    /*
     * Client Authentication (1.3.6.1.5.5.7.3.2): Indicates that the certificate can be used for client authentication.

Code Signing (1.3.6.1.5.5.7.3.3): Indicates that the certificate can be used for signing code.

Email Protection (1.3.6.1.5.5.7.3.4): Indicates that the certificate can be used for email protection (S/MIME).

Time Stamping (1.3.6.1.5.5.7.3.8): Indicates that the certificate can be used for timestamping.


    request.CertificateExtensions.Add(
    new X509EnhancedKeyUsageExtension(
        new OidCollection
        {
            new Oid("1.3.6.1.5.5.7.3.1"), // Server Authentication
            new Oid("1.3.6.1.5.5.7.3.2"), // Client Authentication
            new Oid("1.3.6.1.5.5.7.3.3"), // Code Signing
            // Add more OIDs as needed
        }, false));
    */

}




