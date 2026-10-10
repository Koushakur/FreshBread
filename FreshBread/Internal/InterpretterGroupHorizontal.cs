using BrilliantSkies.Core;
using BrilliantSkies.Ui.Consoles;
using BrilliantSkies.Ui.Consoles.Styles;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace FreshBread.Internal {
    internal class InterpretterGroupHorizontal : IInterpretter {

        private readonly IInterpretter[] _interpretters;
        private Func<bool> _fnConditionalDisplay;

        public InterpretterGroupHorizontal(params IInterpretter[] interpretters) {
            _interpretters = interpretters;
        }

        public ISizing PrescribedWidth { get; set; }
        public ISizing PrescribedMinHeight { get; set; }
        public ISizing PrescribedHeight { get; set; }
        public TextAnchor? Justify { get; set; }
        public bool? WrapText { get; set; }

        public bool Interprettable => false;
        public string ToolTipCode => string.Empty;
        public string PrimaryKey => string.Empty;
        public IToolTip GetToolTip => null!;

        public bool ProcessInput(string input, bool apply, ref string outcome) => false;
        public string GetDisplayString() => string.Empty;
        public List<string> GetKeyStrings() => new List<string>();
        public string GetKeyString(string key) => string.Empty;

        public void Draw(SO_BuiltUi styles) {
            GUILayout.BeginHorizontal();
            foreach (var interpretter in _interpretters) {
                if (interpretter.ShouldIDisplay())
                    interpretter.Draw(styles);
            }
            GUILayout.EndHorizontal();
        }

        public Vector2 EstimateRectRequired() {
            Vector2 total = Vector2.zero;
            foreach (var interpretter in _interpretters) {
                Vector2 est = interpretter.EstimateRectRequired();
                total.x += est.x;
                total.y = Mathf.Max(total.y, est.y);
            }
            return total;
        }

        public IInterpretter SetConditionalDisplayFunction(Func<bool> fn) {
            _fnConditionalDisplay = fn;
            return this;
        }

        public virtual bool ShouldIDisplay() {
            if (_fnConditionalDisplay != null) {
                return _fnConditionalDisplay();
            }

            return true;
        }

    }
}
