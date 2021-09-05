using DigBuild.Engine.Render;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui.Elements
{
    /// <summary>
    /// A basic UI label.
    /// </summary>
    public sealed class UiLabel : IUiElement
    {
        private readonly ITextRenderer _textRenderer;
        private readonly bool _yellow;

        /// <summary>
        /// The text.
        /// </summary>
        public string Text { get; set; }

        public UiLabel(string text = "", ITextRenderer textRenderer = null!, bool yellow = false)
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