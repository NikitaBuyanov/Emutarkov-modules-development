using System.Linq;
using System.Reflection;
using UnityEngine;
using EFT;
using EmuTarkov.Common.Utils.Patching;
using AmmoInfo = GClass1564;

namespace EmuTarkov.SinglePlayer.Patches.Progression
{
    public class WeaponDurabilityPatch : AbstractPatch
    {
        static WeaponDurabilityPatch()
        {
            // compile-time check
            _ = nameof(AmmoInfo.AmmoLifeTimeSec);
        }

        public override MethodInfo TargetMethod()
        {
            //private void method_46(GClass1564 ammo)
            return typeof(Player.FirearmController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance).Single(IsTargetMethod);
        }

        private static bool IsTargetMethod(MethodInfo methodInfo)
        {
            if (methodInfo.IsVirtual)
            {
                return false;
            }

            var parameters = methodInfo.GetParameters();
            var methodBody = methodInfo.GetMethodBody();

            if (parameters.Length != 1
            || parameters[0].ParameterType != typeof(AmmoInfo)
            || parameters[0].Name != "ammo")
            {
                return false;
            }

            if (methodBody.LocalVariables.Any(x => x.LocalType == typeof(Vector3)))
            {
                return true;
            }

            return false;
        }

        public static void Postfix(Player.FirearmController __instance, AmmoInfo ammo)
        {
            var item = __instance.Item;
            float durability = item.Repairable.Durability;
            float deterioration = ammo.Deterioration;
            float operatingResource = (item.Template.OperatingResource > 0) ? item.Template.OperatingResource : 1;

            if (durability <= 0f)
            {
                return;
            }

            durability -= item.Repairable.MaxDurability / operatingResource * deterioration;
            item.Repairable.Durability = (durability > 0) ? durability : 0;
        }
    }
}
