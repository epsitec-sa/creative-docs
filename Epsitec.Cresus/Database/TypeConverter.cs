namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe TypeConverter r�alise les conversions entre les donn�es de bas
	/// niveau (type d�crit par DbRawType) et les donn�es simplifi�es (type d�crit
	/// par DbSimpleType).
	/// </summary>
	public class TypeConverter
	{
		private TypeConverter()
		{
		}
		
		
		public static DbSimpleType MapToSimpleType(DbRawType raw_type)
		{
			DbNumDef num_def;
			return TypeConverter.MapToSimpleType (raw_type, out num_def);
		}
		
		public static DbSimpleType MapToSimpleType(DbRawType raw_type, out DbNumDef num_def)
		{
			//	TODO: impl�menter (par ex. DbRawType.Int8 --> DbSimpleType.Decimal, avec num_def
			//	d�fini...) Si ce n'est pas un nombre, retourner null � la place de num_def.
			
			num_def = null;
			
			return DbSimpleType.Unsupported;
		}
		
		public static DbRawType MapToRawType(DbSimpleType simple_type, DbNumDef num_def)
		{
			//	TODO: impl�menter la correspondance entre un type simple et son �quivalent
			//	brut. Les types num�riques sont d�crits par num_def, ce qui permet de
			//	choisir la taille (Int8, ...) en fonction de num_def.MinimumBits et
			//	des autres propri�t�s.
			
			return DbRawType.Unsupported;
		}
		
		
		public static DbSimpleType AnalyseSimpleType(System.Type type)
		{
			//	TODO: impl�menter
			//
			//	Il suffit de reconna�tre les types correspondant directement � ceux
			//	�num�r�s dans DbSimpleType. Les autres seront d�clar�s comme �tant
			//	non support�s.
			
			return DbSimpleType.Unsupported;
		}
		
		
		/// <summary>
		/// Convertit un objet en provenance de ADO.NET en sa repr�sentation �quivalente
		/// bas�e sur le type simplifi� <c>simple_type</c> et la d�finition de son format
		/// num�rique. Cette m�thode peut transformer une donn�e num�rique de mani�re
		/// significative.
		/// </summary>
		/// <param name="value">objet brut fourni par ADO.NET</param>
		/// <param name="simple_type">type simplifi� attendu</param>
		/// <param name="num_def">format num�rique attendu</param>
		/// <returns>objet converti</returns>
		public static object ConvertToSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Null);
			
			//	TODO: impl�menter
			//
			//	Il faut convertir en fonction de leur appartenance les divers types
			//	bruts en types simplifi�s (tout ce qui est nombre devient decimal,
			//	etc.)
			//
			//	L'appelant sp�cifie ce qu'il s'attend � trouver dans 'raw_value'.
			//	Si cela ne correspond pas, il faut lever une exception signalant
			//	que l'objet est incompatible avec le type.
			//
			//	Si c'est un type num�rique, il faut utiliser DbNumDef.ConversionNeeded
			//	pour voir s'il faut appeler ConvertFromInt64 sur les donn�es extraites
			//	de la base de donn�es.
			//
			//	NB: A ce niveau, on ne va jamais demander de faire de conversion
			//	impossible (par exemple Int32 -> Guid).
			
			return null;
		}
		
		/// <summary>
		/// Convertit un objet bas� sur le type simplifi� <c>simple_type</c> et la d�finition
		/// de son format num�rique en un objet compatible avec ADO.NET. Cette m�thode peut
		/// transformer une donn�e num�rique de mani�re siginificative.
		/// </summary>
		/// <param name="value">objet de type simplifi�</param>
		/// <param name="simple_type">type de l'objet</param>
		/// <param name="num_def">format num�rique de l'objet</param>
		/// <returns>objet converti</returns>
		public static object ConvertFromSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Null);
			
			//	TODO: impl�menter
			//
			//	Il faut convertir la donn�e en quelque chose de brut, compatible avec ADO.NET
			//	et la base de donn�es sous-jacente.
			//
			//	Pour un type num�rique d�fini par DbNumDef, on v�rifie si une conversion
			//	est requise et on appelle ConvertToInt64 si besoin.
			//
			//	NB: A ce niveau, on ne va jamais demander de conversion impossible, par
			//	exemple un type non support� par la base de donn�es sous-jacente. L'appelant
			//	s'assure avec ITypeConverter.CheckNativeSupport que le type brut est bien
			//	compatible avec la base de donn�es.
			
			return null;
		}
	}
}
