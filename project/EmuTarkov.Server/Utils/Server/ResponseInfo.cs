namespace EmuTarkov.Server.Utils.Server
{
    public class ResponseInfo
    {
        public string Body;
        public bool Compressed;

        public ResponseInfo(string body, bool compressed)
        {
            Body = body;
            Compressed = compressed;
        }
    }
}
