using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static System.Console;
#pragma warning disable CRRSP01 // A misspelled word has been found
namespace LunaManager
{
    /// <summary>
    ///     <para>Contains main method.</para>
    ///     <para>Contains checks and validated for KSP.</para>
    /// </summary>
    public abstract class MainMenu
    {
        private const string ApiUrl = "https://github.com/LunaMultiplayer/LunaMultiplayerUpdater/releases/download/1.0.0";

        private const string FileName = "LunaMultiplayerUpdater-Release.zip";

        private static string _projectUrl = $"{ApiUrl}/LunaMultiplayerUpdater-Release.zip";

        private static readonly string ClientFolderToDecompress = Path.Combine (Path.GetTempPath(), "LMPClientUpdater");

        private static readonly string ServerFolderToDecompress = Path.Combine (Path.GetTempPath(), "LMPServerUpdater");

        private static void CleanTempClientFiles()
        {
            try
            {
                if (Directory.Exists (ClientFolderToDecompress))
                {
                    Directory.Delete (ClientFolderToDecompress, true);
                }
            }
            catch (Exception e)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine (e);
            }

            File.Delete (Path.Combine (Path.GetTempPath(), FileName));
        }
        
        private static void CleanTempServerFiles()
        {
            try{
                if (ServerFolderToDecompress != null && Directory.Exists (ServerFolderToDecompress)){
                    Directory.Delete (ServerFolderToDecompress,
                        true);
                }
            } catch (Exception e){
                ForegroundColor = ConsoleColor.Red;
                WriteLine (e);
            }

            File.Delete (Path.Combine (Path.GetTempPath(),
                FileName));
        }

        private static void ClearScreen()
        {
            ResetColor();
            Clear();
        }
        
