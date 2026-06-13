using DeafLink.Core;
using DeafLink.UI;
using HarmonyLib;
using UnityEngine;

namespace DeafLink.Patches;

internal static class MonsterSoundPatch
{
    private const float MONSTER_SOUND_RADIUS = 3.5f;

    private static readonly System.Collections.Generic.Dictionary<string, string> MonsterIconMap = new()
    {
        { "EnemyAnimal", "Animal_Icon.png" },
        { "EnemyDuck", "ApexPredator_Icon.png" },
        { "EnemyBang", "Banger_Icon.png" },
        { "EnemyTricycle", "Bella_Icon.png" },
        { "EnemyBirthdayBoy", "BirthdayBoy_Icon.png" },
        { "EnemyBowtie", "Bowtie_Icon.png" },
        { "EnemyTumbler", "Chef_Icon.png" },
        { "EnemyBombThrower", "CleanupCrew_Icon.png" },
        { "EnemyBeamer", "Clown_Icon.png" },
        { "EnemyElsa", "Elsa_Icon.png" },
        { "EnemySpinny", "Gambit_Icon.png" },
        { "EnemyGnome", "Gnome_Icon.png" },
        { "EnemyHeadGrabber", "HeadGrab_Icon.png" },
        { "EnemyHeartHugger", "HeartHugger_Icon.png" },
        { "EnemyHidden", "Hidden_Icon.png" },
        { "EnemyHunter", "Huntsman_Icon.png" },
        { "EnemyShadow", "Loom_Icon.png" },
        { "EnemyFloater", "Mentalist_Icon.png" },
        { "EnemyOogly", "Oogly_Icon.png" },
        { "EnemyCeilingEye", "Peeper_Icon.png" },
        { "EnemyRunner", "Reaper_Icon.png" },
        { "EnemyRobe", "Robe_Icon.png" },
        { "EnemyValuableThrower", "Rugrat_Icon.png" },
        { "EnemyThinMan", "ShadowChild_Icon.png" },
        { "EnemySlowMouth", "Spewer_Icon.png" },
        { "EnemyTick", "Tick_Icon.png" },
        { "EnemySlowWalker", "Trudge_Icon.png" },
        { "EnemyUpscream", "Upscream_Icon.png" }
        // Headman uses multiple EnemyHead* classes for some reason. IDK why ask SemiWork

    };

    public static void CheckMonsterSound(Sound soundInstance, Vector3 position)
    {
        if (soundInstance.Type == AudioManager.AudioType.AmbienceBreaker)
        {
            return;
        }

        bool isMonster = false;
        string iconName = "";

        var trace = new System.Diagnostics.StackTrace(2, false);
        
        for (int i = 0; i < 4; i++)
        {
            var frame = trace.GetFrame(i);
            if (frame == null) break;

            var method = frame.GetMethod();
            if (method == null || method.DeclaringType == null) continue;

            string className = method.DeclaringType.Name;
            
            if (className.StartsWith("Enemy"))
            {
                isMonster = true;
                
                if (className.EndsWith("Anim"))
                {
                    className = className.Substring(0, className.Length - 4);
                }

                if (className.StartsWith("EnemyHead") && className != "EnemyHeadGrabber")
                {
                    iconName = "Headman_Icon.png";
                }
                else if (MonsterIconMap.TryGetValue(className, out string mappedIcon))
                {
                    iconName = mappedIcon;
                }
                else
                {
                    iconName = "";
                }

                break;
            }
        }

        if (isMonster)
        {
            SoundIndicatorHUD.RegisterSoundEvent(new SoundEvent(position, SoundCategory.Monster, iconName));
        }
    }
}


[HarmonyPatch(typeof(Sound), nameof(Sound.Play), new[] { 
    typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float) 
})]
internal static class MonsterSoundPosPatch
{
    [HarmonyPrefix]
    private static void Prefix(Sound __instance, Vector3 position)
    {
        MonsterSoundPatch.CheckMonsterSound(__instance, position);
    }
}


[HarmonyPatch(typeof(Sound), nameof(Sound.Play), new[] { 
    typeof(Transform), typeof(float), typeof(float), typeof(float), typeof(float) 
})]
internal static class MonsterSoundTransformPatch
{
    [HarmonyPrefix]
    private static void Prefix(Sound __instance, Transform followTarget)
    {
        if (followTarget != null)
        {
            MonsterSoundPatch.CheckMonsterSound(__instance, followTarget.position);
        }
    }
}


[HarmonyPatch(typeof(Sound), nameof(Sound.Play), new[] { 
    typeof(Transform), typeof(Vector3), typeof(float), typeof(float), typeof(float), typeof(float) 
})]
internal static class MonsterSoundContactPatch
{
    [HarmonyPrefix]
    private static void Prefix(Sound __instance, Transform followTarget, Vector3 contactPoint)
    {
        MonsterSoundPatch.CheckMonsterSound(__instance, contactPoint);
    }
}
