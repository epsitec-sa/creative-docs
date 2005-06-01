//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration LeadingMode d�finit les modes utilis�s pour synchroniser
	/// l'interligne avec une grille globale.
	/// </summary>
	public enum LeadingMode
	{
		Undefined,					//	non d�fini
		
		Free,						//	alignment libre
		
		AlignFirst,					//	aligne la premi�re ligne du paragraphe
		AlignAll,					//	aligne toutes les lignes du paragraphe
	}
}
