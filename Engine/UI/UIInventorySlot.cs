using System;
using System.Collections.Generic;
using System.Numerics;
using DigBuild.Engine.Items;
using DigBuild.Engine.Render;
using DigBuild.Engine.Render.Models;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui
{
    public sealed class UiInventorySlot : IUiElement
    {
        public const uint Scale = 32;

        private static readonly Vector4 Color = new(0.0f, 0.01f, 0.05f, 0.7f);
        private static readonly Vector4 MarkerColor = new(0.9f, 0.8f, 0.2f, 0.8f);
        private static readonly UiVertex[] Vertices = new UiVertex[3 * 6];
        private static readonly UiVertex[] MarkerVertices = new UiVertex[3 * 4];

        internal static readonly Matrix4x4 ItemTransform = Matrix4x4.CreateTranslation(-Vector3.One / 2) * Matrix4x4.CreateRotationX(MathF.PI);

        static UiInventorySlot()
        {
            var center = new UiVertex(Vector2.Zero, Vector2.Zero, Color);
            for (var i = 0; i < 6; i++)
            {
                var a = -i * MathF.PI / 3;
                Vertices[i * 3] = center;
                Vertices[(i * 3 + 2) % Vertices.Length] =
                    Vertices[(i * 3 + 4) % Vertices.Length] =
                        new UiVertex(new Vector2(MathF.Sin(a), MathF.Cos(a)), Vector2.Zero, Color);
            }
            
            var offset = new Vector2(0, -0.1f);
            var caratCenter = new Vector2(0, -1f) + offset;
            var caratLeft = new Vector2(MathF.Sin(-MathF.PI / 3), -MathF.Cos(-MathF.PI / 3)) * 0.625f - new Vector2(0, 0.375f) + offset;
            var caratRight = new Vector2(MathF.Sin(MathF.PI / 3), -MathF.Cos(MathF.PI / 3)) * 0.625f - new Vector2(0, 0.375f) + offset;

            var topOff = new Vector2(0, -0.15f);
            MarkerVertices[0] = MarkerVertices[3] = MarkerVertices[6] =  MarkerVertices[9] = new UiVertex(caratCenter, Vector2.Zero, MarkerColor);
            MarkerVertices[1] = new UiVertex(caratLeft, Vector2.Zero, MarkerColor);
            MarkerVertices[2] = MarkerVertices[4] = new UiVertex(caratLeft + topOff, Vector2.Zero, MarkerColor);
            MarkerVertices[5] = MarkerVertices[7] = new UiVertex(caratCenter + topOff, Vector2.Zero, MarkerColor);
            MarkerVertices[8] = MarkerVertices[10] = new UiVertex(caratRight + topOff, Vector2.Zero, MarkerColor);
            MarkerVertices[11] = new UiVertex(caratRight, Vector2.Zero, MarkerColor);
        }
        
        private readonly IInventorySlot _slot, _pickedSlot;
        private readonly IReadOnlyDictionary<Item, IItemModel> _models;
        private readonly IRenderLayer<UiVertex> _layer;
        private readonly Func<bool>? _isActive;
        private readonly TextRenderer _textRenderer;
        private bool _hovered;

        public UiInventorySlot(
            IInventorySlot slot, IInventorySlot pickedSlot,
            IReadOnlyDictionary<Item, IItemModel> models, IRenderLayer<UiVertex> layer,
            Func<bool>? isActive = null, TextRenderer textRenderer = null!
        )
        {
            _slot = slot;
            _pickedSlot = pickedSlot;
            _models = models;
            _layer = layer;
            _isActive = isActive;
            _textRenderer = textRenderer ?? IUiElement.GlobalTextRenderer;
        }

        public void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick)
        {
            var originalTransform = buffer.Transform;
            buffer.Transform = Matrix4x4.CreateScale(Scale) * buffer.Transform;
            buffer.Get(_layer).Accept(Vertices);
            if (_isActive != null && _isActive())
                buffer.Get(_layer).Accept(MarkerVertices);
            
            if (_slot.Item.Count > 0 && _models.TryGetValue(_slot.Item.Type, out var model))
            {
                var transform = ItemTransform * buffer.Transform;
                buffer.Transform = transform;
                var modelData = _slot.Item.Get(ModelData.ItemAttribute);
                model.AddGeometry(buffer, modelData, ItemModelTransform.Inventory, partialTick);
                
                buffer.Transform = Matrix4x4.CreateTranslation(Scale / 6f, Scale / 2f, 0) * originalTransform;
                _textRenderer.DrawLine(buffer, $"{_slot.Item.Count,2:d2}", 3);
            }
        }

        public void OnCursorMoved(IUiElementContext context, int x, int y)
        {
            _hovered = IsInsideHexagon(new Vector2(x, y), Vector2.Zero, Scale);
        }

        public void OnMouseEvent(IUiElementContext context, uint button, MouseAction action)
        {
            if (!_hovered || button != 0 || action != MouseAction.Press) return;

            var currentPicked = _pickedSlot.Item;
            _pickedSlot.Item = _slot.Item;
            _slot.Item = currentPicked;
        }

        private static bool IsInsideHexagon(Vector2 pos, Vector2 center, float radius)
        {
            var _hori = radius * MathF.Cos(MathF.PI / 6);
            var _vert = radius / 2;

            var q2x = MathF.Abs(pos.X - center.X);
            var q2y = MathF.Abs(pos.Y - center.Y);
            if (q2x > _hori || q2y > _vert * 2) return false;
            return 2 * _vert * _hori - _vert * q2x - _hori * q2y >= 0;
        }
    }
}