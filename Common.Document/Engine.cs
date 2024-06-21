using Epsitec.Common.Drawing;

namespace Epsitec.Common.Document
{
    /// <summary>
    /// Engine.
    /// </summary>
    public class Engine : ICanvasEngine
    {
        private Engine() { }

        public bool TryLoadData(byte[] data)
        {
            //if (!this.IsDataCompatible(data))
            //{
            //    return false;
            //}
            using (System.IO.MemoryStream stream = new System.IO.MemoryStream(data))
            {
                if (Debug.Settings.UseOldIconFormat)
                {
                    System.Console.WriteLine("Read old format stream");
                    this.document = new Document(
                        DocumentType.Pictogram,
                        DocumentMode.ReadOnly,
                        InstallType.Full,
                        DebugMode.Release,
                        null,
                        null,
                        null,
                        null
                    );
                    string error = this.document.Read(stream, "");
                    if (error != "")
                    {
                        throw new System.InvalidOperationException(error);
                    }
                }
                else
                {
                    System.Console.WriteLine("Read new format stream");
                    this.document = Document.LoadFromXMLStream(stream, DocumentMode.ReadOnly);
                }
            }
            return true;
        }

        public static void Initialize()
        {
            Common.Widgets.Widget.Initialize();
            Res.Initialize();
            if (Engine.current == null)
            {
                Engine.current = new Engine();
                Drawing.Canvas.RegisterEngine(Engine.current);
            }
        }

        #region ICanvasEngine Members
        private bool IsDataCompatible(byte[] data)
        {
            if (
                (data[0] == (byte)'<')
                && (data[1] == (byte)'?')
                && (data[2] == (byte)'x')
                && (data[3] == (byte)'m')
                && (data[4] == (byte)'l')
            )
            {
                return true;
            }

            return false;
        }

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

        protected static Engine current;

        protected Common.Widgets.IAdorner adorner;
        protected GlyphPaintStyle glyphPaintStyle;
        protected Color uniqueColor;

        private Document document;
    }
}
