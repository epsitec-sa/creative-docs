//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// Summary description for ITextRenderer.
	/// </summary>
	public interface ITextRenderer
	{
		void Render(OpenType.Font font, double size, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy);
	}
}
