//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'énumération LeadingMode définit les modes utilisés pour synchroniser
	/// l'interligne avec une grille globale.
	/// </summary>
	public enum LeadingMode
	{
		Undefined,					//	non défini
		
		Free,						//	alignment libre
		
		AlignFirst,					//	aligne la première ligne du paragraphe
		AlignAll,					//	aligne toutes les lignes du paragraphe
	}
}
