using HarmonyLib;
using System.IO;
using UnityEngine;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Common.Circuits.Ui;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(BreadboardPrefabLoadUi), "BuildInterface")]
    public class Patch_BreadboardPrefabLoadUi {
        static bool Prefix(BreadboardPrefabLoadUi __instance, ConsoleWindow __result) {
            ConsoleWindow consoleWindow = __instance.NewWindow(0, "Load Breadboard Prefab", new CentralRectangle(0.4f, 0.6f));
            consoleWindow.DisplayTextPrompt = false;
            consoleWindow.BackgroundType = BackgroundType.Opaque;

            ScreenSegmentStandardHorizontal screenSegmentStandardHorizontal = consoleWindow.Screen.CreateStandardHorizontalSegment();
            screenSegmentStandardHorizontal.SpaceAbove = 10f;
            screenSegmentStandardHorizontal.AddInterpretter(SubjectiveButton<BreadboardPrefabLoadFocus>.Quick(__instance._focus, "Refresh", new ToolTip("Refresh the list of available prefabs"), delegate (BreadboardPrefabLoadFocus I) {
                I.RefreshFileList();
            }));
            screenSegmentStandardHorizontal.AddInterpretter(SubjectiveDisplay<BreadboardPrefabLoadFocus>.Quick(__instance._focus, M.m((BreadboardPrefabLoadFocus I) => $"Prefabs: {I.PrefabFiles.Count}")));
            ScreenSegmentStandard screenSegmentStandard = consoleWindow.Screen.CreateStandardSegment();
            screenSegmentStandard.SpaceAbove = 20f;

            ScreenSegmentTable prefabListTableSegment = consoleWindow.Screen.CreateTableSegment(2, __instance._focus.PrefabFiles.Count);
            prefabListTableSegment.SetColumnFractionalWidths(new float[] { 0.77f, 0.18f });
            prefabListTableSegment.SqueezeTable = false;

            if (__instance._focus.PrefabFiles.Count > 0) {
                foreach (string prefabFile in __instance._focus.PrefabFiles) {

                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(prefabFile);

                    ScreenSegmentStandardHorizontal screenSegmentStandardHorizontal2 = consoleWindow.Screen.CreateStandardHorizontalSegment();
                    screenSegmentStandardHorizontal2.SpaceAbove = 0f;
                    screenSegmentStandardHorizontal2.AddInterpretter(SubjectiveButton<string>.Quick(prefabFile, fileNameWithoutExtension, new ToolTip("Load prefab: " + fileNameWithoutExtension), delegate (string path) {
                        Vector2 mousePosition = new Vector2(__instance._focus.Display.Width / 2f, __instance._focus.Display.Height / 2f);
                        if (__instance._focus.Display.LoadPrefab(path, mousePosition)) {
                            __instance.DeactivateGui();
                        }
                    }));
                    bool flag = __instance._focus.PendingDeleteFile == prefabFile;

                    var tButton = SubjectiveButton<string>.Quick(prefabFile, flag ? "Confirm Delete?" : "Delete", new ToolTip(flag ? ("Click again to confirm deletion of " + fileNameWithoutExtension) : ("Delete prefab: " + fileNameWithoutExtension)), delegate (string path) {
                        if (__instance._focus.PendingDeleteFile == path) {
                            __instance._focus.DeletePrefab(path);
                            __instance._focus.PendingDeleteFile = null!;
                            __instance.TriggerRebuild();
                        } else {
                            __instance._focus.PendingDeleteFile = path;
                            __instance.TriggerRebuild();
                        }
                    });

                    tButton.PrescribedWidth = new PixelSizing(100, Dimension.Width);

                    screenSegmentStandardHorizontal2.AddInterpretter(tButton);
                }

            } else {
                ScreenSegmentStandardHorizontal screenSegmentStandardHorizontal3 = consoleWindow.Screen.CreateStandardHorizontalSegment();
                screenSegmentStandardHorizontal3.SpaceAbove = 20f;
                screenSegmentStandardHorizontal3.AddInterpretter(SubjectiveDisplay<BreadboardPrefabLoadFocus>.Quick(__instance._focus, M.m((BreadboardPrefabLoadFocus I) => $"No {I.CurrentBoardType} prefabs found. Create some by selecting components and clicking 'Save Selection'.")));
            }
            ScreenSegmentStandardHorizontal screenSegmentStandardHorizontal4 = consoleWindow.Screen.CreateStandardHorizontalSegment();
            screenSegmentStandardHorizontal4.SpaceAbove = 20f;
            screenSegmentStandardHorizontal4.AddInterpretter(SubjectiveButton<BreadboardPrefabLoadUi>.Quick(__instance, "Close", null!, delegate (BreadboardPrefabLoadUi I) {
                I.DeactivateGui();
            }));

            __result = consoleWindow;
            return false;
        }
    }
}
