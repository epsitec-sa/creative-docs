//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// Le mode de mise � jour est d�fini par UpdateMode.
	/// </summary>
	public enum UpdateMode
	{
		Full,						//	mise � jour de toutes les colonnes
		Changed						//	mise � jour uniquement des colonnes modifi�es
	}
}
