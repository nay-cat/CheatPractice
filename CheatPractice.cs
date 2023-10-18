using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.ServiceProcess;
using Pastel;
using System.Drawing;
using System.Security.Principal;
using System.Diagnostics;
using System.Threading;


namespace CheatPractice
{
    /*
     * i love
     * static abuse
     * nwn
     */

    internal class Program
    {
        public static List<Tuple<string, bool>> configuration;
        public static string cheatName;
        public static string cheatPath;
        public static string extensionChangedPath;
        public static int cheatSize;
        public static string language;
        public static string choice;
        public static bool isExec;
        public static bool isCreated;

        static void Main(string[] args)
        {
            Console.Title = "Cheat Practice 1.0.1 by Shadia/Nay <3 ";
            killProcess("taskmgr");

            if (!IsAdmin())
            {
                Console.Title = "Cheat Practice (Needs admin perms)";

                Console.WriteLine("Cheat Practice need admin perms".Pastel(Color.FromArgb(255, 140, 0)));
                Console.WriteLine("This program need to control services & modify traces".Pastel(Color.FromArgb(255, 140, 0)));
                string input = Console.ReadLine();

                // bypass admin perms to test
                // i dont need admin perms -> idnap
                if (!input.Equals("idnap"))
                {
                    Environment.Exit(0);
                }
            }

            configuration = new List<Tuple<string, bool>>();
            configList();

            bool exit = false;
            while (!exit)
            {

                Console.WriteLine("Cheat Practice 1.0.1".Pastel(Color.FromArgb(165, 229, 250)).PastelBg("C123B7"));
                Console.WriteLine("1. Configuration".Pastel(Color.FromArgb(140, 220, 250)));
                Console.WriteLine("2. Hide and execute".Pastel(Color.FromArgb(140, 220, 250)));
                Console.WriteLine("3. Generate Config".Pastel(Color.FromArgb(140, 220, 250)));
                Console.WriteLine("4. Load Config".Pastel(Color.FromArgb(140, 220, 250)));

                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine("5. Exit");
                Console.ForegroundColor = ConsoleColor.Green;

                choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Clear();
                        List<string> numberedConfiguration = new List<string>();

                        for (int i = 0; i < configuration.Count; i++)
                        {
                            numberedConfiguration.Add($"{i + 1}. {configuration[i].Item1}: {configuration[i].Item2}");
                        }
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.WriteLine("Current config:");

                        foreach (var item in numberedConfiguration)
                        {
                            if (item.Contains("True"))
                            {
                                Console.WriteLine(item.Pastel(Color.FromArgb(34, 77, 23)));
                            }
                            else
                            {
                                Console.WriteLine(item.Pastel(Color.FromArgb(255, 204, 203)));
                            }
                        }

                        Console.WriteLine("Write the number to change false/true".Pastel(Color.FromArgb(166, 214, 8)));

                        if (int.TryParse(Console.ReadLine(), out int edit))
                        {
                            if (edit >= 1 && edit <= configuration.Count)
                            {
                                int index = edit - 1;
                                Tuple<string, bool> configItem = configuration[index];

                                Console.Clear();
                                Console.WriteLine($"Editing: {configItem.Item1}, Current Value: {configItem.Item2}".Pastel(Color.FromArgb(173, 216, 230)));
                                Console.Write("Enter 'true' or 'false': ".Pastel(Color.FromArgb(166, 214, 8)));

                                if (bool.TryParse(Console.ReadLine(), out bool newValue))
                                {
                                    configuration[index] = new Tuple<string, bool>(configItem.Item1, newValue);
                                    Console.Clear();
                                    Console.WriteLine("Configuration updated");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input, 'true' or 'false'");
                                }
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Invalid number".Pastel(Color.FromArgb(255, 140, 0)));
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Invalid number");
                        }

                        break;
                    case "3":
                        Console.Clear();
                        SaveConfigurationToJson("CheatPractice.json");
                        break;
                    case "4":
                        Console.WriteLine("Paste the path of the config file".Pastel(Color.FromArgb(34, 140, 193)));
                        string path = Console.ReadLine();
                        LoadConfigurationFromJson(path, configuration);
                        break;
                    case "2":
                        Console.Clear();
                        createCheat();
                        sendResults();
                        break;
                    case "5":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Invalid option");
                        break;
                }
            }

        }
        public static void runCheatCompiler(string path)
        {

            string compilerFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Compiler");
            string randomFolderPath = path;
            string randomFileName = Path.Combine(compilerFolderPath, "FakeCheat.cs");
            if (Directory.Exists(compilerFolderPath))
            {
                string sourceFilePath = Path.Combine(randomFolderPath, randomFileName);
                try
                {
                    // ESTUVE 1 HORA PARA HCER ESTO XD
                    Process.Start(@"cmd.exe", $"/K cd \"{compilerFolderPath}\" & csc.exe /out:\"{randomFolderPath}\" \"{sourceFilePath}\"");
                }
                catch 
                {
                    Console.WriteLine("Compiling error");
                }
            }
            else
            {
                Console.WriteLine("Compiler folder not found");
            }
        }

