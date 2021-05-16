using System;
using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    public interface IUiElement
    {
        public static TextRenderer GlobalTextRenderer { get; set; } = null!;

        void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick);

        void OnCursorMoved(IUiElementContext context, int x, int y) { }

        void OnMouseEvent(IUiElementContext context, uint button, MouseAction action) { }
    }

    public interface IUiElementContext
    {
        public delegate void KeyboardEventDelegate(uint code, KeyboardAction action);

        void SetKeyboardEventHandler(KeyboardEventDelegate? handler, Action? removeCallback = null);

        void RequestRedraw();

        void RequestClose();
    }
}