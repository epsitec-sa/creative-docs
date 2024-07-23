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


namespace Epsitec.Common.Drawing
{
    public delegate RichColor ColorModifierCallback(RichColor color);

    /// <summary>
    /// The IPaintPort interface is used for the Graphics class, but also for
    /// printing (see Common.Printing project).
    /// </summary>
    public interface IPaintPort
    {
        double LineWidth { get; set; }
        JoinStyle LineJoin { get; set; }
        CapStyle LineCap { get; set; }
        double LineMiterLimit { get; set; }
        RichColor RichColor { get; set; }
        Color Color { get; set; }
        RichColor FinalRichColor { get; set; }
        Color FinalColor { get; set; }
        Transform Transform { get; set; }
        FillMode FillMode { get; set; }
        ImageFilter ImageFilter { get; set; }
        Margins ImageCrop { get; set; }
        Size ImageFinalSize { get; set; }

        bool HasEmptyClippingRectangle { get; }

        void PushColorModifier(ColorModifierCallback method);
        ColorModifierCallback PopColorModifier();

        RichColor GetFinalColor(RichColor color);
        Color GetFinalColor(Color color);

        void SetClippingRectangle(Rectangle rect);
        Rectangle SaveClippingRectangle();
        void RestoreClippingRectangle(Rectangle rect);
        void ResetClippingRectangle();

        void Align(ref double x, ref double y);

        void ScaleTransform(double sx, double sy, double cx, double cy);
        void RotateTransformDeg(double angle, double cx, double cy);
        void RotateTransformRad(double angle, double cx, double cy);
        void TranslateTransform(double ox, double oy);
        void MergeTransform(Transform transform);

        void PaintOutline(Path path);
        void PaintSurface(Path path);

        void PaintGlyphs(
            Font font,
            double size,
            ushort[] glyphs,
            double[] x,
            double[] y,
            double[] sx,
            double[] sy
        );

        double PaintText(double x, double y, string text, Font font, double size);
        double PaintText(
            double x,
            double y,
            string text,
            Font font,
            double size,
            FontClassInfo[] infos
        );

        void PaintImage(Image bitmap, Rectangle fill);
        void PaintImage(
            Image bitmap,
            double fillX,
            double fillY,
            double fillWidth,
            double fillHeight
        );
        void PaintImage(Image bitmap, Rectangle fill, Point imageOrigin);
        void PaintImage(Image bitmap, Rectangle fill, Rectangle imageRect);
        void PaintImage(
            Image bitmap,
            double fillX,
            double fillY,
            double fillWidth,
            double fillHeight,
            double imageOriginX,
            double imageOriginY
        );
        void PaintImage(
            Image bitmap,
            double fillX,
            double fillY,
            double fillWidth,
            double fillHeight,
            double imageOriginX,
            double imageOriginY,
            double imageWidth,
            double imageHeight
        );
    }
}
