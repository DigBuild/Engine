using DigBuild.Engine.Render;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.UI
{
    public sealed class UILabel : IUIElement
    {
        private readonly TextRenderer _textRenderer;
        public string Text { get; set; }

        public UILabel(string text, TextRenderer textRenderer = null!)
        {
            _textRenderer = textRenderer ?? IUIElement.GlobalTextRenderer;
            Text = text;
        }

        public void Draw(RenderContext context, GeometryBufferSet buffers)
        {
            _textRenderer.DrawLine(buffers, Text, 3);
        }
    }
}