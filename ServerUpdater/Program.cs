using CommonUpdater;
using System;
using System.IO;
using System.Threading;

namespace ServerUpdater
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var buildVersion = "Debug";
            if (args.Length == 1)
            {

                foreach (var argument in args)
                {
                    var splitted = argument.Split('=');

                    if (splitted[0] == "version")
                    {

                        buildVersion = splitted[1];
                    }
                }
            }


            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "Server.exe")))
                Console.WriteLine("Please drop \"Server LMP Updater\" in the LMP server folder next to Server.exe!");
            else
            {
                Downloader.DownloadAndReplaceFiles(ProductToDownload.Server, buildVersion);
            }

            Thread.Sleep(3000);
        }
    }
}
