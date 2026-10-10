using HarmonyLib;
using BrilliantSkies.Common.Circuits.ComponentTypes;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Localisation;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Numbers;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(OneShot), "PopulateSegment")]
    public class Patch_OneShot {

        public static bool Prefix(OneShot __instance,
                                  CircuitComponentEditorSegment segment,
                                  CircuitBoardDisplay displayer
        ) {
            segment.Resize(1, 10);
            //FileLog.Log("FillMargin before fractional: " + segment.FillMargin.ToString());
            //segment.SetColumnFractionalWidths(0.95f);
            //FileLog.Log("FillMargin after fractional: " + segment.FillMargin.ToString());
            //segment.FillMargin = 35;


            var nubBar = Quick.DoubleNubBar(__instance, (OneShot x) => "ResetTimeLimits");
            nubBar.TextBoxLower.ForceRounding = false;
            nubBar.TextBoxUpper.ForceRounding = false;

            if (Loc.Language == Language.EN)
                nubBar.DisplayString = M.m((VarVector2PairClamp I) => string.Format("Reset between {0} & {1}", I.LowerToString(), I.UpperToString()));

            segment.AddInterpretter(nubBar);

            return false;
        }
    }
}
