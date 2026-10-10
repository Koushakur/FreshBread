using HarmonyLib;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Localisation;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Common.Circuits.ComponentTypes.Inputs;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(RandomInput), "PopulateSegment")]
    public class Patch_RandomInput {

        public static bool Prefix(RandomInput __instance,
                                  CircuitComponentEditorSegment segment,
                                  CircuitBoardDisplay displayer
        ) {
            segment.Resize(1, 10);

            __instance.CreateWidthAdjuster(segment);
            __instance.CreateHeightAdjuster(segment);

            var nubBar = Quick.DoubleNubBar(__instance, (RandomInput x) => "RandomLimits");
            nubBar.TextBoxLower.ForceRounding = false;
            nubBar.TextBoxUpper.ForceRounding = false;

            if (Loc.Language == Language.EN)
                nubBar.DisplayString = M.m((VarVector2PairClamp I) => string.Format("Number between {0} & {1}", I.LowerToString(), I.UpperToString()));

            segment.AddInterpretter(nubBar);

            return false;
        }
    }
}
