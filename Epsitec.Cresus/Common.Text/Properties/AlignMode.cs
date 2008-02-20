//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Properties
{
	/// <summary>
	/// L'�num�ration AlignMode d�finit les modes utilis�s pour synchroniser
	/// l'interligne avec une grille globale.
	/// </summary>
	public enum AlignMode
	{
		Undefined,					//	non d�fini
		
		None,						//	sans alignment
		First,						//	aligne la premi�re ligne du paragraphe
		All,						//	aligne toutes les lignes du paragraphe
	}
}
