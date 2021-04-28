using DigBuild.Engine.Render;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    public sealed class UiLabel : IUiElement
    {
        private readonly TextRenderer _textRenderer;
        public string Text { get; set; }

        public UiLabel(string text, TextRenderer textRenderer = null!)
        {
            _textRenderer = textRenderer ?? IUiElement.GlobalTextRenderer;
            Text = text;
        }

        public void Draw(RenderContext context, GeometryBufferSet buffers, float partialTick)
        {
            _textRenderer.DrawLine(buffers, Text, 3);
        }
    }
}