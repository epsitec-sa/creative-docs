namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'�num�ration DbSimpleType liste les types simplifi�s utilis�s pour le stockage
	/// interne d'informations diverses.
	/// </summary>
	public enum DbSimpleType
	{
		Unsupported,					//	type non support�
		Null,							//	type pas analysable, donn�e absente
		
		Decimal,						//	tout ce qui est num�rique (bool�en, entier, r�el, temps, ...)
		String,							//	texte (Unicode)
		Date,							//	date, uniquement
		Time,							//	heure, uniquement
		DateTime,						//	date et heure, 64 bits (r�solution de 1ms ou mieux)
		ByteArray,						//	tableau de bytes
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
