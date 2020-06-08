using EFT;

namespace EmuTarkov.SinglePlayer.Utils.Bots
{
	public class Difficulty
	{
		public WildSpawnType Role;
		public BotDifficulty BotDifficulty;
		public string Json;

		public Difficulty(WildSpawnType role, BotDifficulty botDifficulty, string json)
		{
			Role = role;
			BotDifficulty = botDifficulty;
			Json = json;
		}
	}
}
