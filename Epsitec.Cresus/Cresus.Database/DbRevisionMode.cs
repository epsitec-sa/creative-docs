//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbRevisionMode
	{
		Unsupported			= 0,		//	mode non support�
		Unknown = Unsupported,			//	mode inconnu (= non support�)
		
		Disabled			= 1,		//	n'utilise pas l'historique des r�visions
		Enabled				= 2,		//	utilise l'historique des r�visions
	}
}
