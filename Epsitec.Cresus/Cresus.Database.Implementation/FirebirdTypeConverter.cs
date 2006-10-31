//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database.Implementation
{
	using FirebirdSql.Data.FirebirdClient;
	
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
				case DbRawType.Int16:
				case DbRawType.Int32:
				case DbRawType.Int64:
				case DbRawType.SmallDecimal:
				case DbRawType.LargeDecimal:
				case DbRawType.String:
				case DbRawType.ByteArray:
				case DbRawType.Date:
				case DbRawType.Time:
				case DbRawType.DateTime:
					return true;
				
				case DbRawType.Boolean:
				case DbRawType.Guid:
					break;
			}
			
			return false;
		}
		
		public bool GetRawTypeConverter(DbRawType type, out IRawTypeConverter converter)
		{
			switch (type)
			{
				case DbRawType.Boolean:	converter = new BooleanConverter ();	return true;
				case DbRawType.Guid:	converter = new GuidConverter ();		return true;
			}
			
			converter = null;
			return false;
		}
		
		#endregion

		protected class BooleanConverter : IRawTypeConverter
		{
			public BooleanConverter()
			{
			}
			
			
			#region IRawTypeConverter Members
			
			public DbRawType					ExternalType
			{
				get { return DbRawType.Boolean; }
			}
			
			public DbRawType					InternalType
			{
				get { return DbRawType.Int16; }
			}

			public int							Length
			{
				get { return 1; }
			}
			
			public bool							IsFixedLength
			{
				get { return true; }
			}
			
			public object ConvertFromInternalType(object value)
			{
				if (value == null)
				{
					return null;
				}
				
				//	Un booléen est représenté au moyen d'une valeur 0 ou 1 dans
				//	Firebird.
				
				return ((short) value) != 0;
			}

			public object ConvertToInternalType(object value)
			{
				if (value == null)
				{
					return null;
				}
				
				//	Un booléen est représenté au moyen d'une valeur 0 ou 1 dans
				//	Firebird. On génère un Int16 en sortie, c'est le mieux que
				//	l'on puisse faire.
				
				bool  test = (bool) value;
				short i16  = test ? (short)1 : (short)0;
				
				return i16;
			}

			#endregion
		}
		
		protected class GuidConverter : IRawTypeConverter
		{
			public GuidConverter()
			{
			}
			
			
			#region IRawTypeConverter Members

			public DbRawType					ExternalType
			{
				get { return DbRawType.Guid; }
			}
			
			public DbRawType					InternalType
			{
				get { return DbRawType.String; }
			}
			
			public int							Length
			{
				get { return 32; }
			}
			
			public bool							IsFixedLength
			{
				get { return true; }
			}
			
			
			public object ConvertFromInternalType(object value)
			{
				if (value == null)
				{
					return null;
				}
				
				System.Diagnostics.Debug.Assert (value is string);
				
				string      text = (string) value;
				System.Guid guid;
				
				text = text.Trim ();
				
				System.Diagnostics.Debug.Assert (text.Length == 32);
				
				guid = new System.Guid (text);
				
				return guid;
			}

			public object ConvertToInternalType(object value)
			{
				if (value == null)
				{
					return null;
				}
				
				System.Diagnostics.Debug.Assert (value is System.Guid);
				
				System.Guid guid = (System.Guid) value;
				string      text = guid.ToString ("N");
				
				System.Diagnostics.Debug.Assert (text.Length == 32);
				
				return text;
			}

			#endregion
		}
	}
}
