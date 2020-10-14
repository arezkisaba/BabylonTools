using System;
using System.IO;

namespace BabylonTools.Sherlock
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Parameter missing : directory");
                return;
            }

            var path = args[0].EndsWith(@"\") ? args[0] : $@"{args[0]}\";
            var fileSystemWatcher = new FileSystemWatcher(path);
            fileSystemWatcher.Filter = "*.*";
            fileSystemWatcher.IncludeSubdirectories = true;
            fileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            fileSystemWatcher.Changed += new FileSystemEventHandler(Watcher_Changed);
            fileSystemWatcher.Created += new FileSystemEventHandler(Watcher_Changed);
            fileSystemWatcher.Deleted += new FileSystemEventHandler(Watcher_Changed);
            fileSystemWatcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
            fileSystemWatcher.EnableRaisingEvents = true;

            Console.WriteLine($"Listening on {path} changes...");
            Console.WriteLine("Press 'q' to quit.");
            while (Console.ReadKey().KeyChar != 'q') ;
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"{e.ChangeType} : {e.OldFullPath} > {e.FullPath}");
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"{e.ChangeType} : {e.FullPath}");
        }
    }
}
