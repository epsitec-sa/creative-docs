namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbColumn d�crit une colonne dans une table de la base de donn�es.
	/// Cette classe ressemble fortement � System.Data.DataColumn.
	/// </summary>
	public class DbColumn
	{
		public DbColumn()
		{
		}
		

		public string					Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public DbSimpleType				SimpleType
		{
			get { return this.simple_type; }
		}
		
		public DbNumDef					NumDef
		{
			get { return this.num_def; }
		}
		
		public int						Length
		{
			get { return this.length; }
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
		
		
		public SqlColumn CreateSqlColumn(ITypeConverter type_converter)
		{
			DbRawType raw_type = TypeConverter.MapToRawType (this.simple_type, this.num_def);
			SqlColumn column   = null;
			
			IRawTypeConverter raw_converter;
			
			if (type_converter.CheckNativeSupport (raw_type))
			{
				column = new SqlColumn ();
				column.SetType (raw_type, this.Length, this.IsFixedLength);
			}
			else if (type_converter.GetRawTypeConverter (raw_type, out raw_converter))
			{
				column = new SqlColumn ();
				column.SetRawConverter (raw_converter);
			}
			else
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Conversion of DbColumn to SqlColumn not possible for {0}.", this.Name));
			}
			
			return column;
		}
		
		public void SetTypeAndLength(DbSimpleType type, int length, bool is_fixed_length)
		{
			System.Diagnostics.Debug.Assert (length > 0);
			
			switch (type)
			{
				case DbSimpleType.String:
				case DbSimpleType.ByteArray:
					//	Ce sont les seuls types qui acceptent des donn�es de longueur autres
					//	que '1'...
					break;
				
				default:
					if ((length != 1) ||
						(is_fixed_length != true))
					{
						throw new System.ArgumentOutOfRangeException ("Length and Type mismatch");
					}
					break;
			}
			
			this.simple_type = type;
			this.length = length;
			this.is_fixed_length = is_fixed_length;
		}
		
		
		protected string				name				= null;
		protected DbSimpleType			simple_type			= DbSimpleType.Null;
		protected DbNumDef				num_def				= null;
		protected bool					is_null_allowed		= false;
		protected bool					is_unique			= false;
		protected bool					is_indexed			= false;
		protected bool					is_fixed_length		= true;
		protected int					length				= 1;
	}
}
