//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/03/2004

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'interface INamedStringSelection définit les propriétés de base nécessaires
	/// à un widget qui permet de choisir un élément (texte/nom) dans une liste.
	/// </summary>
	public interface INamedStringSelection
	{
		int		SelectedIndex	{ get; set; }
		string	SelectedItem	{ get; set; }
		string	SelectedName	{ get; set; }
		
		event EventHandler	SelectedIndexChanged;
	}
}
