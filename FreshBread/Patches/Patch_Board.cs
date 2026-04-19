using HarmonyLib;
using BrilliantSkies.Common.Circuits;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Localisation.Widgets;

namespace FreshBread.Patches {

    [HarmonyPatch(typeof(Board))]
    [HarmonyPatch("GridSize", MethodType.Getter)]
    public class Patch_Board {

        [LocalSliderScraped(1001u, "###Breadboards_Board.Attribute_GridSize", "Grid size {0}", "The size of the grid that the modules are snapped to", 10f, 500f, 1f)]
        public static VarFloatClamp GridSize { get; set; } = new VarFloatClamp(25f, 10f, 500f);

        static void Postfix(ref VarFloatClamp __result) {
            __result = GridSize;
        }
    }

}
