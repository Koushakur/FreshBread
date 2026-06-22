using HarmonyLib;
using System;
using BrilliantSkies.Modding;


namespace FreshBread {
    public class FrDInterface : GamePlugin_PostLoad {
        public string name => "FreshBread";

        public Version version => new Version(1, 3, 0);

        public void OnLoad() {
            new Harmony("FreshBread").PatchAll();
            ModProblems.AddModProblem("Smell that? Bread, fresh out of the oven (v" + version.ToString() + ")", string.Empty, string.Empty, false);
        }

        public bool AfterAllPluginsLoaded() {
            return true;
        }

        public void OnSave() {

        }
    }
}
