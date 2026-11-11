using BrilliantSkies.Breadboards.Circuits.Ui;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Tips;
using HarmonyLib;
using UnityEngine;


namespace FreshBread.Patches {
    public class Patch_BreadboardKeymapHelpUi {

        [HarmonyPatch(typeof(BreadboardKeymapHelpUi), "BuildInterface")]
        public class Patch_BreadboardKeymapHelpUi_BuildInterface {

            public static bool Prefix(
                BreadboardKeymapHelpUi __instance,
                ref ConsoleWindow __result
            ) {
                var _locFile = BreadboardKeymapHelpUi._locFile;

                ConsoleWindow consoleWindow = __instance.NewWindow(0, _locFile.Get("WindowTitle", "Breadboard Keyboard Shortcuts"), new StandardFractional(0.01f, 0.01f, 0.6f, 0.8f));
                consoleWindow.DisplayTextPrompt = false;

                consoleWindow.SetMultipleTabs(new SuperScreen[] {
                    new OldKeysScreen(consoleWindow),
                    new FreshKeysScreen(consoleWindow)
                });

                __result = consoleWindow;
                return false;
            }

            public class FreshKeysScreen : SuperScreen {

                public FreshKeysScreen(ConsoleWindow window) : base(window) {
                    Name = new Content("Fread Bread Shortcuts");
                }

                public override void Build() {
                    ScreenSegmentStandard screenSegmentStandard = CreateStandardSegment();
                    screenSegmentStandard.AddInterpretter(StringDisplay.Quick("<b>Additional keyboard shortcuts added by Fresh Bread</b>\n\nFor the ones with multiple letters press them in order without releasing alt in-between"));
                    screenSegmentStandard.SpaceAbove = 10f;
                    screenSegmentStandard.SpaceBelow = 10f;

                    CreateHeader("Places component at mouse position");

                    ScreenSegmentTable screenSegmentTable = CreateTableSegment(2, 30);
                    screenSegmentTable.SqueezeTable = false;

                    AddKeymapEntry(screenSegmentTable, "Alt+A", "Altitude");

                    AddKeymapEntry(screenSegmentTable, "Alt+C", "Constant");

                    AddKeymapEntry(screenSegmentTable, "Alt+D", "Differencer");

                    AddKeymapEntry(screenSegmentTable, "Alt+GG", "Generic Block Getter");
                    AddKeymapEntry(screenSegmentTable, "Alt+GS", "Generic Block Setter");

                    AddKeymapEntry(screenSegmentTable, "Alt+MM", "Min/Max");
                    AddKeymapEntry(screenSegmentTable, "Alt+MU", "Multiply");

                    AddKeymapEntry(screenSegmentTable, "Alt+O", "Orientation");

                    AddKeymapEntry(screenSegmentTable, "Alt+PP", "Position");
                    AddKeymapEntry(screenSegmentTable, "Alt+PI", "Propulsion Input");
                    AddKeymapEntry(screenSegmentTable, "Alt+PO", "Propulsion Output");
                    AddKeymapEntry(screenSegmentTable, "Alt+PT", "Primary Target");
                    AddKeymapEntry(screenSegmentTable, "Alt+PD", "PiD");

                    AddKeymapEntry(screenSegmentTable, "Alt+R", "Rotation");

                    AddKeymapEntry(screenSegmentTable, "Alt+SS", "Speed");
                    AddKeymapEntry(screenSegmentTable, "Alt+SU", "Sum");
                    AddKeymapEntry(screenSegmentTable, "Alt+SW", "Switch");
                    AddKeymapEntry(screenSegmentTable, "Alt+ST", "Stability");
                    AddKeymapEntry(screenSegmentTable, "Alt+SO", "Sorter");

                    AddKeymapEntry(screenSegmentTable, "Alt+T", "Time");

                    AddKeymapEntry(screenSegmentTable, "Alt+VR", "Variable Reader");
                    AddKeymapEntry(screenSegmentTable, "Alt+VW", "Variable Writer");

                    AddKeymapEntry(screenSegmentTable, "Alt+Q", "Places a group of self-information components (Position, Altitude, Orientation and Rotation)");
                }
            }

            public class OldKeysScreen : SuperScreen {

                public OldKeysScreen(ConsoleWindow window) : base(window) {
                    Name = new Content("Basegame Shortcuts");
                }

