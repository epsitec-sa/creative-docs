namespace Epsitec.Cresus.Support
{
	/// <summary>
	/// L'énumération ResourceFieldType définit les divers type de champs
	/// qu'une ressource peut contenir.
	/// </summary>
	public enum ResourceFieldType
	{
		None,							//	champ n'existe pas
		String,							//	champ contient du texte
		Bundle,							//	champ contient un bundle
		BundleList						//	champ contient une liste de bundles
	}
}
