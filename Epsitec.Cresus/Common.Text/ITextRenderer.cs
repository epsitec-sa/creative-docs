//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		void RenderTab(Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined, bool isTabAuto);
		void Render(Layout.Context layout, OpenType.Font font, double size, string color, Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun);
		void Render(Layout.Context layout, IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun);
		void RenderEndLine(Layout.Context context);
		void RenderEndParagraph(Layout.Context context);
	}
}
