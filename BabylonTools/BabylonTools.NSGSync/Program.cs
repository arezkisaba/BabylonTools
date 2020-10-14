using Lib.Core;
using Lib.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BabylonTools.NSGSync
{
    class Program
    {
        private static string appDataDirectory;
        private static string steamDirectory;
        private static string steamUserDirectory;
        private static string playstation1RomsDirectory;
        private static string playstation1EmulatorExecutable;
        private static string playstation1EmulatorExecutableArguments;
        private static string playstation1EmulatorBios;
        private static string playstation2RomsDirectory;
        private static string playstation2EmulatorExecutable;
        private static string playstation2EmulatorExecutableArguments;

        static void Main(string[] args)
        {
            ExecuteAsync().Wait();
        }

        private static async Task ExecuteAsync()
        {
            var storageService = new StorageService();

            steamDirectory = ConfigurationManager.AppSettings["steamDirectory"];
            steamUserDirectory = ConfigurationManager.AppSettings["steamUserDirectory"];
            playstation1RomsDirectory = ConfigurationManager.AppSettings["playstation1RomsDirectory"];
            playstation1EmulatorExecutable = ConfigurationManager.AppSettings["playstation1EmulatorExecutable"];
            playstation1EmulatorExecutableArguments = ConfigurationManager.AppSettings["playstation1EmulatorExecutableArguments"];
            playstation1EmulatorBios = ConfigurationManager.AppSettings["playstation1EmulatorBios"];
            playstation2RomsDirectory = ConfigurationManager.AppSettings["playstation2RomsDirectory"];
            playstation2EmulatorExecutable = ConfigurationManager.AppSettings["playstation2EmulatorExecutable"];
            playstation2EmulatorExecutableArguments = ConfigurationManager.AppSettings["playstation2EmulatorExecutableArguments"];

            appDataDirectory = Path.Combine(Environment.GetEnvironmentVariable("appdata"), Assembly.GetExecutingAssembly().GetName().Name);
            Directory.CreateDirectory(appDataDirectory);

            var nonSteamGames = new List<NonSteamGameModel>();
            nonSteamGames.AddRange(await GetPlaystation1GamesAsync());
            nonSteamGames.AddRange(await GetPlaystation2GamesAsync());

            var cachedImageFiles = await storageService.GetFilesFromFolderAsync(appDataDirectory);

            var bytes = new List<byte>();
            WriteHeader(ref bytes);
            foreach (var nonSteamGame in nonSteamGames)
            {
                var httpClient = new HttpClient();

                var imagePath = string.Empty;
                foreach (var cachedImageFile in cachedImageFiles)
                {
                    if (cachedImageFile.StartsWith(nonSteamGame.DisplayName))
                    {
                        imagePath = Path.Combine(appDataDirectory, cachedImageFile);
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(imagePath))
                {
                    Console.Write($"Please enter thumbnail URL for {nonSteamGame.DisplayName} : ");
                    var imageUrl = Console.ReadLine();
                    var imageBytes = await httpClient.GetByteArrayAsync(imageUrl);
                    var extension = Path.GetExtension(imageUrl);
                    imagePath = $@"{appDataDirectory}\{nonSteamGame.DisplayName}{extension}";
                    File.WriteAllBytes(imagePath, imageBytes);
                    Console.WriteLine($"{nonSteamGame.DisplayName}{extension} created successfully");
                }

                WriteContentForGame(ref bytes, nonSteamGame, imagePath);
                Console.WriteLine($"{nonSteamGame.DisplayName} added to Steam");
            }

            WriteFooter(ref bytes);

            var outputFile = $"{Path.Combine(steamUserDirectory, "config")}\\shortcuts.vdf";
            File.WriteAllBytes(outputFile, bytes.ToArray());
            Console.WriteLine($"{outputFile} overwritted successfully");

            await RebootSteamAsync();
            Console.WriteLine($"Steam restarted successfully");
        }

        private static async Task RebootSteamAsync()
        {
            var processes = ProcessesHelper.GetProcesses()
                .Where(obj => obj.ProcessName.ContainsIgnoreCase("steam") && !obj.ProcessName.Contains(Assembly.GetExecutingAssembly().GetName().Name));
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception)
                {
                    // IGNORE
                }
            }

            await Task.Delay(TimeHelper.FromSecondsToMilliseconds(2));
            ProcessesHelper.Start($@"{steamDirectory}\steam.exe");
        }

        private static async Task<List<NonSteamGameModel>> GetPlaystation1GamesAsync()
        {
            var storageService = new StorageService();
            var nonSteamGames = new List<NonSteamGameModel>();
            var authorizedFormats = new string[] { ".bin", ".img", ".iso" };
            var gameFiles = await storageService.GetFilesFromFolderAsync(playstation1RomsDirectory, recursive: true, includePath: true);

            var i = 0;
            foreach (var gameFile in gameFiles)
            {
                var fileInfo = new FileInfo(gameFile);
                if (authorizedFormats.Any(obj => obj == fileInfo.Extension))
                {
                    nonSteamGames.Add(new NonSteamGameModel(i, fileInfo.Name, playstation1EmulatorExecutable, string.Format(playstation1EmulatorExecutableArguments, $"\"{playstation1EmulatorBios}\"", $"\"{gameFile}\"")));
                    i++;
                }
            }

            return nonSteamGames;
        }

        private static async Task<List<NonSteamGameModel>> GetPlaystation2GamesAsync()
        {
            var storageService = new StorageService();
            var nonSteamGames = new List<NonSteamGameModel>();
            var authorizedFormats = new string[] { ".bin", ".img", ".iso" };
            var gameFiles = await storageService.GetFilesFromFolderAsync(playstation2RomsDirectory, recursive: true, includePath: true);

            var i = 0;
            foreach (var gameFile in gameFiles)
            {
                var fileInfo = new FileInfo(gameFile);
                if (authorizedFormats.Any(obj => obj == fileInfo.Extension))
                {
                    nonSteamGames.Add(new NonSteamGameModel(i, fileInfo.Name, playstation2EmulatorExecutable, string.Format(playstation2EmulatorExecutableArguments, $"\"{gameFile}\"")));
                    i++;
                }
            }

            return nonSteamGames;
        }

        private static void WriteHeader(ref List<byte> bytes)
        {
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes("shortcuts"));
            bytes.Add(0x00);
        }

        private static void WriteContentForGame(ref List<byte> bytes, NonSteamGameModel nonSteamGame, string imageUrl)
        {
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes($"{nonSteamGame.Index}"));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("AppName"));
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes(nonSteamGame.DisplayName));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("Exe"));
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes($"\"{nonSteamGame.ExecutablePath}\""));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("StartDir"));
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes($"\"{nonSteamGame.WorkingDirectory}\""));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("icon"));
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes($"\"{imageUrl}\""));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("ShortcutPath"));
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("LaunchOptions"));
            bytes.Add(0x00);

            if (!string.IsNullOrWhiteSpace(nonSteamGame.Arguments))
            {
                bytes.AddRange(Encoding.UTF8.GetBytes($"{nonSteamGame.Arguments}"));
            }

            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(Encoding.UTF8.GetBytes("IsHidden"));
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(Encoding.UTF8.GetBytes("AllowDesktopConfig"));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(Encoding.UTF8.GetBytes("AllowOverlay"));
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(Encoding.UTF8.GetBytes("OpenVR"));
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(Encoding.UTF8.GetBytes("Devkit"));
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x01);
            bytes.AddRange(Encoding.UTF8.GetBytes("DevkitGameID"));
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x02);
            bytes.AddRange(Encoding.UTF8.GetBytes("LastPlayTime"));
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.Add(0x00);
            bytes.AddRange(Encoding.UTF8.GetBytes("tags"));
            bytes.Add(0x00);
            bytes.Add(0x08);
            bytes.Add(0x08);
        }

        private static void WriteFooter(ref List<byte> bytes)
        {
            bytes.Add(0x08);
            bytes.Add(0x08);
        }
    }
}