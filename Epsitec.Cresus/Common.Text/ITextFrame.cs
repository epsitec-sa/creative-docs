//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
	}
}
