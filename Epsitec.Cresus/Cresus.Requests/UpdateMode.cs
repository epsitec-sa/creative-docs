//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// Le mode de mise à jour est défini par UpdateMode.
	/// </summary>
	public enum UpdateMode
	{
		Full,						//	met à jour toutes les colonnes
		Changed						//	met à jour uniquement les colonnes modifiées
	}
}
