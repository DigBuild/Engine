using System.Collections.ObjectModel;
using System.Numerics;
using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.UI
{
    public sealed class UIContainer : IUIElement
    {
        private readonly ElementContainer _children = new();

        public void Add(uint x, uint y, IUIElement element)
        {
            _children.Add(new UIElementData(x, y, element));
        }

        public void Remove(IUIElement element)
        {
            _children.Remove(element);
        }

        public void Draw(RenderContext context, GeometryBufferSet buffers)
        {
            var transform = buffers.Transform;
            foreach (var child in _children)
            {
                buffers.Transform = transform * Matrix4x4.CreateTranslation(child.X, child.Y, 0);
                child.Element.Draw(context, buffers);
            }
            buffers.Transform = transform;
        }

        public void OnCursorMoved(IUIElementContext context, int x, int y)
        {
            foreach (var child in _children)
                child.Element.OnCursorMoved(context, (int) (x - child.X), (int) (y - child.Y));
        }

        public void OnMouseEvent(IUIElementContext context, uint button, MouseAction action)
        {
            foreach (var child in _children)
                child.Element.OnMouseEvent(context, button, action);
        }

        private sealed class UIElementData
        {
            internal readonly uint X, Y;
            internal readonly IUIElement Element;

            public UIElementData(uint x, uint y, IUIElement element)
            {
                X = x;
                Y = y;
                Element = element;
            }
        }

        private sealed class ElementContainer : KeyedCollection<IUIElement, UIElementData>
        {
            protected override IUIElement GetKeyForItem(UIElementData item)
            {
                return item.Element;
            }
        }
    }
}