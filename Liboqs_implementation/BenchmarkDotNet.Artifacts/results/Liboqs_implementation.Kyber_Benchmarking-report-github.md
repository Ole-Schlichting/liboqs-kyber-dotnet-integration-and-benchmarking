```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.4351/24H2/2024Update/HudsonValley)
AMD Ryzen 5 3600 3.60GHz, 1 CPU, 12 logical and 6 physical cores
.NET SDK 9.0.300
  [Host]     : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  Job-BOEMXS : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.16 (8.0.1625.21506), X64 RyuJIT AVX2

Runtime=.NET 8.0  InvocationCount=1  UnrollFactor=1  

```
| Method | Job        | Toolchain                | IterationCount | AlgorithmName | Mean      | Error     | StdDev     | Median    | Rank | TotalCycles/Op | Allocated |
|------- |----------- |------------------------- |--------------- |-------------- |----------:|----------:|-----------:|----------:|-----:|---------------:|----------:|
| **KeyGen** | **Job-HRFCSV** | **InProcessNoEmitToolchain** | **Default**        | **Kyber1024**     | **263.99 μs** |  **6.468 μs** |  **18.453 μs** | **262.75 μs** |    **4** |             **NA** |    **4784 B** |
| Encaps | Job-HRFCSV | InProcessNoEmitToolchain | Default        | Kyber1024     | 201.30 μs |  4.753 μs |  13.249 μs | 199.60 μs |    3 |             NA |    2944 B |
| Decaps | Job-HRFCSV | InProcessNoEmitToolchain | Default        | Kyber1024     | 119.10 μs |  2.345 μs |   3.363 μs | 117.95 μs |    1 |             NA |    1064 B |
| KeyGen | Job-BOEMXS | Default                  | 100            | Kyber1024     | 524.08 μs | 43.640 μs | 128.674 μs | 528.30 μs |    6 |      1,879,572 |    4784 B |
| Encaps | Job-BOEMXS | Default                  | 100            | Kyber1024     | 392.20 μs | 34.278 μs |  98.900 μs | 398.85 μs |    6 |      1,447,690 |    1648 B |
| Decaps | Job-BOEMXS | Default                  | 100            | Kyber1024     | 180.73 μs |  8.064 μs |  22.877 μs | 178.30 μs |    2 |        758,252 |      56 B |
| KeyGen | DefaultJob | Default                  | Default        | Kyber1024     | 463.06 μs | 32.297 μs |  95.229 μs | 440.45 μs |    6 |      1,786,511 |    4784 B |
| Encaps | DefaultJob | Default                  | Default        | Kyber1024     | 299.18 μs | 14.807 μs |  42.004 μs | 286.70 μs |    5 |      1,184,891 |    1648 B |
| Decaps | DefaultJob | Default                  | Default        | Kyber1024     | 168.30 μs |  5.480 μs |  15.810 μs | 161.65 μs |    2 |        585,236 |      56 B |
|        |            |                          |                |               |           |           |            |           |      |                |           |
| **KeyGen** | **Job-HRFCSV** | **InProcessNoEmitToolchain** | **Default**        | **Kyber512**      | **188.67 μs** |  **4.109 μs** |  **11.109 μs** | **185.50 μs** |    **5** |             **NA** |    **4112 B** |
| Encaps | Job-HRFCSV | InProcessNoEmitToolchain | Default        | Kyber512      | 131.61 μs |  2.953 μs |   8.033 μs | 128.95 μs |    4 |             NA |    2144 B |
| Decaps | Job-HRFCSV | InProcessNoEmitToolchain | Default        | Kyber512      |  54.97 μs |  1.098 μs |   1.127 μs |  54.50 μs |    1 |             NA |    1688 B |
| KeyGen | Job-BOEMXS | Default                  | 100            | Kyber512      | 271.54 μs |  7.083 μs |  20.207 μs | 267.45 μs |    6 |        894,566 |    2480 B |
| Encaps | Job-BOEMXS | Default                  | 100            | Kyber512      | 189.88 μs |  7.346 μs |  20.599 μs | 181.80 μs |    5 |        672,399 |     848 B |
| Decaps | Job-BOEMXS | Default                  | 100            | Kyber512      | 105.07 μs |  7.964 μs |  23.356 μs | 103.15 μs |    3 |        450,232 |      56 B |
| KeyGen | DefaultJob | Default                  | Default        | Kyber512      | 370.16 μs | 30.181 μs |  88.990 μs | 352.40 μs |    7 |      1,356,595 |    2480 B |
| Encaps | DefaultJob | Default                  | Default        | Kyber512      | 192.46 μs |  7.573 μs |  21.110 μs | 184.90 μs |    5 |        675,676 |     848 B |
| Decaps | DefaultJob | Default                  | Default        | Kyber512      |  76.97 μs |  2.408 μs |   6.593 μs |  76.20 μs |    2 |        317,194 |      56 B |
|        |            |                          |                |               |           |           |            |           |      |                |           |
| **KeyGen** | **Job-HRFCSV** | **InProcessNoEmitToolchain** | **Default**        | **Kyber768**      | **261.34 μs** | **17.261 μs** |  **50.623 μs** | **244.40 μs** |    **4** |             **NA** |    **5264 B** |
| Encaps | Job-HRFCSV | InProcessNoEmitToolchain | Default        | Kyber768      | 190.70 μs | 11.418 μs |  33.308 μs | 181.35 μs |    3 |             NA |    1840 B |
| Decaps | Job-HRFCSV | InProcessNoEmitToolchain | Default        | Kyber768      |  96.90 μs |  4.820 μs |  14.136 μs |  88.30 μs |    1 |             NA |    1688 B |
| KeyGen | Job-BOEMXS | Default                  | 100            | Kyber768      | 330.93 μs | 14.459 μs |  42.178 μs | 320.00 μs |    5 |      1,064,305 |    3632 B |
| Encaps | Job-BOEMXS | Default                  | 100            | Kyber768      | 224.13 μs |  6.461 μs |  18.537 μs | 216.90 μs |    4 |        764,805 |    1168 B |
| Decaps | Job-BOEMXS | Default                  | 100            | Kyber768      | 152.38 μs | 10.790 μs |  31.645 μs | 150.00 μs |    2 |        637,665 |      56 B |
| KeyGen | DefaultJob | Default                  | Default        | Kyber768      | 388.41 μs | 28.356 μs |  82.715 μs | 358.50 μs |    6 |      1,580,728 |    3632 B |
| Encaps | DefaultJob | Default                  | Default        | Kyber768      | 226.01 μs |  6.896 μs |  19.224 μs | 220.25 μs |    4 |        773,980 |    1168 B |
| Decaps | DefaultJob | Default                  | Default        | Kyber768      | 157.45 μs | 11.427 μs |  33.514 μs | 159.40 μs |    2 |        739,901 |      56 B |
