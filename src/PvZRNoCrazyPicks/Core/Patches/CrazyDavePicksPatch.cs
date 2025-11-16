namespace PvZRNoCrazyPicks.Patches;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

using HarmonyLib;

using Il2CppReloaded.Gameplay;
using Il2CppReloaded.TreeStateActivities;

using MelonLoader;

using PvZRNoCrazyPicks.Extensions;

using Coroutine = System.Collections.IEnumerator;

[HarmonyPatch]
public static class CrazyDavePicksPatch
{
    private const int FirstPlayerIndex = 0;

    private const int CrazyDavePickCount = 3;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameplayActivity), nameof(GameplayActivity.NewGame))]
    private static void GameplayActivity_NewGame_Postfix(GameplayActivity __instance)
    {
        AllowCrazyDaveSeedDeselection(__instance);
    }

    private static void AllowCrazyDaveSeedDeselection(GameplayActivity app)
    {
        if (app.IsFirstTimeAdventureMode() ||
            app.IsCloudyDayMode() ||
            app.IsRipAdventureMode() ||
            app.IsVersusMode() ||
            app.Board.HasConveyorBeltSeedBank() ||
            app.IsWhackAZombieLevel())
        {
            return;
        }

        var seedChooser = app.SeedChooserScreen;

        if (seedChooser.m_seedBankInfos[FirstPlayerIndex].mSeedsInBank != CrazyDavePickCount)
        {
            return;
        }

        IReadOnlyCollection<ChosenSeed> davePicks = [
            .. seedChooser.mChosenSeeds.AsEnumerable()
            .Where(s => s.mCrazyDavePicked)
            .OrderBy(s => s.mSeedIndexInBank)];

        if (davePicks.Count == 0)
        {
            Melon<Core>.Logger.Warning("No Crazy Dave picks were found");
            return;
        }

        MelonCoroutines.Start(
            AllowCrazyDaveSeedDeselection(seedChooser, davePicks));
    }

    private static Coroutine AllowCrazyDaveSeedDeselection(
        SeedChooserScreen seedChooser,
        IReadOnlyCollection<ChosenSeed> davePicks)
    {
        foreach (var seed in davePicks.Reverse())
        {
            seed.mCrazyDavePicked = false;
            seedChooser.ClickedSeedInBank(seed, FirstPlayerIndex);
        }

        seedChooser.LandAllFlyingSeeds();

        yield return null;

        foreach (var seed in davePicks)
        {
            seedChooser.ClickedSeedInChooser(seed, FirstPlayerIndex);
        }
    }
}
