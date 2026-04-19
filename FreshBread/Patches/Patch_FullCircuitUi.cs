using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using BrilliantSkies.Common.Circuits.Ui;

namespace FreshBread.Patches {

    [HarmonyPatch(typeof(FullCircuitUi), "BuildInterface")]
    public class Patch_FullCircuitUi {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {

            LocalBuilder local12 = instructions
                .Where(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder lb && lb.LocalIndex == 12)
                .Select(i => (LocalBuilder)i.operand)
                .First();

            foreach (CodeInstruction instruction in instructions) {

                if (instruction.opcode == OpCodes.Ldloc_S
                    && instruction.operand is LocalBuilder lb2
                    && (lb2.LocalIndex == 13 || lb2.LocalIndex == 14)) {

                    yield return new CodeInstruction(OpCodes.Ldloc_S, local12);
                    continue;
                }

                yield return instruction;
            }
        }


        //Old Prefix version below for comparison, Transpilers are very nice once you get it working huh?

        //static bool Prefix(
        //        FullCircuitUi __instance,
        //        ref ConsoleWindow __result,
        //        ref CircuitBoardDisplay ____circuitDisplay,
        //        BreadboardCheckpointUi ____checkpointUi,
        //        BreadboardKeymapHelpUi ____keymapHelpUi,
        //        BreadboardPrefabSaveUi ____prefabSaveUi,
        //        BreadboardPrefabLoadUi ____prefabLoadUi
        //    ) {

        //    int fontSize = ConsoleStyles.Instance.FontSize;
        //    int num = 10;
        //    float num2 = (float)fontSize / (float)num;
        //    float num3 = 1f + (num2 - 1f) * 0.4f;
        //    float num4 = 0.1f * num3;
        //    float num5 = (float)Maths.Clamp(0.2f + num4, 0.2f, 0.4f);
        //    float num6 = 0.97f - num5 - 0.02f;
        //    float x = 0.01f + num6 + 0.01f;
        //ConsoleWindow consoleWindow = __instance.NewWindow(0, FullCircuitUi._locFile.Get("Window_CircuitBoard", "Circuit board"), new StandardFractional(0.01f, 0.01f, num6, 0.97f));
        //    consoleWindow.DisplayTextPrompt = false;
        //    ConsoleWindow consoleWindow2 = __instance.NewWindow(1, FullCircuitUi._locFile.Get("Window_Editor", "Editor"), new StandardFractional(x, 0.01f, num5, 0.97f));
        //    consoleWindow2.DisplayTextPrompt = false;


        //    CircuitSegment circuitSegment = new CircuitSegment(consoleWindow.Screen, __instance._focus);
        //    ____circuitDisplay = circuitSegment.OurDisplay;
        //    ScreenSegmentStandardHorizontal screenSegmentStandardHorizontal = consoleWindow.Screen.CreateStandardHorizontalSegment();
        //    screenSegmentStandardHorizontal.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("FitToView", "Fit to view"), null!, delegate (CircuitBoardDisplay I) {
        //        I.FitToView();
        //    }));
        //    screenSegmentStandardHorizontal.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("FitToSelection", "Fit to selection"), null!, delegate (CircuitBoardDisplay I) {
        //        I.ZoomToSelection();
        //    }));
        //    screenSegmentStandardHorizontal.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("ResetView", "Reset view"), null!, delegate (CircuitBoardDisplay I) {
        //        I.ResetView();
        //    }));
        //    screenSegmentStandardHorizontal.AddInterpretter(SubjectiveDisplay<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, M.m((CircuitBoardDisplay t) => "Zoom: " + t.scale.ToString("F2") + "x")));
        //    if (__instance._focus.UsesControllerName) {
        //        screenSegmentStandardHorizontal.AddInterpretter(TextInput<Board>.Quick(__instance._focus, M.m((Board I) => (Var<string>)I.ControllerName), FullCircuitUi._locFile.Get("BoardName", "Board name:"), null!, delegate (Board I, string s) {
        //            I.ControllerName.Us = s;
        //        }));
        //    }

