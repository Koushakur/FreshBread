using HarmonyLib;
using System;
using BrilliantSkies.Modding;


namespace FreshBread {
    public class FrDInterface : GamePlugin_PostLoad {
        public string name => "FreshBread";

        public Version version => new Version(1, 4, 0);

        public void OnLoad() {
            //Harmony.DEBUG = true;
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
