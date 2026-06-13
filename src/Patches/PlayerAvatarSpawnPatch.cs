using DeafLink.UI;
using HarmonyLib;
using UnityEngine;

namespace DeafLink.Patches;

[HarmonyPatch(typeof(PlayerAvatar), nameof(PlayerAvatar.Start))]
internal static class PlayerAvatarSpawnPatch
{
    [HarmonyPostfix]
    private static void Postfix(PlayerAvatar __instance)
    {
        if (SemiFunc.IsMultiplayer() && !__instance.photonView.IsMine) 
        {
            return;
        }

        if (__instance.gameObject.GetComponent<SoundIndicatorHUD>() == null)
        {
            __instance.gameObject.AddComponent<SoundIndicatorHUD>();

        }
    }
}
