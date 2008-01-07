//	Copyright � 2004-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
