//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface INamedStringSelection définit les propriétés de base nécessaires
	/// à un objet qui permet de choisir un élément (texte/nom) dans une liste.
	/// </summary>
	public interface INamedStringSelection : IStringSelection
	{
		string SelectedName
		{
			get;
			set;
		}
	}
}
