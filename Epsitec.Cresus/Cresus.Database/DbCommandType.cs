//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'�num�ration DbCommandType d�crit les divers types de commandes, permettant
	/// de savoir s'il faut appeler ExecuteNonQuery ou l'une des variantes retournant
	/// des donn�es (ExecuteScalar, ExecuteReader, IDataAdapter.Fill, etc.).
	/// </summary>
	public enum DbCommandType
	{
		None			= 0,			//	pas de commande
		
		Silent			= 1,			//	ExecuteNonQuery retourne -1
		NonQuery		= 2,			//	ExecuteNonQuery retourne nb. lignes affect�es
		
		ReturningData	= 3				//	retourne des donn�es (requ�te SELECT)
	}
}