        //    ScreenSegmentStandardHorizontal screenSegmentStandardHorizontal2 = consoleWindow.Screen.CreateStandardHorizontalSegment();
        //    screenSegmentStandardHorizontal2.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("CreateCheckpoint", "Create checkpoint"), null!, delegate (CircuitBoardDisplay I) {
        //        I.UndoRedoSystem.ManualCheckpointSystem.CreateManualCheckpoint();
        //        if (____checkpointUi != null && ____checkpointUi.MenuActive) {
        //            ____checkpointUi.SelectTab(CheckpointType.Manual);
        //        }
        //    }));
        //    screenSegmentStandardHorizontal2.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("ViewCheckpoint", "View checkpoints"), null!, delegate {
        //        if (____checkpointUi == null) {
        //            ____checkpointUi = new BreadboardCheckpointUi(new BreadboardCheckpointFocus(circuitSegment.OurDisplay.UndoRedoSystem, CheckpointType.Auto));
        //            ____checkpointUi.ActivateGui(GuiActivateType.Add);
        //        } else {
        //            ____checkpointUi.DeactivateGui();
        //            ____checkpointUi = null!;
        //            ____checkpointUi = new BreadboardCheckpointUi(new BreadboardCheckpointFocus(circuitSegment.OurDisplay.UndoRedoSystem, CheckpointType.Auto));
        //            ____checkpointUi.ActivateGui(GuiActivateType.Add);
        //            ____checkpointUi.Windows[0].ShouldFocusWindow = true;
        //        }
        //    }));
        //    screenSegmentStandardHorizontal2.AddInterpretter(SubjectiveButton<Board>.Quick(__instance._focus, FullCircuitUi._locFile.Get("KeyboardShortcuts", "Keyboard shortcuts"), null!, delegate {
        //        if (____keymapHelpUi == null) {
        //            ____keymapHelpUi = new BreadboardKeymapHelpUi(__instance._focus);
        //            ____keymapHelpUi.ActivateGui(GuiActivateType.Add);
        //        } else {
        //            ____keymapHelpUi.DeactivateGui();
        //            ____keymapHelpUi = null!;
        //            ____keymapHelpUi = new BreadboardKeymapHelpUi(__instance._focus);
        //            ____keymapHelpUi.ActivateGui(GuiActivateType.Add);
        //            ____keymapHelpUi.Windows[0].ShouldFocusWindow = true;
        //        }
        //    }));
        //    screenSegmentStandardHorizontal2.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("SaveSelection", "Save selection"), null!, delegate {
        //        if (____prefabSaveUi == null) {
        //            ____prefabSaveUi = new BreadboardPrefabSaveUi(circuitSegment.OurDisplay);
        //            ____prefabSaveUi.ActivateGui(GuiActivateType.Add);
        //        } else {
        //            ____prefabSaveUi.DeactivateGui();
        //            ____prefabSaveUi = null!;
        //            ____prefabSaveUi = new BreadboardPrefabSaveUi(circuitSegment.OurDisplay);
        //            ____prefabSaveUi.ActivateGui(GuiActivateType.Add);
        //            ____prefabSaveUi.Windows[0].ShouldFocusWindow = true;
        //        }
        //    }));
        //    screenSegmentStandardHorizontal2.AddInterpretter(SubjectiveButton<CircuitBoardDisplay>.Quick(circuitSegment.OurDisplay, FullCircuitUi._locFile.Get("LoadPrefab", "Load prefab"), null!, delegate {
        //        if (____prefabLoadUi == null) {
        //            ____prefabLoadUi = new BreadboardPrefabLoadUi(circuitSegment.OurDisplay);
        //            ____prefabLoadUi.ActivateGui(GuiActivateType.Add);
        //        } else {
        //            ____prefabLoadUi.DeactivateGui();
        //            ____prefabLoadUi = null!;
        //            ____prefabLoadUi = new BreadboardPrefabLoadUi(circuitSegment.OurDisplay);
        //            ____prefabLoadUi.ActivateGui(GuiActivateType.Add);
        //            ____prefabLoadUi.Windows[0].ShouldFocusWindow = true;
        //        }
        //    }));


        //    consoleWindow.Screen.AddSegment(circuitSegment);

        //    consoleWindow2.Screen.AddSegment(new CircuitComponentEditorSegment(consoleWindow.Screen, circuitSegment.OurDisplay));
        //    consoleWindow2.Screen.AddSegment(new CircuitComponentStandardEditorSegment(consoleWindow.Screen, circuitSegment.OurDisplay));
        //    consoleWindow2.Screen.AddSegment(new CircuitInputEditorSegment(consoleWindow.Screen, circuitSegment.OurDisplay));
        //    consoleWindow2.Screen.AddSegment(new CircuitOutputEditorSegment(consoleWindow.Screen, circuitSegment.OurDisplay));
        //    consoleWindow2.Screen.AddSegment(new NewCircuitComponentsSegment(consoleWindow.Screen, circuitSegment.OurDisplay));
        //    consoleWindow2.Screen.AddSegment(new BreadboardSettingsEditorSegment(consoleWindow.Screen, circuitSegment.OurDisplay));
        //    __result = consoleWindow;

        //    return false;

    }

}