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
		
		Int8,							//	entier signé, 8 bits
		Int16,							//	entier signé, 16 bits
		Int32,							//	entier signé, 32 bits
		Int64,							//	entier signé, 64 bits
		
		Decimal,						//	nombre à virgule à très haute résolution
		
		Double,							//	nombre flottant, 64 bits
		
		DateTime,						//	date et heure, 64 bits (résolution de 100ns)
		
		String,							//	texte (Unicode)
		ByteArray,						//	tableau de bytes
		
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
