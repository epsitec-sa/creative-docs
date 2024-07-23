/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
    /// <summary>
    /// Engine.
    /// </summary>
    public class Engine : ICanvasEngine
    {
        private Engine(byte[] data)
        {
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
            {
                this.document = Document.LoadFromXMLStream(stream, DocumentMode.Modify);
            }
        }

        public static Engine CreateEngine(byte[] data)
        {
            return new Engine(data);
        }

        public static void Initialize()
        {
            Common.Widgets.Widget.Initialize();
            Res.Initialize();
            Drawing.Canvas.RegisterEngineFactory(Engine.CreateEngine);
        }

        #region ICanvasEngine Members

        public Canvas.IconKey[] IconKeys
        {
            get => this.document.IconKeys;
        }

        public Size Size
        {
            get => this.document.DocumentSize;
        }

        public Point Origin
        {
            get => this.document.HotSpot;
        }

        public void Paint(
            Graphics graphics,
            Size size,
            GlyphPaintStyle style,
            Color color,
            int page,
            object adornerObject
        )
        {
            DrawingContext context = new DrawingContext(this.document, null);

            this.adorner = adornerObject as Common.Widgets.IAdorner;
            this.glyphPaintStyle = style;
            this.uniqueColor = color;
            graphics.PushColorModifier(new ColorModifierCallback(this.ColorModifier));

            context.ContainerSize = size;
            context.PreviewActive = true;
            context.LayerDrawingMode = LayerDrawingMode.ShowInactive;
            context.RootStackPush(page); // première page
            context.RootStackPush(0); // premier calque

            Point scale = context.Scale;
            graphics.ScaleTransform(scale.X, scale.Y, 0, 0);

            this.document.Paint(graphics, context, Rectangle.MaxValue);

            graphics.PopColorModifier();
        }

        protected RichColor ColorModifier(RichColor color)
        {
            //	Adapte une couleur.
            if (this.adorner != null)
            {
                color.Basic = this.adorner.AdaptPictogramColor(
                    color.Basic,
                    this.glyphPaintStyle,
                    this.uniqueColor
                );
            }

            return color;
        }
        #endregion

        protected Common.Widgets.IAdorner adorner;
        protected GlyphPaintStyle glyphPaintStyle;
        protected Color uniqueColor;

        private Document document;
    }
}
