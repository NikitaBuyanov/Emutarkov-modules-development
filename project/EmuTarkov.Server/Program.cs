using EmuTarkov.Server.Utils.Server;

namespace EmuTarkov.Server
{
    /// <summary>
    /// Program entry point
    /// </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            new AssemblyLoader("EmuTarkov_Data/Managed/");

            // get url from config
            new SelfSigned("EmuTarkov", 443);

            Router router = new Router("https://127.0.0.1/");

            router.Start();

            while (router.IsRunning())
            {
                router.Update();
            }

            router.Stop();
        }
    }
}
