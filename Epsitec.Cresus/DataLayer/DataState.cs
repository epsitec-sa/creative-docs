//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 03/11/2003

namespace Epsitec.Cresus.DataLayer
{
	/// <summary>
	/// L'�num�ration DataState indique dans quel �tat se trouve un enregistrement
	/// de donn�es (DataRecord).
	/// </summary>
	public enum DataState
	{
		Invalid			= 0,					//	donn�es non initialis�es
		
		Unchanged		= 1,					//	donn�es originales (inchang�es)
		Modified		= 2,					//	donn�es modifi�es
		Added			= 3,					//	donn�es ajout�es
		Removed			= 4,					//	donn�es supprim�es
	}
}
