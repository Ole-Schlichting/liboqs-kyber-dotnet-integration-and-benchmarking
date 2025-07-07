using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Security.Cryptography;

namespace Liboqs_implementation {
    // Configuration attributes for BenchmarkDotNet
    [MemoryDiagnoser] // Measures memory allocation and GC collections
    [MarkdownExporter, RPlotExporter] // Generates nice output formats
    public class RSA_Benchmarking {
        // Parameterizes the benchmark to run for different key sizes
        [Params(2048, 4096)]
        public int KeySizeInBits { get; set; }

        private readonly byte[] _plaintext;
        private readonly RSAEncryptionPadding _padding;

        // A single RSA instance and corresponding ciphertext for Encrypt/Decrypt benchmarks.
        // These are set up once in GlobalSetup.
        private RSA _rsa;
        private byte[] _ciphertext;

        public RSA_Benchmarking() {
            // The plaintext and padding are constant for all runs.
            _plaintext = RandomNumberGenerator.GetBytes(32);
            _padding = RSAEncryptionPadding.OaepSHA256;
        }

        // Runs once before all benchmark iterations for a given set of parameters.
        [GlobalSetup]
        public void GlobalSetup() {
            // Create one RSA instance to be used for the encryption and decryption benchmarks.
            _rsa = RSA.Create(KeySizeInBits);

            // Encrypt the data once so the Decrypt benchmark has something to work with.
            _ciphertext = _rsa.Encrypt(_plaintext, _padding);
        }

        // Runs once after all benchmark iterations for a given set of parameters.
        [GlobalCleanup]
        public void GlobalCleanup() {
            _rsa?.Dispose();
        }

        // --- Benchmark Methods ---

        [Benchmark(Description = "RSA Key Generation")]
        public RSAParameters KeyGeneration() {
            // We create AND dispose the instance here because that's the operation we are measuring.
            // The 'using' statement ensures disposal.
            using var rsa = RSA.Create(KeySizeInBits);

            // Exporting the parameters forces the key material to be fully generated.
            // We return it to prevent dead-code elimination by the JIT compiler.
            return rsa.ExportParameters(true);
        }

        [Benchmark(Description = "RSA Encryption")]
        public byte[] Encrypt() {
            // This uses the _rsa instance created in GlobalSetup.
            // We are only measuring the Encrypt operation itself.
            return _rsa.Encrypt(_plaintext, _padding);
        }

        [Benchmark(Description = "RSA Decryption")]
        public byte[] Decrypt() {
            // This uses the _rsa instance and _ciphertext from GlobalSetup.
            // We are only measuring the Decrypt operation.
            return _rsa.Decrypt(_ciphertext, _padding);
        }
    }
}