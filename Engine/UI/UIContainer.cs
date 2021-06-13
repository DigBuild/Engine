using System.Collections.ObjectModel;
using System.Numerics;
using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    public sealed class UiContainer : IUiElement
    {
        private readonly ElementContainer _children = new();

        public bool Visible { get; set; } = true;

        public void Add(int x, int y, IUiElement element)
        {
            _children.Add(new UIElementData(x, y, element));
        }

        public void Add(uint x, uint y, IUiElement element)
        {
            Add((int) x, (int) y, element);
        }

        public void Remove(IUiElement element)
        {
            _children.Remove(element);
        }

        public void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick)
        {
            if (!Visible)
                return;

            var transform = buffer.Transform;
            foreach (var child in _children)
            {
                buffer.Transform = transform * Matrix4x4.CreateTranslation(child.X, child.Y, 0);
                child.Element.Draw(context, buffer, partialTick);
            }
            buffer.Transform = transform;
        }

        public void OnCursorMoved(IUiElementContext context, int x, int y)
        {
            if (!Visible)
                return;

            foreach (var child in _children)
                child.Element.OnCursorMoved(context, x - child.X, y - child.Y);
        }

        public void OnMouseEvent(IUiElementContext context, uint button, MouseAction action)
        {
            if (!Visible)
                return;

            foreach (var child in _children)
                child.Element.OnMouseEvent(context, button, action);
        }

        private sealed class UIElementData
        {
            internal readonly int X, Y;
            internal readonly IUiElement Element;

            public UIElementData(int x, int y, IUiElement element)
            {
                X = x;
                Y = y;
                Element = element;
            }
        }

        private sealed class ElementContainer : KeyedCollection<IUiElement, UIElementData>
        {
            protected override IUiElement GetKeyForItem(UIElementData item)
            {
                return item.Element;
            }
        }
    }
}