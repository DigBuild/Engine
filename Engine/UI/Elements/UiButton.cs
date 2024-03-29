﻿using System;
using System.Numerics;
using DigBuild.Engine.Render;
using DigBuild.Engine.Textures;
using DigBuild.Platform.Input;
using DigBuild.Platform.Render;

namespace DigBuild.Engine.Ui.Elements
{
    /// <summary>
    /// A basic UI button.
    /// </summary>
    public sealed class UiButton : IUiElement
    {
        private readonly uint _width, _height;
        private readonly IRenderLayer<UiVertex> _layer;
        private readonly UiVertex[][] _vertices = new UiVertex[3][];
        private bool _hovered, _clicked;

        /// <summary>
        /// Fired when the button is pressed.
        /// </summary>
        public Action? Pressed = null;
        /// <summary>
        /// Fired when the button is released.
        /// </summary>
        public Action? Released = null;
        /// <summary>
        /// Fired when the button is hovered over.
        /// </summary>
        public Action? Hovered = null;
        /// <summary>
        /// Fired when the button is not hovered over anymore.
        /// </summary>
        public Action? UnHovered = null;

        public UiButton(uint width, uint height, IRenderLayer<UiVertex> layer, ISprite inactiveSprite, ISprite hoveredSprite, ISprite clickedSprite)
        {
            _width = width;
            _height = height;
            _layer = layer;

            var sprites = new[] {inactiveSprite, hoveredSprite, clickedSprite};
            for (var i = 0; i < sprites.Length; i++)
            {
                var sprite = sprites[i];
                
                var v1 = new UiVertex(
                    new Vector2(0, 0),
                    sprite.GetInterpolatedUV(0, 0),
                    Vector4.One
                );
                var v2 = new UiVertex(
                    new Vector2(width, 0),
                    sprite.GetInterpolatedUV(1, 0),
                    Vector4.One
                );
                var v3 = new UiVertex(
                    new Vector2(width, height),
                    sprite.GetInterpolatedUV(1, 1),
                    Vector4.One
                );
                var v4 = new UiVertex(
                    new Vector2(0, height),
                    sprite.GetInterpolatedUV(0, 1),
                    Vector4.One
                );

                _vertices[i] = new[] { v1, v2, v3, v3, v4, v1 };
            }
        }

        public void Draw(RenderContext context, IGeometryBuffer buffer, float partialTick)
        {
            var buf = buffer.Get(_layer);
            var i = _clicked ? 2 : _hovered ? 1 : 0;
            buf.Accept(_vertices[i]);
        }

        public void OnCursorMoved(IUiElementContext context, int x, int y)
        {
            var wasHovered = _hovered;
            _hovered = x >= 0 && x < _width && y >= 0 && y < _height;
            if (_hovered != wasHovered)
            {
                (_hovered ? Hovered : UnHovered)?.Invoke();
                context.RequestRedraw();
            }
        }

        public void OnMouseEvent(IUiElementContext context, uint button, MouseAction action)
        {
            if (button != 0) return;

            var wasClicked = _clicked;
            if (_hovered && action == MouseAction.Press)
            {
                Pressed?.Invoke();
                _clicked = true;
            }
            else if (action == MouseAction.Release)
            {
                if (_clicked)
                    Released?.Invoke();
                _clicked = false;
            }

            if (_clicked != wasClicked)
                context.RequestRedraw();
        }
    }
}