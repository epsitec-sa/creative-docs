namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'énumération DbRawType liste tous les type bruts supportés par la base
	/// de données, lesquels correspondent aux types fournis par ADO.NET.
	/// </summary>
	public enum DbRawType
	{
		Unsupported,					//	type non supporté
		Null,							//	type pas analysable, donnée absente
		
		Boolean,						//	1 bit
		
		Int16,							//	entier signé, 16 bits
		Int32,							//	entier signé, 32 bits
		Int64,							//	entier signé, 64 bits
		
		SmallDecimal,					//	nombre à virgule à haute résolution (9 chiffres après la virgule)
		LargeDecimal,					//	nombre à virgule de grande taille (mais seulement 3 chiffres après la virgule)
		
		DateTime,						//	date et heure, 64 bits (pas de 100ns, résolution <= 1s)
		
		String,							//	texte (Unicode)
		ByteArray,						//	tableau de bytes
		
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
