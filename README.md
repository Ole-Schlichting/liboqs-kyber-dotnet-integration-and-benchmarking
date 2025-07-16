Post-Quantum (Kyber) and RSA Benchmark Suite

This project provides a command-line tool to benchmark the performance and measure the data sizes of the Kyber post-quantum cryptography algorithm against traditional RSA with various key lengths.
It is built using .NET 8 and the BenchmarkDotNet library for accurate performance measurements.

Getting Started
Follow these instructions to set up the project and run the tests on your local machine.

Prerequisites
To run this project, you will need:
.NET 8 SDK (or newer)
Git

Installation

Clone the repository to your local machine:
git clone https://github.com/Ole-Schlichting/liboqs-kyber-dotnet-integration-and-benchmarking.git
Navigate to the project's root directory
All commands must be executed from the root directory of the project. The base command is dotnet run -c Release, which is followed by -- and the specific command arguments.

Running Performance Benchmarks
To run the performance benchmarks, use the benchmark command followed by the algorithm name.
Available Commands:

benchmark kyber  
benchmark rsa  
benchmark rsa-7680  
benchmark rsa-15360  

Example:
To run the benchmark for the Kyber algorithm, execute the following command:

dotnet run -c Release -- benchmark kyber


Note: The RSA benchmarks are split into multiple commands due to the significant increase in execution time with larger key sizes.

Measuring Data Sizes

To measure the size of the generated keys and ciphertexts, use the sizes command.
Available Commands:

sizes kyber  
sizes rsa

It is not possible to publish this tool as a standalone executable for running the performance benchmarks.
The BenchmarkDotNet library is intrinsically linked to the source code and the .NET SDK. To ensure fair and accurate measurements, it requires access to the original project files (.csproj) to compile a new, isolated test environment at runtime.
Therefore, the benchmarks must always be run from the source code directory as described in the usage instructions above.
