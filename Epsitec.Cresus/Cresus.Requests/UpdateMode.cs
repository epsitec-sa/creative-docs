//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Requests
{
	/// <summary>
	/// Le mode de mise à jour est défini par UpdateMode.
	/// </summary>
	public enum UpdateMode
	{
		Full,						//	mise à jour de toutes les colonnes
		Changed						//	mise à jour uniquement des colonnes modifiées
	}
}
