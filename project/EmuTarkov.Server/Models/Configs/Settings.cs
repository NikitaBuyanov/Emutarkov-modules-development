using System;

namespace EmuTarkov.Server.Models.Configs
{
    public static class Settings
    {
        static string Version;
        static string RootPath;

        static Settings()
        {
            Version = "NXT 0.12.6-A1";
            RootPath = Environment.CurrentDirectory;
        }
    }
}
