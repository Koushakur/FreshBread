using HarmonyLib;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using BrilliantSkies.Blocks;
using BrilliantSkies.Blocks.BreadBoards;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Ftd.Avatar.Movement;
using BrilliantSkies.Ui.Consoles;

namespace FreshBread.Patches {

    public static class FreshBreadGlobal {

        public static KeyCode ReopenBreadKey = (KeyCode)121; //Y default
        public static bool BreadWasClosed = true;
        static FreshBreadGlobal() {

            string modFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var keycodePath = Path.Combine(modFolder, "keycode.txt");

            if (File.Exists(keycodePath)) {

                int readCode;
                var parseSuccess = int.TryParse(File.ReadLines(keycodePath).First(), out readCode);
                if (parseSuccess) ReopenBreadKey = (KeyCode)readCode;

            }
        }
    }

    public static class LastActiveBread {

        private static AiBreadboard? _aiBreadboard;
        private static BreadBoard? _breadBoard;

        public static void SetAiBoard(AiBreadboard board) {
            _aiBreadboard = board;
            _breadBoard = null;
        }

        public static void SetNonAiBoard(BreadBoard board) {
            _aiBreadboard = null;
            _breadBoard = board;
        }

        public static void ActivateLastBread() {
            if (FreshBreadGlobal.BreadWasClosed) {

                if ((Block)(object)_breadBoard! != (Block)null! && ((Block)_breadBoard).IsAlive) {
                    ((Block)_breadBoard).Secondary((Transform)null!);
                    FreshBreadGlobal.BreadWasClosed = false;

                } else if ((Block)(object)_aiBreadboard! != (Block)null! && ((Block)_aiBreadboard).IsAlive) {
                    ((Block)_aiBreadboard).Secondary((Transform)null!);
                    FreshBreadGlobal.BreadWasClosed = false;

                }
            }
        }
    }

    [HarmonyPatch(typeof(BreadBoard), "Secondary")]
    public class PatchBreadboardSecondaryPostfix {
        private static void Postfix(BreadBoard __instance) {
            LastActiveBread.SetNonAiBoard(__instance);
        }
    }

    [HarmonyPatch(typeof(AiBreadboard), "Secondary")]
    public class PatchAiBreadboardSecondaryPostfix {
        private static void Postfix(AiBreadboard __instance) {
            LastActiveBread.SetAiBoard(__instance);
        }
    }


    [HarmonyPatch(typeof(cCameraControl), "RunLateUpdate")]
    public class PatchcCameraControlRunLateUpdatePrefix {
        static void Postfix() {

            if (Input.GetKeyDown(FreshBreadGlobal.ReopenBreadKey) && !Input.GetKey((KeyCode)306) && !Input.GetKey((KeyCode)305) && !Input.GetKey((KeyCode)308) && !Input.GetKey((KeyCode)307) && !Input.GetKey((KeyCode)304) && !Input.GetKey((KeyCode)303)) {
                LastActiveBread.ActivateLastBread();
            }
        }
    }

    [HarmonyPatch(typeof(ConsoleWindow), "OnDeactivateGui")]
    public class Patch_ConsoleWindow {
        static void Postfix(ConsoleWindow __instance) {
            if (!FreshBreadGlobal.BreadWasClosed
                && __instance.Name.Contains(FullCircuitUi._locFile.Get("Window_CircuitBoard", "Circuit board"))) {

                FreshBreadGlobal.BreadWasClosed = true;
            }
        }
    }
}