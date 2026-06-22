using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Component = BrilliantSkies.Common.Circuits.Component;


namespace FreshBread.Patches {
    public static class FreshBreadGlobal {

        public static KeyCode ReopenBreadKey = (KeyCode)121; //Y default;
        public static bool BreadWasClosed = true;

        public static string LayoutFile = "Layout_FreshBread.json";
        public static Layout? ComponentLayout;

        private static string SettingsFilename = "Fresh_Settings.json";
        private static string SettingsPath;

        public static List<Component> ComponentCopyBuffer = new List<Component>();

        static FreshBreadGlobal() {

            SettingsPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), SettingsFilename);

            LoadSettings();
        }

        public static void SaveSettings() {

            try {
                var saveJson = JsonConvert.SerializeObject(
                    new FreshSettings {
                        ChosenLayoutFile = LayoutFile,
                        ReopenBreadKey = ReopenBreadKey.ToString()
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

            } catch { }
        }

        public static void SetKeyCodeFromString(string newKey) {
            ReopenBreadKey = (KeyCode)Enum.Parse(typeof(KeyCode), newKey[..1].ToUpper());
        }

        public static void ReadLayoutFile() {

            try {

                var layoutPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Layouts", LayoutFile);
                var layoutJson = File.ReadAllText(layoutPath);

                ComponentLayout = JsonConvert.DeserializeObject<Layout>(layoutJson)!;

            } catch { }
        }
    }

    public class FreshSettings {
        public string? ReopenBreadKey;
        public string? ChosenLayoutFile;
    }

    public class Layout_Component {
        public string? NameOverride;
        public string? ComponentClass;
    }

    public class Layout_Panel {
        public string? PanelName;
        public int NumberOfColumns;
        public List<Layout_Component>? Components;
    }

    public class Layout {
        public string? LayoutName;
        public float SpacingBetweenPanels;
        public float ComponentHeight;
        public List<Layout_Panel>? Panels;
    }
}
