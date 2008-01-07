//	Copyright � 2005-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface ITextFrame permet de d�crire une zone dans laquelle coule
	/// du texte.
	/// </summary>
	public interface ITextFrame
	{
		int		PageNumber		{ get; set; }
		
		
		bool ConstrainLineBox(double y_dist, double ascender, double descender, double height, double leading, bool sync_to_grid, out double ox, out double oy, out double width, out double next_y_dist);
		
		void MapToView(ref double x, ref double y);
		void MapFromView(ref double x, ref double y);
	}
}
