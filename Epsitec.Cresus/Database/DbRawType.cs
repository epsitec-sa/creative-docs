namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'�num�ration DbRawType liste tous les type bruts support�s par la base
	/// de donn�es, lesquels correspondent aux types fournis par ADO.NET.
	/// </summary>
	public enum DbRawType
	{
		Unsupported,					//	type non support�
		Null,							//	type pas analysable, donn�e absente
		
		Boolean,						//	1 bit
		
		Int16,							//	entier sign�, 16 bits
		Int32,							//	entier sign�, 32 bits
		Int64,							//	entier sign�, 64 bits
		
		SmallDecimal,					//	nombre � virgule � haute r�solution (9 chiffres apr�s la virgule)
		LargeDecimal,					//	nombre � virgule de grande taille (mais seulement 3 chiffres apr�s la virgule)
		
		DateTime,						//	date et heure, 64 bits (pas de 100ns, r�solution <= 1s)
		
		String,							//	texte (Unicode)
		ByteArray,						//	tableau de bytes
		
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
