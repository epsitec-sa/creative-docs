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
			//	TODO: impl�menter (par ex. DbRawType.Int8 --> DbSimpleType.Decimal)
			
			return DbSimpleType.Unsupported;
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
			//	bruts en types simplifi�s (tout ce qui est nombre devient Decimal,
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
			//	de perte de donn�es (si la source est un Decimal=400 et la destination Int8,
			//	par exemple). En cas de perte de donn�es, lever l'exception InvalidCastException.
			
			return null;
		}
	}
}
