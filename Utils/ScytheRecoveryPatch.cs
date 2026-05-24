using HarmonyLib;
using StardewValley;

namespace SmithYourself.Utils
{
    internal static class ScytheRecoveryPatch
    {
        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), "checkIsMissingTool"),
                postfix: new HarmonyMethod(typeof(ScytheRecoveryPatch), nameof(Postfix))
            );
        }

        // The vanilla check only recognises (W)47 as "the scythe".
        // Count the golden (W)53 and iridium (W)66 scythes as equivalent so
        // upgrading via the anvil doesn't trigger the lost-and-found recovery.
        private static void Postfix(ref int missingScythes, Item item)
        {
            if (item?.QualifiedItemId is "(W)53" or "(W)66")
                missingScythes--;
        }
    }
}
