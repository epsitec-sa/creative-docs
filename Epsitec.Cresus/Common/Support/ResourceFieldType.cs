//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

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
		List,							//	champ contient une liste (de bundles)
	}
}
