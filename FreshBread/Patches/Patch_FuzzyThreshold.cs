using HarmonyLib;
using BrilliantSkies.Common.Circuits.ComponentTypes;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Localisation;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;
using UnityEngine;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(FuzzyThreshold), "PopulateSegment")]
    public class Patch_FuzzyThreshold {

        public static bool Prefix(FuzzyThreshold __instance,
                                  CircuitComponentEditorSegment segment,
                                  CircuitBoardDisplay displayer
        ) {

            segment.Resize(1, 10);
            __instance.CreateWidthAdjuster(segment);
            __instance.CreateHeightAdjuster(segment);

            var nubBar = Quick.DoubleNubBar(__instance, (FuzzyThreshold x) => "ThresholdLimits");
            nubBar.TextBoxLower.ForceRounding = false;
            nubBar.TextBoxUpper.ForceRounding = false;

            if (Loc.Language == Language.EN)
                nubBar.DisplayString = M.m((VarVector2PairClamp I) => string.Format("Threshold between {0} & {1}", I.LowerToString(), I.UpperToString()));

            segment.AddInterpretter(nubBar);
            segment.AddInterpretter(Quick.Toggle(__instance, (FuzzyThreshold x) => "Above"));

            return false;
        }
    }
}
