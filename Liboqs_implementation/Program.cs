using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers; // Wichtig für MemoryDiagnoser
using BenchmarkDotNet.Exporters; // Wichtig für Exporter
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers; // Wichtig für ILogger
using BenchmarkDotNet.Running;
using Liboqs_implementation;
using System;
using static System.Net.Mime.MediaTypeNames;

public class Program {

    /*
    cmake -G "Visual Studio 17 2022" -A x64 -DCMAKE_TOOLCHAIN_FILE="C:/path/to/your/vcpkg/scripts/buildsystems/vcpkg.cmake" -DOQS_USE_AVX2_INSTRUCTIONS=ON -DBUILD_SHARED_LIBS=ON ..
    cmake --build . --config Release
    */
    public static void Main(string[] args) {
        Console.WriteLine("Starting Kyber benchmarks...");

        // Erstellen einer benutzerdefinierten Konfiguration
        var config = ManualConfig.CreateEmpty() // Starten mit einer LEEREN Konfiguration
            .AddJob(Job.Default.WithId("DefaultJob")) // Fügen Sie einen Job hinzu, sonst läuft nichts
            .AddDiagnoser(MemoryDiagnoser.Default)   // Fügen Sie den MemoryDiagnoser hinzu
            .AddColumnProvider(DefaultColumnProviders.Instance) // Fügen Sie Standardspalten hinzu (Mean, StdDev, etc.)
            .AddLogger(new ConsoleLogger()) // Fügen Sie den Logger hinzu, der die finale Tabelle schreibt
            .AddExporter(MarkdownExporter.GitHub) // Optional: Schreibt die Tabelle auch in eine .md-Datei
            .AddJob(Job.Default.WithToolchain(
                BenchmarkDotNet.Toolchains.InProcess.NoEmit.InProcessNoEmitToolchain.Instance));
        // Führen Sie den Benchmark mit dieser sauberen Konfiguration aus
        BenchmarkRunner.Run<Kyber_Benchmarking>(config);
    }
}


