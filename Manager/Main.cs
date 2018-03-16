using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

namespace LunaManager
{
    public class MainMenu
    {
        [STAThread]
        public static void Main()
        {
            ProcessCheck();
            LunaCheck();
            Menu();
        }

        private static void ProcessCheck()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("KSP_x64"))
                {
                    proc.Kill();
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            try
            {
                foreach (var proc in Process.GetProcessesByName("KSP"))
                {
                    proc.Kill();
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
            try
            {
                foreach (var proc in Process.GetProcessesByName("Updater"))
                {
                    proc.Kill();
                }
            }
            catch (Exception e)
            {
                Console.Write(e);
            }

        }

        private static void Menu()
        {
            while (true)
            {
                Console.WriteLine("Luna Manager for Kerbal Space Program.");
                Console.WriteLine("Here are your options:");
                ShowCommands();
                Console.WriteLine("Please enter a number to decide");
                var input = int.Parse(Console.ReadLine());
                if (input == 1)
                {
                    ClearScreen();
                    ProcessCheck();
                    LunaCheck();
                    KerbalLaunch();
                }

                if (input == 2)
                {
                    ClearScreen();
                    ProcessCheck();
                    LunaCheck();
                    LunaMultiplayerUpdate();
                }
                else
                    ClearScreen();

                Console.WriteLine("Invalid Option");
            }
        }

        private static void ShowCommands()
        {
            Console.WriteLine("1. Start up Kerbal Space Program ");
            Console.WriteLine("2. Install/Update LunaMultiplayer");
        }

        private static void KerbalLaunch()
        {
            ProcessCheck();
            Console.WriteLine("Booting Kerbal Space Program");
            if (File.Exists("KSP_x64.exe"))
                Process.Start("KSP_x64.exe");
            else
                Console.WriteLine("Can not start Kerbal Space Program. Did you place this in the KSP installation folder?");
            Menu();
        }

        private static void ClearScreen()
        {
            Console.Clear();
        }

        private static void LunaMultiplayerUpdate()
        {
            ProcessCheck();
            LunaCheck();
            Process.Start("ClientUpdater.exe");
            Menu();
        }

        private static void LunaCheck()
        {
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "ClientUpdater.exe")))
            {
                Console.WriteLine("The \"Updater.exe\" is not in the main KSP folder");
                Console.WriteLine("Installing Luna Updater....");

                using (var wb = new WebClient())
                {
                    wb.DownloadFile("https://github.com/LunaMultiplayer/LunaMultiplayerUpdater/releases/download/1.0.0/LunaMultiplayerUpdater-Release.zip", "LunaMultiplayerUpdater-Release.zip");
                }

                Console.WriteLine("Download completed. Extraction Needed.");
                Thread.Sleep(1000);
                Console.ReadKey();
                Environment.Exit(1);
            }
            else
            {
                Console.WriteLine("Luna Updater is located!\n");
            }
        }
    }
}



