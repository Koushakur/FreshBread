using HarmonyLib;
using BrilliantSkies.Common.Circuits.ComponentTypes.Inputs;
using BrilliantSkies.Ui.Consoles.Interpretters;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace FreshBread.Patches {

    [HarmonyPatch(typeof(Comment), "PopulateSegment")]
    public class Patch_Comment {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

            MethodInfo setter = AccessTools.PropertySetter(typeof(InterpretterAbstract), "PrescribedHeight");

            foreach (var instruction in instructions) {

                //When it would set PrescribedHeight instead don't and pop the two variables on the stack it would've used up
                if (instruction.Calls(setter)) {

                    yield return new CodeInstruction(OpCodes.Pop);
                    yield return new CodeInstruction(OpCodes.Pop);


                } else
                    yield return instruction;
            }
        }
    }
}
