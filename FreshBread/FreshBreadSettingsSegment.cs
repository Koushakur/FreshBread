using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Choices;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Consoles.Styles;
using BrilliantSkies.Ui.Layouts.DropDowns;
using BrilliantSkies.Ui.Tips;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;


namespace FreshBread {
    public class FreshBreadSettingsSegment : ScreenSegmentTable {

        public FreshBreadSettingsSegment(ConsoleUiScreen ourScreen, CircuitBoardDisplay boardDisplay)
            : base(ourScreen, 4, 4) {
            base.SqueezeTable = false;
            base.SpaceAbove = 20f;
            base.SpaceBelow = 5f;
            base.NameWhereApplicable = "FreshBread Settings";
            base.BackgroundStyleWhereApplicable = ConsoleStyles.Instance.Styles.Segments.OptionalSegmentDarkBackgroundWithHeader.Style;
        }

        protected override void UiEntry(ScreenSegmentDisplayOptions options) {


            if (base.Width != 2 || base.Height != 3) {

                Resize(2, 3);

                //Layout selector

                DropDownMenuAlt<string> dropDownMenuAlt = new DropDownMenuAlt<string>((TextAnchor)4);

                try {

                    string layoutFolder = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Layouts");

                    var layoutFiles = Directory.GetFiles(layoutFolder, "Layout_*.json");
                    var dropDownItems = new List<DropDownMenuAltItem<string>>();

                    string fileText;
                    Layout layoutJson;

                    foreach (var file in layoutFiles) {

                        fileText = File.ReadAllText(file);
                        layoutJson = JsonConvert.DeserializeObject<Layout>(fileText)!;
                        dropDownItems.Add(new DropDownMenuAltItem<string> { ObjectForAction = Path.GetFileName(file), Name = $"Layout: {layoutJson!.LayoutName}" });
                    }

                    dropDownMenuAlt.SetItems(dropDownItems.ToArray());

                } catch { }

                AddInterpretter(new DropDown<FreshBreadSettingsSegment, string>(
                    this,
                    dropDownMenuAlt,
                    (FreshBreadSettingsSegment seg, string val) => FreshBreadGlobal.LayoutFile == val,
                    delegate (FreshBreadSettingsSegment seg, string val) {
                        FreshBreadGlobal.LayoutFile = val;
                        FreshBreadGlobal.SaveSettings();
                        FreshBreadGlobal.ReadLayoutFile();
                    }
                ));


                //Variable reader/writer ID display toggle

                AddInterpretter(SubjectiveToggle<Var<bool>>.Quick(
                    FreshBreadGlobal.ShowIDs,
                    "Show IDs on VR/VW",
                    new ToolTip("Whether to show IDs on Variable Reader/Writer"),
                    delegate (Var<bool> I, bool b) {
                        I.Us = b;
                        FreshBreadGlobal.SaveSettings();
                    },
                    I => I.Us
                ));


                //Hotkey setter

                AddInterpretter(TextInput<FreshBreadSettingsSegment>.Quick(
                    this,
                    M.m<FreshBreadSettingsSegment>((FreshBreadSettingsSegment seg) => FreshBreadGlobal.ReopenBreadKey.ToString()),
                    "Bread Key:",
                    new ToolTip("Key to re-open last opened breadboard with. Expects only A through Z"),
                    delegate (FreshBreadSettingsSegment seg, string s) {

                        if (s != FreshBreadGlobal.ReopenBreadKey.ToString()) {
                            FreshBreadGlobal.SetKeyCodeFromString(s[..1]);
                            FreshBreadGlobal.SaveSettings();
                        }
                    }
                ));

            }

            base.UiEntry(options);
        }
    }
}