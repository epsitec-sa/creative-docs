//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	using Converter = Epsitec.Common.Support.Data.Converter;
	
	public enum DbColumnLocalisation : byte
	{
		None			= 0,	//	n'est pas/ne peut pas être localisée
		Default			= 1,	//	peut être localisée, elle contient les valeurs par défaut
		Localised		= 2		//	est localisée pour une langue particulière
	}
	
	public enum DbColumnClass : byte
	{
		Data			= 0,	//	contient des données
		KeyId			= 1,	//	définit une clef (ID)
		KeyRevision		= 2,	//	définit une clef (révision)
		Ref				= 3,	//	définit une référence à une clef (ID, révision = 0)
		RefId			= 4,	//	définit une référence à une clef (ID)
		RefRevision		= 5		//	définit une référence à une clef (révision)
	}
	
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
		
		
		internal void DefineInternalKey(DbKey key)
		{
			if (this.internal_column_key == key)
			{
				return;
			}
			
			if (this.internal_column_key != null)
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its internal key.", this.Name));
			}
			
			this.internal_column_key = key.Clone () as DbKey;
		}
		
		internal void DefineColumnLocalisation(DbColumnLocalisation column_localisation)
		{
			this.column_localisation = column_localisation;
		}
		
		internal void DefineColumnClass(DbColumnClass column_class)
		{
			this.column_class = column_class;
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
			
			if (this.column_class != DbColumnClass.Data)
			{
				buffer.Append (@" class=""");
				buffer.Append (Converter.ToString ((int) this.column_class));
				buffer.Append (@"""");
			}
			
			if (this.column_localisation != DbColumnLocalisation.None)
			{
				int num = (int) this.column_localisation;
				buffer.Append (@" local=""");
				buffer.Append (Converter.ToString ((int) this.column_localisation));
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
			
			int column_class_code;
			int column_localisation_code;
			
			Converter.Convert (xml.GetAttribute ("class"), out column_class_code);
			Converter.Convert (xml.GetAttribute ("local"), out column_localisation_code);
			
			this.column_class        = (DbColumnClass) column_class_code;
			this.column_localisation = (DbColumnLocalisation) column_localisation_code;
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
		
		public DbKey					InternalKey
		{
			get { return this.internal_column_key; }
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
		
		
		public DbColumnLocalisation		ColumnLocalisation
		{
			get { return this.column_localisation; }
		}
		
		public DbColumnClass			ColumnClass
		{
			get { return this.column_class; }
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
				column.Name          = this.CreateSqlName ();
				column.IsNullAllowed = this.IsNullAllowed;
				column.IsUnique      = this.IsUnique;
				column.IsIndexed     = this.IsIndexed;
			}
			
			return column;
		}
		
		public string CreateSqlName()
		{
			return DbSqlStandard.CreateSimpleSqlName (this.Name);
		}
		
		
		public SqlField CreateSqlField(ITypeConverter type_converter, int value)
		{
			//	TODO: implémenter pour de vrai
			SqlField field = SqlField.CreateConstant (value, DbRawType.Int32);
			field.Alias = this.Name;
			return field;
		}
		
		public SqlField CreateSqlField(ITypeConverter type_converter, long value)
		{
			//	TODO: implémenter pour de vrai
			SqlField field = SqlField.CreateConstant (value, DbRawType.Int64);
			field.Alias = this.Name;
			return field;
		}
		
		public SqlField CreateSqlField(ITypeConverter type_converter, string value)
		{
			//	TODO: implémenter pour de vrai
			SqlField field = SqlField.CreateConstant (value, DbRawType.String);
			field.Alias = this.Name;
			return field;
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
		protected DbKey					internal_column_key;
		
		protected DbColumnLocalisation	column_localisation;
		protected DbColumnClass			column_class;
		
		
		internal const string			TagId				= "CR_ID";
		internal const string			TagRevision			= "CR_REV";
		internal const string			TagStatus			= "CR_STAT";
		internal const string			TagName				= "CR_NAME";
		internal const string			TagCaption			= "CR_CAPTION";
		internal const string			TagDescription		= "CR_DESCRIPTION";
		internal const string			TagInfoXml			= "CR_INFO";
		internal const string			TagNextId			= "CR_NEXT_ID";
		
		internal const string			TagRefTable			= "CREF_TABLE";
		internal const string			TagRefType			= "CREF_TYPE";
		internal const string			TagRefColumn		= "CREF_COLUMN";
		internal const string			TagRefSource		= "CREF_SOURCE";
		internal const string			TagRefTarget		= "CREF_TARGET";
		
		internal const int				MaxNameLength		= 40;
		internal const int				MaxCaptionLength	= 100;
		internal const int				MaxDescriptionLength= 500;
		internal const int				MaxInfoXmlLength	= 500;
	}
}
