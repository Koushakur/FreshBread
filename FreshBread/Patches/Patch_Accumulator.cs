using HarmonyLib;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Localisation;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using BrilliantSkies.Common.Circuits.ComponentTypes;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(Accumulator), "PopulateSegment")]
    public class Patch_Accumulator {

        public static bool Prefix(Accumulator __instance,
                                  CircuitComponentEditorSegment segment,
                                  CircuitBoardDisplay displayer
        ) {
            segment.Resize(1, 3);

            __instance.CreateWidthAdjuster(segment);
            __instance.CreateHeightAdjuster(segment);

            var nubBar = Quick.DoubleNubBar(__instance, (Accumulator x) => "ThresholdLimits");
            nubBar.TextBoxLower.ForceRounding = false;
            nubBar.TextBoxUpper.ForceRounding = false;

            if (Loc.Language == Language.EN)
                nubBar.DisplayString = M.m((VarVector2PairClamp I) => string.Format("Threshold between {0} & {1}", I.LowerToString(), I.UpperToString()));

            segment.AddInterpretter(nubBar);

            return false;
        }
    }
}
