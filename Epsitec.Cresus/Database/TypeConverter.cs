namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe TypeConverter réalise les conversions entre les données de bas
	/// niveau (type décrit par DbRawType) et les données simplifiées (type décrit
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
			//	TODO: implémenter (par ex. DbRawType.Int8 --> DbSimpleType.Decimal, avec num_def
			//	défini...) Si ce n'est pas un nombre, retourner null à la place de num_def.
			
			num_def = null;
			
			return DbSimpleType.Unsupported;
		}
		
		public static DbRawType MapToRawType(DbSimpleType simple_type, DbNumDef num_def)
		{
			//	TODO: implémenter la correspondance entre un type simple et son équivalent
			//	brut. Les types numériques sont décrits par num_def, ce qui permet de
			//	choisir la taille (Int8, ...) en fonction de num_def.MinimumBits et
			//	des autres propriétés.
			
			return DbRawType.Unsupported;
		}
		
		
		public static DbSimpleType AnalyseSimpleType(System.Type type)
		{
			//	TODO: implémenter
			//
			//	Il suffit de reconnaître les types correspondant directement à ceux
			//	énumérés dans DbSimpleType. Les autres seront déclarés comme étant
			//	non supportés.
			
			return DbSimpleType.Unsupported;
		}
		
		
		/// <summary>
		/// Convertit un objet en provenance de ADO.NET en sa représentation équivalente
		/// basée sur le type simplifié <c>simple_type</c> et la définition de son format
		/// numérique. Cette méthode peut transformer une donnée numérique de manière
		/// significative.
		/// </summary>
		/// <param name="value">objet brut fourni par ADO.NET</param>
		/// <param name="simple_type">type simplifié attendu</param>
		/// <param name="num_def">format numérique attendu</param>
		/// <returns>objet converti</returns>
		public static object ConvertToSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Null);
			
			//	TODO: implémenter
			//
			//	Il faut convertir en fonction de leur appartenance les divers types
			//	bruts en types simplifiés (tout ce qui est nombre devient decimal,
			//	etc.)
			//
			//	L'appelant spécifie ce qu'il s'attend à trouver dans 'raw_value'.
			//	Si cela ne correspond pas, il faut lever une exception signalant
			//	que l'objet est incompatible avec le type.
			//
			//	Si c'est un type numérique, il faut utiliser DbNumDef.ConversionNeeded
			//	pour voir s'il faut appeler ConvertFromInt64 sur les données extraites
			//	de la base de données.
			//
			//	NB: A ce niveau, on ne va jamais demander de faire de conversion
			//	impossible (par exemple Int32 -> Guid).
			
			return null;
		}
		
		/// <summary>
		/// Convertit un objet basé sur le type simplifié <c>simple_type</c> et la définition
		/// de son format numérique en un objet compatible avec ADO.NET. Cette méthode peut
		/// transformer une donnée numérique de manière siginificative.
		/// </summary>
		/// <param name="value">objet de type simplifié</param>
		/// <param name="simple_type">type de l'objet</param>
		/// <param name="num_def">format numérique de l'objet</param>
		/// <returns>objet converti</returns>
		public static object ConvertFromSimpleType(object value, DbSimpleType simple_type, DbNumDef num_def)
		{
			if (value == null)
			{
				return null;
			}
			
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Unsupported);
			System.Diagnostics.Debug.Assert (simple_type != DbSimpleType.Null);
			
			//	TODO: implémenter
			//
			//	Il faut convertir la donnée en quelque chose de brut, compatible avec ADO.NET
			//	et la base de données sous-jacente.
			//
			//	Pour un type numérique défini par DbNumDef, on vérifie si une conversion
			//	est requise et on appelle ConvertToInt64 si besoin.
			//
			//	NB: A ce niveau, on ne va jamais demander de conversion impossible, par
			//	exemple un type non supporté par la base de données sous-jacente. L'appelant
			//	s'assure avec ITypeConverter.CheckNativeSupport que le type brut est bien
			//	compatible avec la base de données.
			
			return null;
		}
	}
}
