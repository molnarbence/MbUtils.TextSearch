﻿// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using Benchmarks;

var runner = BenchmarkRunner.Run<SearchComparison>();