//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003

namespace Epsitec.Cresus.Database
{
	public enum Nullable
	{
		Undefined,
		No,
		Yes
	}
	
	/// <summary>
	/// La classe SqlColumn décrit une colonne dans une table de la base de données.
	/// Cette classe ressemble fortement à System.Data.DataColumn.
	/// </summary>
	public class SqlColumn
	{
		public SqlColumn()
		{
		}
		
		public SqlColumn(string name)
		{
			this.Name = name;
		}
		
		public SqlColumn(string name, DbRawType raw_type)
		{
			this.Name = name;
			this.SetType (raw_type);
		}
		
		public SqlColumn(string name, DbRawType raw_type, int length, bool is_fixed_length)
		{
			this.Name = name;
			this.SetType (raw_type, length, is_fixed_length);
		}
		
		public SqlColumn(string name, DbRawType raw_type, Nullable nullable)
		{
			this.Name = name;
			this.SetType (raw_type);
			this.IsNullAllowed = (nullable == Nullable.Yes);
		}
		
		public SqlColumn(string name, DbRawType raw_type, int length, bool is_fixed_length, Nullable nullable)
		{
			this.Name = name;
			this.SetType (raw_type, length, is_fixed_length);
			this.IsNullAllowed = (nullable == Nullable.Yes);
		}
		
		
		public string							Name
		{
			get { return this.name; }
			set { this.name = value; }
		}
		
		public DbRawType						Type
		{
			get { return this.type; }
		}
		
		public int								Length
		{
			get { return this.length; }
		}
		
		public bool								IsFixedLength
		{
			get { return this.is_fixed_length; }
		}
		
		
		public bool								HasRawConverter
		{
			get { return this.raw_converter != null; }
		}
		
		public IRawTypeConverter				RawConverter
		{
			get { return this.raw_converter; }
		}
		
		
		public bool								IsNullAllowed
		{
			get { return this.is_null_allowed; }
			set { this.is_null_allowed = value; }
		}
		
		public bool								IsUnique
		{
			get { return this.is_unique; }
			set { this.is_unique = value; }
		}
		
		public bool								IsIndexed
		{
			get { return this.is_indexed; }
			set { this.is_indexed = value; }
		}
		
		
		public bool Validate(ISqlValidator validator)
		{
			return validator.ValidateName (this.name);
		}
		
		
		public void SetRawConverter(IRawTypeConverter raw_converter)
		{
			this.raw_converter = raw_converter;
			this.SetType (raw_converter.InternalType, raw_converter.Length, raw_converter.IsFixedLength);
		}
		
		public void SetType(DbRawType type)
		{
			this.SetType (type, 1, true);
		}
		
		public void SetType(DbRawType type, int length, bool is_fixed_length)
		{
			if (length < 1)
			{
				throw new System.ArgumentOutOfRangeException ("Invalid length");
			}
			
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
		
		
		protected string						name				= null;
		protected DbRawType						type				= DbRawType.Null;
		protected bool							is_null_allowed		= false;
		protected bool							is_unique			= false;
		protected bool							is_indexed			= false;
		protected bool							is_fixed_length		= true;
		protected int							length				= 1;
		protected IRawTypeConverter				raw_converter		= null;
	}
}
