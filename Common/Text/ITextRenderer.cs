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


namespace Epsitec.Common.Text
{
    /// <summary>
    /// L'interface ITextRenderer permet d'abstraire le code nécessaire au rendu
    /// du texte (que ce soit à l'écran ou sous une autre représentation).
    /// </summary>
    public interface ITextRenderer
    {
        bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height);

        void RenderStartParagraph(Layout.Context context);
        void RenderStartLine(Layout.Context context);
        void RenderTab(
            Layout.Context layout,
            string tag,
            double tabOrigin,
            double tabStop,
            ulong tabCode,
            bool isTabDefined,
            bool isTabAuto
        );
        void Render(
            Layout.Context layout,
            OpenType.Font font,
            double size,
            string color,
            Layout.TextToGlyphMapping mapping,
            ushort[] glyphs,
            double[] x,
            double[] y,
            double[] sx,
            double[] sy,
            bool isLastRun
        );
        void Render(
            Layout.Context layout,
            IGlyphRenderer glyphRenderer,
            string color,
            double x,
            double y,
            bool isLastRun
        );
        void RenderEndLine(Layout.Context context);
        void RenderEndParagraph(Layout.Context context);
    }
}
