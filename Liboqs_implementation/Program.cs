// Program.cs
using System;
using System.Diagnostics; // For Stopwatch
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Liboqs_implementation; // For List

public class Program {
    static void Main(string[] args) {
        Kyber_Benchmarking kyber512 = new Kyber_Benchmarking();
        RSA_Benchmarking rsa = new RSA_Benchmarking();
        rsa.RunSystemRsaBenchmark(100, 2048);

        kyber512.Run("Kyber1024", 10000);

    }

    

    // ToHexString function (from previous example, not strictly needed for benchmark but useful for debugging)
    /*public static string ToHexString(byte[] bytes, int maxLength = 0) {
        if (bytes == null) return "null";
        var sb = new StringBuilder();
        int count = (maxLength > 0 && bytes.Length > maxLength && maxLength > 0) ? maxLength : bytes.Length;
        for (int i = 0; i < count; i++) {
            sb.Append(bytes[i].ToString("X2"));
        }
        return sb.ToString();
    }*/
}