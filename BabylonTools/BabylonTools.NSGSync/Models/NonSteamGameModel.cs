using System.IO;

namespace BabylonTools.NSGSync
{
    public class NonSteamGameModel
    {
        public int Index { get; set; }

        public string DisplayName { get; set; }

        public string ExecutablePath { get; set; }

        public string Arguments { get; set; }

        public string WorkingDirectory { get; set; }

        public NonSteamGameModel(int index, string displayName, string executablePath, string arguments)
        {
            Index = index;
            DisplayName = displayName;
            ExecutablePath = executablePath;
            Arguments = arguments;
            WorkingDirectory = new FileInfo(this.ExecutablePath).Directory.FullName;
        }
    }
}
