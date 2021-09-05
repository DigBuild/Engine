using System;
using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    /// <summary>
    /// A user interface element.
    /// </summary>
    public interface IUiElement
    {
        /// <summary>
        /// The text renderer shared by all UI elements that do not wish to have their own.
        /// Must be initialized manually.
        /// </summary>
        public static ITextRenderer GlobalTextRenderer { get; set; } = null!;

        /// <summary>
        /// Draws this UI element to the geometry buffer.
        /// </summary>
        /// <param name="context">The render context</param>
        /// <param name="buffer">The geometry buffer</param>
        /// <param name="partialTick">The interpolated tick</param>
        void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick);

        /// <summary>
        /// Handles cursor movement events.
        /// </summary>
        /// <param name="context">The UI element context</param>
        /// <param name="x">The relative cursor X position</param>
        /// <param name="y">The relative cursor Y position</param>
        void OnCursorMoved(IUiElementContext context, int x, int y) { }

        /// <summary>
        /// Handles mouse click events.
        /// </summary>
        /// <param name="context">The UI element context</param>
        /// <param name="button">The button</param>
        /// <param name="action">The button's action</param>
        void OnMouseEvent(IUiElementContext context, uint button, MouseAction action) { }

        /// <summary>
        /// Handles mouse scroll events.
        /// </summary>
        /// <param name="context">The UI element context</param>
        /// <param name="xOffset">The X scroll offset</param>
        /// <param name="yOffset">The Y scroll offset</param>
        void OnScrollEvent(IUiElementContext context, double xOffset, double yOffset) { }
    }

    /// <summary>
    /// A UI element context.
    /// </summary>
    public interface IUiElementContext
    {
        public delegate void KeyboardEventDelegate(uint code, KeyboardAction action);

        /// <summary>
        /// Claims control over the keyboard, and optionally sets a callback for when it gets taken away.
        /// </summary>
        /// <param name="handler">The keyboard handler</param>
        /// <param name="removeCallback">The removal callback</param>
        void SetKeyboardEventHandler(KeyboardEventDelegate? handler, Action? removeCallback = null);

        /// <summary>
        /// Requests that this element be redrawn.
        /// </summary>
        void RequestRedraw();

        /// <summary>
        /// Requests that this UI be closed.
        /// </summary>
        void RequestClose();
    }
}