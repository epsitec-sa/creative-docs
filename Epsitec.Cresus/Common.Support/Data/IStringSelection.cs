//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface IStringSelection d�finit les propri�t�s de base n�cessaires
	/// � un objet qui permet de choisir un �l�ment (texte) dans une liste.
	/// </summary>
	public interface IStringSelection
	{
		int		SelectedIndex	{ get; set; }
		string	SelectedItem	{ get; set; }
		
		event EventHandler	SelectedIndexChanged;
	}
}
