// Fügen Sie die using-Direktiven für BenchmarkDotNet hinzu
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System.Linq;
using System.Security.Cryptography; // Nur als Beispiel, falls Sie auch RSA benchen

namespace Liboqs_implementation {
    // 1. Klasse 'public' machen und mit Attributen versehen
    [MemoryDiagnoser] // Misst Speicher und GC-Läufe - ESSENTIELL!
    [RankColumn]      // Zeigt einen Rang an
    [GroupBenchmarksBy(BenchmarkDotNet.Configs.BenchmarkLogicalGroupRule.ByParams)] // Gruppiert Ergebnisse nach Algorithmus
    [SimpleJob(RuntimeMoniker.Net80, iterationCount: 100)]
    [HardwareCounters(HardwareCounter.TotalCycles)]
    public class Kyber_Benchmarking // Früher: internal class
    {
        
        // 2. Die Algorithmus-Namen werden zu Parametern
        // BenchmarkDotNet wird für jeden dieser Werte einen kompletten Satz von Benchmarks ausführen.
        [Params("Kyber512", "Kyber768", "Kyber1024")]
        public string AlgorithmName { get; set; }

        // 3. Felder für den Zustand, den wir zwischen den Operationen benötigen
        private KyberKEM _kem;
        private byte[] _publicKey;
        private byte[] _secretKey;
        private byte[] _ciphertext;

        // 4. [GlobalSetup] wird einmal pro Parameter (pro Algorithmus) ausgeführt.
        //    Es ersetzt den Code am Anfang Ihrer alten 'Run'-Methode.
        [GlobalSetup]
        public void GlobalSetup() {
            // Erstellt die KEM-Instanz für den aktuellen Algorithmus
            _kem = new KyberKEM(AlgorithmName);
            Console.WriteLine("TEST");
            // Wir generieren einmalig Daten, die für die Setups benötigt werden.
            // Dies stellt sicher, dass die Operationen selbst nicht durch
            // unvorhersehbare Zustände beeinflusst werden.
            (_publicKey, _secretKey) = _kem.GenerateKeypair();
            (_ciphertext, _) = _kem.Encapsulate(_publicKey);
        }

        // 5. [IterationSetup] wird VOR JEDER EINZELNEN Messung ausgeführt.
        //    Es dient dazu, die Eingabedaten für den Benchmark vorzubereiten.
        //    Die Zeit, die hier verbracht wird, wird NICHT gemessen.
        [IterationSetup]
        public void IterationSetup() {
            // Für Encapsulation und Decapsulation benötigen wir frische Schlüssel/Ciphertexte,
            // um Cache-Effekte der CPU zu vermeiden, die das Ergebnis verfälschen könnten.
            (_publicKey, _secretKey) = _kem.GenerateKeypair();
            (_ciphertext, _) = _kem.Encapsulate(_publicKey);
        }

        // 6. Jede zu messende Operation wird eine eigene [Benchmark]-Methode.
        //    Die Methode sollte nur die EINE Operation enthalten, die gemessen werden soll.

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