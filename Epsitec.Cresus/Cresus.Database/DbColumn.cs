//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 19/11/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	using Converter = Epsitec.Common.Converters.Converter;
	
	public enum DbColumnLocalisation : byte
	{
		None				= 0,	//	n'est pas/ne peut pas être localisée
		Default				= 1,	//	peut être localisée, elle contient les valeurs par défaut
		Localised			= 2		//	est localisée pour une langue particulière
	}
	
	public enum DbColumnClass : byte
	{
		Data				= 0,	//	contient des données
		KeyId				= 1,	//	définit une clef (ID)
		KeyRevision			= 2,	//	définit une clef (révision)
		KeyStatus			= 3,	//	définit un statut
		
		RefSimpleId			= 4,	//	définit une référence à une clef (ID, version simplifiée)
		RefLiveId			= 5,	//	définit une référence à une clef (ID, révision = 0)
		RefTupleId			= 6,	//	définit une référence à une clef (tuple: ID)
		RefTupleRevision	= 7,	//	définit une référence à une clef (tuple: révision)
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
		
		public DbColumn(string name, DbType type, DbColumnClass column_class, DbElementCat category) : this (name, type, Nullable.Undefined)
		{
			this.DefineColumnClass (column_class);
			this.DefineCategory (category);
		}
		
		public DbColumn(string name, DbType type, Nullable nullable, DbColumnClass column_class, DbElementCat category) : this (name, type, nullable)
		{
			this.DefineColumnClass (column_class);
			this.DefineCategory (category);
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
			return (xml.Name == "null") ? null : new DbColumn (xml);
		}
		
		public static DbColumn NewRefColumn(string column_name, string target_table_name, DbColumnClass column_class, DbType type)
		{
			System.Diagnostics.Debug.Assert (type != null);
			
			DbColumn column = new DbColumn (column_name, type);
			
			column.DefineColumnClass (column_class);
			column.DefineCategory (DbElementCat.UserDataManaged);
			column.DefineTargetTableName (target_table_name);
			
			return column;
		}
		
		public static string SerialiseToXml(DbColumn column, bool full)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbColumn.SerialiseToXml (buffer, column, full);
			return buffer.ToString ();
		}
		
		public static void SerialiseToXml(System.Text.StringBuilder buffer, DbColumn column, bool full)
		{
			if (column == null)
			{
				buffer.Append ("<null/>");
			}
			else
			{
				column.SerialiseXmlDefinition (buffer, full);
			}
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
		
		
		
		internal void DefineTargetTableName(string target_table_name)
		{
			string current_name = this.TargetTableName;
			
			if (current_name == target_table_name)
			{
				return;
			}
			
			if (current_name != null)
			{
				string message = string.Format ("Column '{0}' cannot change its target table name from '{1}' to '{2}'.",
					/**/						this.Name, current_name, target_table_name);
				throw new System.InvalidOperationException (message);
			}

			this.Attributes.SetAttribute (Tags.Target, target_table_name);
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
		
		
		internal bool					IsPrimaryKey
		{
			get { return this.is_primary_key; }
			set { this.is_primary_key = value; }
		}
		
		
		protected void SerialiseXmlDefinition(System.Text.StringBuilder buffer, bool full)
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
			
			if (this.is_primary_key)
			{
				buffer.Append (@" pk=""1""");
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
			
			if (full)
			{
				DbKey.SerialiseToXmlAttributes (buffer, this.internal_column_key);
				this.Attributes.SerialiseXmlAttributes (buffer);
				buffer.Append (@">");
				
				DbTypeFactory.SerialiseToXml (buffer, this.type, true);
				
				buffer.Append (@"</col>");
			}
			else
			{
				buffer.Append (@"/>");
			}
		}
		
		protected void ProcessXmlDefinition(System.Xml.XmlElement xml)
		{
			if (xml.Name != "col")
			{
				throw new System.FormatException (string.Format ("Expected root element named <col>, but found <{0}>.", xml.Name));
			}
			
			string arg_null   = xml.GetAttribute ("null");
			string arg_unique = xml.GetAttribute ("unique");
			string arg_index  = xml.GetAttribute ("index");
			string arg_cat    = xml.GetAttribute ("cat");
			string arg_pk     = xml.GetAttribute ("pk");
			
			this.is_null_allowed = (arg_null   == "1");
			this.is_unique       = (arg_unique == "1");
			this.is_indexed      = (arg_index  == "1");
			this.is_primary_key  = (arg_pk     == "1");
			this.category        = DbTools.ParseElementCategory (arg_cat);
			
			int column_class_code;
			int column_localisation_code;
			
			Converter.Convert (xml.GetAttribute ("class"), out column_class_code);
			Converter.Convert (xml.GetAttribute ("local"), out column_localisation_code);
			
			this.column_class        = (DbColumnClass) column_class_code;
			this.column_localisation = (DbColumnLocalisation) column_localisation_code;
			this.internal_column_key = DbKey.DeserialiseFromXmlAttributes (xml);
			this.Attributes.DeserialiseXmlAttributes (xml);
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
		
		public string					TargetTableName
		{
			get { return this.Attributes[Tags.Target]; }
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
				if (this.Type is DbTypeString)
				{
					DbTypeString type = this.type as DbTypeString;
					return type.Length;
				}
				if (this.Type is DbTypeEnum)
				{
					DbTypeEnum type = this.Type as DbTypeEnum;
					return type.MaxNameLength;
				}

				// DD TODO: ajouter DbTypeByteArray
				
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
			
			//	Vérifie que la définition de la colonne est bien correcte. On ne permet ainsi
			//	pas de localiser des colonnes de type référence (ça n'aurait pas de sens).
			
			switch (this.column_localisation)
			{
				case DbColumnLocalisation.Default:
				case DbColumnLocalisation.Localised:
					if (this.column_class != DbColumnClass.Data)
					{
						string message = string.Format ("Column '{0}' specifies localisation {1} for class {2}.",
							/**/						this.Name, this.column_localisation, this.column_class);
						throw new System.InvalidOperationException (message);
					}
					break;
				
				case DbColumnLocalisation.None:
					break;
				
				default:
					throw new System.InvalidOperationException (string.Format ("Column '{0}' specifies invalid localisation.", this.Name));
			}
			
			switch (this.column_class)
			{
				case DbColumnClass.KeyId:
				case DbColumnClass.KeyRevision:
				case DbColumnClass.KeyStatus:
					if (this.Category != DbElementCat.Internal)
					{
						throw new System.InvalidOperationException (string.Format ("Column '{0}' category should be internal, but is {1}.", this.Name, this.Category));
					}
					break;
				default:
					break;
			}
			
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
		
		public string CreateDisplayName()
		{
			//	Ajoute un suffixe éventuel au nom de la colonne pour distinguer plusieurs
			//	colonnes qui auraient un même nom. C'est le cas pour les références qui
			//	sont visibles par l'utilisateur sous le même nom.
			
			switch (this.column_class)
			{
				case DbColumnClass.RefLiveId:			return this.Name + " (live)";
				case DbColumnClass.RefSimpleId:			return this.Name;
				case DbColumnClass.RefTupleId:			return this.Name + " (ID)";
				case DbColumnClass.RefTupleRevision:	return this.Name + " (REV)";
			}
			
			return this.Name;
		}
		
		public string CreateSqlName()
		{
			//	Crée le nom SQL de la colonne. Ce nom peut être différent du nom "haut niveau"
			//	utilisé par DbColumn, indépendamment des caractères autorisés.
			
			if (this.Category == DbElementCat.Internal)
			{
				//	Les colonnes "internes" doivent déjà avoir un nom valide et elles sont
				//	traitées de manière spéciale ici :
				
				return DbSqlStandard.CreateSimpleSqlName (this.Name, DbElementCat.Internal);
			}
			
			string prefix;
			string suffix;
			
			switch (this.column_class)
			{
				case DbColumnClass.Data:		return DbSqlStandard.CreateSimpleSqlName (this.Name, this.Category);
				case DbColumnClass.KeyId:		return DbColumn.TagId;
				case DbColumnClass.KeyRevision:	return DbColumn.TagRevision;
				case DbColumnClass.KeyStatus:	return DbColumn.TagStatus;
				
				case DbColumnClass.RefLiveId:
				case DbColumnClass.RefSimpleId:
				case DbColumnClass.RefTupleId:
					prefix = "REF";
					suffix = "ID";
					break;
				
				case DbColumnClass.RefTupleRevision:
					prefix = "REF";
					suffix = "REV";
					break;
				
				default:
					throw new System.NotSupportedException (string.Format ("Column '{0}' column class not supported.", this.Name));
			}
			
			return DbSqlStandard.CreateSimpleSqlName (this.Name, prefix, suffix);;
		}
		
		
		public SqlField CreateSqlField(ITypeConverter type_converter, int value)
		{
			//	TODO: implémenter pour de vrai
			//	DD:	c'est à dire ??
			SqlField field = SqlField.CreateConstant (value, DbRawType.Int32);
			field.Alias = this.Name;
			return field;
		}
		
		public SqlField CreateSqlField(ITypeConverter type_converter, long value)
		{
			//	TODO: implémenter pour de vrai
			//	DD:	c'est à dire ??
			SqlField field = SqlField.CreateConstant (value, DbRawType.Int64);
			field.Alias = this.Name;
			return field;
		}
		
		public SqlField CreateSqlField(ITypeConverter type_converter, string value)
		{
			//	TODO: implémenter pour de vrai
			//	DD:	c'est à dire ??
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

//					this.type = new DbType (DbSimpleType.ByteArray);	
					// DD TODO	il faudra créer une classe DbTypeByteArray
					break;
					
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
		protected bool					is_primary_key		= false;
		protected DbElementCat			category;
		protected DbKey					internal_column_key;
		
		protected DbColumnLocalisation	column_localisation	= DbColumnLocalisation.None;
		protected DbColumnClass			column_class		= DbColumnClass.Data;
		
		
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
