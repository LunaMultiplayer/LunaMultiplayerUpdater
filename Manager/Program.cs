using System;
using System.Diagnostics;

namespace Manager
{

    class MainMenu
    {
        static void Main()

        {

            kerbalCheck();

            Menu();

        }

        private static void kerbalCheck()
        {
            Process[] kerbalProcess = Process.GetProcessesByName("KSP");
            Process[] kerbalProcess64 = Process.GetProcessesByName("KSP_x64");
            if (kerbalProcess.Length > 0)
            {
                Console.WriteLine("Kerbal Space Program is already Running ");
            }
            if (kerbalProcess64.Length > 0)
            {
                Console.WriteLine("Kerbal Space Program is already Running ");
            }
            else
            {
                Console.WriteLine("Kerbal Space Program is not running.");
            }

        }
        private static void Menu()
        {
            var menu = new EasyConsole.Menu()
   .Add("Run Kerbal Space Program", () => Console.WriteLine("Booting Kerbal Space Program"))
   .Add("Install/Update LunaMultiplayer", () => Console.WriteLine("Luna Selected"));
            menu.Display();
        }
    }
}