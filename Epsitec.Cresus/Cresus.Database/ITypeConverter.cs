//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		/// Vérifie si le type brut spécifié est supporté par la base de données
		/// sous-jacente. Le type <c>DbRawType.Guid</c> n'est par exemple pas supporté
		/// par Firebird.
		/// </summary>
		/// <param name="type">type brut à analyser</param>
		/// <returns><c>true</c> si le type est supporté de manière native</returns>
		bool CheckNativeSupport(DbRawType type);
		
		/// <summary>
		/// Trouve le convertisseur de types correspondant.
		/// </summary>
		/// <param name="type">type brut à analyser</param>
		/// <param name="converter">convertisseur (ou null si le type est supporté
		/// nativement par la base</param>
		/// <returns>true si un convertisseur existe</returns>
		bool GetRawTypeConverter(DbRawType type, out IRawTypeConverter converter);
		
		/// <summary>
		/// Transforme un objet tel qu'il a été fourni par le provider ADO.NET en
		/// un objet simplifié. De manière interne, cette méthode appelle à son
		/// tour TypeConverter.ConvertToSimpleType pour gérer tous les cas de
		/// conversion normaux.
		/// En aucun cas, il faut appeler cette méthode pour des types non supportés par
		/// la base de données (vérifier avec CheckNativeSupport).
		/// </summary>
		/// <param name="value">objet brut fourni par ADO.NET</param>
		/// <param name="simple_type">type simplifié attendu</param>
		/// <param name="num_def">format numérique attendu</param>
		/// <returns>objet converti</returns>
		object ConvertToSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def);
		
		/// <summary>
		/// Transforme un objet simplifié en une représentation compréhensible par
		/// le provider ADO.NET. C'est la réciproque de la méthode ConvertToSimpleType.
		/// De manière interne, cette méthode appelle TypeConverter.ConvertFromSimpleType
		/// pour gérer les cas de conversion normaux.
		/// En aucun cas, il faut appeler cette méthode pour des types non supportés par
		/// la base de données (vérifier avec CheckNativeSupport).
		/// </summary>
		/// <param name="value">objet de type simplifié</param>
		/// <param name="simple_type">type de l'objet</param>
		/// <param name="num_def">format numérique de l'objet</param>
		/// <returns>objet converti</returns>
		object ConvertFromSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def);
	}
}
