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
using BrilliantSkies.Core.Logger;


namespace FreshBread.Patches {

    public class Patch_NewCircuitComponentsSegment {

        [HarmonyPatch(typeof(NewCircuitComponentsSegment), "UiEntry")]
        public class Patch_UiEntry {

            static bool Prefix(
                NewCircuitComponentsSegment __instance,
                ScreenSegmentDisplayOptions options,
                CircuitBoardDisplay ____ourBoardDisplay
            ) {
                var _locFile = (BrilliantSkies.Localisation.Runtime.FileManagers.Files.LocFile)NewCircuitComponentsSegment._locFile;

                int count = ____ourBoardDisplay.OurBoard.Packages.Count;
                int num = 9990;
                options._s.Display.DisplayText.Layout(_locFile.Format("Display_ComponentsUsed", "<b>{0} / {1} components used</b>", count, num), "compCount");
                if (GUI.tooltip == "compCount") {
                    TipDisplayer.Instance.SetTip(new ToolTip(_locFile.Get("Tip_NotSave", "You cannot exceed this maximum number of components (or the breadboard will not save!)")));
                }
                GUILayout.Space(15f);
                if (count >= num) {
                    return false;
                }

                try {

                    var allowedByName = ____ourBoardDisplay.OurBoard.AvailableComponentTypes.ToList().ToDictionary(c => c.Type.FullName, c => c);
                    var validComponents = new List<(BoardTypes.ComponentType, string?)>();

                    foreach (var panel in FreshBreadGlobal.ComponentLayout!.Panels!) {
                        validComponents = panel.Components
                            .Where(c => allowedByName.ContainsKey(c.ComponentClass!))
                            .Select(c => (allowedByName[c.ComponentClass!], c.NameOverride))
                            .ToList();

                        if (validComponents.Any()) {
                            DoPanelExtended(options, panel.PanelName!, validComponents!, panel.NumberOfColumns, FreshBreadGlobal.ComponentLayout.ComponentHeight, ____ourBoardDisplay);
                            GUILayout.Space(FreshBreadGlobal.ComponentLayout.SpacingBetweenPanels!);
                        }
                    }

                } catch (Exception e) {
                    AdvLogger.LogError("Exception while trying to create component layout: " + e.Message + "\n" + e.StackTrace, LogOptions._AlertDevInGame);
                }

                return false;
            }
        }

        private static void DoPanelExtended(ScreenSegmentDisplayOptions options, string panelName, IEnumerable<(BoardTypes.ComponentType ComponentType, string NameOverride)> list, int numColumns, float buttonHeight, CircuitBoardDisplay _ourBoardDisplay) {

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            if (panelName == string.Empty)
                GUILayout.BeginVertical(ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackground.Style);
            else
                GUILayout.BeginVertical(panelName, ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackgroundWithHeader.Style);

            try {
                int colCount = 0;
                bool colFlag = false;
                float buttonWidth = options.MasterRect.width * (0.95f / numColumns);

                foreach (var component in list) {
                    if (colCount % numColumns == 0) {
                        if (colFlag) {
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.BeginHorizontal();
                        colFlag = true;
                    }
                    string text = component.GetHashCode().ToString();
                    options._s.Circuits.Component.BackgroundTintOnceOff = new Color(0f, 0.7f, 1f, 1f);

                    InputType inputType = options._s.Circuits.Component.LayoutButton(
                        component.NameOverride == "" ? component.ComponentType.GetName() : component.NameOverride,
                        text,
                        GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)
                    );

                    if (inputType == InputType.LeftClick) {
                        BrilliantSkies.Common.Circuits.Component newComponent = (BrilliantSkies.Common.Circuits.Component)Activator.CreateInstance(component.ComponentType.Type);
                        _ourBoardDisplay.AddANewComponent(newComponent);
                    }
                    if (GUI.tooltip == text) {
                        TipDisplayer.Instance.SetTip(component.ComponentType.GetToolTip());
                    }
                    colCount++;
                }

                if (colFlag) {
                    GUILayout.EndHorizontal();
                }

            } finally {
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

    }
}
