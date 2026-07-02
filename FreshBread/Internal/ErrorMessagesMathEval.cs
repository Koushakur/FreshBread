using BrilliantSkies.Common.Circuits.ComponentTypes;
using System.Runtime.CompilerServices;

namespace FreshBread.Internal {
    public static class ErrorMessagesMathEval {

        private static readonly ConditionalWeakTable<Evaluator, MessageWrapper> _messages = new ConditionalWeakTable<Evaluator, MessageWrapper>();

        private class MessageWrapper {
            public string Message = "";
        }

        public static void SetError(Evaluator instance, string message) {
            var err = _messages.GetOrCreateValue(instance);

            err.Message = "<color=red>" + message + "</color>";
        }

        public static string GetError(Evaluator instance) {
            if (_messages.TryGetValue(instance, out var err)) {
                return err.Message;
            }
            return "";
        }

        public static bool HasError(Evaluator instance) {
            return _messages.TryGetValue(instance, out var err);
        }

        public static void RemoveError(Evaluator instance) {
            _messages.Remove(instance);
        }
    }

}