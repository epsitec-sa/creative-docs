namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// L'interface ITypeConverter est utilis�e pour convertir des types entre
	/// une repr�sentation brute publique et une repr�sentation interne propre au
	/// provider ADO.NET, et vice versa.
	/// </summary>
	public interface ITypeConverter
	{
		/// <summary>
		/// Analyse la donn�e d�finie par l'interface IDataRecord et trouve le type
		/// brut correspondant.
		/// </summary>
		/// <param name="record">interface d'acc�s � la donn�e � analyser</param>
		/// <returns>type brut correspondant</returns>
		DbRawType AnalyseRecord(System.Data.IDataRecord record);
		
		/// <summary>
		/// Analyse le type .NET et trouve le type brut correspondant.
		/// </summary>
		/// <param name="type">type .NET � analyser</param>
		/// <returns>type brut correspondant</returns>
		DbRawType AnalyseType(System.Type type);
		
		/// <summary>
		/// Analyse un objet et trouve le type brut correspondant.
		/// </summary>
		/// <param name="value">objet � analyser</param>
		/// <returns>type brut correspondant</returns>
		DbRawType AnalyseValue(object value);
		
		
		/// <summary>
		/// Transforme un objet tel qu'il a �t� fourni par le provider ADO.NET en
		/// un objet "normal". Ceci permet de g�rer les cas particuliers o� le
		/// provider ADO.NET ne fournirait pas les types de base correspondant �
		/// l'�num�ration d�finie par DbRawType. En r�gle g�n�rale, cet appel
		/// retourne simplement l'objet inchang�.
		/// </summary>
		/// <param name="value">objet cr�� par le provider ADO.NET</param>
		/// <returns>objet "normal"</returns>
		object ConvertToPublic(object value);
		
		/// <summary>
		/// Transforme un objet en une repr�sentation compr�hensible par le
		/// provider ADO.NET. C'est la r�ciproque de la m�thode ConvertToPublic.
		/// </summary>
		/// <param name="value">objet "normal"</param>
		/// <returns>objet reconnu par le provider ADO.NET</returns>
		object ConvertFromPublic(object value);
	}
}