                public override void Build() {
                    var _locFile = BreadboardKeymapHelpUi._locFile;

                    ScreenSegmentStandard screenSegmentStandard = CreateStandardSegment();
                    screenSegmentStandard.AddInterpretter(StringDisplay.Quick(_locFile.Get("Display_Intro", "<b>Breadboard Keyboard Shortcuts</b>\n\nUse these keyboard shortcuts to work more efficiently with breadboards.")));
                    screenSegmentStandard.SpaceAbove = 10f;
                    screenSegmentStandard.SpaceBelow = 20f;

                    CreateHeader(_locFile.Get("Header_Selection", "Selection"), new ToolTip(_locFile.Get("Header_Selection_Tip", "Commands for selecting components")));

                    ScreenSegmentTable screenSegmentTable = CreateTableSegment(2, 30);
                    screenSegmentTable.SqueezeTable = false;
                    AddKeymapEntry(screenSegmentTable, "Ctrl+H", _locFile.Get("Key_ToggleHelp", "Toggle help labels on the canvas"));
                    AddKeymapEntry(screenSegmentTable, "Ctrl+A", _locFile.Get("Key_SelectAll", "Select all components on the board"));
                    AddKeymapEntry(screenSegmentTable, "Ctrl+S", _locFile.Get("Key_SelectInView", "Select all components visible in current view"));
                    AddKeymapEntry(screenSegmentTable, "Left Click", _locFile.Get("Key_SelectSingle", "Select single component"));
                    AddKeymapEntry(screenSegmentTable, "Shift+Left Click", _locFile.Get("Key_SelectMultiple", "Add/remove component to/from selection group"));
                    AddKeymapEntry(screenSegmentTable, "Ctrl + Tab", _locFile.Get("Key_CycleAnchors", "Cycle through our available anchor point(s)"));
                    CreateHeader(_locFile.Get("Header_Editing", "Editing"), new ToolTip(_locFile.Get("Header_Editing_Tip", "Commands for editing components and connections")));

                    ScreenSegmentTable screenSegmentTable2 = CreateTableSegment(2, 30);
                    screenSegmentTable2.SqueezeTable = false;
                    AddKeymapEntry(screenSegmentTable2, "Ctrl+C", _locFile.Get("Key_Copy", "Copy selected component(s)"));
                    AddKeymapEntry(screenSegmentTable2, "Ctrl+V", _locFile.Get("Key_Paste", "Paste copied component(s) at mouse position"));
                    AddKeymapEntry(screenSegmentTable2, "Ctrl+Shift+Left click", _locFile.Get("Key_DuplicateComponent", "Duplicate selected component(s)"));
                    AddKeymapEntry(screenSegmentTable2, "Delete", _locFile.Get("Key_Delete", "Delete selected component, connection, or pin"));
                    AddKeymapEntry(screenSegmentTable2, "Shift+Right click", _locFile.Get("Key_Delete", "Delete selected component, connection, or pin"));
                    AddKeymapEntry(screenSegmentTable2, "Ctrl+Z", _locFile.Get("Key_Undo", "Undo last action"));
                    AddKeymapEntry(screenSegmentTable2, "Ctrl+Y", _locFile.Get("Key_Redo", "Redo last undone action"));
                    CreateHeader(_locFile.Get("Header_QuickPlace", "Quick Component Placement"), new ToolTip(_locFile.Get("Header_QuickPlace_Tip", "Shortcuts to quickly place common components at mouse position")));

                    ScreenSegmentTable screenSegmentTable3 = CreateTableSegment(2, 30);
                    screenSegmentTable3.SqueezeTable = false;
                    AddKeymapEntry(screenSegmentTable3, "Ctrl+P", _locFile.Get("Key_PlacePrinter", "Place Printer component at mouse position"));
                    AddKeymapEntry(screenSegmentTable3, "Ctrl+B", _locFile.Get("Key_PlaceAnchor", "Place Anchor component at mouse position"));
                    AddKeymapEntry(screenSegmentTable3, "Ctrl+N", _locFile.Get("Key_PlaceComment", "Place Comment component at mouse position"));
                    AddKeymapEntry(screenSegmentTable3, "Ctrl+M", _locFile.Get("Key_PlaceMathEval", "Place Math Evaluator component at mouse position"));
                    CreateHeader(_locFile.Get("Header_View", "View & Navigation"), new ToolTip(_locFile.Get("Header_View_Tip", "Commands for navigating and viewing the board")));

                    ScreenSegmentTable screenSegmentTable4 = CreateTableSegment(2, 30);
                    screenSegmentTable4.SqueezeTable = false;
                    AddKeymapEntry(screenSegmentTable4, "Mouse Wheel", _locFile.Get("Key_Zoom", "Zoom in/out"));
                    AddKeymapEntry(screenSegmentTable4, "Left Mouse Down + Drag", _locFile.Get("Key_PanCanvas", "Pan the view"));
                    AddKeymapEntry(screenSegmentTable4, "Right Mouse Down + Drag", _locFile.Get("Key_MoveComponent", "Move component (snaps to grid if enabled)"));

                    ScreenSegmentStandard screenSegmentStandard2 = CreateStandardSegment();
                    screenSegmentStandard2.AddInterpretter(StringDisplay.Quick(_locFile.Get("Display_Tips", "<b>Tips:</b>\n• Use checkpoints to save your work and experiment safely\n• Hold Shift while clicking to select multiple components\n• Delete key works on components, connections, and pins depending on what's selected\n• Grid snapping can be enabled/disabled in board settings\n• Enable canvas help labels with ctrl+h or at the bottom right toggle\n• Name an anchor 'start' to automatically center on it when opening the breadboard")));
                    screenSegmentStandard2.SpaceAbove = 20f;
                }
            }

            public static void AddKeymapEntry(IScreenSegment table, string keyCombo, string description) {

                var tempStringDisplay = StringDisplay.Quick("<color=lime><b>" + keyCombo + "</b></color>");
                tempStringDisplay.Justify = TextAnchor.MiddleRight;
                table.AddInterpretter(tempStringDisplay);

                tempStringDisplay = StringDisplay.Quick(description);
                tempStringDisplay.Justify = TextAnchor.MiddleLeft;
                table.AddInterpretter(tempStringDisplay);
            }
        }
    }
}
