//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 22/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'énumération DbElementCat définit les catégories des éléments
	/// (table, colonne) de la base.
	/// </summary>
	public enum DbElementCat
	{
		Unsupported			= 0,	//	catégorie non supportée
		Unknown = Unsupported,		//	catégorie inconnue (= non supportée)
		
		Internal			= 1,	//	élément à usage interne
		UserDataManaged		= 2,	//	élément sous contrôle de l'utilisateur, géré par Crésus
		UserDataExternal	= 3,	//	élément sous contrôle de l'utilisateur, non géré (source externe)
	}
}
