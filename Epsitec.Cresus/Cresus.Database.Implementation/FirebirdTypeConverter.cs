//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.FirebirdClient;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdTypeConverter</c> class implements raw type conversions
	/// for the Firebird engine.
	/// </summary>
	internal sealed class FirebirdTypeConverter : ITypeConverter
	{
		public FirebirdTypeConverter()
		{
		}
		
		#region ITypeConverter Members

		public object ConvertFromSimpleType(object value, Epsitec.Cresus.Database.DbSimpleType simpleType, DbNumDef numDef)
		{
			return TypeConverter.ConvertFromSimpleType (value, simpleType, numDef);
		}

		public object ConvertToSimpleType(object value, Epsitec.Cresus.Database.DbSimpleType simpleType, DbNumDef numDef)
		{
			return TypeConverter.ConvertToSimpleType (value, simpleType, numDef);
		}

		public bool CheckNativeSupport(Epsitec.Cresus.Database.DbRawType type)
		{
			System.Diagnostics.Debug.Assert (type != DbRawType.Unknown);
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
					return false;

				default:
					throw new System.NotSupportedException (string.Format ("Unsupported DbRawType.{0}", type));
			}
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

		private class BooleanConverter : IRawTypeConverter
		{
			public BooleanConverter()
			{
			}
			
			
			#region IRawTypeConverter Members

			public DbRawType					ExternalType
			{
				get
				{
					return DbRawType.Boolean;
				}
			}

			public DbRawType					InternalType
			{
				get
				{
					return DbRawType.Int16;
				}
			}

			public int							Length
			{
				get
				{
					return 1;
				}
			}

			public bool							IsFixedLength
			{
				get
				{
					return true;
				}
			}

			public DbCharacterEncoding			Encoding
			{
				get
				{
					return DbCharacterEncoding.Unicode;
				}
			}
			
			public object ConvertFromInternalType(object value)
			{
				if (value == null)
				{
					return null;
				}
				
				//	Un booléen est représenté au moyen d'une valeur 0 ou 1 dans
				//	Firebird.

				System.Diagnostics.Debug.Assert (value.GetType () == typeof (short));
				
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

				System.Diagnostics.Debug.Assert (value.GetType () == typeof (bool));
				
				bool  test = (bool) value;
				short i16  = (short) (test ? 1 : 0);
				
				return i16;
			}

			#endregion
		}

		private class GuidConverter : IRawTypeConverter
		{
			public GuidConverter()
			{
			}
			
			
			#region IRawTypeConverter Members

			public DbRawType					ExternalType
			{
				get
				{
					return DbRawType.Guid;
				}
			}

			public DbRawType					InternalType
			{
				get
				{
					return DbRawType.String;
				}
			}

			public int							Length
			{
				get
				{
					return 32;
				}
			}

			public bool							IsFixedLength
			{
				get
				{
					return true;
				}
			}

			public DbCharacterEncoding Encoding
			{
				get
				{
					return DbCharacterEncoding.Ascii;
				}
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
