using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Liboqs_implementation {
    internal class Kyber_Verification {

        public void Verification(string kyberAlgorithmName) {
            // --- Basic KEM Flow (for verification and warm-up) ---
            Console.WriteLine("\n--- Performing initial KEM flow for verification and JIT warm-up ---");
            try {
                using (var kem = new KyberKEM(kyberAlgorithmName)) {
                    Console.WriteLine($"Using KEM: {kem.AlgorithmName}");
                    Console.WriteLine($"  PK: {kem.PublicKeyLength} B, SK: {kem.SecretKeyLength} B, CT: {kem.CiphertextLength} B, SS: {kem.SharedSecretLength} B");

                    var (pk, sk) = kem.GenerateKeypair();
                    var (ct, ss_bob) = kem.Encapsulate(pk);
                    var ss_alice = kem.Decapsulate(ct, sk);

                    if (ss_alice.SequenceEqual(ss_bob)) {
                        Console.WriteLine("Initial KEM flow successful: Shared secrets match.\n");
                    } else {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Initial KEM flow FAILED: Shared secrets DO NOT match.\n");
                        Console.ResetColor();
                        // Potentially exit if basic functionality fails
                        // return;
                    }
                }
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error during initial KEM flow: {ex.Message}");
                Console.ResetColor();
                Console.WriteLine("\nBenchmarking cannot proceed if the basic flow fails.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
                return;
            }
        }

        private KyberKEM _kem;
        private byte[] _publicKey;
        private byte[] _secretKey;
        private byte[] _ciphertext;
        public void Sizes() {
            string[] kybers = ["Kyber512", "Kyber768", "Kyber1024"];
            foreach(string s in kybers) {
                _kem = new KyberKEM(s);
                (_publicKey, _secretKey) = _kem.GenerateKeypair();
                (_ciphertext, _) = _kem.Encapsulate(_publicKey);
                Console.WriteLine(s);
                Console.WriteLine("Public Key Length: " + _publicKey.Length + " Bytes");
                Console.WriteLine("Secret Key Length: " + _secretKey.Length + " Bytes");
                Console.WriteLine("Ciphertext Length: " + _ciphertext.Length + " Bytes");
            }
        }
    }
}
