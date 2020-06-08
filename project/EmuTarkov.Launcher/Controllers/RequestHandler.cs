using EmuTarkov.Common.Utils.App;
using EmuTarkov.Common.Utils.HTTP;

namespace EmuTarkov.Launcher
{
	public static class RequestHandler
	{
		private static Request request = new Request(null, "");

		public static string GetBackendUrl()
		{
			return request.RemoteEndPoint;
		}

		public static void ChangeBackendUrl(string remoteEndPoint)
		{
			request.RemoteEndPoint = remoteEndPoint;
		}

        public static void ChangeSession(string session)
        {
            request.Session = session;
        }

        public static string RequestConnect()
		{
			return request.GetJson("/launcher/server/connect");
		}

		public static string RequestLogin(LoginRequestData data)
		{
			return request.PostJson("/launcher/profile/login", Json.Serialize(data));
		}

		public static string RequestRegister(RegisterRequestData data)
		{
			return request.PostJson("/launcher/profile/register", Json.Serialize(data));
		}

		public static string RequestRemove(LoginRequestData data)
		{
			return request.PostJson("/launcher/profile/remove", Json.Serialize(data));
		}

		public static string RequestAccount(LoginRequestData data)
		{
			return request.PostJson("/launcher/profile/get", Json.Serialize(data));
		}

		public static string RequestChangeEmail(ChangeRequestData data)
		{
			return request.PostJson("/launcher/profile/change/email", Json.Serialize(data));
		}

		public static string RequestChangePassword(ChangeRequestData data)
		{
			return request.PostJson("/launcher/profile/change/password", Json.Serialize(data));
		}

		public static string RequestWipe(RegisterRequestData data)
		{
			return request.PostJson("/launcher/profile/change/wipe", Json.Serialize(data));
		}
	}
}
