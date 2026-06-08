using BrilliantSkies.Common.Circuits;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Builders;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FreshBread.Patches {
    public class Patch_Component {

        [HarmonyPatch(typeof(Component), "CreateColorPickers")]
        public class Patch_Component_CreateColorPickers {
            static readonly FieldInfo _selectedComponents = AccessTools.Field(typeof(CircuitBoardDisplay), "_selectedComponents");

            static bool Prefix(Component __instance, IScreenSegment segment, CircuitBoardDisplay CB_display) {
                var selected = (List<Component>)_selectedComponents.GetValue(CB_display);

                VarColor proxy = new VarColor(__instance.OutlineColor.Us);
                proxy.SetChangeAction((Action)delegate {
                    foreach (Component c in selected)
                        c.OutlineColor.Us = proxy.Us;
                    CircuitBoardDisplay.EDITING_COLORS.Now();
                }, callItNow: false);

                new ColorBuilder(segment).RgbAdjust(proxy, withAlpha: true, 0.1f);
                CircuitBoardDisplay.LISTEN_TO_ME(proxy);

                return false;
            }
        }
    }
}
