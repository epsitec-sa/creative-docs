//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Requests
{
	/// <summary>
	/// L'énumération Type définit le type des requêtes courantes.
	/// </summary>
	public enum Type
	{
		Unknown = 0,
		
		Group,										//	groupe de requêtes
		
		InsertStaticData,							//	requête INSERT, données statiques
		
		UpdateStaticData,							//	requête UPDATE, données statiques
		UpdateDynamicData,							//	requête UPDATE, données dynamiques (calculées)
	}
}
