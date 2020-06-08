using System.Reflection;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Bots;
using BotDifficultyHandler = GClass236;

namespace EmuTarkov.SinglePlayer.Patches.Bots
{
	public class CoreDifficultyPatch : AbstractPatch
	{
		public CoreDifficultyPatch()
		{
			methodName = "LoadCoreByString";
			flags = BindingFlags.Public | BindingFlags.Static;
		}

		public override MethodInfo TargetMethod()
		{
			return typeof(BotDifficultyHandler).GetMethod(methodName, flags);
		}

		public static bool Prefix(ref string __result)
		{
			__result = BotSettings.CoreDifficulty;

			if (string.IsNullOrEmpty(__result))
			{
				return true;
			}

			return false;
		}
	}
}
