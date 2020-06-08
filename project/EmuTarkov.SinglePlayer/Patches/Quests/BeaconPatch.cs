using System.Linq;
using System.Reflection;
using EFT;
using EFT.InventoryLogic;
using EmuTarkov.Common.Utils.Patching;

namespace EmuTarkov.SinglePlayer.Patches.Quests
{
    public class BeaconPatch : AbstractPatch
    {
        public override MethodInfo TargetMethod()
        {
            return typeof(Player).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly).Single(IsTargetMethod);
        }

        private bool IsTargetMethod(MethodInfo method)
        {
            if (!method.IsVirtual)
            {
                return false;
            }

            var parameters = method.GetParameters();

            if (parameters.Length != 2
            || parameters[0].ParameterType != typeof(Item)
            || parameters[0].Name != "item"
            || parameters[1].ParameterType != typeof(string)
            || parameters[1].Name != "zone")
            {
                return false;
            }

            return true;
        }

        public static bool Prefix(Player __instance, Item item, string zone)
        {
            __instance.Profile.ItemDroppedAtPlace(item.TemplateId, zone);

            return false;
        }
    }
}