//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/03/2004

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
