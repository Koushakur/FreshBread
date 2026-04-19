using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using BrilliantSkies.Common.Circuits.Ui.Segments;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(CircuitSegment), "UiEntry")]
    public class Patch_CircuitSegment {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

            MethodInfo _GUILayoutSpaceMethod = AccessTools.Method(
               typeof(GUILayout),
               nameof(GUILayout.Space),
               new[] { typeof(float) });

            foreach (CodeInstruction instruction in instructions) {

                if (instruction.opcode == OpCodes.Ldc_R4
                    && instruction.operand is float f) {

                    if (f == 145f)
                        //Change spacing between top of window and bread circuit panel
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 77f);

                    else if (f == 60f)
                        //Change spacing between bottom of window and bread circuit panel
                        yield return new CodeInstruction(OpCodes.Ldc_R4, 40f);

                    else
                        yield return instruction;


                    //Remove empty space segment being added to the bottom
                } else if (instruction.opcode == OpCodes.Call
                    && instruction.operand is MethodInfo mi
                    && mi == _GUILayoutSpaceMethod) {

                    yield return new CodeInstruction(OpCodes.Pop);


                } else {
                    yield return instruction;
                }
            }
        }
    }

    // Old Prefix patch before I switched to Transpiler, saved for comparison
    //
    //[HarmonyPatch(typeof(CircuitSegment), "UiEntry")]
    //public class Patch_CircuitSegment {
    //    static bool Prefix(
    //            ScreenSegmentDisplayOptions options,
    //            CircuitSegment __instance,
    //            bool ____showManualCheckpointPopup,
    //            bool ____showAutoCheckpointPopup
    //        ) {

    //        __instance.OurDisplay.LastScreenDisplayOptions = options;
    //        float num = (float)options._s.Circuits.Value.Style.fontSize / 14f;
    //        float num2 = 75f * num; //Magic number is room left for buttons
    //        GUILayout.Space(10f);
    //        Rect screenRect = new Rect(5f, num2, options.MasterRect.width - 10f, options.MasterRect.height - num2 - 40f);
    //        GUI.SetNextControlName("CircuitBoardArea");
    //        GUILayout.BeginArea(screenRect, options._s.Circuits.Board.Style);
    //        __instance.OurDisplay.IsOverlayOpen = ____showManualCheckpointPopup || ____showAutoCheckpointPopup;
    //        __instance.OurDisplay.DisplayBoard(0f, 0f, screenRect.width, screenRect.height);
    //        GUILayout.EndArea();

    //        return false;
    //    }
    //}
}
