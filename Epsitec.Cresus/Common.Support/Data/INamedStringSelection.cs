//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 10/03/2004

namespace Epsitec.Common.Support.Data
{
	/// <summary>
	/// L'interface INamedStringSelection d�finit les propri�t�s de base n�cessaires
	/// � un objet qui permet de choisir un �l�ment (texte/nom) dans une liste.
	/// </summary>
	public interface INamedStringSelection : IStringSelection
	{
		string	SelectedName	{ get; set; }
	}
}
