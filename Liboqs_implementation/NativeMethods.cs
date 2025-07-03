// NativeMethods.cs
using System;
using System.Runtime.InteropServices;
using System.Text; // Required if you use CharSet.Ansi for string marshalling

internal static class NativeMethods {
    private const string OqsDll = "oqs.dll"; // Name of the liboqs DLL

    // OQS_STATUS enum
    public enum OqsStatus {
        Success = 0,
        Error = -1
    }

    // --- KEM Functions ---
    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    public static extern IntPtr OQS_KEM_new(string method_name);

    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void OQS_KEM_free(IntPtr kem);

    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern OqsStatus OQS_KEM_keypair(IntPtr kem, byte[] public_key, byte[] secret_key);

    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern OqsStatus OQS_KEM_encaps(IntPtr kem, byte[] ciphertext, byte[] shared_secret, byte[] public_key);

    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern OqsStatus OQS_KEM_decaps(IntPtr kem, byte[] shared_secret, byte[] ciphertext, byte[] secret_key);

    // Structure to read KEM details (lengths) from the OQS_KEM object
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct OqsKemDetails {
        public IntPtr method_name_ptr; // const char*
        public IntPtr alg_version_ptr; // const char*
        public byte claimed_nist_level; // uint8_t
        [MarshalAs(UnmanagedType.U1)] // C bool is typically 1 byte
        public bool ind_cca;
        public UIntPtr length_public_key;
        public UIntPtr length_secret_key;
        public UIntPtr length_ciphertext;
        public UIntPtr length_shared_secret;
    }

    // --- Common/Initialization Functions ---
    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void OQS_init();

    [DllImport(OqsDll, CallingConvention = CallingConvention.Cdecl)]
    public static extern void OQS_destroy(); // If needed for cleanup

    // Static constructor to call OQS_init() once when this class is first accessed
    static NativeMethods() {
        try {
            OQS_init();
        } catch (DllNotFoundException ex) {
            Console.Error.WriteLine($"CRITICAL ERROR: {OqsDll} not found. Ensure it is in the application's output directory (e.g., bin/Debug/netX.Y) AND that the application's Platform Target (Project Properties > Build) matches the DLL's architecture (x86/x64).");
            Console.Error.WriteLine($"Details: {ex.Message}");
            // It's often best to let the application crash here or handle this critical failure appropriately
            // For a console app, printing an error and then allowing it to throw is reasonable.
            throw;
        } catch (Exception ex) // Catch other potential initialization errors
          {
            Console.Error.WriteLine($"CRITICAL ERROR during OQS_init: {ex.Message}");
            throw;
        }
    }
}