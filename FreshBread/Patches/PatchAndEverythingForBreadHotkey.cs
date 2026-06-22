using HarmonyLib;
using UnityEngine;
using BrilliantSkies.Blocks;
using BrilliantSkies.Blocks.BreadBoards;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Ftd.Avatar.Movement;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Blocks.MissileBreadboard;


namespace FreshBread.Patches {

    public static class LastActiveBread {

        private static AiBreadboard? _aiBreadboard;
        private static BreadBoard? _breadBoard;
        private static MissileBreadboardBlock? _missileBreadboard;

        public static void SetAiBoard(AiBreadboard board) {
            _aiBreadboard = board;
            _breadBoard = null;
            _missileBreadboard = null;
        }

        public static void SetNonAiBoard(BreadBoard board) {
            _aiBreadboard = null;
            _breadBoard = board;
            _missileBreadboard = null;
        }

        public static void SetMissileBoard(MissileBreadboardBlock board) {
            _aiBreadboard = null;
            _breadBoard = null;
            _missileBreadboard = board;
        }

        public static void ActivateLastBread() {
            if (FreshBreadGlobal.BreadWasClosed) {

                if ((Block)(object)_breadBoard! != (Block)null! && ((Block)_breadBoard).IsAlive) {
                    ((Block)_breadBoard).Secondary((Transform)null!);
                    FreshBreadGlobal.BreadWasClosed = false;

                } else if ((Block)(object)_aiBreadboard! != (Block)null! && ((Block)_aiBreadboard).IsAlive) {
                    ((Block)_aiBreadboard).Secondary((Transform)null!);
                    FreshBreadGlobal.BreadWasClosed = false;

                } else if ((Block)(object)_missileBreadboard! != (Block)null! && ((Block)_missileBreadboard).IsAlive) {
                    ((Block)_missileBreadboard).Secondary((Transform)null!);
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

    [HarmonyPatch(typeof(MissileBreadboardBlock), "Secondary")]
    public class PatchMissileBreadboardSecondaryPostfix {
        private static void Postfix(MissileBreadboardBlock __instance) {
            LastActiveBread.SetMissileBoard(__instance);
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