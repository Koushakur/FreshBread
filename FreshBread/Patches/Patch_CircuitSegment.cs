using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using UnityEngine;
using BrilliantSkies.Common.Circuits.Ui.Segments;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(CircuitSegment), "UiEntry")]
    public class Patch_CircuitSegment {

        private static float _buttonsReservedHeight = 66f;

        private static float GetCurrentOffset() {
            if (Event.current.type == EventType.Repaint) {
                _buttonsReservedHeight = GUILayoutUtility.GetLastRect().y + 3;
            }
            return _buttonsReservedHeight;
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

            MethodInfo _GUILayoutSpaceMethod = AccessTools.Method(
               typeof(GUILayout),
               nameof(GUILayout.Space),
               new[] { typeof(float) });

            var offsetGetter = AccessTools.Method(typeof(Patch_CircuitSegment), nameof(GetCurrentOffset));

            foreach (CodeInstruction instruction in instructions) {

                //Change spacing between bottom of window and bread circuit panel
                if (instruction.opcode == OpCodes.Ldc_R4
                    && instruction.operand is float f
                    && f == 60f) {

                    yield return new CodeInstruction(OpCodes.Ldc_R4, 40f);


                    //Remove empty space segment being added to the bottom
                } else if (instruction.opcode == OpCodes.Call
                       && instruction.operand is MethodInfo mi
                       && mi == _GUILayoutSpaceMethod) {

                    yield return new CodeInstruction(OpCodes.Pop);


                    //Makes offset to top of window be what's actually needed instead of a hardcoded one
                } else if (instruction.opcode == OpCodes.Stloc_1) {

                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Call, offsetGetter);
                    yield return new CodeInstruction(OpCodes.Stloc_1);


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
