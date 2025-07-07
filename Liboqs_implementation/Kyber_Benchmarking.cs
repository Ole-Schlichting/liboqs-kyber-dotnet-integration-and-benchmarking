using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using System.Linq;
using System.Security.Cryptography;

namespace Liboqs_implementation {
    [MemoryDiagnoser]
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
        }

        [IterationSetup]
        public void IterationSetup() {
            (_publicKey, _secretKey) = _kem.GenerateKeypair();
            (_ciphertext, _) = _kem.Encapsulate(_publicKey);
        }

        [Benchmark(Description = "KeyGen")]
        public (byte[], byte[]) KeyGeneration() {
            return _kem.GenerateKeypair();
        }

        [Benchmark(Description = "Encaps")]
        public (byte[], byte[]) Encapsulation() {
            return _kem.Encapsulate(_publicKey);
        }

        [Benchmark(Description = "Decaps")]
        public byte[] Decapsulation() {
            return _kem.Decapsulate(_ciphertext, _secretKey);
        }
    }
}