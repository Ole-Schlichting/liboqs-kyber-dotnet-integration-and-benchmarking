using System;
using System.Linq;
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
            int[] keysizes = [2048, 4096];
            foreach(int keysize in keysizes) {
                using var rsa = RSA.Create(keysize);

                // --- Key Sizes ---
                // Exporting parameters is the canonical way to get the key components.
                var publicParams = rsa.ExportParameters(false);
                var privateParams = rsa.ExportParameters(true);

                long publicKeySize = GetParametersSize(publicParams);
                long privateKeySize = GetParametersSize(privateParams);

                // --- Ciphertext Size ---
                // The ciphertext size in RSA is determined by the modulus size.
                // We encrypt a small dummy payload to get an example ciphertext.
                byte[] plaintext = new byte[32];
                byte[] ciphertext = rsa.Encrypt(plaintext, RSAEncryptionPadding.OaepSHA256);
                int ciphertextSize = ciphertext.Length;

                // --- Signature Size ---
                // The signature size is also determined by the modulus size.
                byte[] signature = rsa.SignData(plaintext, HashAlgorithmName.SHA256, RSASignaturePadding.Pss);
                int signatureSize = signature.Length;
                Console.WriteLine("RSA " + keysize);
                Console.WriteLine("publicKeySize: " + publicKeySize);
                Console.WriteLine("privateKeySize: " + privateKeySize);
                Console.WriteLine("ciphertextSize: " + ciphertextSize);
                Console.WriteLine("signatureSize: " + signatureSize);
            }
        }

        private static long GetParametersSize(RSAParameters parameters) {
            // Sum the lengths of all non-null byte arrays in the struct.
            return (parameters.Modulus?.Length ?? 0) +
                   (parameters.Exponent?.Length ?? 0) +
                   (parameters.D?.Length ?? 0) +
                   (parameters.P?.Length ?? 0) +
                   (parameters.Q?.Length ?? 0) +
                   (parameters.DP?.Length ?? 0) +
                   (parameters.DQ?.Length ?? 0) +
                   (parameters.InverseQ?.Length ?? 0);
        }


    }
}