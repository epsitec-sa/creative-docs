//	Copyright � 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// Le mode de mise � jour est d�fini par UpdateMode.
	/// </summary>
	public enum UpdateMode
	{
		Full,						//	met � jour toutes les colonnes
		Changed						//	met � jour uniquement les colonnes modifi�es
	}
}
