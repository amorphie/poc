using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace amorphie.poc.mtls.certificate;


public class KeyPair
{
    public int Id { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
}
/*
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using amorphie.poc.mtls.certificate;



// Define an entity class to store keys in the database





string certPassword = "certificate_password";

// Generate a self-signed certificate
X509Certificate2 certificate = GenerateSelfSignedCertificate();

// SignSample();

// EncryptSample();

// ExportPEMSample();

SaveSample();

Console.ReadLine();

void SignSample()
{
    // Sample data to sign
    string originalData = "This is the original data to be signed and verified.";
    byte[] dataBytes = Encoding.UTF8.GetBytes(originalData);

    // Sign the data
    byte[] signature = SignData(dataBytes, certificate.PrivateKey);

    // Verify the signature
    bool isVerified = VerifyData(dataBytes, signature, certificate.PublicKey.Key);

    Console.WriteLine("Original Data: " + originalData);
    Console.WriteLine("Signature Verified: " + isVerified);

    byte[] SignData(byte[] data, AsymmetricAlgorithm privateKey)
    {
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

void EncryptSample()
{

    // Encrypt and decrypt a message using this a secret message!";
    string originalMessage = "Hello, this is a secret message!";
    byte[] encryptedMessage = EncryptWithCertificate(originalMessage, certificate);
    string decryptedMessage = DecryptWithCertificate(encryptedMessage, certificate);

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
        byte[] certData = certificate.Export(X509ContentType.Cert);
        System.IO.File.WriteAllBytes(fileName, certData);
    }

    void SaveCertificateToPfxFile(X509Certificate2 certificate, string fileName, string password)
    {
        byte[] pfxData = certificate.Export(X509ContentType.Pfx, password);
        System.IO.File.WriteAllBytes(fileName, pfxData);
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

    string ExportCertificateToPem(X509Certificate2 certificate)
    {
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
}

X509Certificate2 GenerateSelfSignedCertificate()
{
    using (RSA rsa = RSA.Create(2048))
    {
        var request = new CertificateRequest("CN=localhost", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        request.CertificateExtensions.Add(
            new X509BasicConstraintsExtension(
                certificateAuthority: false,
                hasPathLengthConstraint: false,
                pathLengthConstraint: 0,
                critical: true
            )
        );

        request.CertificateExtensions.Add(
            new X509KeyUsageExtension(
                keyUsages:
                    X509KeyUsageFlags.DigitalSignature
                    | X509KeyUsageFlags.KeyEncipherment,
                critical: false
            )
        );

        request.CertificateExtensions.Add(
            new X509SubjectKeyIdentifierExtension(
                key: request.PublicKey,
                critical: false
            )
        );

        request.CertificateExtensions.Add(
            new X509EnhancedKeyUsageExtension(
                new OidCollection {
                    new Oid("1.3.6.1.5.5.7.3.1")
                    },
                    false));


        var notBefore = DateTime.UtcNow;
        var notAfter = notBefore.AddYears(1);
        X509Certificate2 certificate = request.CreateSelfSigned(notBefore, notAfter);



        // return certificate;

        // Export certificate with private key
        return new X509Certificate2(
            certificate.Export(X509ContentType.Cert),
            (string)null,
            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
        ).CopyWithPrivateKey(rsa);
    }
}

 TODO : Create root certificate
X509Certificate2 GenerateFromRootCertificate()
{
    using (RSA rsa = RSA.Create(2048))
    {
        var request = new CertificateRequest("CN=MySelfSignedCert", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags., false));
        request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));

        var notBefore = DateTime.UtcNow;
        var notAfter = notBefore.AddYears(1);
        X509Certificate2 certificate = request.Create(notBefore, notAfter);

        return certificate;
    }
}*/




