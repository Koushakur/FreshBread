using HarmonyLib;
using BrilliantSkies.Common.Circuits;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Simple;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Consoles.Segments;
using BrilliantSkies.Ui.Examples;
using BrilliantSkies.Ui.Tips;
using BrilliantSkies.Localisation;
using System.Collections.Generic;
using UnityEngine;
using static FreshBread.Patches.Patch_ContextDocumentationUi.Patch_ContextDocumentationUi_BuildInterface;

namespace FreshBread.Patches {

    public class Patch_ContextDocumentationUi {

        [HarmonyPatch(typeof(ContextDocumentationUi), "BuildInterface")]
        public class Patch_ContextDocumentationUi_BuildInterface {

            public class SearchBox { public string Value = ""; }

            public static bool Prefix(
                ContextDocumentationUi __instance,
                ref ConsoleWindow __result
                ) {

                var searchBox = new SearchBox();
                var _locFile = ContextDocumentationUi._locFile;

                ConsoleWindow consoleWindow = __instance.NewWindow(0, _locFile.Get("WindowTitle", "Maths evaluator documentation"), new StandardFractional(0.01f, 0.01f, 0.95f, 0.95f));
                consoleWindow.DisplayTextPrompt = false;

                ScreenSegmentStandard searchSegment = consoleWindow.Screen.CreateStandardSegment();
                searchSegment.AddInterpretter(TextInput<SearchBox>.Quick(
                    searchBox,
                    M.m<SearchBox>((SearchBox b) => b.Value),
                    "Search",
                    new ToolTip("Filter the entries below by typing here"),
                    (SearchBox b, string newValue) => {
                        b.Value = newValue;
                    }
                ));
                searchSegment.SpaceBelow = 15f;
                ScreenSegmentStandard screenSegmentStandard = consoleWindow.Screen.CreateStandardSegment();

                StringDisplay formatsDisplay = screenSegmentStandard.AddInterpretter(StringDisplay.Quick(_locFile.Get("Formats", "<i>Double precision floating point</i> <color=lime>numbers</color> can be typed in like 1 \t 1.0 \t .2 \t 1423.592991009090922\n<i>64 bit long signed</i><color=lime>integers</color> can be entered as: 0L\t10l\t-13l\t14221343535435L. <b>Note the <color=lime>L</color> or <color=lime>l</color></b>.\nNot all operations are supported when mixing these two types so use <color=lime>numbers</color> when you don't specifically need an <color=lime>integer</color>.")));

                screenSegmentStandard.SpaceBelow = 15f;
                screenSegmentStandard.SpaceAbove = 15f;

                var numCols = consoleWindow.GetRectRequired().width <= 1600 ? 3 : 4;

                consoleWindow.Screen.CreateHeader(_locFile.Get("Header_Constants/Inputs", "Constants/Inputs"), new ToolTip(_locFile.Get("Header_Constants/Inputs_Tip", "The constants and properties that are available in this maths evaluator")));
                ScreenSegmentTableSearchable screenSegmentTable = consoleWindow.Screen.AddSegment(new ScreenSegmentTableSearchable(consoleWindow.Screen, numCols + 1, 30));
                screenSegmentTable.SqueezeTable = false;
                DisplayAllNoAlphabet(__instance._focus.GetPropertyTokens(), screenSegmentTable, searchBox);
                screenSegmentTable.ApplySearchRelevanceToCells(searchBox);

                consoleWindow.Screen.CreateHeader(_locFile.Get("Header_Functions", "Functions"), new ToolTip(_locFile.Get("Header_Functions_Tip", "The functions that are available in this maths evaluator")));
                ScreenSegmentTableSearchable screenSegmentTable2 = consoleWindow.Screen.AddSegment(new ScreenSegmentTableSearchable(consoleWindow.Screen, numCols, 30));
                screenSegmentTable2.SqueezeTable = false;
                DisplayAll(__instance._focus.GetFunctionTokens(), screenSegmentTable2, searchBox);
                screenSegmentTable2.ApplySearchRelevanceToCells(searchBox);

                consoleWindow.Screen.CreateHeader(_locFile.Get("Header_Properties", "Properties"), new ToolTip(_locFile.Get("Header_Properties_Tip", "The properties that are available in this maths evaluator")));
                ScreenSegmentTableSearchable screenSegmentTable3 = consoleWindow.Screen.AddSegment(new ScreenSegmentTableSearchable(consoleWindow.Screen, numCols, 30));
                screenSegmentTable3.SqueezeTable = false;
                DisplayAll(__instance._focus.GetObjectPropertyTokens(), screenSegmentTable3, searchBox);
                screenSegmentTable3.ApplySearchRelevanceToCells(searchBox);

                consoleWindow.Screen.CreateHeader(_locFile.Get("Header_Operators", "Operators"), new ToolTip(_locFile.Get("Header_Operators_Tip", "The operators that are available in this maths evaluator")));
                ScreenSegmentTableSearchable screenSegmentTable4 = consoleWindow.Screen.AddSegment(new ScreenSegmentTableSearchable(consoleWindow.Screen, numCols, 30));
                screenSegmentTable4.SqueezeTable = false;
                DisplayAll(__instance._focus.GetOperatorTokens(), screenSegmentTable4, searchBox);
                screenSegmentTable4.ApplySearchRelevanceToCells(searchBox);

                __result = consoleWindow;

                return false;
            }
        }

