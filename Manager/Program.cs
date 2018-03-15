using System;
using System.Diagnostics;
using System.Linq;

namespace Manager
{

    class MainMenu
    {
        static void Main()

        {

           processCheck();

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
        private static void Menu()
        {
            var menu = new EasyConsole.Menu()
   .Add("Run Kerbal Space Program", () => kerbalLaunch())
   .Add("Install/Update LunaMultiplayer", () => lunaMultiplayerUpdate());
            menu.Display();
        }
        private static void kerbalLaunch()
        {
            Console.WriteLine("Booting Kerbal Space Program");
            Process.Start(@"F:\SteamLibrary\steamapps\common\Kerbal Space Program\KSP_x64.exe");

        }
        private static void lunaMultiplayerUpdate()
        {
            Console.WriteLine("Booting Kerbal Space Program");
            Process.Start(@"F:\SteamLibrary\steamapps\common\Kerbal Space Program\Updater.exe");

        }
    }
}