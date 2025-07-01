using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Liboqs_implementation {
    internal class RSA_Benchmarking {
        public void RunSystemRsaBenchmark(int numIterations, int keySizeInBits) {
            var padding = RSAEncryptionPadding.OaepSHA256;
            Console.WriteLine($"Starte Benchmark für System.Security.Cryptography.RSA ({keySizeInBits}-bit, Padding: {padding.OaepHashAlgorithm.Name})...");

            // Listen zum Speichern der einzelnen Messungen in Millisekunden
            var keyGenTimes = new List<double>(numIterations);
            var encryptTimes = new List<double>(numIterations);
            var decryptTimes = new List<double>(numIterations);

            var stopwatch = new Stopwatch();

            // Temporäre Variablen für die Ergebnisse, um sie in der Schleife zu verwenden
            byte[] secretToEncrypt = RandomNumberGenerator.GetBytes(32);
            byte[] rsaCiphertext = null;
            byte[] decryptedSecret = null;



            
            // Einmaliges Erstellen einer RSA-Instanz für die Encrypt/Decrypt-Schleifen
            using var rsaForOps = RSA.Create(keySizeInBits);
            List<RSA> rsaList = new List<RSA>();

            var rsa1 = RSA.Create(keySizeInBits);
            _ = rsa1.ExportParameters(true);
            // --- 1. Schlüsselgenerierung ---
            for (int i = 0; i < numIterations; i++) {
                stopwatch.Restart();
                var rsa = RSA.Create(keySizeInBits);
                _ =rsa.ExportParameters(true);
                stopwatch.Stop();
                keyGenTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
            }

            rsaCiphertext = rsaForOps.Encrypt(secretToEncrypt, padding);
            // --- 2. Kapselung (Verschlüsselung) ---
            for (int i = 0; i < numIterations; i++) {
                stopwatch.Restart();
                rsaCiphertext = rsaForOps.Encrypt(secretToEncrypt, padding);
                stopwatch.Stop();
                Console.WriteLine(stopwatch.Elapsed.TotalMilliseconds);
                encryptTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
            }

            decryptedSecret = rsaForOps.Decrypt(rsaCiphertext, padding);
            // --- 3. Entkapselung (Entschlüsselung) ---
            for (int i = 0; i < numIterations; i++) {
                stopwatch.Restart();
                decryptedSecret = rsaForOps.Decrypt(rsaCiphertext, padding);
                stopwatch.Stop();
                decryptTimes.Add(stopwatch.Elapsed.TotalMilliseconds);
            }

            Console.WriteLine("  Schlüsselgenerierung:");
            PrintStatistics(keyGenTimes);

            Console.WriteLine("  Kapselung (Encrypt):");
            PrintStatistics(encryptTimes);

            Console.WriteLine("  Entkapselung (Decrypt):");
            PrintStatistics(decryptTimes);

            // Verifizierung (nur ein Check am Ende)
            Console.WriteLine(secretToEncrypt.SequenceEqual(decryptedSecret)
                ? "  Verifizierung: ERFOLGREICH\n"
                : "  Verifizierung: FEHLGESCHLAGEN\n");
        }

        private static void PrintStatistics(List<double> timings) {
            if (timings == null || timings.Count == 0) {
                Console.WriteLine("    Keine Daten für die Statistik vorhanden.");
                return;
            }

            timings.Sort();

            double min = timings[0];
            double max = timings[timings.Count - 1];
            double average = timings.Average();
            double median;

            if (timings.Count % 2 == 0) {
                // Gerade Anzahl: Durchschnitt der beiden mittleren Elemente
                int midIndex = timings.Count / 2;
                median = (timings[midIndex - 1] + timings[midIndex]) / 2.0;
            } else {
                // Ungerade Anzahl: Das mittlere Element
                median = timings[timings.Count / 2];
            }

            Console.WriteLine($"    Avg:    {average:F4} ms");
            Console.WriteLine($"    Median: {median:F4} ms");
            Console.WriteLine($"    Min:    {min:F4} ms");
            Console.WriteLine($"    Max:    {max:F4} ms");
        }

        static string ToHexString(byte[] bytes) {
            if (bytes == null) return "null";

            // Ab .NET 5.0 ist dies der einfachste Weg:
            // return Convert.ToHexString(bytes);

            // Für ältere .NET-Frameworks oder als Alternative:
            return string.Join("", bytes.Select(b => b.ToString("X2")));
        }
    }

}