//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération AlignMode définit les modes utilisés pour synchroniser
	/// l'interligne avec une grille globale.
	/// </summary>
	public enum AlignMode
	{
		Undefined,					//	non défini
		
		None,						//	sans alignment
		First,						//	aligne la première ligne du paragraphe
		All,						//	aligne toutes les lignes du paragraphe
	}
}
