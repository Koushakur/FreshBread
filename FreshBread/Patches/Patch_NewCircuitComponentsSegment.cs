using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using BrilliantSkies.Common.Circuits;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Styles;
using BrilliantSkies.Ui.Elements;
using BrilliantSkies.Ui.Tips;
using System.Reflection.Emit;
using System.Reflection;


namespace FreshBread.Patches {

    public class Patch_NewCircuitComponentsSegment {

        [HarmonyPatch(typeof(NewCircuitComponentsSegment), "UiEntry")]
        public class Patch_UiEntry {

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                MethodInfo _GUILayoutSpaceMethod = AccessTools.Method(
                  typeof(GUILayout),
                  nameof(GUILayout.Space),
                  new[] { typeof(float) });

                var codes = new List<CodeInstruction>(instructions);

                for (var i = 0; i < codes.Count(); i++) {
                    if (codes[i].opcode == OpCodes.Call
                        && codes[i].operand is MethodInfo mi
                        && mi == _GUILayoutSpaceMethod) {

                        codes[i - 1].operand = 15f;
                    }
                }

                return codes.AsEnumerable();
            }
        }

        [HarmonyPatch(typeof(NewCircuitComponentsSegment), "DoPanel")]
        public class Patch_DoPanel {

            static bool Prefix(ScreenSegmentDisplayOptions options, string panelName, IEnumerable<BoardTypes.ComponentType> list, CircuitBoardDisplay ____ourBoardDisplay) {

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical(panelName, ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackgroundWithHeader.Style);

                try {
                    int num = 0;
                    bool flag = false;
                    float num2 = ((options.MasterRect.width > 0f) ? options.MasterRect.width : ((float)Screen.width * 0.2f));
                    float width = num2 * 0.47f;
                    float height = Mathf.Max(30f, (float)Screen.height * 0.03f);
                    foreach (BoardTypes.ComponentType item in list) {
                        if (num % 2 == 0) {
                            if (flag) {
                                GUILayout.EndHorizontal();
                            }

                            GUILayout.BeginHorizontal();
                            flag = true;
                        }
                        string text = item.GetHashCode().ToString();
                        options._s.Circuits.Component.BackgroundTintOnceOff = new Color(0f, 0.7f, 1f, 1f);
                        InputType inputType = options._s.Circuits.Component.LayoutButton(item.GetName(), text, GUILayout.Width(width), GUILayout.Height(height));
                        if (inputType == InputType.LeftClick) {
                            BrilliantSkies.Common.Circuits.Component newComponent = (BrilliantSkies.Common.Circuits.Component)Activator.CreateInstance(item.Type);
                            ____ourBoardDisplay.AddANewComponent(newComponent);
                        }
                        if (GUI.tooltip == text) {
                            TipDisplayer.Instance.SetTip(item.GetToolTip());
                        }
                        num++;
                    }
                    if (flag) {
                        GUILayout.EndHorizontal();
                    }
                } finally {
                    GUILayout.EndVertical();
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                return false;
            }
        }

    }
}
