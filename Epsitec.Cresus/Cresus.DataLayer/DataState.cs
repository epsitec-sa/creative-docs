//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// L'énumération DataState indique dans quel état se trouve un enregistrement
	/// de données (DataRecord).
	/// </summary>
	public enum DataState
	{
		Invalid			= 0,					//	données non initialisées
		
		Unchanged		= 1,					//	données originales (inchangées)
		Modified		= 2,					//	données modifiées
		Added			= 3,					//	données ajoutées
		Removed			= 4,					//	données supprimées
	}
}
