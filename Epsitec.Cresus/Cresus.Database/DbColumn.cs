//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
	/// <summary>
	/// La classe DbColumn décrit une colonne dans une table de la base de données.
	/// Cette classe ressemble dans l'esprit à System.Data.DataColumn.
	/// </summary>
	public class DbColumn : IDbAttributesHost
	{
		public DbColumn()
		{
		}
		
		public DbColumn(string name)
		{
			this.attributes[Tags.Name] = name;
		}
		
		public DbColumn(string name, DbSimpleType type) : this (name, type, 1, true, null, Nullable.Undefined)
		{
		}
		
		public DbColumn(string name, DbSimpleType type, Nullable nullable) : this (name, type, 1, true, null, nullable)
		{
		}
		
		public DbColumn(string name, DbSimpleType type, int length, bool is_fixed_length) : this (name, type, length, is_fixed_length, null, Nullable.Undefined)
		{
		}
		
		public DbColumn(string name, DbSimpleType type, int length, bool is_fixed_length, Nullable nullable) : this (name, type, length, is_fixed_length, null, nullable)
		{
		}
		
		public DbColumn(string name, DbNumDef num_def) : this (name, DbSimpleType.Decimal, 1, true, num_def, Nullable.Undefined)
		{
		}
		
		public DbColumn(string name, DbNumDef num_def, Nullable nullable) : this (name, DbSimpleType.Decimal, 1, true, num_def, nullable)
		{
		}
		
		public DbColumn(string name, DbSimpleType type, int length, bool is_fixed_length, DbNumDef num_def, Nullable nullable) : this (name)
		{
			this.SetTypeAndLength (type, length, is_fixed_length, num_def);
			this.IsNullAllowed = (nullable == Nullable.Yes);
		}
		
		public DbColumn(string name, DbType type) : this (name, type, Nullable.Undefined)
		{
		}
		
		public DbColumn(string name, DbType type, Nullable nullable) : this (name)
		{
			this.type = type;
			this.IsNullAllowed = (nullable == Nullable.Yes);
		}
		
		public DbColumn(System.Xml.XmlElement xml)
		{
			this.ProcessXmlDefinition (xml);
		}
		
		
		public static DbColumn NewColumn(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml (xml);
			
			return DbColumn.NewColumn (doc.DocumentElement);
		}
		
		public static DbColumn NewColumn(System.Xml.XmlElement xml)
		{
			return new DbColumn (xml);
		}

		
		public static string ConvertColumnToXml(DbColumn column)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			DbColumn.ConvertColumnToXml (buffer, column);
			
			return buffer.ToString ();
		}
		
		public static void ConvertColumnToXml(System.Text.StringBuilder buffer, DbColumn column)
		{
			column.SerialiseXmlDefinition (buffer);
		}
		
		
		public void DefineCategory(DbElementCat category)
		{
			if (this.category == category)
			{
				return;
			}
			
			if (this.category != DbElementCat.Unknown)
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot change its category.", this.Name));
			}
			
			this.category = category;
		}
		
		public void DefineAttributes(params string[] attributes)
		{
			this.attributes.SetFromInitialisationList (attributes);
		}
		
		
		protected void SerialiseXmlDefinition(System.Text.StringBuilder buffer)
		{
			buffer.Append (@"<col");
			
			if (this.IsNullAllowed)
			{
				buffer.Append (@" null=""1""");
			}
			
			if (this.IsUnique)
			{
				buffer.Append (@" unique=""1""");
			}
			
			if (this.IsIndexed)
			{
				buffer.Append (@" index=""1""");
			}
			
			string arg_cat = DbTools.ElementCategoryToString (this.category);
			
			if (arg_cat != null)
			{
				buffer.Append (@" cat=""");
				buffer.Append (arg_cat);
				buffer.Append (@"""");
			}
			
			buffer.Append (@"/>");
		}
		
		protected void ProcessXmlDefinition(System.Xml.XmlElement xml)
		{
			if (xml.Name != "col")
			{
				throw new System.ArgumentException (string.Format ("Expected root element named <col>, but found <{0}>.", xml.Name));
			}
			
			string arg_null   = xml.GetAttribute ("null");
			string arg_unique = xml.GetAttribute ("unique");
			string arg_index  = xml.GetAttribute ("index");
			string arg_cat    = xml.GetAttribute ("cat");
			
			this.is_null_allowed = (arg_null == "1");
			this.is_unique       = (arg_unique == "1");
			this.is_indexed      = (arg_index == "1");
			this.category        = DbTools.ParseElementCategory (arg_cat);
		}
		
		#region IDbAttributesHost Members
		public DbAttributes				Attributes
		{
			get
			{
				return this.attributes;
			}
		}
		#endregion
		
		public string					Name
		{
			get { return this.Attributes[Tags.Name, ResourceLevel.Default]; }
		}
		
		public string					Caption
		{
			get { return this.Attributes[Tags.Caption]; }
		}
		
		public string					Description
		{
			get { return this.Attributes[Tags.Description]; }
		}
		
		public DbType					Type
		{
			get { return this.type; }
		}
		public DbSimpleType				SimpleType
		{
			get { return this.type.SimpleType; }
		}
		
		public DbNumDef					NumDef
		{
			get
			{
				if (this.type is DbTypeNum)
				{
					DbTypeNum type = this.type as DbTypeNum;
					return type.NumDef;
				}
				
				return null;
			}
		}
		
		public int						Length
		{
			get
			{
				if (this.type is DbTypeString)
				{
					DbTypeString type = this.type as DbTypeString;
					return type.Length;
				}
				if (this.Type is DbTypeEnum)
				{
					DbTypeEnum type = this.Type as DbTypeEnum;
					return type.MaxNameLength;
				}
				
				return 1;
			}
		}
		
		public DbElementCat				Category
		{
			get { return this.category; }
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
			get
			{
				if (this.type is DbTypeString)
				{
					DbTypeString type = this.type as DbTypeString;
					return type.IsFixedLength;
				}
				
				return true;
			}
		}
		
		
		public SqlColumn CreateSqlColumn(ITypeConverter type_converter)
		{
			DbRawType raw_type = TypeConverter.MapToRawType (this.SimpleType, this.NumDef);
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
			
			if (column != null)
			{
				column.Name          = this.Name;
				column.IsNullAllowed = this.IsNullAllowed;
				column.IsUnique      = this.IsUnique;
				column.IsIndexed     = this.IsIndexed;
			}
			
			return column;
		}
		
		
		internal void SetType(DbSimpleType type)
		{
			this.SetTypeAndLength (type, 1, true, null);
		}
		
		internal void SetType(DbSimpleType type, DbNumDef num_def)
		{
			this.SetTypeAndLength (type, 1, true, num_def);
		}
		
		internal void SetTypeAndLength(DbSimpleType type, int length, bool is_fixed_length)
		{
			this.SetTypeAndLength (type, length, is_fixed_length, null);
		}
		
		internal void SetTypeAndLength(DbSimpleType type, int length, bool is_fixed_length, DbNumDef num_def)
		{
			if (this.type != null)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type of column.");
			}
			
			if (length < 1)
			{
				throw new System.ArgumentOutOfRangeException ("length", length, "Invalid length");
			}
			
			switch (type)
			{
				case DbSimpleType.String:
				case DbSimpleType.ByteArray:
					//	Ce sont les seuls types qui acceptent des données de longueur autres
					//	que '1'...
					break;
				
				default:
					if (length != 1)
					{
						throw new System.ArgumentOutOfRangeException ("length", length, "Length must be 1.");
					}
					if (is_fixed_length != true)
					{
						throw new System.ArgumentOutOfRangeException ("is_fixed_length", is_fixed_length, "Type must be of fixed length.");
					}
					break;
			}
			
			switch (type)
			{
				case DbSimpleType.String:
					this.type = new DbTypeString (length, is_fixed_length);
					break;
				
				case DbSimpleType.ByteArray:
					throw new System.NotImplementedException ("ByteArray not implemented yet");
					
				case DbSimpleType.Decimal:
					this.type = new DbTypeNum (num_def);
					break;
				
				default:
					this.type = new DbType (type);
					break;
			}
		}
		
		internal void SetType(DbType type)
		{
			if (this.type != null)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type of column.");
			}
			
			this.type = type;
		}
		
		
		#region Equals and GetHashCode support
		public override bool Equals(object obj)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des colonnes, pas sur les
			//	détails internes...
			
			DbColumn that = obj as DbColumn;
			
			if (that == null)
			{
				return false;
			}
			
			return (this.Name == that.Name);
		}
		
		public override int GetHashCode()
		{
			string name = this.Name;
			return (name == null) ? 0 : name.GetHashCode ();
		}

		#endregion
		
		protected DbAttributes			attributes = new DbAttributes ();
		protected DbType				type;
		protected bool					is_null_allowed		= false;
		protected bool					is_unique			= false;
		protected bool					is_indexed			= false;
		protected DbElementCat			category;
		
		
		internal const string			TagId				= "CR_ID";
		internal const string			TagRevision			= "CR_REV";
		internal const string			TagStatus			= "CR_STAT";
	}
}
