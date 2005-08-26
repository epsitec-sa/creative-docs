//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface INamedStringSelection définit les propriétés de base nécessaires
	/// à un objet qui permet de choisir un élément (texte/nom) dans une liste.
	/// </summary>
	public interface INamedStringSelection : IStringSelection
	{
		string	SelectedName	{ get; set; }
	}
}
