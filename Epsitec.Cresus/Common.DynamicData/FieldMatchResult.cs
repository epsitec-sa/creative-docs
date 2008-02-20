//	Copyright � 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.DynamicData
{
	/// <summary>
	/// L'�num�ration FieldMatchResult d�finit si un champ se trouve dans l'une
	/// des structures DataTable, DataRow ou DataColumn.
	/// </summary>
	public enum FieldMatchResult
	{
		Zero,									//	non, aucune correspondance
		One,									//	oui, exactement une correspondance
		Some,									//	peut-�tre, 0..n correspondances
		All										//	oui, exactement n correspondances
	}
}