        private static void DisplayAll(IEnumerable<MethodToken> methodTokens, ScreenSegmentTableSearchable table, SearchBox searchBox) {
            foreach (MethodToken methodToken in methodTokens) {
                DisplayAll(methodToken.GetDescriptionString(), table, searchBox);
            }
        }

        private static void DisplayAll(IEnumerable<string> strings, ScreenSegmentTableSearchable table, SearchBox searchBox) {
            foreach (string @string in strings) {
                AddCell(@string, table, searchBox);
            }
        }

        private static void DisplayAllNoAlphabet(IEnumerable<Property> propertyTokens, ScreenSegmentTableSearchable table, SearchBox searchBox) {

            foreach (Property p in propertyTokens) {

                //Modify "a" description to describe all inputs and ignore the rest, instead of having 26 of the same thing
                if (p.Name[0] >= 'a' && p.Name[0] <= 'z') {
                    if (p.Name == "a") {
                        var descriptionString = p.GetDescriptionString();
                        descriptionString[0] = LocaliseA(descriptionString[0].Replace("<b>a", "<b>a...z"));

                        DisplayAll(descriptionString, table, searchBox);
                    }

                } else {
                    DisplayAll(p.GetDescriptionString(), table, searchBox);
                }
            }
        }

        private static void AddCell(string text, ScreenSegmentTableSearchable table, SearchBox searchBox) {

            StringDisplay stringDisplay = table.AddInterpretter(StringDisplay.Quick(text, ContextDocumentationUi._locFile.Get("Display_DescriptionOf", "Description of a method you have access to")));
            stringDisplay.Justify = TextAnchor.MiddleLeft;

            string plainText = FreshBreadGlobal.StripRichText(text);

            float height;
            float descLength = plainText.Split('\n')[1].Length;

            if (descLength > 110) height = 160f;
            else if (descLength > 60) height = 150f;
            else height = 125f;

            stringDisplay.PrescribedMinHeight = new PixelSizingScaledToFontSize(height, Dimension.Height);
        }

        private static string LocaliseA(string text) {

            switch (Loc.Language) {
                case Language.EN:
                    return text.Replace("first", "first through twenty-sixth");

                case Language.JP:
                    return text.Replace("第一", "第一から第二十六");

                case Language.KR:
                    return text.Replace("첫 번째", "첫 번째부터 스물여섯 번째까지");

                case Language.ZH:
                    return text.Replace("第一个", "第一到第二十六个");

                case Language.RU:
                    return text.Replace("№1", "№1 до №26");
            }

            return text;
        }
    }
}