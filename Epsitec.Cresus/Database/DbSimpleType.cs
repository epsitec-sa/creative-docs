//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'énumération DbSimpleType liste les types simplifiés utilisés pour le stockage
	/// interne d'informations diverses.
	/// </summary>
	public enum DbSimpleType
	{
		Unsupported,					//	type non supporté
		Null,							//	type pas analysable, donnée absente
		
		Decimal,						//	tout ce qui est numérique (booléen, entier, réel, temps, ...)
		String,							//	texte (Unicode)
		Date,							//	date, uniquement
		Time,							//	heure, uniquement
		DateTime,						//	date et heure, 64 bits (résolution de 1ms ou mieux)
		ByteArray,						//	tableau de bytes
		Guid,							//	identificateur globalement unique, 128 bits
	}
}
