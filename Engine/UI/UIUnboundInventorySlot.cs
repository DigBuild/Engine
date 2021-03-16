using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Items;
using DigBuild.Engine.Render;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.UI
{
    public sealed class UIUnboundInventorySlot : IUIElement
    {
        private const uint Scale = 48;
        
        private readonly InventorySlot _slot;
        private readonly Dictionary<Item, IItemModel> _models;
        private readonly TextRenderer _textRenderer;

        public int PosX { get; set; }
        public int PosY { get; set; }

        public UIUnboundInventorySlot(InventorySlot slot, Dictionary<Item, IItemModel> models, TextRenderer textRenderer = null!)
        {
            _slot = slot;
            _models = models;
            _textRenderer = textRenderer ?? IUIElement.GlobalTextRenderer;
        }

        public void Draw(RenderContext context, GeometryBufferSet buffers)
        {
            if (_slot.Item.Count > 0 && _models.TryGetValue(_slot.Item.Type, out var model))
            {
                var originalTransform = Matrix4x4.CreateTranslation(PosX, PosY, Scale) * buffers.Transform;
                var transform = Matrix4x4.CreateTranslation(-Vector3.One / 2) *
                                Matrix4x4.CreateScale(Scale) *
                                originalTransform;
                buffers.Transform = transform;
                model.AddGeometry(ItemModelTransform.Inventory, buffers);
                if (model.HasDynamicGeometry)
                {
                    buffers.Transform = transform;
                    model.AddDynamicGeometry(ItemModelTransform.Inventory, buffers);
                }
                
                buffers.Transform = Matrix4x4.CreateTranslation(Scale / 6f, Scale / 2f, 0) * originalTransform;
                _textRenderer.DrawLine(buffers, $"{_slot.Item.Count,2:d2}", 3);
            }
        }

        public void OnCursorMoved(IUIElementContext context, int x, int y)
        {
            PosX = x;
            PosY = y;
        }
    }
}