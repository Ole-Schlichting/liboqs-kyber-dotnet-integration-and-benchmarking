using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Liboqs_implementation {
    internal class RSA_Benchmarking {
        public void RunSystemRsaBenchmark(int iterations, int keySizeInBits) {
            var padding = RSAEncryptionPadding.OaepSHA256;
            Console.WriteLine($"Starting benchmark for System.Security.Cryptography.RSA ({keySizeInBits}-bit, Padding: {padding.OaepHashAlgorithm.Name})...");

            // Lists for saving the individual measurements in milliseconds
            var keyGenTimes = new List<double>(iterations);
            var encryptTimes = new List<double>(iterations);
            var decryptTimes = new List<double>(iterations);

            var stopwatch = new Stopwatch();

            // A single plaintext is sufficient, as we are benchmarking the cryptographic operations, not data handling.
            byte[] secretToEncrypt = RandomNumberGenerator.GetBytes(32);
            byte[] finalDecryptedSecret = null; // To verify the last operation

            // *** NEW: Lists to store the generated data ***
            // We store all RSA instances (containing key pairs) and ciphertexts.
            // This ensures each benchmark iteration works on different data.
            Console.WriteLine($"Preparing to run {iterations} iterations...");
            var rsaInstances = new List<RSA>(iterations);
            var ciphertexts = new List<byte[]>(iterations);

            try {
                // --- 1. Key Generation ---
                // Generate 'iterations' RSA instances and store them.
                Console.WriteLine("\n[1/3] Benchmarking Key Generation...");
                for (int i = 0; i < iterations; i++) {
                    stopwatch.Restart();
                    var rsa = RSA.Create(keySizeInBits);
                    // Force the key material to be fully generated.
                    _ = rsa.ExportParameters(true);
                    stopwatch.Stop();

                    keyGenTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
                    rsaInstances.Add(rsa); // Store the generated instance
                }

                PrintStatistics("Key Generation", keyGenTimes);

                // --- 2. Encryption ---
                // Use each generated RSA instance to encrypt the same secret.
                Console.WriteLine("\n[2/3] Benchmarking Encryption (using the previously generated keys)...");
                for (int i = 0; i < iterations; i++) {
                    var currentRsa = rsaInstances[i];

                    stopwatch.Restart();
                    byte[] rsaCiphertext = currentRsa.Encrypt(secretToEncrypt, padding);
                    stopwatch.Stop();

                    encryptTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
                    ciphertexts.Add(rsaCiphertext); // Store the resulting ciphertext
                }

                PrintStatistics("Encryption", encryptTimes);


                // --- 3. Decryption ---
                // Use each RSA instance to decrypt its corresponding ciphertext.
                Console.WriteLine("\n[3/3] Benchmarking Decryption (using the previously generated ciphertexts)...");
                for (int i = 0; i < iterations; i++) {
                    var currentRsa = rsaInstances[i];
                    var currentCiphertext = ciphertexts[i];

                    stopwatch.Restart();
                    finalDecryptedSecret = currentRsa.Decrypt(currentCiphertext, padding);
                    stopwatch.Stop();

                    decryptTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
                }

                PrintStatistics("Decryption", decryptTimes);

                // Verification (check only the last operation)
                Console.WriteLine(secretToEncrypt.SequenceEqual(finalDecryptedSecret)
                    ? "\nVerification: SUCCESSFUL\n"
                    : "\nVerification: FAILED\n");

            } finally {
                // It's crucial to dispose of all created RSA instances to free up cryptographic resources.
                foreach (var rsa in rsaInstances) {
                    rsa.Dispose();
                }
            }
        }

        private static void PrintStatistics(string operationName, List<double> timingsMs) {
            if (timingsMs == null || timingsMs.Count == 0) {
                Console.WriteLine($"    No data available for {operationName}.");
                return;
            }

            Console.WriteLine($"  Results for {operationName} ({timingsMs.Count} iterations):");

            timingsMs.Sort();

            double min = timingsMs[0];
            double max = timingsMs[timingsMs.Count - 1];
            double average = timingsMs.Average();
            double median;

            if (timingsMs.Count % 2 == 0) {
                // Even number of elements: average of the two middle elements
                int midIndex = timingsMs.Count / 2;
                median = (timingsMs[midIndex - 1] + timingsMs[midIndex]) / 2.0;
            } else {
                // Odd number of elements: the middle element
                median = timingsMs[timingsMs.Count / 2];
            }

            Console.WriteLine($"    Average: {average:F4} ms");
            Console.WriteLine($"    Median:  {median:F4} ms");
            Console.WriteLine($"    Min:     {min:F4} ms");
            Console.WriteLine($"    Max:     {max:F4} ms");
        }

        static string ToHexString(byte[] bytes) {
            if (bytes == null) return "null";
            return Convert.ToHexString(bytes);
        }
    }
}