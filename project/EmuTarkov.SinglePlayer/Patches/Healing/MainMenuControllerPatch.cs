using System.Reflection;
using EmuTarkov.Common.Utils.Patching;
using MainMenuController = GClass1108;
using IHealthController = GInterface147;

namespace EmuTarkov.SinglePlayer.Patches.Healing
{
    class MainMenuControllerPatch : GenericPatch<MainMenuControllerPatch>
    {
        static MainMenuControllerPatch()
        {
            _ = nameof(IHealthController.HydrationChangedEvent);
            _ = nameof(MainMenuController.HealthController);
        }

        public MainMenuControllerPatch() : base(postfix: nameof(PatchPostfix)) { }

        protected override MethodBase GetTargetMethod()
        {
            return typeof(MainMenuController).GetMethod("method_1", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        static void PatchPostfix(MainMenuController __instance)
        {
            var healthController = __instance.HealthController;
            var listener = Utils.Player.HealthListener.Instance;
            listener.Init(healthController, false);
        }
    }
}
