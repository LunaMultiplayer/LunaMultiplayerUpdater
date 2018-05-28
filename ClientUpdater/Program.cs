using CommonUpdater;
using System;
using System.IO;
using System.Threading;

namespace ClientUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("This program will download the latest UNSTABLE version and replace your current LMPClient");

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "KSP.exe")) || 
                !File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "KSP_x64.exe")))
                Console.WriteLine("Please drop \"Client LMP Updater\" in the main KSP folder next to KSP.exe/KSP_x64.exe!");
            else
            {
                Downloader.DownloadAndReplaceFiles(ProductToDownload.Client);
            }

            Thread.Sleep(3000);
        }
    }
}
