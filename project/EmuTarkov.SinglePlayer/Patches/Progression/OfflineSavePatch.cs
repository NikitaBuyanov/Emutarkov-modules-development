using System;
using System.Reflection;
using Comfort.Common;
using EFT;
using EmuTarkov.Common.Utils.Patching;
using EmuTarkov.SinglePlayer.Utils.Player;
using ClientMetrics = GClass1260;

namespace EmuTarkov.SinglePlayer.Patches.Progression
{
    class OfflineSaveProfilePatch : AbstractPatch
    {
        static OfflineSaveProfilePatch()
        {
            // compile-time check
            _ = nameof(ClientMetrics.Metrics);
        }

        public override MethodInfo TargetMethod()
        {
            return PatcherConstants.MainApplicationType.GetMethod("method_37", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        }

        public static void Prefix(ESideType ___esideType_0, Result<ExitStatus, TimeSpan, ClientMetrics> result)
        {
            var session = Utils.Config.BackEndSession;
            string backendUrl = Utils.Config.BackendUrl;
            bool isPlayerScav = false;
            Profile profile = session.Profile;

            if (___esideType_0 == ESideType.Savage)
            {
                profile = session.ProfileOfPet;
                isPlayerScav = true;
            }

            var currentHealth = Utils.Player.HealthListener.Instance.CurrentHealth;

            SaveLootUtil.SaveProfileProgress(backendUrl, session.GetPhpSessionId(), result.Value0, profile, currentHealth, isPlayerScav);
        }
    }
}
