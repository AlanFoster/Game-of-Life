using System;
using System.Linq;
using System.Text.RegularExpressions;
using TomShane.Neoforce.Controls;

namespace GameOfLife.BoilerPlate.GUI {
    class ValidationTextField : TextBox {
        private readonly Regex[] _validationRegex;
        private readonly String _defaultValue;

        public ValidationTextField(Manager manager, String defaultValue = "", params Regex[] validationRegex)
            : base(manager) {
            _validationRegex = validationRegex;
            _defaultValue = defaultValue;
        }

        protected override void OnKeyPress(KeyEventArgs e) {
            var previousText = Text;
            var previousCursorPosition = CursorPosition;
            base.OnKeyPress(e);
            if (Text.Length == 0 && _defaultValue != String.Empty) {
                Text = _defaultValue;
            } else if (_validationRegex.Any(i => !i.IsMatch(Text))) {
                Text = previousText;
                CursorPosition = previousCursorPosition;
            }
        }
    }
}
