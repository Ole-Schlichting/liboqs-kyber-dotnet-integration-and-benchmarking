using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liboqs_implementation {
    internal class Kyber_Benchmarking {

        public void Run(string kyberAlgorithmName, int iterations = 1000) {
            Console.WriteLine("Kyber512 KEM C# Console Demo & Benchmark");
            Console.WriteLine("========================================");

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


            // --- Benchmarking ---
            Console.WriteLine("\n--- Starting Benchmarks ---");

            try {
                // Re-initialize KEM for a clean benchmark slate if desired,
                // though the class itself is lightweight. The expensive parts are the OQS calls.
                using (var kem = new KyberKEM(kyberAlgorithmName)) {
                    Stopwatch stopwatch = new Stopwatch();
                    List<long> keypairTimes = new List<long>(iterations);
                    List<long> encapsTimes = new List<long>(iterations);
                    List<long> decapsTimes = new List<long>(iterations);

                    // Pre-allocate some keys for encaps/decaps benchmarks to avoid measuring keygen there
                    byte[] PkForBench = new byte[kem.PublicKeyLength];
                    byte[] SkForBench = new byte[kem.SecretKeyLength];
                    byte[] CtForBench = new byte[kem.CiphertextLength];

                    // Generate one key pair to be used for encaps/decaps benchmarks
                    (PkForBench, SkForBench) = kem.GenerateKeypair();
                    // Encapsulate once to have a ciphertext for decapsulation benchmark
                    (CtForBench, _) = kem.Encapsulate(PkForBench);


                    Console.WriteLine($"Benchmarking {iterations} iterations for each operation...");
                    List<byte[]> bytes = new List<byte[]>();

                    // 1. Benchmark Key Generation
                    for (int i = 0; i < iterations; i++) {
                        stopwatch.Restart(); // Resets and starts
                        kem.GenerateKeypair();
                        //bytes.Add(t.PublicKey);
                        //bytes.Add(t.SecretKey);
                        //Console.WriteLine(ToHexString(t.PublicKey));
                        //Console.WriteLine(ToHexString(t.SecretKey));
                        stopwatch.Stop();
                        keypairTimes.Add(stopwatch.ElapsedTicks);
                    }

                    foreach (byte[] b in bytes) { 
                        Console.WriteLine(ToHexString(b));
                    }

                    PrintBenchmarkResults("Key Generation", keypairTimes);


                    // 2. Benchmark Encapsulation
                    // Using the pre-generated PkForBench
                    for (int i = 0; i < iterations; i++) {
                        stopwatch.Restart();
                        kem.Encapsulate(PkForBench);
                        stopwatch.Stop();
                        encapsTimes.Add(stopwatch.ElapsedTicks);
                    }
                    PrintBenchmarkResults("Encapsulation", encapsTimes);


                    // 3. Benchmark Decapsulation
                    // Using the pre-generated CtForBench and SkForBench
                    for (int i = 0; i < iterations; i++) {
                        stopwatch.Restart();
                        kem.Decapsulate(CtForBench, SkForBench);
                        stopwatch.Stop();
                        decapsTimes.Add(stopwatch.ElapsedTicks);
                    }
                    PrintBenchmarkResults("Decapsulation", decapsTimes);
                }
            } catch (DllNotFoundException) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"FATAL: oqs.dll not found. Ensure oqs.dll is in the output directory ({System.IO.Directory.GetCurrentDirectory()}) and matches the application's architecture.");
                Console.ResetColor();
            } catch (ArgumentException ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ARGUMENT ERROR: {ex.Message}");
                if (ex.Message.Contains("Algorithm not supported")) {
                    Console.WriteLine($"Please ensure the algorithm name '{kyberAlgorithmName}' is correct and supported by your liboqs build.");
                }
                Console.ResetColor();
            } catch (Exception ex) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"AN ERROR OCCURRED DURING BENCHMARKING: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                Console.ResetColor();
            }

            Console.WriteLine("\nBenchmarking complete. Press any key to exit.");
            Console.ReadKey();
        }
        static void PrintBenchmarkResults(string operationName, List<long> elapsedTicks) {
            if (elapsedTicks == null || !elapsedTicks.Any()) {
                Console.WriteLine($"No data for {operationName}.");
                return;
            }

            // Calculate statistics
            double averageTicks = elapsedTicks.Average();
            long minTicks = elapsedTicks.Min();
            long maxTicks = elapsedTicks.Max();

            // Convert ticks to milliseconds (Stopwatch.Frequency is ticks per second)
            double averageMs = (averageTicks / Stopwatch.Frequency) * 1000.0;
            double minMs = ((double)minTicks / Stopwatch.Frequency) * 1000.0;
            double maxMs = ((double)maxTicks / Stopwatch.Frequency) * 1000.0;

            // Median
            var sortedTicks = elapsedTicks.OrderBy(t => t).ToList();
            long medianTicks = sortedTicks[sortedTicks.Count / 2];
            double medianMs = ((double)medianTicks / Stopwatch.Frequency) * 1000.0;


            Console.WriteLine($"\nResults for {operationName} ({elapsedTicks.Count} iterations):");
            Console.WriteLine($"  Average: {averageMs:F4} ms ({averageTicks:F0} ticks)");
            Console.WriteLine($"  Min:     {minMs:F4} ms ({minTicks} ticks)");
            Console.WriteLine($"  Max:     {maxMs:F4} ms ({maxTicks} ticks)");
            Console.WriteLine($"  Median:  {medianMs:F4} ms ({medianTicks} ticks)");
            // Optionally, calculate standard deviation for more insight into variability
        }

        static string ToHexString(byte[] bytes) {
            if (bytes == null) return "null";

            // Ab .NET 5.0 ist dies der einfachste Weg:
            return Convert.ToHexString(bytes);

            // Für ältere .NET-Frameworks oder als Alternative:
            // return string.Join("", bytes.Select(b => b.ToString("X2")));
        }
    }
}
