using HarmonyLib;
using BrilliantSkies.Blocks.BreadBoards;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(VariableBaseClass), nameof(VariableBaseClass.KeySourceString))]
    public class Patch_VariableBaseClass {

        public static bool Prefix(VariableBaseClass __instance, ref string __result) {

            if (FreshBreadGlobal.ShowIDs.Us)
                __result = $"{__instance.OurKeySource.Us} <{__instance.GetKey()}></size>";
            else
                __result = $"{__instance.OurKeySource.Us}</size>";

            return false;
        }
    }
}