        // ReSharper disable once FunctionRecursiveOnAllPaths
        private static void ClientMenu()
        {
            WriteLine (
                "Welcome to a Kerbal Space Program CLI. This is for actively updating Luna Multiplayer during beta testing. \nBelow are some options that will hopefully make things a lot more simpler.");

            WriteLine ("Options:");
            ResetColor();
            ShowClientCommands();

            WriteLine ("Enter a number to choose:");

            var i = int.Parse (ReadLine() ?? throw new InvalidOperationException());
            if (i == 1){
                ClearScreen();
                KerbalSafeLaunch();
            }

            if (i == 2){
                ClearScreen();
                LunaSafeClientUpdate();
            }

            if (i == 3){
                ClearScreen();
                ServerMenu();
            }

            if (i == 4){
                ClearScreen();
                ShowKspDir();
            }
            if (i == 66){
                ClearScreen();
                GuiTest();
            }

            ForegroundColor = ConsoleColor.Red;

            WriteLine ("Invalid Option\n");
            ResetColor();
            ClientMenu();
        }
        private static void ShowKspDir()
        {
            try
            {
                var kspDir = Directory.GetCurrentDirectory();
                using (var lunaServerProcess = new Process { StartInfo = new ProcessStartInfo(kspDir) { FileName = kspDir, CreateNoWindow = true, UseShellExecute = true } })
                {
                    lunaServerProcess.Start();
                }

            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            ClientMenu();
        }
        private static void GuiTest()
        {
            try
            {
                var lunaServer = @"LunaLauncher.dll";
                using (var lunaServerProcess = new Process { StartInfo = new ProcessStartInfo(lunaServer) { FileName = lunaServer, CreateNoWindow = true, UseShellExecute = true } })
                {
                    lunaServerProcess.Start();
                }

            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            ServerMenu();
        }

        /// <summary>
        /// </summary>
        private static void CopyClientFilesFromTempToDestination()
        {
            var productFolderName = "LMPClientUpdater";
            foreach (var dirPath in Directory.GetDirectories(Path.Combine(ClientFolderToDecompress,
                                                                         productFolderName),
                                                            "*",
                                                            SearchOption.AllDirectories))
            {
                var destFolder = dirPath.Replace(Path.Combine(ClientFolderToDecompress,
                                                              productFolderName),
                                                 Directory.GetCurrentDirectory());
                WriteLine($"Creating dest folder: {destFolder}");
                Directory.CreateDirectory(destFolder);
            }

            foreach (var newPath in Directory.GetFiles(Path.Combine(ClientFolderToDecompress,
                                                                   productFolderName),
                                                      "*.*",
                                                      SearchOption.AllDirectories))
            {
                var destPath = newPath.Replace(Path.Combine(ClientFolderToDecompress,
                                                            productFolderName),
                                               Directory.GetCurrentDirectory());
                WriteLine($"Copying {Path.GetFileName(newPath)} to {destPath}");
                File.Copy(newPath,
                          destPath,
                          true);
            }
        }

        /// <summary>
        /// </summary>
        private static void CopyServerFilesFromTempToDestination()
        {
            var productFolderName = "LMPServerUpdater";
            var serverFolder = Directory.GetCurrentDirectory() + "\\Server";
            foreach (var dirPath in Directory.GetDirectories(Path.Combine(ServerFolderToDecompress,
                                                                         productFolderName),
                                                            "*",
                                                            SearchOption.AllDirectories))
            {
                var destFolder = dirPath.Replace(Path.Combine(ServerFolderToDecompress,
                                                              productFolderName),
                                                 serverFolder);
                WriteLine($"Creating dest folder: {destFolder}");
            }

            foreach (var newPath in Directory.GetFiles(Path.Combine(ServerFolderToDecompress,
                                                                   productFolderName),
                                                      "*.*",
                                                      SearchOption.AllDirectories))
            {
                var destPath = newPath.Replace(Path.Combine(ServerFolderToDecompress,
                                                            productFolderName),
                                               serverFolder);
                WriteLine($"Copying {Path.GetFileName(newPath)} to {destPath}");
                File.Copy(newPath,
                          destPath,
                          true);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="product"></param>
        /// <exception cref="InvalidEnumArgumentException"></exception>
        private static void DownloadAndReplaceFiles(ProductToDownload product)
        {
            if (!Enum.IsDefined(typeof(ProductToDownload),
                               product))
                throw new InvalidEnumArgumentException(nameof(product),
                                                       (int)product,
                                                       typeof(ProductToDownload));
            string downloadUrl;
            using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
            {
                downloadUrl = GetDownloadUrlAsync(client).Result;
            }

            if (!string.IsNullOrEmpty(downloadUrl))
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine($"Downloading LMP from: {downloadUrl} Please wait...");
                try
                {
                    CleanTempClientFiles();
                    CleanTempServerFiles();
                    using (var client = new WebClient())
                    {
                        client.DownloadFile(downloadUrl,
                                            Path.Combine(Path.GetTempPath(),
                                                         FileName));
                        WriteLine($"Downloading succeeded! Path: {Path.Combine(Path.GetTempPath(), FileName)}");
                    }

                    WriteLine($"Decompressing file to {ClientFolderToDecompress}");
                    ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(),
                                                            FileName),
                                               ClientFolderToDecompress);

                    CopyClientFilesFromTempToDestination();
                    CopyServerFilesFromTempToDestination();
                    ForegroundColor = ConsoleColor.Green;
                    WriteLine("-----------------===========FINISHED===========-----------------");
                }
                catch (Exception e)
                {
                    ForegroundColor = ConsoleColor.Red;
                    WriteLine(e);
                    ResetColor();
                    throw;
                }
                finally
                {
                    CleanTempClientFiles();
                    CleanTempServerFiles();
                }
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="client"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        private static async Task<string> GetDownloadUrlAsync(HttpClient client)
        {
            var httpResponseMessage = await client.GetAsync(_projectUrl).ConfigureAwait(false);
            if (httpResponseMessage == null) throw new ArgumentNullException(nameof(httpResponseMessage));
            using (var response = httpResponseMessage)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// </summary>
        private static void InstallDirCheck()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var folder = new DirectoryInfo(path).Name;

            var target = @"Kerbal Space Program";
            if (folder != target)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine("HALT Criminal Scum!\nThis is not the Kerbal Space Program Folder! ");
                WriteLine("The manager will now end until this is resolved.");
                ReadLine();
                Environment.Exit(0);
            }
        }

        /// <summary>
        /// </summary>
        private static void KerbalLaunch()
        {
            InstallDirCheck();
            SanityCheck();
            BackgroundColor = ConsoleColor.Green;
            WriteLine("Booting Kerbal Space Program");
            var kerbal64 = @"KSP_x64.exe";
            if (File.Exists(kerbal64))
            {
                Process.Start(kerbal64);
            }
            else
            {
                WriteLine("Can not start Kerbal Space Program. Did you place this in the KSP installation folder?");
            }

            ResetColor();
            ClientMenu();
        }

        /// <summary>
        /// </summary>
        private static void KerbalSafeLaunch()
        {
            ClearScreen();
            SanityCheck();
            InstallDirCheck();
            LunaClientCheck();
            KerbalLaunch();
        }

        /// <summary>
        /// </summary>
        private static void LunaClientCheck()
        {
            var lunaUpdater = @"ClientUpdater.exe";

            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(),
                                         lunaUpdater)))
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(" The \"ClientUpdater.exe\" is not in the main KSP folder");
                ResetColor();
                WriteLine("Installing Luna Updater....");
                _projectUrl = "https://github.com/LunaMultiplayer/LunaMultiplayerUpdater/releases/download/1.0.0/LunaMultiplayerUpdater-Release.zip";
                if (!string.IsNullOrEmpty(_projectUrl))
                {
                    WriteLine($"Downloading LMP from: {_projectUrl} Please wait...");
                    try
                    {
                        CleanTempClientFiles();
                        using (var client = new WebClient())
                        {
                            client.DownloadFile(_projectUrl,
                                                Path.Combine(Path.GetTempPath(),
                                                             FileName));
                            WriteLine($"Downloading succeeded! Path: {Path.Combine(Path.GetTempPath(), FileName)}");
                        }

                        WriteLine($"Decompressing file to {ClientFolderToDecompress}");
                        ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(),
                                                                FileName),
                                                   ClientFolderToDecompress);
                        DownloadAndReplaceFiles(ProductToDownload.Client);
                        CopyClientFilesFromTempToDestination();
                        BackgroundColor = ConsoleColor.DarkGreen;
                        ForegroundColor = ConsoleColor.White;
                        WriteLine("-----------------===========FINISHED===========-----------------");
                        ResetColor();
                    }
                    catch (Exception e)
                    {
                        BackgroundColor = ConsoleColor.Red;
                        WriteLine(e);
                        CleanTempClientFiles();
                        throw;
                    }
                    finally
                    {
                        CleanTempClientFiles();
                    }
                }
            }
            else
            {
                ClearScreen();
            }
        }

        /// <summary>
        /// </summary>
        private static void LunaClientUpdate()
        {
            var lunaClientUpdater = @"ClientUpdater.exe";
            InstallDirCheck();
            SanityCheck();
            LunaClientCheck();
            var lunaClientProcess = new Process();
            var newProcessStartInfo = new ProcessStartInfo(lunaClientUpdater)
            {
                UseShellExecute = false
            };
            lunaClientProcess.StartInfo = newProcessStartInfo;

            lunaClientProcess.Start();
            lunaClientProcess.WaitForExit();
            ClientMenu();
        }

        /// <summary>
        /// </summary>
        private static void LunaSafeClientUpdate()
        {
            ClearScreen();
            SanityCheck();
            InstallDirCheck();
            LunaClientCheck();
            LunaClientUpdate();
        }

        /// <summary>
        /// </summary>
        private static void LunaSafeServerUpdate()
        {
            ClearScreen();
            InstallDirCheck();
            Installed++;
            WriteLine(Installed);
            LunaServerUpdate();
        }

        /// <summary>
        /// </summary>
        private static void LunaServerCheck()
        {
            var lunaUpdater = @"ServerUpdater.exe";
            if (!File.Exists(Path.Combine(Directory.GetCurrentDirectory(),
                                         "Server",
                                         lunaUpdater)))
            {
                Installed = 0;
                ForegroundColor = ConsoleColor.Red;
                WriteLine(" The file \"ServerUpdater.exe\" is not in the Luna Server folder...");
                ResetColor();
                WriteLine("Installing Luna Updater....");
                var extractPath = Directory.GetCurrentDirectory() + "\\Server";
                Directory.CreateDirectory(extractPath);
                _projectUrl =
                    "https://github.com/LunaMultiplayer/LunaMultiplayerUpdater/releases/download/1.0.0/LunaMultiplayerUpdater-Release.zip";
                var webClient = new WebClient();
                WriteLine($"Downloading LMP from: {_projectUrl} Please wait...");
                try
                {
                    CleanTempServerFiles();
                    using (new WebClient())
                    {
                        webClient.DownloadFile(_projectUrl,
                                Path.Combine(Path.GetTempPath(),
                                    FileName));
                        WriteLine($"Downloading succeeded! Path: {Path.Combine(Path.GetTempPath(), FileName)}");
                    }
                    WriteLine($"Decompressing file to {ServerFolderToDecompress}");
                    ZipFile.ExtractToDirectory(Path.Combine(Path.GetTempPath(), FileName),
                                               ServerFolderToDecompress);
                    DownloadAndReplaceFiles(ProductToDownload.Server);
                    CopyServerFilesFromTempToDestination();
                    var serverExeStub = @"\Server\Server.exe";
                    if (!File.Exists(serverExeStub))
                    {
                        using (var fs = File.Create(Directory.GetCurrentDirectory() + serverExeStub))
                        {
                            var exeStub = new UTF8Encoding(true).GetBytes(
                                "Hi, thanks for all the fish! I am a stub until you boot the Luna Manager or run the  Server Updater independently. I do nothing, but tell our updaters that I exist! And should update the folder I am sat in to be filled with Luna server files! If you are reading me then you have not started using the Luna Manager to operate the server which will upset the great god Zeus and he shall rain a eternal damnation of you and your kerbals. Your Pa's will never be perfectly circlular and you will be forced to forget vital parts of your staging causing complete restarts of your launches! Now, I dont think anyone wants to go through that. I personally would hate to see someone fight fruitlessly towards the great gods and be banished from being able to reach the stars. But, This is clearly a warning to be taken seriously before its too late!");
                            // Luna Updater requires Server.exe to be in directory, this creates the application as a stub for update.
                            fs.Write(exeStub,
                                     0,
                                     exeStub.Length);
                        }
                    }

                    Installed++;
                    BackgroundColor = ConsoleColor.DarkGreen;
                    WriteLine("-----------------===========FINISHED===========-----------------");
                    ResetColor();
                }
                catch (Exception e)
                {
                    BackgroundColor = ConsoleColor.Red;
                    WriteLine(e);
                    CleanTempServerFiles();
                    throw;
                }
                finally
                {
                    Installed = 1;
                    CleanTempServerFiles();
                }
            }
            else
            {
                Installed = 1;
                ClearScreen();
            }
        }

        /// <summary>
        /// </summary>
        private static void LunaServerUpdate()
        {
            SanityCheck();
            LunaServerCheck();
            var lunaServerUpdater = @"ServerUpdater.exe";
            using (var lunaServerUpdateProcess = new Process
            {
                StartInfo = new ProcessStartInfo(lunaServerUpdater)
                    { WorkingDirectory = @"Server", FileName = lunaServerUpdater, CreateNoWindow = false, UseShellExecute = true }
            })
            {
                lunaServerUpdateProcess.Start();
                lunaServerUpdateProcess.WaitForExit();

                ServerMenu();
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="InvalidOperationException">Condition.</exception>
        [STAThread]
        private static void Main()
        { MainAsync(); }

        private static void MainAsync()
        {
            var arguments = Environment.GetCommandLineArgs();
            foreach (var unused in arguments){
                if (arguments.Contains ("-server")){
                    LunaServerCheck();
                    LunaSafeServerUpdate();
                    RunLunaServer();
                }

                if (arguments.Contains ("-client")){
                    LunaClientCheck();
                    LunaSafeClientUpdate();
                }
            }

            InstallDirCheck();
            LunaClientCheck();
            ClientMenu();
        }

        /// <summary>
        /// </summary>
        private static void RunLunaServer()
        {
            try
            {
                var lunaServer = @"Server.exe";
                using (var lunaServerProcess = new Process
                    { StartInfo = new ProcessStartInfo(lunaServer) { WorkingDirectory = @"Server", FileName = lunaServer, CreateNoWindow = false, UseShellExecute = true } })
                {
                    lunaServerProcess.Start();
                }

            }
            catch (Exception e)
            {
                WriteLine(e.Message);
            }

            ServerMenu();
        }

        /// <summary>
        /// </summary>
        private static void SanityCheck()
        {
            try
            {
                foreach (var proc in Process.GetProcessesByName("KSP_x64"))
                {
                    proc.Kill();
                    BackgroundColor = ConsoleColor.Green;
                    WriteLine("Kerbal Space Program was found running and has been killed.");
                    ResetColor();
                }
            }
            catch (Exception ex)
            {
                Write(ex.Message);
            }

            try
            {
                foreach (var proc in Process.GetProcessesByName("KSP"))
                {
                    proc.Kill();
                    BackgroundColor = ConsoleColor.Green;
                    WriteLine("Kerbal Space Program was found running and has been killed.");
                    ResetColor();
                }
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                Write(ex.Message);
            }

            try
            {
                foreach (var proc in Process.GetProcessesByName("Updater"))
                {
                    proc.Kill();
                    BackgroundColor = ConsoleColor.Green;
                    WriteLine("Luna Updater was found running and has been killed.");
                    ResetColor();
                }
            }
            catch (Exception ex)
            {
                ForegroundColor = ConsoleColor.Red;
                Write(ex.Message);
            }
        }

        /// <summary>
        /// </summary>
        private static void ServerMenu()
        {
            LunaServerCheck();
            WriteLine(
                "Here you can operate and manage your Luna Multiplayer servers by choosing one of the options below.");
            WriteLine("Options:");
            ResetColor();
            ShowServerCommands();

            WriteLine("Enter a number to choose:");
            var input = int.Parse(ReadLine() ?? throw new InvalidOperationException());
            if (input == 1)
            {
                LunaSafeServerUpdate();
            }

            if (input == 2)
            {
                ClearScreen();

                RunLunaServer();
            }

            if (input == 3)
            {
                //ConfigureServer();
            }

            if (input == 4)
            {
                ClearScreen();
                ClientMenu();
            }

            if (input == 000)
            {
                ClearScreen();
                UninstallLuna();
            }

            ForegroundColor = ConsoleColor.Red;

            WriteLine("Invalid Option\n");
            ResetColor();
            ServerMenu();
        }

        /// <summary>
        /// 
        /// </summary>
        private static void ShowClientCommands()
        {
            ForegroundColor = ConsoleColor.Magenta;
            WriteLine("1. Start up Kerbal Space Program ");
            WriteLine("2. Install/Update LunaMultiplayer");
            WriteLine("3. Run LunaMultiplayer Server");
            WriteLine("4. Open KSP Directory");
            ResetColor();
        }

        /// <summary>
        /// </summary>
        private static void ShowServerCommands()
        {
            var fCount = Directory.GetFiles("Server",
                                            "*",
                                            SearchOption.AllDirectories)
                .Length;

            ClearScreen();
            ForegroundColor = ConsoleColor.Magenta;
            if (fCount == 3)
            {
                WriteLine("1. Install LunaMultiplayer");
            }
            else if (fCount > 3)
            {
                WriteLine("1. Update LunaMultiplayer");
                WriteLine("2. Start up Luna Server ");
                WriteLine("3. Configure LunaMultiplayer");
            }

            ForegroundColor = ConsoleColor.DarkGreen;
            WriteLine("4. Return to Luna Client options");
            ResetColor();
        }

        /// <summary>
        /// </summary>
        private static void UninstallLuna()
        {
            WriteLine("============= Sad to see you go =============");
            WriteLine("Which would you like to remove?");
            WriteLine("1. Luna Client");
            WriteLine("2. Luna Server");
            var input = int.Parse(ReadLine() ?? throw new InvalidOperationException());
            if (input == 1)
            {
                if (Directory.Exists(@"GameData\LunaMultiplayer"))
                    Directory.Delete(@"GameData\LunaMultiplayer");

                if (File.Exists(@"ClientUpdater.exe"))
                    File.Delete(@"ClientUpdater.exe");

                if (File.Exists(@"CommonUpdater.dll"))
                    File.Delete(@"CommonUpdater.dll");
            }

            if (input == 2)
            {
                ForegroundColor = ConsoleColor.Red;
                WriteLine(
                    "This will purge your /Server folder leaving anything not backed up, gone forever! Are you sure you wish to delete?");
                var confirm = ReadLine();
                if ((confirm == "y") | (confirm == "Y") | (confirm == "YES") | (confirm == "yes"))
                    try
                    {
                        if (Directory.Exists(Directory.GetCurrentDirectory() + "\\Server"))
                        {
                            foreach (var proc in Process.GetProcessesByName("LMPServer"))
                            {
                                proc.Kill();
                                BackgroundColor = ConsoleColor.Green;
                                WriteLine(
                                    "Luna Server was found running and has been killed. Continuing to uninstall");
                                ResetColor();
                            }

                            Directory.Delete(Directory.GetCurrentDirectory() + "\\Server");
                            WriteLine("============= Files have been removed =============");
                        }
                    }
                    catch (Exception ex)
                    {
                        ForegroundColor = ConsoleColor.Red;
                        Write(ex.Message);
                    }
                else
                    WriteLine("Removal concluded");
            }
        }


        /// <summary>
        /// </summary>
        private static int Installed
        {
            get; set;
        }

        /// <summary>
        /// </summary>
        private enum ProductToDownload
        {
            Client = 0,

            Server = 1
        }
    }
}
