using EmuTarkov.Common.Utils.App;

namespace EmuTarkov.Launcher
{
	public class Account
	{
		private LauncherConfig launcherConfig;
		public AccountInfo SelectedAccount { get; private set; }

		public Account(LauncherConfig launcherConfig)
		{
			this.launcherConfig = launcherConfig;
			SelectedAccount = null;
		}

		public int Login(string email, string password)
		{
			LoginRequestData data = new LoginRequestData(email, password);
			string id = "FAILED";
			string json = "";

			try
			{
				id = RequestHandler.RequestLogin(data);
				json = RequestHandler.RequestAccount(data);

				if (id == "FAILED")
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			SelectedAccount = Json.Deserialize<AccountInfo>(json);
            RequestHandler.ChangeSession(SelectedAccount.id);

            launcherConfig.Email = email;
			launcherConfig.Password = password;
			JsonHandler.SaveLauncherConfig(launcherConfig);

            return 1;
		}

		public int Register(string email, string password, string edition)
		{
			RegisterRequestData data = new RegisterRequestData(email, password, edition);
			string registerStatus = "FAILED";

			try
			{
				registerStatus = RequestHandler.RequestRegister(data);

				if (registerStatus != "OK")
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			int loginStatus = Login(email, password);

			if (loginStatus != 1)
			{
				switch (loginStatus)
				{
					case -1:
						return -3;

					case -2:
						return -2;
				}
			}

			return 1;
		}

		public int Remove()
		{
			LoginRequestData data = new LoginRequestData(SelectedAccount.email, SelectedAccount.password);
			string json = "FAILED";

			try
			{
				json = RequestHandler.RequestAccount(data);

				if (json != "OK")
				{
					return -1;
				}
			}
			catch
			{
				return -1;
			}

			SelectedAccount = null;

			launcherConfig.Email = "";
			launcherConfig.Password = "";
			JsonHandler.SaveLauncherConfig(launcherConfig);
			return 1;
		}

		public int ChangeEmail(string email)
		{
			ChangeRequestData data = new ChangeRequestData(SelectedAccount.email, SelectedAccount.password, email);
			string json = "FAILED";

			try
			{
				json = RequestHandler.RequestChangeEmail(data);

				if (json != "OK")
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			launcherConfig.Email = email;
			SelectedAccount.email = email;
			JsonHandler.SaveLauncherConfig(launcherConfig);
			return 1;
		}

		public int ChangePassword(string password)
		{
			ChangeRequestData data = new ChangeRequestData(SelectedAccount.email, SelectedAccount.password, password);
			string json = "FAILED";

			try
			{
				json = RequestHandler.RequestChangePassword(data);

				if (json != "OK")
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			SelectedAccount.password = password;
			launcherConfig.Password = password;
			JsonHandler.SaveLauncherConfig(launcherConfig);
			return 1;
		}

		public int Wipe(string edition)
		{
			RegisterRequestData data = new RegisterRequestData(SelectedAccount.email, SelectedAccount.password, edition);
			string json = "FAILED";

			try
			{
				json = RequestHandler.RequestWipe(data);

				if (json != "OK")
				{
					return -1;
				}
			}
			catch
			{
				return -2;
			}

			SelectedAccount.edition = edition;
			return 1;
		}
	}
}
