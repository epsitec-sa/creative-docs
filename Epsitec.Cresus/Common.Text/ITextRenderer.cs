//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for ITextRenderer.
	/// </summary>
	public interface ITextRenderer
	{
		bool IsFrameAreaVisible(ITextFrame frame, double x, double y, double width, double height);
		
		void Render(ITextFrame frame, OpenType.Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy);
	}
}
