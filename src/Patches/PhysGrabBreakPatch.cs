using DeafLink.Core;
using DeafLink.UI;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace DeafLink.Patches;

[HarmonyPatch]
internal static class PhysGrabBreakPatch
{
    [HarmonyTargetMethod]
    private static System.Reflection.MethodBase TargetMethod()
    {
        var method = AccessTools.Method(
            typeof(PhysGrabObjectImpactDetector),
            "BreakRPC",
            new[] {
                typeof(float),
                typeof(Vector3),
                typeof(int),
                typeof(bool),
                typeof(PhotonMessageInfo)
            }
        );

        if (method == null)
            DeafLinkPlugin.Log.LogError("[DeafLink] FAILED to find PhysGrabObjectImpactDetector.BreakRPC via AccessTools!");

        return method!;
    }

    [HarmonyPostfix]
    private static void Postfix(
        PhysGrabObjectImpactDetector __instance,
        float valueLost,
        Vector3 _contactPoint,
        int breakLevel,
        bool _loseValue,
        PhotonMessageInfo _info)
    {
        var position = (_contactPoint != Vector3.zero) ? _contactPoint : __instance.transform.position;

        SoundIndicatorHUD.RegisterSoundEvent(new SoundEvent(position, SoundCategory.ItemBreak, "ItemBreakFall_Icon.png"));
    }
}
