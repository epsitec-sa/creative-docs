namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ITypeConverter est utilisée pour convertir des types entre
	/// une représentation brute publique et une représentation interne propre au
	/// provider ADO.NET, et vice versa.
	/// </summary>
	public interface ITypeConverter
	{
		/// <summary>
		/// Analyse la donnée définie par l'interface IDataRecord et trouve le type
		/// brut correspondant.
		/// </summary>
		/// <param name="record">interface d'accès à la donnée à analyser</param>
		/// <returns>type brut correspondant</returns>
		DbRawType AnalyseRecord(System.Data.IDataRecord record);
		
		/// <summary>
		/// Analyse le type .NET et trouve le type brut correspondant.
		/// </summary>
		/// <param name="type">type .NET à analyser</param>
		/// <returns>type brut correspondant</returns>
		DbRawType AnalyseType(System.Type type);
		
		/// <summary>
		/// Analyse un objet et trouve le type brut correspondant.
		/// </summary>
		/// <param name="value">objet à analyser</param>
		/// <returns>type brut correspondant</returns>
		DbRawType AnalyseValue(object value);
		
		
		/// <summary>
		/// Transforme un objet tel qu'il a été fourni par le provider ADO.NET en
		/// un objet "normal". Ceci permet de gérer les cas particuliers où le
		/// provider ADO.NET ne fournirait pas les types de base correspondant à
		/// l'énumération définie par DbRawType. En règle générale, cet appel
		/// retourne simplement l'objet inchangé.
		/// </summary>
		/// <param name="value">objet créé par le provider ADO.NET</param>
		/// <returns>objet "normal"</returns>
		object ConvertToPublic(object value);
		
		/// <summary>
		/// Transforme un objet en une représentation compréhensible par le
		/// provider ADO.NET. C'est la réciproque de la méthode ConvertToPublic.
		/// </summary>
		/// <param name="value">objet "normal"</param>
		/// <returns>objet reconnu par le provider ADO.NET</returns>
		object ConvertFromPublic(object value);
	}
}
