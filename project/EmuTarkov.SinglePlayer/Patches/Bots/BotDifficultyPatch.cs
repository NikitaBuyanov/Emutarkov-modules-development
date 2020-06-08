using System.Reflection;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Bots;
using EFT;
using BotDifficultyHandler = GClass236;

namespace EmuTarkov.SinglePlayer.Patches.Bots
{
	public class BotDifficultyPatch : AbstractPatch
	{
		public BotDifficultyPatch()
		{
			methodName = "LoadDifficultyStringInternal";
			flags = BindingFlags.Public | BindingFlags.Static;
		}

		public override MethodInfo TargetMethod()
		{
			return typeof(BotDifficultyHandler).GetMethod(methodName, flags);
		}

		public static bool Prefix(ref string __result, BotDifficulty botDifficulty, WildSpawnType role)
		{
			foreach (Difficulty difficulty in BotSettings.Difficulties)
			{
				if (difficulty.Role == role && difficulty.BotDifficulty == botDifficulty)
				{
					__result = difficulty.Json;
				}
			}

			if (string.IsNullOrEmpty(__result))
			{
				return true;
			}

			return false;
		}
	}
}
