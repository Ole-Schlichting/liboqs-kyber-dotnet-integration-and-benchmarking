using System.Security.Cryptography;

namespace Liboqs_implementation {
    /// <summary>
    /// Performs a one-shot validation of the RSA cryptographic operations
    /// to ensure they are working correctly before running performance benchmarks.
    /// </summary>
    public static class RSA_Verification {
        public static bool RunValidation(int keySizeInBits) {
            Console.WriteLine($"\n--- Running Validation for RSA {keySizeInBits}-bit ---");
            bool isSuccess = true;

            try {
                // --- Step 1: Key Generation ---
                Console.Write("[1/4] Performing Key Generation... ");
                using var rsa = RSA.Create(keySizeInBits);
                // Force key creation
                _ = rsa.ExportParameters(true);
                
                Console.WriteLine("SUCCESS");

                // --- Step 2: Data Preparation ---
                Console.Write("[2/4] Preparing test data... ");
                byte[] originalData = RandomNumberGenerator.GetBytes(32);
                var padding = RSAEncryptionPadding.OaepSHA256;
                Console.WriteLine("SUCCESS");

                // --- Step 3: Encryption ---
                Console.Write("[3/4] Performing Encryption... ");
                byte[] encryptedData = rsa.Encrypt(originalData, padding);
                if (encryptedData == null || encryptedData.Length == 0) {
                    throw new InvalidOperationException("Encryption produced null or empty output.");
                }
                // A simple check: encrypted data should not be the same as original data.
                if (originalData.SequenceEqual(encryptedData)) {
                    throw new InvalidOperationException("Encryption failed: output is identical to input.");
                }
                Console.WriteLine("SUCCESS");


                // --- Step 4: Decryption & Verification ---
                Console.Write("[4/4] Performing Decryption and Verification... ");
                byte[] decryptedData = rsa.Decrypt(encryptedData, padding);
                if (!originalData.SequenceEqual(decryptedData)) {
                    throw new CryptographicException("Verification FAILED: Decrypted data does not match original data.");
                }
                Console.WriteLine("SUCCESS");

            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nVALIDATION FAILED for RSA {keySizeInBits}-bit: {ex.Message}");
                Console.ResetColor();
                isSuccess = false;
            }

            if (isSuccess) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"--- Validation for RSA {keySizeInBits}-bit PASSED ---\n");
                Console.ResetColor();
            }

            return isSuccess;
        }

        public static void Sizes() {
            int[] keysizes = [2048, 3072, 4096, 7680, 15360];
            foreach (int keysize in keysizes) {
                using (RSA rsa = RSA.Create(keysize)) {
                    int privateKeyFileSize = rsa.ExportRSAPrivateKey().Length;
                    int publicKeyFileSize = rsa.ExportSubjectPublicKeyInfo().Length;


                    Console.WriteLine("RSA-" + keysize);
                    Console.WriteLine($"Public Key Size: {publicKeyFileSize} Bytes");
                    Console.WriteLine($"Private Key Size: {privateKeyFileSize} Bytes");

                    byte[] dataToEncrypt = new byte[32]; // Some sample data
                    byte[] encryptedData = rsa.Encrypt(dataToEncrypt, RSAEncryptionPadding.OaepSHA256);

                    Console.WriteLine($"Ciphertext Size: {encryptedData.Length} Bytes");
                }


            }
        }
    }
}