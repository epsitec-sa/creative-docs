//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'énumération DbRawType liste tous les type bruts "supportés" par la base
	/// de données, lesquels correspondent aux types fournis par ADO.NET.
	/// </summary>
	public enum DbRawType
	{
		Unsupported,					//	type non supporté
		Unknown = Unsupported,			//	type inconnu (= type non supporté)
		Null,							//	type pas analysable, donnée absente
		
		Boolean,						//	1 bit
		
		Int16,							//	entier signé, 16 bits
		Int32,							//	entier signé, 32 bits
		Int64,							//	entier signé, 64 bits
		
		SmallDecimal,					//	nombre à virgule à haute résolution (9 chiffres après la virgule)
		LargeDecimal,					//	nombre à virgule de grande taille (mais seulement 3 chiffres après la virgule)
		
		Date,							//	date, uniquement
		Time,							//	heure, uniquement
		DateTime,						//	date et heure, 64 bits (résolution de 1ms ou mieux)
		
		String,							//	texte (Unicode)
		ByteArray,						//	tableau de bytes
		
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