        public static void sendResults()
        {
            Console.ReadLine();
            Console.WriteLine($"Cheat name: {cheatName}");
            Console.WriteLine($"Original Cheat path: {cheatPath}");
            Console.WriteLine($"Cheat Size: {cheatSize} bytes");
            bool changeExtension = getConfiguration("ChangeExtension", configuration);
            if (changeExtension)
            {
                Console.WriteLine($"Extension Changed Cheat: {extensionChangedPath}");
            }
            Console.ReadLine();
        }

        static string GenerateRandomColor(Random random)
        {
            byte r = (byte)random.Next(256);
            byte g = (byte)random.Next(256);
            byte b = (byte)random.Next(256);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        public static void createCheat()
        {
            Random random = new Random();
            bool changeExtension = getConfiguration("ChangeExtension", configuration);
            string randomFileName;
            bool stopSvc = getConfiguration("StopMainServices", configuration);
            bool startSvc = getConfiguration("StartServicesAfterExecute", configuration);
            bool moreSvc = getConfiguration("StopMoreServices", configuration);
            bool stopBam = getConfiguration("DeleteBam", configuration);
            bool changeTime = getConfiguration("ChangeTime", configuration);
            bool stringCleaner = getConfiguration("StringCleaner", configuration);

            randomFileName = GenerateRandomFileName(".exe");
            string randomFolderPath = GetRandomAccessibleFolder();
            if (randomFolderPath == null)
            {
                Console.WriteLine("Invalid path");
            }
            string fullPath = Path.Combine(randomFolderPath, randomFileName);
            if (stopSvc)
            {
                StopService("sysmain");
                StopService("pcasvc");

                if (moreSvc)
                {
                    StopService("dps");
                    StopService("diagtrack");
                    StopService("eventlog");
                }
            }
            try
            {
                runCheatCompiler(fullPath);
                Thread.Sleep(750);
                Process.Start(fullPath);
                Thread.Sleep(1500);
                killProcess("cmd");
                killProcess("csc");
                killProcess(randomFileName.Replace(".exe", ""));
                killProcess(fullPath);
                Console.WriteLine("File executed successfully.");
                isExec = true;

            }
            catch
            {
                Console.WriteLine("Error executing file");
                isExec = false;
            }

            try
            {
                using (FileStream fs = new FileStream(fullPath, FileMode.Create))
                {
                    Console.WriteLine("Created executable");
                    isCreated = true;
                    fs.Close();
                }

            }
            catch (Exception ex)
            {
                Thread.Sleep(150);
                Console.WriteLine("Error creating executable" + ex);
                isCreated = false;
            }

            if (!isCreated && !isExec)
            {
                Console.WriteLine("Cheat not working");
                Console.ReadLine();
                Environment.Exit(0);

            }

            bool deletePrefetch = getConfiguration("NoPrefetch", configuration);
            if (deletePrefetch)
            {
                if (stopSvc)
                {
                    Console.WriteLine("idk why enable prefetch deletion with stopped services?");
                }
                string[] files = Directory.GetFiles(@"C:\Windows\Prefetch\", "*.pf");
                foreach (string file in files)
                {
                    if (file.Contains(randomFileName))
                    {
                        File.Delete(file);
                        Console.WriteLine("Deleting prefetch");
                    }
                }
            }

            bool randomSize = getConfiguration("RandomSize", configuration);

            bool hasCommonStrings = getConfiguration("CommonCheatStrings", configuration);
            if (hasCommonStrings)
            {
                string[] stringList = new string[] { "AutoClick", "Vape", "mouse_event", "autoclick", "loader", ".xyz", ".gg", ".lite", "modules", "module" };

                string randomWord = stringList[random.Next(stringList.Length)];

                using (StreamWriter sw = new StreamWriter(fullPath))
                {
                    sw.Write(randomWord);
                    Console.WriteLine("Common string added");
                    sw.Close();
                }

            }
            using (FileStream fs = new FileStream(fullPath, FileMode.Open))
            {
                if (randomSize)
                {
                    int dataSize = random.Next(1, 3000000 + 1);
                    byte[] data = new byte[dataSize];
                    random.NextBytes(data);
                    fs.Write(data, 0, data.Length);
                    cheatSize = dataSize;
                    Console.WriteLine("Generating random size...");
                    fs.Close();
                }
                else
                {
                    byte[] data = new byte[1024];
                    fs.Write(data, 0, data.Length);
                    cheatSize = 1024;
                    fs.Close();
                }
            }

            if (stopBam)
            {
                StopService("bam");
                Process.Start("cmd.exe", "/C PsExec.exe -s -d -i reg delete \"HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Services\\bam\\State\\UserSettings\" /d /n c:");
                string reg = "reg delete \"HKEY_LOCAL_MACHINE\\SYSTEM\\ControlSet001\\Services\\bam\\State\\UserSettings\" /f";
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "PsExec.exe",
                    Arguments = $"-s -d -i {reg}",
                    UseShellExecute = false,
                    Verb = "runas",
                    RedirectStandardOutput = true,
                    CreateNoWindow = false
                };

                Process process = new Process
                {
                    StartInfo = psi
                };

                process.Start();
                process.WaitForExit();

                Console.WriteLine("Stopping bam...");
            }

            if (changeExtension)
            {
                string[] newExtensions = { ".txt", ".png", ".dll", ".ini", ".msi" };

                if (File.Exists(fullPath))
                {
                    try
                    {
                        using (FileStream fsn = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.None))
                        {
                            fsn.Close();
                            string newExtension = newExtensions[random.Next(newExtensions.Length)];

                            if (Path.GetExtension(fullPath) != newExtension)
                            {
                                string newFilePath = Path.ChangeExtension(fullPath, newExtension);
                                File.Move(fullPath, newFilePath);
                                Console.WriteLine("Extension changed");
                                extensionChangedPath = newFilePath;
                            }
                        }
                    }
                    catch (Exception echepchion)
                    {
                        Console.WriteLine("Error changing extension" + echepchion);
                    }
                }
            }

            if (changeTime) {
                DateTime newDate = DateTime.Now.AddDays(-random.Next(1, 11));
                try
                {
                    FileInfo fileInfo = new FileInfo(fullPath);
                    fileInfo.LastWriteTime = newDate;
                    Console.WriteLine("Changing file date...".Pastel(Color.FromArgb(15, 153, 203)));
                }
                catch 
                {
                    Console.WriteLine("Error changing file date".Pastel(Color.FromArgb(255, 0, 0)));
                }
            }

            if (stringCleaner)
            {
                string compilerFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Compiler");
                if (Directory.Exists(compilerFolderPath))
                {
                    try
                    {
                        Process.Start(@"cmd.exe", $"/K cd \"{compilerFolderPath}\" & stringhelper.exe {randomFileName}");
                        Console.WriteLine("Deleting strings...".Pastel(Color.FromArgb(15, 153, 203)));
                        Thread.Sleep(1250);
                        killProcess("stringhelper");
                        killProcess("cmd");
                    }
                    catch
                    {
                        Console.WriteLine("Error deleting strings".Pastel(Color.FromArgb(255, 0, 0)));
                    }
                }
                else
                {
                    Console.WriteLine("Compiler folder not found");
                }
            }

            bool deleteJournal = getConfiguration("DeleteJournal", configuration);
            if (deleteJournal)
            {
                Process.Start("cmd.exe", "/C fsutil usn deletejournal /d /n c:");
                Console.WriteLine("Deleting journal...".Pastel(Color.FromArgb(63, 169, 74)));
            }

            if (startSvc)
            {
                StartService("sysmain");
                StartService("pcasvc");
                StartService("dps");
                StartService("diagtrack");
                StartService("eventlog");

                if (stopBam)
                {
                    StartService("bam");
                }
            }

            bool fuckEventViewer = getConfiguration("FuckEventViewer", configuration);
            if (fuckEventViewer)
            {
                try
                {
                    EventLog[] eventLogs = EventLog.GetEventLogs();

                    foreach (EventLog eventLog in eventLogs)
                    {
                        eventLog.Clear();
                    }

                    Console.WriteLine("Deleting events...");
                }
                catch 
                {
                    Console.WriteLine("Error deleting events");
                }
            }

            Console.WriteLine("\nPress enter to reveal cheat".Pastel(Color.FromArgb(81, 63, 169)));

            cheatName = randomFileName;
            cheatPath = randomFolderPath;
        }
        public static string GenerateRandomFileName(string fileExtension)
        {
            Random random = new Random();
            const string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

            string fileName = new string(Enumerable.Repeat(chars, random.Next(3, 10))
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return fileName + fileExtension;
        }

        public static bool StartService(string serviceName)
        {
            try
            {
                using (ServiceController serviceController = new ServiceController(serviceName))
                {
                    if (serviceController.Status != ServiceControllerStatus.Running)
                    {
                        serviceController.Start();
                        serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30));
                        Console.WriteLine("Starting service ".Pastel(Color.FromArgb(166, 214, 8)));
                    }
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Error starting service".Pastel(Color.FromArgb(255, 0, 0)));
                return false;
            }
        }
        public static bool StopService(string serviceName)
        {
            try
            {
                using (ServiceController serviceController = new ServiceController(serviceName))
                {
                    if (serviceController.Status != ServiceControllerStatus.Stopped)
                    {
                        serviceController.Stop();
                        serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30));
                        Console.WriteLine("Stopping service ".Pastel(Color.FromArgb(255, 182, 193)));

                    }
                    return true;
                }
            }
            catch
            {
                Console.WriteLine("Error stopping service".Pastel(Color.FromArgb(255, 0, 0)));
                return false;
            }
        }


        public static string GetRandomAccessibleFolder()
        {

            List<string> validPaths = new List<string>();

            string[] subdirectories = Directory.GetDirectories("C:\\");

            foreach (string subdirectory in subdirectories)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("Searching valid folders...".Pastel(Color.FromArgb(255, 140, 0)));
                    Directory.GetAccessControl(subdirectory);

                    bool allowWindowsPath = getConfiguration("HideInWindowsFolders", configuration);
                    if (subdirectories.Contains("Windows") && allowWindowsPath == false)
                    {
                        GetRandomAccessibleFolder();
                    }
                    validPaths.Add(subdirectory);
                }
                catch (UnauthorizedAccessException)
                {
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Acess error '{subdirectory}': {ex.Message}");
                }
            }

            if (validPaths.Count > 0)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, validPaths.Count);
                return validPaths[randomIndex];
            }

            return null;
        }

        public static void configList()
        {
            configuration.Add(new Tuple<string, bool>("HideInWindowsFolders", false));
            configuration.Add(new Tuple<string, bool>("DeleteBam", false));
            configuration.Add(new Tuple<string, bool>("StopMainServices", false));
            configuration.Add(new Tuple<string, bool>("StartServicesAfterExecute", false));
            configuration.Add(new Tuple<string, bool>("NoPrefetch", false));
            configuration.Add(new Tuple<string, bool>("RandomSize", false));
            configuration.Add(new Tuple<string, bool>("ChangeExtension", false));
            configuration.Add(new Tuple<string, bool>("ChangeTime", false));
            configuration.Add(new Tuple<string, bool>("StringCleaner", false));
            configuration.Add(new Tuple<string, bool>("DeleteJournal", false));
            configuration.Add(new Tuple<string, bool>("CommonCheatStrings", false));
            configuration.Add(new Tuple<string, bool>("StopMoreServices", false));
            configuration.Add(new Tuple<string, bool>("FuckEventViewer", false));

        }


        public static void SaveConfigurationToJson(string fileName)
        {
            try
            {
                File.WriteAllText(fileName, JsonSerializer.Serialize(configuration));
                Console.WriteLine($"Configuration saved to {fileName}");
            }
            catch
            {
                Console.WriteLine("Error saving configuration".Pastel(Color.FromArgb(255, 140, 0)));
            }
        }

        public static bool getConfiguration(string name, List<Tuple<string, bool>> list)
        {
            foreach (Tuple<string, bool> tuple in list)
            {
                if (tuple.Item1 == name)
                {
                    return tuple.Item2;
                }
            }
            throw new InvalidOperationException("Config: " + name + " not found");
        }

        public bool SetConfiguration(string name, List<Tuple<string, bool>> list, bool newValue)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Item1 == name)
                {
                    list[i] = new Tuple<string, bool>(name, newValue);
                    return true;
                }
            }

            throw new InvalidOperationException("Config: " + name + " not found");
        }
        static bool IsAdmin()
        {
            WindowsPrincipal principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static String AddQuotesIfRequired(string path)
        {
            return !string.IsNullOrWhiteSpace(path) ?
                path.Contains(" ") && (!path.StartsWith("\"") && !path.EndsWith("\"")) ?
                    "\"" + path + "\"" : path :
                    string.Empty;
        }

        public static void killProcess(String name)
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                process.Kill();
            }
        }

        public static void LoadConfigurationFromJson(string fileName, List<Tuple<string, bool>> configurationList)
        {
            if (File.Exists(fileName))
            {
                try
                {
                    var deserializedConfiguration = JsonSerializer.Deserialize<List<Tuple<string, bool>>>(File.ReadAllText(fileName));

                    if (deserializedConfiguration.Count == configurationList.Count)
                    {
                        for (int i = 0; i < configurationList.Count; i++)
                        {
                            var deserializedTuple = deserializedConfiguration[i];
                            configurationList[i] = new Tuple<string, bool>(deserializedTuple.Item1, deserializedTuple.Item2);
                        }

                        Console.WriteLine("Configuration loaded and updated.".Pastel(Color.FromArgb(81, 63, 169)));
                    }
                    else
                    {
                        Console.WriteLine("Error: Outdated config file".Pastel(Color.FromArgb(255, 182, 193)));
                    }
                }
                catch 
                {
                    Console.WriteLine("Error loading configuration".Pastel(Color.FromArgb(255, 182, 193)));
                }
            }
            else
            {
                Console.WriteLine("Configuration file not found".Pastel(Color.FromArgb(255, 182, 193)));
            }
        }
    }

}
