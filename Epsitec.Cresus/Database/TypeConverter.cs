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
		
		
		public static DbRawType AnalyseRawType(System.Type type)
		{
			//	TODO: impl�menter
			//
			//	Il suffit de reconna�tre les types correspondant directement � ceux
			//	�num�r�s dans DbRawType. Les autres seront d�clar�s comme �tant
			//	non support�s.
			
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
		
		
		public static object ConvertToSimpleType(object raw_value)
		{
			if (raw_value == null)
			{
				return null;
			}
			
			DbRawType raw_type = TypeConverter.AnalyseRawType (raw_value.GetType ());
			
			System.Diagnostics.Debug.Assert (raw_type != DbRawType.Unsupported);
			System.Diagnostics.Debug.Assert (raw_type != DbRawType.Null);
			
			//	TODO: impl�menter
			//
			//	Il faut convertir en fonction de leur appartenance les divers types
			//	bruts en types simplifi�s (tout ce qui est nombre devient decimal,
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
			
			//	TODO: impl�menter
			//
			//	Convertit le type simple en un type brut. V�rifie au passage qu'il n'y a pas
			//	de perte de donn�es (si la source est un decimal=400.0M et la destination Int8,
			//	par exemple). En cas de perte de donn�es, lever l'exception InvalidCastException.
			//
			//	Id�e : Pour les nombres, utiliser new DbNumDef (raw_type), puis faire un appel �
			//	CheckCompatibility pour v�rifier que la valeur d�cimale est acceptable.
			
			return null;
		}
	}
}
