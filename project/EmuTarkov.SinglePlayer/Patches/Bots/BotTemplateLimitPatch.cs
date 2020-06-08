using System.Collections.Generic;
using System.Reflection;
using EFT;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Bots;
using WaveInfo = GClass865;
using BotsPresets = GClass294;

namespace EmuTarkov.SinglePlayer.Patches.Bots
{
    public class BotTemplateLimitPatch : AbstractPatch
    {
        static BotTemplateLimitPatch()
        {
            // compile-time checks
            _ = nameof(BotsPresets.CreateProfile);
            _ = nameof(WaveInfo.Difficulty);
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(BotsPresets).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void Postfix(List<WaveInfo> __result, List<WaveInfo> wavesProfiles, List<WaveInfo> delayed)
        {
            /*
                In short this method sums Limits by grouping wavesPropfiles collection by Role and Difficulty
                then in each group sets Limit to 30, the remainder is stored in "delayed" collection.
                So we change Limit of each group.
                Clear delayed waves, we don't need them if we have enough loaded profiles and in method_2 it creates a lot of garbage.
            */

            delayed?.Clear();
            
            foreach (WaveInfo wave in __result)
            {
				wave.Limit = BotSettings.Limits[wave.Role];
            }
        }
    }
}
