//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/03/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface IStringSelection définit les propriétés de base nécessaires
	/// à un objet qui permet de choisir un élément (texte) dans une liste.
	/// </summary>
	public interface IStringSelection
	{
		int		SelectedIndex	{ get; set; }
		string	SelectedItem	{ get; set; }
		
		event EventHandler	SelectedIndexChanged;
	}
}
