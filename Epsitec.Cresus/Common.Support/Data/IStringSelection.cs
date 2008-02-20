//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
