//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface ITextFrame permet de décrire une zone dans laquelle coule
	/// du texte.
	/// </summary>
	public interface ITextFrame
	{
		int		PageNumber		{ get; set; }
		
		
		bool ConstrainLineBox(double yDist, double ascender, double descender, double height, double leading, bool syncToGrid, out double ox, out double oy, out double width, out double nextYDist);
		
		void MapToView(ref double x, ref double y);
		void MapFromView(ref double x, ref double y);
	}
}
