//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// L'interface IGlyphRenderer permet d'abstraire le code n�cessaire au
	/// rendu de glyphes/pictogrammes/images quelconques.
	/// </summary>
	public interface IGlyphRenderer
	{
		bool GetGeometry(out double ascender, out double descender, out double advance, out double x1, out double x2);
	}
}
