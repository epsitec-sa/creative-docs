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
		
		
		public static DbRawType AnalyseRawType(System.Type type)
		{
			//	TODO: implémenter
			//
			//	Il suffit de reconnaître les types correspondant directement à ceux
			//	énumérés dans DbRawType. Les autres seront déclarés comme étant
			//	non supportés.
			
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
		
		
		public static object ConvertToSimpleType(object raw_value)
		{
			if (raw_value == null)
			{
				return null;
			}
			
			DbRawType raw_type = TypeConverter.AnalyseRawType (raw_value.GetType ());
			
			System.Diagnostics.Debug.Assert (raw_type != DbRawType.Unsupported);
			System.Diagnostics.Debug.Assert (raw_type != DbRawType.Null);
			
			//	TODO: implémenter
			//
			//	Il faut convertir en fonction de leur appartenance les divers types
			//	bruts en types simplifiés (tout ce qui est nombre devient decimal,
			//	etc.)
			
			return null;
		}
		
		public static object ConvertToRawType(object simple_value, DbRawType raw_type)
		{
			if (simple_value == null)
			{
				return null;
			}
			
			DbSimpleType simple_type = TypeConverter.AnalyseSimpleType (simple_value.GetType ());
			
			System.Diagnostics.Debug.Assert (raw_type != DbRawType.Unsupported);
			System.Diagnostics.Debug.Assert (raw_type != DbRawType.Null);
			
			System.Diagnostics.Debug.Assert (TypeConverter.MapToSimpleType (raw_type) == simple_type);
			
			//	TODO: implémenter
			//
			//	Convertit le type simple en un type brut. Vérifie au passage qu'il n'y a pas
			//	de perte de données (si la source est un decimal=400.0M et la destination Int8,
			//	par exemple). En cas de perte de données, lever l'exception InvalidCastException.
			//
			//	Idée : Pour les nombres, utiliser new DbNumDef (raw_type), puis faire un appel à
			//	CheckCompatibility pour vérifier que la valeur décimale est acceptable.
			
			return null;
		}
	}
}
