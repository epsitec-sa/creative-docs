//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// L'énumération RequestType définit le type des requêtes courantes.
	/// </summary>
	public enum RequestType
	{
		Unknown = 0,
		
		Group,										//	groupe de requêtes
		
		InsertStaticData,							//	requête INSERT, données statiques
		
		UpdateStaticData,							//	requête UPDATE, données statiques
		UpdateDynamicData,							//	requête UPDATE, données dynamiques (calculées)
	}
}
