//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 29/10/2003

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'énumération ResourceFieldType définit les divers type de champs
	/// qu'une ressource peut contenir.
	/// </summary>
	public enum ResourceFieldType
	{
		None,							//	champ n'existe pas
		Data,							//	champ contient des données (string)
		Binary,							//	champ contient des données binaires
		Bundle,							//	champ contient un bundle
		List							//	champ contient une liste (de bundles)
	}
}
