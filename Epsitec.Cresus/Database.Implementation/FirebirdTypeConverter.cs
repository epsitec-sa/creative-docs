using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// La classe FirebirdTypeConverter implémente la conversion des types de
	/// bruts pour Firebird.
	/// </summary>
	public class FirebirdTypeConverter : ITypeConverter
	{
		public FirebirdTypeConverter()
		{
		}
		
		#region ITypeConverter Members

		public object ConvertFromSimpleType(object value, Epsitec.Cresus.Database.DbSimpleType simple_type, DbNumDef num_def)
		{
			return TypeConverter.ConvertFromSimpleType (value, simple_type, num_def);
		}

		public object ConvertToSimpleType(object value, Epsitec.Cresus.Database.DbSimpleType simple_type, DbNumDef num_def)
		{
			return TypeConverter.ConvertToSimpleType (value, simple_type, num_def);
		}

		public bool CheckNativeSupport(Epsitec.Cresus.Database.DbRawType type)
		{
			System.Diagnostics.Debug.Assert (type != DbRawType.Unsupported);
			System.Diagnostics.Debug.Assert (type != DbRawType.Null);
			
			switch (type)
			{
				case DbRawType.Boolean:
				case DbRawType.Int16:
				case DbRawType.Int32:
				case DbRawType.Int64:
				case DbRawType.SmallDecimal:
				case DbRawType.LargeDecimal:
				case DbRawType.String:
				case DbRawType.ByteArray:
					return true;
				
				case DbRawType.DateTime:
				case DbRawType.Guid:
					break;
			}
			
			return false;
		}

		#endregion
	}
}
