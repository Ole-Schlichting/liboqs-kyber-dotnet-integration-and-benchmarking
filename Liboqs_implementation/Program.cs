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
        Kyber_Verification kyber_Verification = new Kyber_Verification();
        kyber_Verification.Verification("Kyber512");
        kyber_Verification.Verification("Kyber768");
        kyber_Verification.Verification("Kyber1024");

        // Benchmark Configuration
        var config = ManualConfig.CreateEmpty()
            .AddJob(Job.Default.
                WithWarmupCount(10).
                WithIterationCount(50))
            .AddDiagnoser(MemoryDiagnoser.Default)
            .AddColumnProvider(DefaultColumnProviders.Instance)
            .AddLogger(new ConsoleLogger())
            .AddExporter(MarkdownExporter.GitHub)
            .AddColumn(StatisticColumn.Min)
            .AddColumn(StatisticColumn.Max)
            .AddColumn(StatisticColumn.Error)
            .AddColumn(StatisticColumn.OperationsPerSecond);


        BenchmarkRunner.Run<Kyber_Benchmarking>(config);

        /*
                     .AddJob(Job.Default.WithToolchain(
                BenchmarkDotNet.Toolchains.InProcess.NoEmit.InProcessNoEmitToolchain.Instance));
         
         */
    }
}


