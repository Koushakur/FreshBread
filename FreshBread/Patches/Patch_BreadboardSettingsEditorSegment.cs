using HarmonyLib;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Layouts.DropDowns;
using BrilliantSkies.Ui.Tips;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(BreadboardSettingsEditorSegment), "UiEntry")]
    public class Patch_BreadboardSettingsEditorSegment {

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

            var codes = new List<CodeInstruction>(instructions);

            //Look for the last call for AddInterpretter, right after it we want to insert our own Interpretters
            var addInterpretterIndex = -1;
            for (int i = 0; i < codes.Count; i++) {
                if (codes[i].opcode == OpCodes.Callvirt
                    && codes[i].operand is MethodInfo mi
                    && mi.Name == "AddInterpretter") {

                    addInterpretterIndex = i;
                }
            }

            int index = 0;
            foreach (var instruction in instructions) {

                //Resize(2,3) -> Resize(2,5)
                if (instruction.opcode == OpCodes.Ldc_I4_3)
                    yield return new CodeInstruction(OpCodes.Ldc_I4_5);

                //Insert our settings Interpretters
                else if (index == addInterpretterIndex) {

                    yield return instruction;
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patch_BreadboardSettingsEditorSegment), nameof(AddFreshSettings)));

                } else
                    yield return instruction;

                index++;
            }
        }

        static void AddFreshSettings(BreadboardSettingsEditorSegment __instance) {

            //Layout selector

            DropDownMenuAlt<string> dropDownMenuAlt = new DropDownMenuAlt<string>((TextAnchor)4);

            try {

                string layoutFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Layouts");

                var layoutFiles = Directory.GetFiles(layoutFolder, "Layout_*.json");
                var dropDownItems = new List<DropDownMenuAltItem<string>>();

                string fileText;
                Layout layoutJson;

                foreach (var file in layoutFiles) {

                    fileText = File.ReadAllText(file);
                    layoutJson = JsonConvert.DeserializeObject<Layout>(fileText)!;
                    dropDownItems.Add(new DropDownMenuAltItem<string> { ObjectForAction = Path.GetFileName(file), Name = $"Layout: {layoutJson!.LayoutName}" });
                }

                dropDownMenuAlt.SetItems(dropDownItems.ToArray());

            } catch { }

            DropDown<BreadboardSettingsEditorSegment, string> newInterpretter = new DropDown<BreadboardSettingsEditorSegment, string>(
                __instance,
                dropDownMenuAlt,
                (BreadboardSettingsEditorSegment seg, string val) => FreshBreadGlobal.LayoutFile == val,
                delegate (BreadboardSettingsEditorSegment seg, string val) {
                    FreshBreadGlobal.LayoutFile = val;
                    FreshBreadGlobal.SaveSettings();
                    FreshBreadGlobal.ReadLayoutFile();
                }
            );

            __instance.AddInterpretter(newInterpretter);


            //Hotkey setter

            TextInput<BreadboardSettingsEditorSegment> newTextInput = TextInput<BreadboardSettingsEditorSegment>.Quick(
                __instance,
                M.m<BreadboardSettingsEditorSegment>((BreadboardSettingsEditorSegment seg) => FreshBreadGlobal.ReopenBreadKey.ToString()),
                "Bread Key:",
                new ToolTip("Key to re-open last opened breadboard with. Expects only A through Z"),
                delegate (BreadboardSettingsEditorSegment seg, string s) {

                    FreshBreadGlobal.SetKeyCodeFromString(s[..1]);
                    FreshBreadGlobal.SaveSettings();
                }
            );
            __instance.AddInterpretter(newTextInput);
        }
    }
}
