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
			set { this.type = value; }
		}
		
		public bool						AllowNull
		{
			get { return this.is_null_allowed; }
			set { this.is_null_allowed = value; }
		}
		
		public bool						IsUnique
		{
			get { return this.is_unique; }
			set { this.is_unique = value; }
		}

		public bool						IsForeignKey
		{
			get { return this.is_foreign_key; }
			set { this.is_foreign_key = value; }
		}
		
		public int						Length
		{
			get { return this.length; }
		}
		
		public bool						IsFixedLength
		{
			get { return this.is_fixed_length; }
		}
		
		
		public void SetTypeAndLength(DbRawType type, int length, bool is_fixed_length)
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
						throw new System.ArgumentOutOfRangeException ("Length and Type mismatch");
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
		protected bool					is_foreign_key		= false;
		protected bool					is_fixed_length		= true;
		protected int					length				= 1;
	}
}
