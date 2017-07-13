﻿using MbUtils.TextSearch.Business;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace MbUtils.TextSearch.ConsoleHost
{
    class Program
    {
        static void Main(string[] args)
        {
            // check count of arguments, should be 3 or more
            if (args.Length < 3)
            {
                PrintUsage("Not enough input parameters");
                return;
            }

            // create business logic class
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddConsole();
            var filePathProvider = new FilePathProvider(loggerFactory);
            var mainLogic = new MainLogic(loggerFactory, filePathProvider);

            // input parameters
            var inputFolderPath = args[0];
            var searchTerm = args[1];
            var outputFilePath = args[2];

            try
            {
                mainLogic.Search(inputFolderPath, searchTerm, outputFilePath);
            }
            catch (ArgumentException ex)
            {
                PrintUsage(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Done...");
            Console.Read();
        }

        static void PrintUsage(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Correct usage is: [source path] [search term] [output file]");
            Console.WriteLine("e.g.: c:\\InputFolder \".net rocks\" c:\\OutFolder\\output.csv");
        }
    }
}