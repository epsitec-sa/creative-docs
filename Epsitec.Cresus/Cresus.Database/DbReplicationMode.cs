//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	public enum DbReplicationMode
	{
		Unsupported			= 0,		//	mode non support�
		Unknown = Unsupported,			//	mode inconnu (= non support�)
		
		Private				= 1,		//	information priv�e, pas de r�plication
		Shared				= 2,		//	information partag�e par tous, r�pliqu�e
	}
}
