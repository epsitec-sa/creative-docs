namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlColumn décrit une colonne dans une table de la base de données.
	/// Cette classe ressemble fortement à System.Data.DataColumn.
	/// </summary>
	public class SqlColumn
	{
		public SqlColumn()
		{
		}
		

		public string					Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public DbRawType				Type
		{
			get { return this.type; }
		}
		
		public int						Length
		{
			get { return this.length; }
		}
		
		public IRawTypeConverter		RawConverter
		{
			get { return this.raw_converter; }
		}
		
		
		public bool						IsNullAllowed
		{
			get { return this.is_null_allowed; }
			set { this.is_null_allowed = value; }
		}
		
		public bool						IsUnique
		{
			get { return this.is_unique; }
			set { this.is_unique = value; }
		}
		
		public bool						IsIndexed
		{
			get { return this.is_indexed; }
			set { this.is_indexed = value; }
		}
		
		public bool						IsFixedLength
		{
			get { return this.is_fixed_length; }
		}
		
		public bool						HasRawConverter
		{
			get { return this.raw_converter != null; }
		}
		
		
		public void SetRawConverter(IRawTypeConverter raw_converter)
		{
			this.raw_converter = raw_converter;
			this.SetType (raw_converter.MatchingType, raw_converter.Length, true);
		}
		
		public void SetType(DbRawType type)
		{
			this.SetType (type, 1, true);
		}
		
		public void SetType(DbRawType type, int length, bool is_fixed_length)
		{
			System.Diagnostics.Debug.Assert (length > 0);
			
			switch (type)
			{
				case DbRawType.String:
				case DbRawType.ByteArray:
					//	Ce sont les seuls types qui acceptent des données de longueur autres
					//	que '1'...
					break;
				
				default:
					if ((length != 1) ||
						(is_fixed_length != true))
					{
						throw new System.ArgumentOutOfRangeException ("Length/Type mismatch");
					}
					break;
			}
			
			this.type = type;
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		
		protected string				name				= null;
		protected DbRawType				type				= DbRawType.Null;
		protected bool					is_null_allowed		= false;
		protected bool					is_unique			= false;
		protected bool					is_indexed			= false;
		protected bool					is_fixed_length		= true;
		protected int					length				= 1;
		protected IRawTypeConverter		raw_converter		= null;
	}
}
