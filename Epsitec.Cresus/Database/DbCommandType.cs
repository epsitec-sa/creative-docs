//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'énumération DbCommandType décrit les divers types de commandes, permettant
	/// de savoir s'il faut appeler ExecuteNonQuery ou l'une des variantes retournant
	/// des données (ExecuteScalar, ExecuteReader, IDataAdapter.Fill, etc.).
	/// </summary>
	[System.Flags]
	public enum DbCommandType
	{
		None			= 0,			//	pas de commande
		
		Silent			= 1,			//	ExecuteNonQuery retourne -1
		NonQuery		= 2,			//	ExecuteNonQuery retourne nb. lignes affectées
		
		ReturningData	= 3,			//	retourne des données (requête SELECT)
		
		Mask			= 0x00FF,		//	masque pour les valeurs
		
		FlagMultiple	= 0x0100		//	commande multiple
	}
}
