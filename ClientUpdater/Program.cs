﻿using CommonUpdater;
using System;
using System.IO;
using System.Threading;

namespace ClientUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var buildVersion = "Debug";
            if (args.Length == 1) {

                foreach (var argument in args)
                {
                    var splitted = argument.Split('=');

                    if (splitted[0] == "version")
                    {
                        buildVersion = splitted[1];
                    }
                }
            }

            Console.WriteLine($"This program will download the latest {buildVersion} version and replace your current LMPClient");

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "KSP_x64.exe")) && !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "KSP.x86_64")))
                Console.WriteLine("Please drop \"Client LMP Updater\" in the main KSP folder next to KSP.x86_64/KSP_x64.exe!");
            else
            {
                Downloader.DownloadAndReplaceFiles(ProductToDownload.Client, buildVersion);
            }

            Thread.Sleep(3000);
        }
    }
}
