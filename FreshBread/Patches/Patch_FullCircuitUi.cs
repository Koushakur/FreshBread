using HarmonyLib;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using FreshBread.Internal;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(FullCircuitUi), "BuildInterface")]
    public class Patch_FullCircuitUi {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, MethodBase original) {

            LocalBuilder local12 = instructions
                .Where(i => i.opcode == OpCodes.Stloc_S && i.operand is LocalBuilder lb && lb.LocalIndex == 12)
                .Select(i => (LocalBuilder)i.operand)
                .First();

            ConstructorInfo freshBreadConstructor = AccessTools.Constructor(
                    typeof(FreshBreadSettingsSegment),
                    new[] { typeof(ConsoleUiScreen), typeof(CircuitBoardDisplay) });

            MethodInfo addSegmentMethod = AccessTools.Method(typeof(ConsoleUiScreen), "AddSegment").MakeGenericMethod(typeof(FreshBreadSettingsSegment));

            foreach (CodeInstruction instruction in instructions) {

                // Join buttons into same segment by inserting all into the horizontal2 variable one instead of their own
                if (instruction.opcode == OpCodes.Ldloc_S
                    && instruction.operand is LocalBuilder lb2
                    && (lb2.LocalIndex == 13 || lb2.LocalIndex == 14)) {

                    yield return new CodeInstruction(OpCodes.Ldloc_S, local12);


                    //Add own Settings segment after 'NewCircuitComponentsSegment',
                    //replicates the same IL instruction stack as the existing segments
                } else if (instruction.opcode == OpCodes.Callvirt
                           && instruction.operand is MethodInfo mi
                           && mi.IsGenericMethod
                           && mi.Name == "AddSegment"
                           && mi.GetGenericArguments()[0] == typeof(NewCircuitComponentsSegment)) {

                    yield return instruction;

                    yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)10);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ConsoleWindow), "Screen"));

                    yield return new CodeInstruction(OpCodes.Ldloc_S, (byte)9);
                    yield return new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ConsoleWindow), "Screen"));

                    yield return new CodeInstruction(OpCodes.Ldloc_0);
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(FullCircuitUi).GetNestedType("<>c__DisplayClass14_0", BindingFlags.NonPublic), "circuitSegment"));
                    yield return new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(CircuitSegment), "OurDisplay"));

                    yield return new CodeInstruction(OpCodes.Newobj, freshBreadConstructor);
                    yield return new CodeInstruction(OpCodes.Ldc_I4_0);
                    yield return new CodeInstruction(OpCodes.Callvirt, addSegmentMethod);
                    yield return new CodeInstruction(OpCodes.Pop);


                } else
                    yield return instruction;
            }
        }

    }
}