namespace Epsitec.Common.Drawing
{
    /// <summary>
    /// L'interface ICanvasEngine définit un moteur capable de peindre dans
    /// une surface de type Bitmap pour remplir un Canvas à une taille donnée.
    /// </summary>
    public interface ICanvasEngine
    {
        bool TryLoadData(byte[] data);

        Canvas.IconKey[] IconKeys { get; }

        Size Size { get; }

        Point Origin { get; }

        void Paint(
            Graphics graphics,
            Size size,
            GlyphPaintStyle style,
            Color color,
            int page,
            object adorner
        );
    }
}
