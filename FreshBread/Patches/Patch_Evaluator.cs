using HarmonyLib;
using FreshBread.Internal;
using BrilliantSkies.Common.Circuits;
using BrilliantSkies.Common.Circuits.ComponentTypes;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.Segments;
using BrilliantSkies.DataManagement.Vars;
using BrilliantSkies.Ui.Consoles.Getters;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Buttons;
using BrilliantSkies.Ui.Consoles.Interpretters.Subjective.Texts;
using BrilliantSkies.Ui.Tips;
using System;


namespace FreshBread.Patches {

    public class Patch_Evaluator {

        [HarmonyPatch(typeof(Evaluator), "ExpressionChanged")]
        public class Patch_Evaluator_ExpressionChanged {

            public static bool Prefix(
                Evaluator __instance,
                string newExpression,
                ParserAlt ____parser,
                TokenizerAlt ____tokenizer
                ) {

                try {
                    ____tokenizer.SetString("setOutput(" + newExpression.Replace("\n", "").Replace("\r", "") + ")");
                    ____parser.ParseToReversePolishNotation();

                    ErrorMessagesMathEval.RemoveError(__instance);

                } catch (Exception e) {

                    if (e.Message[0..22] == "Unrecognised character") {
                        ErrorMessagesMathEval.SetError(__instance, "Error probably near '" + e.Message[23..24] + "'");
                    } else {
                        ErrorMessagesMathEval.SetError(__instance, e.Message);
                    }

                    ____parser.Clear();
                    ____tokenizer.SetString("setOutput(0)");
                    ____parser.ParseToReversePolishNotation();
                }

                return false;
            }
        }


        [HarmonyPatch(typeof(Evaluator), "GetString")]
        public class Patch_Evaluator_GetString {

            public static void Postfix(
                Evaluator __instance,
                ref string __result
                ) {

                if (ErrorMessagesMathEval.HasError(__instance))
                    __result = "<color=red>" + __instance.Expression.Us + "</color>";
                else
                    __result = __instance.Expression.Us;
            }
        }


        [HarmonyPatch(typeof(Evaluator), "PopulateSegment")]
        public class Patch_Evaluator_PopulateSegment {

            public static bool Prefix(
                Evaluator __instance,
                CircuitComponentEditorSegment segment, CircuitBoardDisplay displayer
                ) {

                var _locFile = Evaluator._locFile;

                segment.Resize(1, 10);
                TextInput<Evaluator> textInput = segment.AddInterpretter(TextInput<Evaluator>.Quick(__instance, M.m((Evaluator I) => (Var<string>)I.Expression), _locFile.Get("Input_Expression", "Expression:"), new ToolTip(_locFile.Get("Input_Expression_Tip", "Write the expression you want to evaluate here")), delegate (Evaluator I, string s) {
                    I.Expression.Us = s;
                }));
                textInput.UseTextArea = true;
                textInput.AllowRichTextInEditor = false;

                segment.AddInterpretter(SubjectiveButton<Evaluator>.Quick(__instance, _locFile.Get("Button_ShowDocumentation", "Show documentation"), new ToolTip(_locFile.Get("Button_ShowDocumentation_Tip", "Shows all the functions and operators vailable in the maths evaluator")), delegate (Evaluator I) {
                    I.ShowDocumentation();
                }));

                segment.AddInterpretter(SubjectiveDisplay<Evaluator>.Quick(__instance,
                    M.m<Evaluator>((Evaluator e) => ErrorMessagesMathEval.GetError(e)),
                    "Shows vaguely if something is broken in the expression\nNot getting an error is not a guarantee that it will work"));

                return false;
            }
        }

    }
}
