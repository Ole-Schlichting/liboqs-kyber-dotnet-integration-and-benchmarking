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

        while(true) {
            Console.Write("Enter the Algorithmen Name, Iterations. e.g. Kyber512,100 \n");
            Console.Write("The Algorithms are Kyber512, Kyber768, Kyber1024, RSA2048, RSA3072, RSA4096 \n");
            string input = Console.ReadLine();
            try {
                string[] parts = input.Split(',');
                string algo = parts[0];
                int iterations = Convert.ToInt32(parts[1]);

                if (algo.Equals("Kyber512", StringComparison.OrdinalIgnoreCase)) {
                    kyber512.Run("Kyber512", iterations);
                }
                if (algo.Equals("Kyber768", StringComparison.OrdinalIgnoreCase)) {
                    kyber512.Run("Kyber768", iterations);
                }
                if (algo.Equals("Kyber1024", StringComparison.OrdinalIgnoreCase)) {
                    kyber512.Run("Kyber1024", iterations);
                }



                if (algo.Equals("rsa2048", StringComparison.OrdinalIgnoreCase)) {
                    rsa.RunSystemRsaBenchmark(iterations, 2048);
                }
                if (algo.Equals("rsa3072", StringComparison.OrdinalIgnoreCase)) {
                    rsa.RunSystemRsaBenchmark(iterations, 3072);
                }
                if (algo.Equals("rsa4096", StringComparison.OrdinalIgnoreCase)) {
                    rsa.RunSystemRsaBenchmark(iterations, 4096);
                }
            } catch (Exception e) { 
                Console.WriteLine(e.ToString());
            }

            
        }
        

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