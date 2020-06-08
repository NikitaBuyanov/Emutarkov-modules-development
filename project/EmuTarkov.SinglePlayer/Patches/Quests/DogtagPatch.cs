using System;
using System.Reflection;
using UnityEngine;
using EFT;
using EFT.InventoryLogic;
using EmuTarkov.Common.Utils.Patching;
using Equipment = GClass1547;
using DamageInfo = GStruct203;

namespace EmuTarkov.SinglePlayer.Patches.Quests
{
    class DogtagPatch : AbstractPatch
    {
        private static readonly Func<Player, Equipment> getEquipmentProperty;

        static DogtagPatch()
        {
            // compile-time checks
            _ = nameof(Equipment.GetSlot);
            _ = nameof(DamageInfo.Weapon);

            getEquipmentProperty = typeof(Player)
                .GetProperty("Equipment", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetGetMethod(true)
                .CreateDelegate(typeof(Func<Player, Equipment>)) as Func<Player, Equipment>;
        }

        public override MethodInfo TargetMethod()
        {
            return typeof(Player).GetMethod("OnBeenKilledByAggressor", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public static void Postfix(Player __instance, Player aggressor, DamageInfo damageInfo)
        {
            if (__instance.Profile.Info.Side == EPlayerSide.Savage)
            {
                return;
            }

            var equipment = getEquipmentProperty(__instance);
            var dogtagSlot = equipment.GetSlot(EquipmentSlot.Dogtag);
            var dogtagItem = dogtagSlot.ContainedItem as Item;

            if (dogtagItem == null)
            {
                Debug.LogError("DogtagPatch error > DogTag slot item is null somehow.");
                return;
            }

            DogtagComponent itemComponent = dogtagItem.GetItemComponent<DogtagComponent>();

            if (itemComponent == null)
            {
                Debug.LogError("DogtagPatch error > DogTagComponent on dog tag slot is null. Something went horrifically wrong!");
                return;
            }

            var victimProfileInfo = __instance.Profile.Info;

            itemComponent.Nickname = victimProfileInfo.Nickname;
            itemComponent.Side = victimProfileInfo.Side;
            itemComponent.KillerName = aggressor.Profile.Info.Nickname;
            itemComponent.Time = DateTime.Now;
            itemComponent.Status = "Killed by";
            itemComponent.WeaponName = damageInfo.Weapon.Name;

            if (__instance.Profile.Info.Experience > 0)
            {
                itemComponent.Level = victimProfileInfo.Level;
            }
        }
    }
}
