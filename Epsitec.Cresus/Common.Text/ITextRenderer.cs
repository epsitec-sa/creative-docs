//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		void Render(ITextFrame frame, OpenType.Font font, double size, Drawing.Color color, Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool is_last_run);
		void Render(ITextFrame frame, IGlyphRenderer glyph_renderer, Drawing.Color color, double x, double y, bool is_last_run);
		void RenderEndLine(Layout.Context context);
		void RenderEndParagraph(Layout.Context context);
	}
}
