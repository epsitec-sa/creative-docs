//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Remoting
{
	/// <summary>
	/// Summary description for ProgressStatus.
	/// </summary>
	public enum ProgressStatus
	{
		None,
		
		Running		= 1,		//	en cours d'exécution
		
		Succeeded	= 2,		//	terminé, l'opération a été couronnée de succès
		Cancelled	= 3,		//	terminé, l'opération a été annulée
		Failed		= 4,		//	terminé, l'opération a échoué
	}
}
