using DeafLink.Core;
using DeafLink.UI;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace DeafLink.Patches;

[HarmonyPatch(typeof(ItemGun), nameof(ItemGun.ShootRPC))]
internal static class WeaponPatch
{
    [HarmonyPostfix]
    private static void Postfix(ItemGun __instance)
    {
        var position = __instance.gunMuzzle != null ? __instance.gunMuzzle.position : __instance.transform.position;

        SoundIndicatorHUD.RegisterSoundEvent(new SoundEvent(position, SoundCategory.Weapon, "Weapons_Icon.png"));
    }
}
