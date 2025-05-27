// Program.cs
using System;
using System.Linq; // For SequenceEqual
using System.Text;  // For BitConverter

public class Program {
    static void Main(string[] args) {
        Console.WriteLine("Kyber512 KEM C# Console Demo");
        Console.WriteLine("============================");

        // The Kyber512 algorithm name string used by your liboqs build.
        // Common examples: "Kyber512", "KYBER_512". Adjust if necessary.
        string kyberAlgorithmName = "Kyber512";

        try {
            // Using statement ensures Dispose() is called on the kem object
            using (var kem = new KyberKEM(kyberAlgorithmName)) {
                Console.WriteLine($"Successfully initialized KEM for: {kem.AlgorithmName}");
                Console.WriteLine($"  Public Key Length: {kem.PublicKeyLength} bytes");
                Console.WriteLine($"  Secret Key Length: {kem.SecretKeyLength} bytes");
                Console.WriteLine($"  Ciphertext Length: {kem.CiphertextLength} bytes");
                Console.WriteLine($"  Shared Secret Length: {kem.SharedSecretLength} bytes");
                Console.WriteLine();

                // 1. Key Generation (Alice)
                Console.WriteLine("Generating keypair (Alice)...");
                var (publicKey, secretKey) = kem.GenerateKeypair();
                Console.WriteLine($"  Public Key (first 16 bytes): {ToHexString(publicKey, 16)}...");
                Console.WriteLine($"  Secret Key (first 16 bytes): {ToHexString(secretKey, 16)}...");
                Console.WriteLine();

                // 2. Encapsulation (Bob wants to send a secret to Alice)
                Console.WriteLine("Encapsulating secret (Bob)...");
                var (ciphertext, sharedSecret_Bob) = kem.Encapsulate(publicKey);
                Console.WriteLine($"  Ciphertext (first 16 bytes): {ToHexString(ciphertext, 16)}...");
                Console.WriteLine($"  Shared Secret (Bob's side, first 8 bytes): {ToHexString(sharedSecret_Bob, 8)}...");
                Console.WriteLine();

                // 3. Decapsulation (Alice receives ciphertext from Bob)
                Console.WriteLine("Decapsulating secret (Alice)...");
                byte[] sharedSecret_Alice = kem.Decapsulate(ciphertext, secretKey);
                Console.WriteLine($"  Shared Secret (Alice's side, first 8 bytes): {ToHexString(sharedSecret_Alice, 8)}...");
                Console.WriteLine();

                // 4. Verification
                if (sharedSecret_Alice.SequenceEqual(sharedSecret_Bob)) {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("SUCCESS: Shared secrets match!");
                } else {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Shared secrets DO NOT match!");
                }
                Console.ResetColor();
            }
        } catch (DllNotFoundException) {
            // This exception is now typically caught and handled within NativeMethods' static constructor.
            // If it still reaches here, it means something went very wrong early on.
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"FATAL: oqs.dll not found. This should have been caught earlier. Ensure oqs.dll is in the output directory ({System.IO.Directory.GetCurrentDirectory()}) and matches the application's architecture.");
            Console.ResetColor();
        } catch (ArgumentException ex) // For KEM init failure or bad arguments to wrapper methods
          {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ARGUMENT ERROR: {ex.Message}");
            if (ex.Message.Contains("Algorithm not supported")) {
                Console.WriteLine($"Please ensure the algorithm name '{kyberAlgorithmName}' is correct and supported by your liboqs build.");
            }
            Console.ResetColor();
        } catch (Exception ex) // For other errors from OQS or the wrapper
          {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"AN ERROR OCCURRED: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key to exit.");
        Console.ReadKey();
    }

    public static string ToHexString(byte[] bytes, int maxLength = 0) {
        if (bytes == null) return "null";
        var sb = new StringBuilder();
        int count = (maxLength > 0 && bytes.Length > maxLength && maxLength > 0) ? maxLength : bytes.Length;
        for (int i = 0; i < count; i++) {
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
    }
}