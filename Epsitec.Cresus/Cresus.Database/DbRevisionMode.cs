//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRevisionMode
	{
		Unsupported			= 0,		//	mode non supporté
		Unknown = Unsupported,			//	mode inconnu (= non supporté)
		
		Disabled			= 1,		//	n'utilise pas l'historique des révisions
		Enabled				= 2,		//	utilise l'historique des révisions
	}
}
