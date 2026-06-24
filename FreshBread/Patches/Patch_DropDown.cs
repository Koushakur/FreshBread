using HarmonyLib;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(DropDown<object, object>), nameof(DropDown<object, object>.Draw))]
    public class Patch_DropDown {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

            foreach (var instruction in instructions) {

                if (instruction.opcode == OpCodes.Ldc_R4
                    && instruction.operand is float f
                    && f == 20f) {

                    yield return new CodeInstruction(OpCodes.Ldc_R4, 35f);

                } else
                    yield return instruction;
            }
        }
    }
}
