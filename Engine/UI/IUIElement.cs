using System;
using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.UI
{
    public interface IUIElement
    {
        public static TextRenderer GlobalTextRenderer { get; set; } = null!;

        void Draw(RenderContext context, GeometryBufferSet buffers);

        void OnCursorMoved(IUIElementContext context, int x, int y) { }

        void OnMouseEvent(IUIElementContext context, uint button, MouseAction action) { }
    }

    public interface IUIElementContext
    {
        public delegate void KeyboardEventDelegate(uint code, KeyboardAction action);

        void SetKeyboardEventHandler(KeyboardEventDelegate? handler, Action? removeCallback = null);

        void RequestRedraw();
    }
}