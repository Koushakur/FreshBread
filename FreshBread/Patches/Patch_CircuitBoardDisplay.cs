using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using BrilliantSkies.Blocks;
using BrilliantSkies.Blocks.BreadBoards;
using BrilliantSkies.Blocks.BreadBoards.Ai;
using BrilliantSkies.Blocks.BreadBoards.GenericGetter;
using BrilliantSkies.Common.Circuits;
using BrilliantSkies.Common.Circuits.ComponentTypes;
using BrilliantSkies.Common.Circuits.ComponentTypes.Inputs;
using BrilliantSkies.Common.Circuits.Ui;
using BrilliantSkies.Common.Circuits.Ui.UndoRedo;
using BrilliantSkies.Core.Timing;
using BrilliantSkies.Core.UiSounds;

using static UnityEngine.Input;
using Component = BrilliantSkies.Common.Circuits.Component;


namespace FreshBread.Patches {
    public class Patch_CircuitBoardDisplay {

        static MethodInfo RemoveScaleAndOffset = AccessTools.Method(typeof(CircuitBoardDisplay), "RemoveScaleAndOffset");

        public static void CopyCopiedIntoBuffer(List<Component> copied) {
            FreshBreadGlobal.ComponentCopyBuffer = copied;
        }

        [HarmonyPatch(typeof(CircuitBoardDisplay), MethodType.Constructor, new[] { typeof(Board) })]
        public class Patch_CircuitBoardDisplay_Constructor {

            public static void Postfix(CircuitBoardDisplay __instance, ref List<Component> ____copiedComponents) {

                if (FreshBreadGlobal.ComponentCopyBuffer.Any()) {
                    var allowedComponents = __instance.OurBoard.AvailableComponentTypes.Select(c => c.Type).ToHashSet();
                    ____copiedComponents = FreshBreadGlobal.ComponentCopyBuffer.Where(c => allowedComponents.Contains(c.GetType())).ToList();
                }
            }
        }

        [HarmonyPatch(typeof(CircuitBoardDisplay), nameof(CircuitBoardDisplay.DisplayBoard))]
        public class Patch_CircuitBoardDisplay_DisplayBoard {

            static Event? inputEvent;

            enum InputState { Base, G, M, P, S, V }
            static InputState currState = InputState.Base;

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {

                var codes = new List<CodeInstruction>(instructions);
                var outCodes = new List<CodeInstruction>();

                var targetField = AccessTools.Field(typeof(Patch_CircuitBoardDisplay_DisplayBoard), nameof(Patch_CircuitBoardDisplay_DisplayBoard.inputEvent));

                FieldInfo copiedComponentsField = AccessTools.Field(typeof(CircuitBoardDisplay), "_copiedComponents");
                MethodInfo copyToBufferFunc = AccessTools.Method(typeof(Patch_CircuitBoardDisplay), nameof(Patch_CircuitBoardDisplay.CopyCopiedIntoBuffer));

                for (int i = 0; i < codes.Count; i++) {

                    if (codes[i].opcode == OpCodes.Ret) {
                        //Weirdly roundabout way to get access to "current2" created inside the original method, doesn't seem to exists any easier way
                        outCodes.Add(new CodeInstruction(OpCodes.Ldloc_S, 5));
                        outCodes.Add(new CodeInstruction(OpCodes.Stsfld, targetField));
                    }

                    outCodes.Add(codes[i]);

                    if (i - 3 < 0)
                        continue;

                    if (codes[i].opcode == OpCodes.Callvirt
                        && codes[i].operand is MethodInfo mi
                        && (mi.Name == "Add" || mi.Name == "AddRange")
                    ) {
                        var maybeCopiedComponents = codes[i - 3];

                        if (maybeCopiedComponents.opcode == OpCodes.Ldfld
                            && maybeCopiedComponents.operand is FieldInfo fi
                            && fi == copiedComponentsField
                        ) {
                            //Whenever _copiedComponents are set send it to the function to be copied into a buffer
                            outCodes.Add(new CodeInstruction(OpCodes.Ldarg_0));
                            outCodes.Add(new CodeInstruction(OpCodes.Ldfld, copiedComponentsField));
                            outCodes.Add(new CodeInstruction(OpCodes.Call, copyToBufferFunc));
                        }
                    }
                }

                return outCodes;
            }

