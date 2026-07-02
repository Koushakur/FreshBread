using BrilliantSkies.Blocks;
using BrilliantSkies.Blocks.BreadBoards;
using BrilliantSkies.Blocks.MissileBreadboard;
using BrilliantSkies.DataManagement.Vars;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Component = BrilliantSkies.Common.Circuits.Component;
using FreshBread.Layout;

namespace FreshBread {
    public class FreshSettings {
        public string? ReopenBreadKey;
        public string? ChosenLayoutFile;
        public bool ShowIDs;
    }

    public static class FreshBreadGlobal {

        public static KeyCode ReopenBreadKey = (KeyCode)121; //Y default;
        public static bool BreadWasClosed = true;

        public static List<Component> ComponentCopyBuffer = new List<Component>();

        private static string SettingsFilename = "Fresh_Settings.json";
        private static string SettingsPath;

        public static string LayoutFile = "Layout_FreshBread.json";
        public static Layout_Info? ComponentLayout;
        public static VarBool ShowIDs { get; set; } = new VarBool(false);

        private static AiBreadboard? _aiBreadboard;
        private static BreadBoard? _breadBoard;
        private static MissileBreadboardBlock? _missileBreadboard;

        static FreshBreadGlobal() {

            SettingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SettingsFilename);

            LoadSettings();
        }

        public static void SaveSettings() {

            try {
                var saveJson = JsonConvert.SerializeObject(
                    new FreshSettings {
                        ChosenLayoutFile = LayoutFile,
                        ReopenBreadKey = ReopenBreadKey.ToString(),
                        ShowIDs = ShowIDs
                    }, Formatting.Indented);

                File.WriteAllText(SettingsPath, saveJson);

            } catch { }
        }

        public static void LoadSettings() {

            try {
                if (!File.Exists(SettingsPath))
                    SaveSettings();

                var settingsText = File.ReadAllText(SettingsPath);
                var settings = JsonConvert.DeserializeObject<FreshSettings>(settingsText);

                SetKeyCodeFromString(settings!.ReopenBreadKey!);
                LayoutFile = settings.ChosenLayoutFile!;
                ReadLayoutFile();

                ShowIDs.Us = settings.ShowIDs;

            } catch { }
        }

        public static void SetKeyCodeFromString(string newKey) {
            ReopenBreadKey = (KeyCode)Enum.Parse(typeof(KeyCode), newKey[..1].ToUpper());
        }

        public static void ReadLayoutFile() {

            try {

                var layoutPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Layouts", LayoutFile);
                var layoutJson = File.ReadAllText(layoutPath);

                ComponentLayout = JsonConvert.DeserializeObject<Layout_Info>(layoutJson)!;

            } catch { }
        }


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
            if (BreadWasClosed) {

                if ((Block)(object)_breadBoard! != (Block)null! && ((Block)_breadBoard).IsAlive) {
                    ((Block)_breadBoard).Secondary((Transform)null!);
                    BreadWasClosed = false;

                } else if ((Block)(object)_aiBreadboard! != (Block)null! && ((Block)_aiBreadboard).IsAlive) {
                    ((Block)_aiBreadboard).Secondary((Transform)null!);
                    BreadWasClosed = false;

                } else if ((Block)(object)_missileBreadboard! != (Block)null! && ((Block)_missileBreadboard).IsAlive) {
                    ((Block)_missileBreadboard).Secondary((Transform)null!);
                    BreadWasClosed = false;
                }
            }
        }
    }

}