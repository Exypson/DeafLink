using DeafLink.Core;
using DeafLink.UI;
using HarmonyLib;
using UnityEngine;

namespace DeafLink.Patches;

[HarmonyPatch(typeof(AudioScare), nameof(AudioScare.PlayImpact))]
internal static class MonsterScarePatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        var player = PlayerAvatar.instance;
        if (player == null || EnemyDirector.instance == null) return;
        
        EnemyParent closest = null;
        float minDist = float.MaxValue;
        
        foreach (var enemy in EnemyDirector.instance.enemiesSpawned)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(player.transform.position, enemy.transform.position);
            if (dist < minDist) 
            { 
                minDist = dist; 
                closest = enemy; 
            }
        }
        
        if (closest != null)
        {

            SoundIndicatorHUD.RegisterSoundEvent(new SoundEvent(closest.transform.position, SoundCategory.Monster));
        }
    }
}
