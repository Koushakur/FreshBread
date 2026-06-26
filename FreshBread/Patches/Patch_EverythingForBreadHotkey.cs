using HarmonyLib;
using UnityEngine;
using BrilliantSkies.Blocks;
using BrilliantSkies.Blocks.BreadBoards;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Ftd.Avatar.Movement;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Blocks.MissileBreadboard;


namespace FreshBread.Patches {

    [HarmonyPatch(typeof(BreadBoard), "Secondary")]
    public class Patch_BreadboardSecondaryPostfix {
        private static void Postfix(BreadBoard __instance) {
            FreshBreadGlobal.SetNonAiBoard(__instance);
        }
    }

    [HarmonyPatch(typeof(AiBreadboard), "Secondary")]
    public class Patch_AiBreadboardSecondaryPostfix {
        private static void Postfix(AiBreadboard __instance) {
            FreshBreadGlobal.SetAiBoard(__instance);
        }
    }

    [HarmonyPatch(typeof(MissileBreadboardBlock), "Secondary")]
    public class Patch_MissileBreadboardSecondaryPostfix {
        private static void Postfix(MissileBreadboardBlock __instance) {
            FreshBreadGlobal.SetMissileBoard(__instance);
        }
    }

    [HarmonyPatch(typeof(cCameraControl), "RunLateUpdate")]
    public class Patchc_CameraControlRunLateUpdatePrefix {
        static void Postfix() {
            if (Input.GetKeyDown(FreshBreadGlobal.ReopenBreadKey) && !Input.GetKey((KeyCode)306) && !Input.GetKey((KeyCode)305) && !Input.GetKey((KeyCode)308) && !Input.GetKey((KeyCode)307) && !Input.GetKey((KeyCode)304) && !Input.GetKey((KeyCode)303)) {
                FreshBreadGlobal.ActivateLastBread();
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