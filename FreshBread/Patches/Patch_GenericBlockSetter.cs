using HarmonyLib;
using BrilliantSkies.Blocks.BreadBoards;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using static BrilliantSkies.Blocks.BreadBoards.GenericBlockSetter;


namespace FreshBread.Patches {

    public class Patch_GenericBlockSetter {

        [HarmonyPatch(typeof(GenericBlockSetter), nameof(GenericBlockSetter.PopulateSegment))]
        public class Patch_GenericBlockSetter_PopulateSegment {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                var codes = instructions.ToList();
                var outCodes = new List<CodeInstruction>();

                for (int i = 0; i < codes.Count; i++) {

                    outCodes.Add(codes[i]);

                    if (codes[i].opcode == OpCodes.Newobj
                        && codes[i].operand is ConstructorInfo ci
                        && ci.DeclaringType != null
                        && ci.DeclaringType.IsGenericType
                        && ci.DeclaringType == typeof(DropDown<GenericBlockSetter, ModuleAttributePair>)) {

                        MethodInfo setter = AccessTools.PropertySetter(ci.DeclaringType, "LimitToMasterRectWidth");

                        outCodes.Add(new CodeInstruction(OpCodes.Dup));
                        outCodes.Add(new CodeInstruction(OpCodes.Ldc_I4_1));
                        outCodes.Add(new CodeInstruction(OpCodes.Callvirt, setter));
                    }
                }

                return outCodes;
            }
        }


        [HarmonyPatch(typeof(GenericBlockSetter), nameof(GenericBlockSetter.GetString))]
        public class Patch_GenericBlockSetter_GetString {
            public static bool Prefix(
                GenericBlockSetter __instance,
                ModuleAttributePair ____moduleAttributePair,
                string ____errorString,
                out string __result
            ) {
                var _locFile = GenericBlockSetter._locFile;

                __result = string.Format("{0} (#{1}{4})\n{2}\n{3}", __instance.BlockType?.Name ?? _locFile.Get("String_PickABlock", "[pick a block]"), __instance.PotentiallyAffectedBlocks.Count, ____moduleAttributePair?.Name ?? _locFile.Get("String_PickAProperty", "[pick a property]"), ____errorString, __instance.BlockFilter.Us != string.Empty ? ": '" + __instance.BlockFilter.Us + "'" : "");

                return false;

            }
        }

    }
}
