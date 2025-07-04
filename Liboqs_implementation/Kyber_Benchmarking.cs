using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liboqs_implementation {
    internal class Kyber_Benchmarking {

        public void Run(string kyberAlgorithmName, int iterations = 10000) {
            Console.WriteLine($"Kyber {kyberAlgorithmName} KEM C# Console Demo & Benchmark");
            Console.WriteLine("==================================================");

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
                using (var kem = new KyberKEM(kyberAlgorithmName)) {
                    Stopwatch stopwatch = new Stopwatch();
                    List<long> keypairTimes = new List<long>(iterations);
                    List<long> encapsTimes = new List<long>(iterations);
                    List<long> decapsTimes = new List<long>(iterations);

                    // *** NEW: Lists to store the generated data ***
                    // We store all key pairs and ciphertexts to ensure a more realistic benchmark.
                    // Note: This increases memory consumption. For 10,000 Kyber512 iterations, this will use roughly 32 MB of RAM.
                    Console.WriteLine($"Preparing memory for {iterations} iterations...");
                    List<byte[]> publicKeys = new List<byte[]>(iterations);
                    List<byte[]> secretKeys = new List<byte[]>(iterations);
                    List<byte[]> ciphertexts = new List<byte[]>(iterations);


                    Console.WriteLine($"Benchmarking {iterations} iterations for each operation...");

                    // 1. Benchmark Key Generation
                    //    Generates 'iterations' key pairs and stores them.
                    Console.WriteLine("\n[1/3] Benchmarking Key Generation...");
                    for (int i = 0; i < iterations; i++) {
                        stopwatch.Restart();
                        var (pk, sk) = kem.GenerateKeypair();
                        stopwatch.Stop();

                        keypairTimes.Add(stopwatch.ElapsedTicks);
                        publicKeys.Add(pk);
                        secretKeys.Add(sk);
                    }
                    PrintBenchmarkResults("Key Generation", keypairTimes);


                    // 2. Benchmark Encapsulation
                    //    Uses a different, previously generated public key for each iteration.
                    //    Stores the resulting ciphertext.
                    Console.WriteLine("\n[2/3] Benchmarking Encapsulation (using the previously generated keys)...");
                    for (int i = 0; i < iterations; i++) {
                        // Get the public key for this iteration
                        byte[] currentPk = publicKeys[i];

                        stopwatch.Restart();
                        // We don't need the shared secret (ss) here, but the method returns it.
                        var (ct, ss) = kem.Encapsulate(currentPk);
                        stopwatch.Stop();

                        encapsTimes.Add(stopwatch.ElapsedTicks);
                        ciphertexts.Add(ct);
                    }
                    PrintBenchmarkResults("Encapsulation", encapsTimes);


                    // 3. Benchmark Decapsulation
                    //    For each iteration, uses the corresponding ciphertext and its associated secret key.
                    Console.WriteLine("\n[3/3] Benchmarking Decapsulation (using the previously generated ciphertexts)...");
                    for (int i = 0; i < iterations; i++) {
                        // Get the ciphertext and the corresponding secret key for this iteration
                        byte[] currentCt = ciphertexts[i];
                        byte[] currentSk = secretKeys[i];

                        stopwatch.Restart();
                        kem.Decapsulate(currentCt, currentSk);
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


            Console.WriteLine($"Results for {operationName} ({elapsedTicks.Count} iterations):");
            Console.WriteLine($"  Average: {averageMs:F4} ms ({averageTicks:F0} ticks)");
            Console.WriteLine($"  Min:     {minMs:F4} ms ({minTicks} ticks)");
            Console.WriteLine($"  Max:     {maxMs:F4} ms ({maxTicks} ticks)");
            Console.WriteLine($"  Median:  {medianMs:F4} ms ({medianTicks} ticks)");
        }

        static string ToHexString(byte[] bytes) {
            if (bytes == null) return "null";
            return Convert.ToHexString(bytes);
        }
    }
}