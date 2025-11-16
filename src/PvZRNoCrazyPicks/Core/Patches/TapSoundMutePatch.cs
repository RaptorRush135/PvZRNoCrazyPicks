namespace PvZRNoCrazyPicks.Patches;

using HarmonyLib;

using Il2CppReloaded;
using Il2CppReloaded.Services;

[HarmonyPatch]
public static class TapSoundMutePatch
{
    public static bool MuteTapSound { get; set; }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(AudioService), nameof(AudioService.PlaySample), typeof(Constants.Sound))]
    private static bool AudioService_PlaySample_Prefix(Constants.Sound soundId)
    {
        return soundId != Constants.Sound.SOUND_TAP || !MuteTapSound;
    }
}
