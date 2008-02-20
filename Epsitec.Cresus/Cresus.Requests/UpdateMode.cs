//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
