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
		
		Int8,							//	entier sign�, 8 bits
		Int16,							//	entier sign�, 16 bits
		Int32,							//	entier sign�, 32 bits
		Int64,							//	entier sign�, 64 bits
		
		Decimal,						//	nombre � virgule � tr�s haute r�solution
		
		Double,							//	nombre flottant, 64 bits
		
		DateTime,						//	date et heure, 64 bits (r�solution de 100ns)
		
		String,							//	texte (Unicode)
		ByteArray,						//	tableau de bytes
		
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
