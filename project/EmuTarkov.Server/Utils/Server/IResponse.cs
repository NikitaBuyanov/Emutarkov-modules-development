namespace EmuTarkov.Server.Utils.Server
{
    public interface IResponse
    {
        ResponseInfo HandleResponse(RequestInfo reqInfo);
    }
}
