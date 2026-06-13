using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace DeafLink;

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class DeafLinkPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;
    private Harmony _harmony = null!;

    private void Awake()
    {
        Log = Logger;
        Log.LogMessage($"=== {PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} starting ===");

        try
        {
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            _harmony.PatchAll();
            Log.LogMessage("Harmony patches applied successfully.");
        }
        catch (System.Exception e)
        {
            Log.LogError($"FAILED to apply Harmony patches: {e}");
        }
    }
}

internal static class PluginInfo
{
    public const string PLUGIN_GUID    = "deaflink.accessibility";
    public const string PLUGIN_NAME    = "DeafLink";
    public const string PLUGIN_VERSION = "1.0.0";
}