            static void Postfix(
                CircuitBoardDisplay __instance,
                List<Component> ____selectedComponents,
                ulong ____lastComponentPlacementFrame,
                BoardUndoRedoSystem ____undoRedoSystem
            ) {

                if (inputEvent!.alt && inputEvent.type == EventType.KeyDown) {

                    ulong frameCounterUpdate = GameTimer.Instance.FrameCounterUpdate;

                    if (currState == InputState.Base) {

                        //Altitude
                        if (GetKeyDown(KeyCode.A)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<AltitudeInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Behaviour select   
                        } else if (GetKeyDown(KeyCode.B)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<AiBehaviourSelect>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Constant
                        } else if (GetKeyDown(KeyCode.C)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<ConstantInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Differencer
                        } else if (GetKeyDown(KeyCode.D)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<Differencer>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Orientation
                        } else if (GetKeyDown(KeyCode.O)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<OrientationInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Rotation
                        } else if (GetKeyDown(KeyCode.R)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<RotationInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Time
                        } else if (GetKeyDown(KeyCode.T)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<TimeInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                            //Self-info group
                        } else if (GetKeyDown(KeyCode.Q)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                var offset = 50;
                                CreateAndPlaceNewComponent<PositionInput>(inputEvent.mousePosition + new Vector2(offset, -offset), __instance, ____selectedComponents, ____undoRedoSystem);
                                CreateAndPlaceNewComponent<AltitudeInput>(inputEvent.mousePosition + new Vector2(-offset, offset), __instance, ____selectedComponents, ____undoRedoSystem);
                                CreateAndPlaceNewComponent<OrientationInput>(inputEvent.mousePosition + new Vector2(offset, offset), __instance, ____selectedComponents, ____undoRedoSystem);
                                CreateAndPlaceNewComponent<RotationInput>(inputEvent.mousePosition + new Vector2(-offset, -offset), __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            return;

                        } else if (GetKeyDown(KeyCode.G)) {
                            currState = InputState.G;
                            return;

                        } else if (GetKeyDown(KeyCode.M)) {
                            currState = InputState.M;
                            return;

                        } else if (GetKeyDown(KeyCode.P)) {
                            currState = InputState.P;
                            return;

                        } else if (GetKeyDown(KeyCode.S)) {
                            currState = InputState.S;
                            return;

                        } else if (GetKeyDown(KeyCode.V)) {
                            currState = InputState.V;
                            return;
                        }

                        //G pressed, disambiguate
                    } else if (currState == InputState.G) {

                        //Generic Getter
                        if (GetKeyDown(KeyCode.G)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<GenericBlockGetter>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Generic Setter
                        } else if (GetKeyDown(KeyCode.S)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<GenericBlockSetter>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;
                        }

                        //M pressed, disambiguate
                    } else if (currState == InputState.M) {

                        //Min/Max
                        if (GetKeyDown(KeyCode.M)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<MaxMin>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Multiply
                        } else if (GetKeyDown(KeyCode.U)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<Multiply>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Math Evaluator
                        } else if (GetKeyDown(KeyCode.E)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<Evaluator>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Manoeuvre Select
                        } else if (GetKeyDown(KeyCode.S)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<AiManoeuvreSelect>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;
                        }


                        //P pressed, disambiguate
                    } else if (currState == InputState.P) {

                        //Propulsion input
                        if (GetKeyDown(KeyCode.I)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<ControlInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Propulsion Output
                        } else if (GetKeyDown(KeyCode.O)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<PropulsionModule>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Primary Target
                        } else if (GetKeyDown(KeyCode.T)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<AiTargetInfo>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Position
                        } else if (GetKeyDown(KeyCode.P)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<PositionInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //PID
                        } else if (GetKeyDown(KeyCode.D)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<Pid>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;
                        }

                        //S pressed, disambiguate
                    } else if (currState == InputState.S) {

                        //Speed
                        if (GetKeyDown(KeyCode.S)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<SpeedInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Sum
                        } else if (GetKeyDown(KeyCode.U)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<Adder>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Switch
                        } else if (GetKeyDown(KeyCode.W)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<BrilliantSkies.Common.Circuits.ComponentTypes.Switch>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Stability
                        } else if (GetKeyDown(KeyCode.T)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<StabilityInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Sorter
                        } else if (GetKeyDown(KeyCode.O)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<Sorter>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;
                        }

                        //V pressed, disambiguate
                    } else if (currState == InputState.V) {

                        //Variable Reader
                        if (GetKeyDown(KeyCode.R)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<VariableReaderInput>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;

                            //Variable Writer
                        } else if (GetKeyDown(KeyCode.W)) {
                            if (____lastComponentPlacementFrame != frameCounterUpdate) {
                                ____lastComponentPlacementFrame = frameCounterUpdate;
                                CreateAndPlaceNewComponent<VariableWriter>(inputEvent.mousePosition, __instance, ____selectedComponents, ____undoRedoSystem);
                            }
                            inputEvent.Use();
                            currState = InputState.Base;
                            return;
                        }
                    }

                }

                if (inputEvent.type == EventType.KeyUp && (inputEvent.keyCode == KeyCode.LeftAlt || inputEvent.keyCode == KeyCode.RightAlt)) {
                    currState = InputState.Base;
                }
            }

            static void CreateAndPlaceNewComponent<T>(
                    Vector2 mousePosition,
                    CircuitBoardDisplay instance,
                    List<Component> selectedComponents,
                    BoardUndoRedoSystem undoRedoSystem
                ) where T : Component, new() {

                try {
                    T newComponent = new T();
                    Vector2 vector = (Vector2)RemoveScaleAndOffset.Invoke(instance, new object[] { mousePosition });

                    if (instance.OurBoard.GridEnabled.Us) {
                        vector /= instance.OurBoard.GridSize.Us;
                        vector.x = Mathf.Round(vector.x);
                        vector.y = Mathf.Round(vector.y);
                        vector *= instance.OurBoard.GridSize.Us;
                    }

                    newComponent.X.AlterDefault(vector.x);
                    newComponent.Y.AlterDefault(vector.y);
                    AddComponentCommand command = new AddComponentCommand(instance.OurBoard, newComponent);

                    undoRedoSystem.ExecuteCommand(command);
                    selectedComponents.Clear();
                    selectedComponents.Add(newComponent);
                    instance.SelectedComponent = newComponent;
                    GUISoundManager.GetSingleton().PlayBeep();

                } catch {
                    GUISoundManager.GetSingleton().PlayFailure();
                }

            }
        }
    }
}
