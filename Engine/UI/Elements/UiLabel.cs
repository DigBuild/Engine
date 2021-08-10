using DigBuild.Engine.Render;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui.Elements
{
    public sealed class UiLabel : IUiElement
    {
        private readonly TextRenderer _textRenderer;
        private readonly bool _yellow;
        public string Text { get; set; }

        public UiLabel(string text = "", TextRenderer textRenderer = null!, bool yellow = false)
        {
            _textRenderer = textRenderer ?? IUiElement.GlobalTextRenderer;
            _yellow = yellow;
            Text = text;
        }

        public void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick)
        {
            _textRenderer.DrawLine(buffer, Text, 3, _yellow);
        }
    }
}