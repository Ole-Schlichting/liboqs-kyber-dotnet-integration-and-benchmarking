// KyberKEM.cs
using System;
using System.Runtime.InteropServices; // For Marshal

public class KyberKEM : IDisposable {
    private IntPtr _kemPtr; // Pointer to the native OQS_KEM object
    private bool _disposed = false;

    public string AlgorithmName { get; }
    public int PublicKeyLength { get; }
    public int SecretKeyLength { get; }
    public int CiphertextLength { get; }
    public int SharedSecretLength { get; }

    /// <summary>
    /// Initializes a new KEM instance for the specified Kyber algorithm.
    /// </summary>
    /// <param name="kyberAlgorithmName">Example: "Kyber512", "Kyber768", "Kyber1024"</param>
    public KyberKEM(string kyberAlgorithmName) {
        AlgorithmName = kyberAlgorithmName;
        // The static constructor of NativeMethods will call OQS_init()
        // Ensure NativeMethods is accessed before this point if OQS_init isn't called elsewhere.
        // Calling a static method or accessing a static field in NativeMethods would trigger its static ctor.
        // For simplicity, we rely on it being triggered by the DllImports.

        _kemPtr = NativeMethods.OQS_KEM_new(kyberAlgorithmName);

        if (_kemPtr == IntPtr.Zero) {
            throw new ArgumentException($"Failed to initialize KEM for algorithm '{kyberAlgorithmName}'. Algorithm not supported or OQS DLL problem (see console error if DllNotFoundException occurred earlier).");
        }

        // Read the KEM details (lengths) from the native structure
        NativeMethods.OqsKemDetails details = Marshal.PtrToStructure<NativeMethods.OqsKemDetails>(_kemPtr);

        PublicKeyLength = (int)details.length_public_key.ToUInt32();
        SecretKeyLength = (int)details.length_secret_key.ToUInt32();
        CiphertextLength = (int)details.length_ciphertext.ToUInt32();
        SharedSecretLength = (int)details.length_shared_secret.ToUInt32();
    }

    public (byte[] PublicKey, byte[] SecretKey) GenerateKeypair() {
        if (_disposed) throw new ObjectDisposedException(nameof(KyberKEM));

        byte[] publicKey = new byte[PublicKeyLength];
        byte[] secretKey = new byte[SecretKeyLength];

        NativeMethods.OqsStatus status = NativeMethods.OQS_KEM_keypair(_kemPtr, publicKey, secretKey);
        if (status != NativeMethods.OqsStatus.Success) {
            throw new Exception($"OQS_KEM_keypair failed for {AlgorithmName} with status: {status}");
        }
        return (publicKey, secretKey);

    }

    public (byte[] Ciphertext, byte[] SharedSecret) Encapsulate(byte[] publicKey) {
        if (_disposed) throw new ObjectDisposedException(nameof(KyberKEM));
        if (publicKey == null || publicKey.Length != PublicKeyLength) {
            throw new ArgumentException($"Public key length is invalid. Expected {PublicKeyLength}, got {publicKey?.Length ?? 0}.", nameof(publicKey));
        }
        byte[] ciphertext = new byte[CiphertextLength];
        byte[] sharedSecret = new byte[SharedSecretLength];
        NativeMethods.OqsStatus status = NativeMethods.OQS_KEM_encaps(_kemPtr, ciphertext, sharedSecret, publicKey);
        if (status != NativeMethods.OqsStatus.Success) {
            throw new Exception($"OQS_KEM_encaps failed for {AlgorithmName} with status: {status}");
        }
        return (ciphertext, sharedSecret);
    }

    public byte[] Decapsulate(byte[] ciphertext, byte[] secretKey) {
        if (_disposed) throw new ObjectDisposedException(nameof(KyberKEM));
        if (ciphertext == null || ciphertext.Length != CiphertextLength) {
            throw new ArgumentException($"Ciphertext length is invalid. Expected {CiphertextLength}, got {ciphertext?.Length ?? 0}.", nameof(ciphertext));
        }
        if (secretKey == null || secretKey.Length != SecretKeyLength) {
            throw new ArgumentException($"Secret key length is invalid. Expected {SecretKeyLength}, got {secretKey?.Length ?? 0}.", nameof(secretKey));
        }

        byte[] sharedSecret = new byte[SharedSecretLength];

        NativeMethods.OqsStatus status = NativeMethods.OQS_KEM_decaps(_kemPtr, sharedSecret, ciphertext, secretKey);
        if (status != NativeMethods.OqsStatus.Success) {
            throw new Exception($"OQS_KEM_decaps failed for {AlgorithmName} with status: {status}. This might indicate an invalid ciphertext or secret key.");
        }
        return sharedSecret;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing) {
        if (!_disposed) {
            if (_kemPtr != IntPtr.Zero) {
                NativeMethods.OQS_KEM_free(_kemPtr);
                _kemPtr = IntPtr.Zero;
            }
            _disposed = true;
        }
    }
    public static class LogHelper {
        public static void LogByteArray(byte[] data, string name = "Data") {
            if (data == null) {
                Console.WriteLine($"{name}: null");
                return;
            }

            Console.Write($"{name} ({data.Length} bytes): ");

            if (data.Length == 0) {
                Console.WriteLine("<empty>");
                return;
            }

            string hexString = string.Join(" ", data.Select(b => b.ToString("X2")));
            Console.WriteLine(hexString);
        }
    }
    ~KyberKEM() {
        Dispose(false);
    }
    
}