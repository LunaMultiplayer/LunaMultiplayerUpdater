using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.IO.Compression;
using System.Threading;
using System.Windows.Forms;
using System.Net.Http;

namespace LunaManager
{
    class MainMenu
    {
        private static Thread thread;
        public static string FolderToDecompress = Path.Combine(Path.GetTempPath(), "LMPClientUpdater");
        public const string FileName = "LunaMultiplayerUpdater-Release.zip";
        [STAThread]
        static void Main()
        {
            
     
            thread = new Thread(BusyWorkThread);
            processCheck();
            LunaCheck();
            Menu();

        }
     
       
        
        private static void processCheck()
        {
            try
            {
                foreach (Process proc in Process.GetProcessesByName("KSP_x64"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            try
            {
                foreach (Process proc in Process.GetProcessesByName("KSP"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }
            try
            {
                foreach (Process proc in Process.GetProcessesByName("Updater"))
                {
                    proc.Kill();
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }
        
        private static void kerbalSafeLaunch()
        {
            clearScreen();
            processCheck();
            LunaCheck();
            kerbalLaunch();
        }
        private static void lunaSafeUpdate()
        {
            clearScreen();
            processCheck();
            LunaCheck();
            lunaMultiplayerUpdate();
        }
        private static void Menu()
        {
            Console.WriteLine("Luna Manager for Kerbal Space Program.");
            Console.WriteLine("Here are your options:");
            showCommands();
            Console.WriteLine("Please enter a number to decide");
            var input = double.Parse(Console.ReadLine());
            if (input == 1)
            {
                kerbalSafeLaunch();

            }
            if (input == 2)
            {
               lunaSafeUpdate();
            }
            else
                clearScreen();
            Console.WriteLine("Invalid Option");
            Menu();
        }

  

        private static void showCommands()
        {
            Console.WriteLine("1. Start up Kerbal Space Program ");
            Console.WriteLine("2. Install/Update LunaMultiplayer");
        }
        private static void kerbalLaunch()
        {

            processCheck();
            Console.WriteLine("Booting Kerbal Space Program");
            string kerbal64 = @"KSP_x64.exe";
            if (File.Exists(kerbal64))
                Process.Start(kerbal64);
            else
                Console.WriteLine("Can not start Kerbal Space Program. Did you place this in the KSP installation folder?");
            Menu();
        }
        private static void clearScreen()
        {
            Console.Clear();
        }

        private static void lunaMultiplayerUpdate()
        {
            string lunaUpdater = @"ClientUpdater.exe";

            processCheck();
            LunaCheck();
            Process.Start(lunaUpdater);
            Menu();

        }
        private static void LunaCheck()
        {
            string lunaUpdater = @"ClientUpdater.exe";

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), lunaUpdater)))
            {
                Console.WriteLine(" The \"Updater.exe\" is not in the main KSP folder");
                Console.WriteLine("Installing Luna Updater....");
                WebClient wb = new WebClient();
                wb.DownloadFile("https://github.com/LunaMultiplayer/LunaMultiplayerUpdater/releases/download/1.0.0/LunaMultiplayerUpdater-Release.zip", "LunaMultiplayerUpdater-Release.zip");
               
                string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "LunaMultiplayerUpdater-Release.zip");
                string extractPath = Directory.GetCurrentDirectory();
                ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(), FileName), FolderToDecompress);
                string downloadUrl;
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
                {
                    downloadUrl = "https://github.com/LunaMultiplayer/LunaMultiplayerUpdater/releases/download/1.0.0/LunaMultiplayerUpdater-Release.zip";
                }

                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    Console.WriteLine($"Downloading LMP from: {downloadUrl} Please wait...");
                    try
                    {
                        CleanTempFiles();
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(downloadUrl, Path.Combine(Path.GetTempPath(), FileName));
                            Console.WriteLine($"Downloading succeeded! Path: {Path.Combine(Path.GetTempPath(), FileName)}");
                        }

                        Console.WriteLine($"Decompressing file to {FolderToDecompress}");
                        ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(), FileName), FolderToDecompress);

                        CopyFilesFromTempToDestination(product);

                        Console.WriteLine("-----------------===========FINISHED===========-----------------");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        CleanTempFiles();
                    }
                }

            }
            else
            {
                clearScreen();
            }
        
        }
        public static void BusyWorkThread()
        {
              
            
        }
    }
}



