using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    public sealed class UiTextbox : IUiElement
    {
        private readonly uint _width, _height;
        private readonly TextRenderer _textRenderer;
        private bool _hovered, _hasFocus;

        public string Text { get; set; }

        public UiTextbox(uint width, uint height, string text = "", TextRenderer textRenderer = null!)
        {
            _width = width;
            _height = height;
            _textRenderer = textRenderer ?? IUiElement.GlobalTextRenderer;
            Text = text;
        }

        public void Draw(RenderContext context, GeometryBufferSet buffers)
        {
            _textRenderer.DrawLine(buffers, Text, 3);
        }

        public void OnCursorMoved(IUiElementContext context, int x, int y)
        {
            var wasHovered = _hovered;
            _hovered = x >= 0 && x < _width && y >= 0 && y < _height;
            if (_hovered != wasHovered)
                context.RequestRedraw();
        }

        public void OnMouseEvent(IUiElementContext context, uint button, MouseAction action)
        {
            if (button != 0) return;

            var hadFocus = _hasFocus;
            if (action == MouseAction.Press)
                _hasFocus = _hovered;

            if (_hasFocus == hadFocus)
                return;
            
            context.RequestRedraw();

            if (!_hasFocus)
            {
                context.SetKeyboardEventHandler(null);
                return;
            }

            context.SetKeyboardEventHandler(OnKeyboardEvent, () =>
            {
                _hasFocus = false;
                context.RequestRedraw();
            });
        }

        private void OnKeyboardEvent(uint code, KeyboardAction action)
        {
            if (action is KeyboardAction.Press or KeyboardAction.Repeat && code == 14)
            {
                if (Text.Length > 0)
                    Text = Text[..^1];
                return;
            }

            if (action == KeyboardAction.Character)
                Text += char.ConvertFromUtf32((int) code);
        }
    }
}