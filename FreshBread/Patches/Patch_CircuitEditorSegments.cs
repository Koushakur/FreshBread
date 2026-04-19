using HarmonyLib;
using System;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.Ui.Consoles;

namespace FreshBread.Patches {

    [HarmonyPatch(typeof(CircuitComponentEditorSegment), MethodType.Constructor, new Type[] { typeof(ConsoleUiScreen), typeof(CircuitBoardDisplay) })]
    public class Patch_CircuitComponentEditorSegment {
        static void Postfix(CircuitComponentEditorSegment __instance) {

            __instance.SpaceAbove = 5f;
            __instance.SpaceBelow = 5f;
        }
    }

    [HarmonyPatch(typeof(CircuitComponentStandardEditorSegment), MethodType.Constructor, new Type[] { typeof(ConsoleUiScreen), typeof(CircuitBoardDisplay) })]
    public class Patch_CircuitComponentStandardEditorSegment {
        static void Postfix(CircuitComponentStandardEditorSegment __instance) {

            __instance.SpaceAbove = 5f;
            __instance.SpaceBelow = 5f;
        }
    }

    [HarmonyPatch(typeof(CircuitInputEditorSegment), MethodType.Constructor, new Type[] { typeof(ConsoleUiScreen), typeof(CircuitBoardDisplay) })]
    public class Patch_CircuitInputEditorSegment {
        static void Postfix(CircuitInputEditorSegment __instance) {

            __instance.SpaceAbove = 5f;
            __instance.SpaceBelow = 5f;
        }
    }

    [HarmonyPatch(typeof(CircuitOutputEditorSegment), MethodType.Constructor, new Type[] { typeof(ConsoleUiScreen), typeof(CircuitBoardDisplay) })]
    public class Patch_CircuitEditorSegments {
        static void Postfix(CircuitOutputEditorSegment __instance) {

            __instance.SpaceAbove = 5f;
            __instance.SpaceBelow = 5f;
        }
    }
}
