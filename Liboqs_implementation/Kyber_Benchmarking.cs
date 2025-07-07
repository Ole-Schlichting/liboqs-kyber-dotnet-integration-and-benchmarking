using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System.Linq;
using System.Security.Cryptography;

namespace Liboqs_implementation {
    [MemoryDiagnoser] // Misst Speicher und GC-Läufe - ESSENTIELL!
    //[RankColumn]      // Zeigt einen Rang an
    //[GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)] // Gruppiert Ergebnisse nach Algorithmus
    //[SimpleJob(RuntimeMoniker.Net80, iterationCount: 50)]
    //[HardwareCounters(HardwareCounter.TotalCycles)]
    public class Kyber_Benchmarking
    {

        

        [Params("Kyber512", "Kyber768", "Kyber1024")]
        public string AlgorithmName { get; set; }

        private KyberKEM _kem;
        private byte[] _publicKey;
        private byte[] _secretKey;
        private byte[] _ciphertext;

        [GlobalSetup]
        public void GlobalSetup() {
            _kem = new KyberKEM(AlgorithmName);
            (_publicKey, _secretKey) = _kem.GenerateKeypair();
            (_ciphertext, _) = _kem.Encapsulate(_publicKey);
            Console.WriteLine("Public Key Length: " + _publicKey.Length);
            Console.WriteLine("Secret Key Length: " + _secretKey.Length);
            Console.WriteLine("Ciphertext Length: " + _ciphertext.Length);
        }

        [IterationSetup]
        public void IterationSetup() {
            // Für Encapsulation und Decapsulation benötigen wir frische Schlüssel/Ciphertexte,
            // um Cache-Effekte der CPU zu vermeiden, die das Ergebnis verfälschen könnten.
            (_publicKey, _secretKey) = _kem.GenerateKeypair();
            (_ciphertext, _) = _kem.Encapsulate(_publicKey);
        }

        [Benchmark(Description = "KeyGen")]
        public (byte[], byte[]) KeyGeneration() {
            // Misst nur die reine Schlüsselgenerierung.
            return _kem.GenerateKeypair();
        }

        [Benchmark(Description = "Encaps")]
        public (byte[], byte[]) Encapsulation() {
            // Misst nur die Kapselung.
            // Der _publicKey wurde im [IterationSetup] vorbereitet.
            return _kem.Encapsulate(_publicKey);
        }

        [Benchmark(Description = "Decaps")]
        public byte[] Decapsulation() {
            // Misst nur die Entkapselung.
            // _ciphertext und _secretKey wurden im [IterationSetup] vorbereitet.
            return _kem.Decapsulate(_ciphertext, _secretKey);
        }
    }
}