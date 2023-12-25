using System.Security.Cryptography;
using System.Text;

namespace amorphie.poc.mtls.certificate
{

    class hello
    {

        static KeyPair keyPair { get; set; } = null;


        // Create some data to sign


        static byte[] SignData(byte[] data, RSA rsa)
        {
            // Sign the data with the private key
            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            return signature;
        }

        static void SaveKeysToDatabase(RSA rsa)
        {
            keyPair = new KeyPair
            {
                PublicKey = Convert.ToBase64String(rsa.ExportRSAPublicKey()),
                PrivateKey = Convert.ToBase64String(rsa.ExportRSAPrivateKey())
            };

        }

        static RSA LoadKeysFromDatabase(bool usePrivateKey = false)
        {

            if (keyPair != null)
            {
                RSA rsa = RSA.Create();

                if (usePrivateKey)
                {
                    rsa.ImportRSAPrivateKey(Convert.FromBase64String(keyPair.PrivateKey), out _);
                }
                else
                {
                    rsa.ImportRSAPublicKey(Convert.FromBase64String(keyPair.PublicKey), out _);
                }

                return rsa;
            }
            return null;
        }

        static bool VerifySignature(byte[] data, byte[] signature)
        {
            using (RSA rsa = LoadKeysFromDatabase(usePrivateKey: false))
            {
                // Verify the signature with the public key retrieved from the database
                bool isSignatureValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                return isSignatureValid;
            }
        }


        static void Main()
        {
            string dataToSign = "Hello, World!";

            byte[] dataBytes = Encoding.UTF8.GetBytes(dataToSign);

            // Create an RSA key pair (private and public keys)
            using (RSA rsa = RSA.Create())
            {
                // Sign the data with the private key
                byte[] signature = SignData(dataBytes, rsa);

                // Save the keys to the database
                SaveKeysToDatabase(rsa);

                // Verify the signature with the public key retrieved from the database
                bool isSignatureValid = VerifySignature(dataBytes, signature);

                Console.WriteLine("Original Data: " + dataToSign);
                Console.WriteLine("Signature Valid: " + isSignatureValid);
            }

            Console.WriteLine("Finished");
        }
    }
}
