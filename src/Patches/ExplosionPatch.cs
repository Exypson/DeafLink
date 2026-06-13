using DeafLink.Core;
using DeafLink.UI;
using HarmonyLib;
using Photon.Pun;
using UnityEngine;

namespace DeafLink.Patches;

[HarmonyPatch(typeof(ParticleScriptExplosion), nameof(ParticleScriptExplosion.Spawn))]
internal static class ExplosionPatch
{
    [HarmonyPostfix]
    private static void Postfix(Vector3 position, float size)
    {

        SoundIndicatorHUD.RegisterSoundEvent(new SoundEvent(position, SoundCategory.Explosion, "Explosions_Icon.png"));
    }
}
