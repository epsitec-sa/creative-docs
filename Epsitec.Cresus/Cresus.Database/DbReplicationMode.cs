//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbReplicationMode
	{
		Unsupported			= 0,		//	mode non supporté
		Unknown = Unsupported,			//	mode inconnu (= non supporté)
		
		Private				= 1,		//	information privée, pas de réplication
		Shared				= 2,		//	information partagée par tous, répliquée
	}
}
