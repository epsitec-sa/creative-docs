//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

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
		/// V�rifie si le type brut sp�cifi� est support� par la base de donn�es
		/// sous-jacente. Le type <c>DbRawType.Guid</c> n'est par exemple pas support�
		/// par Firebird.
		/// </summary>
		/// <param name="type">type brut � analyser</param>
		/// <returns><c>true</c> si le type est support� de mani�re native</returns>
		bool CheckNativeSupport(DbRawType type);
		
		/// <summary>
		/// Trouve le convertisseur de types correspondant.
		/// </summary>
		/// <param name="type">type brut � analyser</param>
		/// <param name="converter">convertisseur (ou null si le type est support�
		/// nativement par la base</param>
		/// <returns>true si un convertisseur existe</returns>
		bool GetRawTypeConverter(DbRawType type, out IRawTypeConverter converter);
		
		/// <summary>
		/// Transforme un objet tel qu'il a �t� fourni par le provider ADO.NET en
		/// un objet simplifi�. De mani�re interne, cette m�thode appelle � son
		/// tour TypeConverter.ConvertToSimpleType pour g�rer tous les cas de
		/// conversion normaux.
		/// En aucun cas, il faut appeler cette m�thode pour des types non support�s par
		/// la base de donn�es (v�rifier avec CheckNativeSupport).
		/// </summary>
		/// <param name="value">objet brut fourni par ADO.NET</param>
		/// <param name="simple_type">type simplifi� attendu</param>
		/// <param name="num_def">format num�rique attendu</param>
		/// <returns>objet converti</returns>
		object ConvertToSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def);
		
		/// <summary>
		/// Transforme un objet simplifi� en une repr�sentation compr�hensible par
		/// le provider ADO.NET. C'est la r�ciproque de la m�thode ConvertToSimpleType.
		/// De mani�re interne, cette m�thode appelle TypeConverter.ConvertFromSimpleType
		/// pour g�rer les cas de conversion normaux.
		/// En aucun cas, il faut appeler cette m�thode pour des types non support�s par
		/// la base de donn�es (v�rifier avec CheckNativeSupport).
		/// </summary>
		/// <param name="value">objet de type simplifi�</param>
		/// <param name="simple_type">type de l'objet</param>
		/// <param name="num_def">format num�rique de l'objet</param>
		/// <returns>objet converti</returns>
		object ConvertFromSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def);
	}
}
