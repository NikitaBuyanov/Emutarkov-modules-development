namespace EmuTarkov.Server.Models.Configs
{
    public class ServerConfig
    {
        public string name;
        public string url;
        public int port;

        public ServerConfig(string Name, string Url, int Port)
        {
            name = Name;
            url = Url;
            port = Port;
        }
    }
}
