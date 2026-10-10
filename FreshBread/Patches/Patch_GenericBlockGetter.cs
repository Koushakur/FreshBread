using HarmonyLib;
using BrilliantSkies.Blocks.BreadBoards.GenericGetter;


namespace FreshBread.Patches {

    public class Patch_GenericBlockGetter {

        [HarmonyPatch(typeof(GenericBlockGetter), nameof(GenericBlockGetter.GetString))]
        public class Patch_GenericBlockGetter_GetString {

            public static bool Prefix(
                GenericBlockGetter __instance,
                IBaseCompute ____computer,
                string ____errorString,
                out string __result
            ) {
                var _locFile = GenericBlockGetter._locFile;

                __result = string.Format("{0} (#{1}{4})\n{2}\n{3}", __instance.BlockType?.Name ?? _locFile.Get("String_PickABlock", "[pick a block]"), __instance.PotentiallyAffectedBlocks.Count, ____computer?.Name ?? _locFile.Get("String_PickAProperty", "[pick a property]"), ____errorString, __instance.BlockFilter.Us != string.Empty ? ": '" + __instance.BlockFilter.Us + "'" : "");

                return false;

            }
        }
    }
}
