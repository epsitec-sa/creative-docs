//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Support
{
	/// <summary>
	/// L'�num�ration ResourceFieldType d�finit les divers type de champs
	/// qu'une ressource peut contenir.
	/// </summary>
	public enum ResourceFieldType
	{
		None,							//	champ n'existe pas
		Data,							//	champ contient des donn�es (string)
		Binary,							//	champ contient des donn�es binaires
		Bundle,							//	champ contient un bundle
		List,							//	champ contient une liste (de bundles)
	}
}
