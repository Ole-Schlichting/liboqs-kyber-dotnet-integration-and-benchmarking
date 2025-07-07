using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers; // Wichtig für MemoryDiagnoser
using BenchmarkDotNet.Exporters; // Wichtig für Exporter
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers; // Wichtig für ILogger
using BenchmarkDotNet.Running;
using Liboqs_implementation;
using System.Linq.Expressions;

public class Program {

    /*
    cmake -G "Visual Studio 17 2022" -A x64 -DCMAKE_TOOLCHAIN_FILE="C:/path/to/your/vcpkg/scripts/buildsystems/vcpkg.cmake" -DOQS_USE_AVX2_INSTRUCTIONS=ON -DBUILD_SHARED_LIBS=ON ..
    cmake --build . --config Release
    */
    public static void Main(string[] args) {
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


            if (args.Length < 2 || args[0].ToLower() != "benchmark" && args[0].ToLower() != "sizes") {
                Console.WriteLine("Nutzung:");
                Console.WriteLine("Liboqs_implementation.exe benchmark rsa");
                Console.WriteLine("Liboqs_implementation.exe benchmark kyber");
                return;
            }
            Kyber_Verification kyber_Verification = new Kyber_Verification();


            if (args[0].ToLower() == "sizes") {
                switch (args[1]) {
                    case "kyber":
                        kyber_Verification.Sizes();
                        break;
                    case "rsa":
                        RSA_Verification.Sizes();
                        break;
                    default:
                        Console.WriteLine("Unbekannter Benchmark: " + args[1]);
                        break;
                }
            }

            if(args[0].ToLower() == "benchmark") {
                switch (args[1].ToLower()) {
                    case "rsa":
                        RSA_Verification.RunValidation(2048);
                        RSA_Verification.RunValidation(4096);
                        BenchmarkRunner.Run<RSA_Benchmarking>(config);
                        break;
                    case "kyber":
                        kyber_Verification.Verification("Kyber512");
                        kyber_Verification.Verification("Kyber768");
                        kyber_Verification.Verification("Kyber1024");
                        BenchmarkRunner.Run<Kyber_Benchmarking>(config);
                        break;

                    default:
                        Console.WriteLine("Unbekannter Benchmark: " + args[1]);
                        break;
                }
            }
            


    }
}


