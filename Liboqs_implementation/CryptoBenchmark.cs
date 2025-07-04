using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Liboqs_implementation; // Ihre Bibliothek
using System.Security.Cryptography; // Für RSA

[MemoryDiagnoser] // UNBEDINGT verwenden! Zeigt Speicherverbrauch und GC-Aktivität.
[RankColumn]      // Zeigt einen Rang an, um die schnellsten zu sehen.
public class CryptoBenchmark {
    // [Params] deklariert die verschiedenen "Fälle", die getestet werden sollen.
    // BenchmarkDotNet wird für jeden dieser Werte einen separaten Lauf starten.
    [Params("Kyber512", "Kyber768", "Kyber1024", "RSA2048", "RSA3072", "RSA4096")]
    public string Algorithm { get; set; }

    // Private Felder für die Objekte, die wir benchen wollen
    private Kyber_Benchmarking _kyberBench;
    private RSA _rsa;
    private int _rsaKeySize;

    [GlobalSetup]
    public void Setup() {
        // Diese Methode wird einmal pro Parameter-Satz aufgerufen.
        _kyberBench = new Kyber_Benchmarking();

        // Wir bereiten RSA basierend auf dem Parameter vor
        if (Algorithm.StartsWith("RSA")) {
            _rsaKeySize = int.Parse(Algorithm.Substring(3));
            _rsa = RSA.Create(_rsaKeySize);
        }
    }

    [Benchmark]
    public void KeyGeneration() {
        // Hier kommt die Logik rein, die wir messen wollen.
        // Wir verwenden einen switch, um den richtigen Code auszuführen.
        switch (Algorithm) {
            case "Kyber512":
                _kyberBench.Run("Kyber512"); // Annahme: Sie haben eine Methode für eine einzelne Operation
                break;
            case "Kyber768":
                _kyberBench.Run("Kyber768");
                break;
            case "Kyber1024":
                _kyberBench.Run("Kyber1024");
                break;
            case "RSA2048":
            case "RSA3072":
            case "RSA4096":
                // Die Erzeugung ist bereits in Setup passiert, was bei RSA üblich ist.
                // Um die reine Schlüsselgenerierung zu messen, müssen wir sie hier reinpacken.
                using (var rsaInstance = RSA.Create(_rsaKeySize)) {
                    // Diese Zeile wird gemessen
                }
                break;
        }
    }

    // Sie können weitere Benchmarks für andere Operationen hinzufügen!
    // [Benchmark]
    // public void EncryptDecrypt() { ... }
}