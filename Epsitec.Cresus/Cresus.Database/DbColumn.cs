//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumn</c> class describes a column in a database table.
	/// This is our version of the column metadata wrapper (<see cref="System.Data.DataColumn"/>).
	/// </summary>
	public class DbColumn : IName, ICaption
	{
		public DbColumn()
		{
		}

		public DbColumn(string name)
		{
			this.DefineName (name);
		}

		public DbColumn(string name, INamedType type)
			: this (name)
		{
			this.DefineType (type);
		}
		
		public DbColumn(string name, INamedType type, DbColumnClass columnClass)
			: this (name, type)
		{
			this.DefineColumnClass (columnClass);
		}

		public DbColumn(string name, INamedType type, DbColumnClass columnClass, DbElementCat category)
			: this (name, type, columnClass)
		{
			this.DefineCategory (category);
		}

		public DbColumn(System.Xml.XmlElement xml)
		{
			this.ProcessXmlDefinition (xml);
		}


		public static DbColumn CreateColumn(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			doc.LoadXml (xml);
			return DbColumn.CreateColumn (doc.DocumentElement);
		}

		public static DbColumn CreateColumn(System.Xml.XmlElement xml)
		{
			return (xml.Name == "null") ? null : new DbColumn (xml);
		}

		public static DbColumn CreateRefColumn(string column_name, string parent_table_name, DbColumnClass column_class, INamedType type)
		{
			return DbColumn.CreateRefColumn (column_name, parent_table_name, column_class, type, Nullable.Undefined);
		}

		public static DbColumn CreateRefColumn(string column_name, string parent_table_name, DbColumnClass column_class, INamedType type, Nullable nullable)
		{
			System.Diagnostics.Debug.Assert (type != null);

			DbColumn column = new DbColumn (column_name, type);

			column.DefineColumnClass (column_class);
			column.DefineCategory (DbElementCat.UserDataManaged);
			column.DefineParentTableName (parent_table_name);
			column.IsNullAllowed = (nullable == Nullable.Yes);

			return column;
		}

		public static DbColumn CreateUserDataColumn(string column_name, INamedType type)
		{
			return new DbColumn (column_name, type, DbColumnClass.Data, DbElementCat.UserDataManaged);
		}


		#region IName Members

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					Caption caption = this.Caption;
					return this.caption == null ? null this.caption.Name
				}
				else
				{
					return this.name;
				}
			}
		}

		#endregion

		#region ICaption Members

		public Druid CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		#endregion

		public Caption Caption
		{
			get
			{
				if (this.caption == null)
				{
					this.caption = DbContext.Current.ResourceManager.GetCaption (this.captionId) ?? DbColumn.nullCaption;
				}

				if (this.caption == DbColumn.nullCaption)
				{
					return null;
				}
				else
				{
					return this.caption;
				}
			}
		}


		public DbKey InternalKey
		{
			get
			{
				return this.internal_column_key;
			}
		}


		public string ParentTableName
		{
			//	Lorsqu'une colonne appartient à l'une des classes DbColumnClass.RefXyz, cela signifie
			//	que le colonne pointe sur une autre table (foreign key); le nom de cette table est défini
			//	par la propriété ParentTableName :

			get
			{
				return this.Attributes[Tags.Parent];
			}
		}

		public string ParentColumnName
		{
			get
			{
				switch (this.column_class)
				{
					case DbColumnClass.RefId:
						return Tags.ColumnId;
				}

				throw new System.ArgumentException (string.Format ("Column of invalid class {0}.", this.column_class));
			}
		}


		public DbTable Table
		{
			get
			{
				return this.table;
			}
		}

		public int TableColumnIndex
		{
			get
			{
				if (this.table != null)
				{
					int index = this.table.Columns.IndexOf (this);

					if (index >= 0)
					{
						return index;
					}
				}

				throw new System.InvalidOperationException ("Column not in valid table.");
			}
		}


		public INamedType Type
		{
			get
			{
				return this.type;
			}
		}
		
		public int Length
		{
			get
			{
				if (this.Type is DbTypeString)
				{
					DbTypeString type = this.type as DbTypeString;
					return type.MaximumLength;
				}
				if (this.Type is DbTypeEnum)
				{
					DbTypeEnum type = this.Type as DbTypeEnum;
					return type.MaxNameLength;
				}

				return 1;
			}
		}

		public bool IsFixedLength
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


		public DbElementCat Category
		{
			get
			{
				return this.category;
			}
		}

		public DbRevisionMode RevisionMode
		{
			get
			{
				return this.revision_mode;
			}
		}


		public bool IsNullAllowed
		{
			get
			{
				return this.is_null_allowed;
			}
			set
			{
				this.is_null_allowed = value;
			}
		}

		public bool IsUnique
		{
			get
			{
				return this.is_unique;
			}
			set
			{
				this.is_unique = value;
			}
		}

		public bool IsIndexed
		{
			get
			{
				return this.is_indexed;
			}
			set
			{
				this.is_indexed = value;
			}
		}

		public bool IsPrimaryKey
		{
			get
			{
				return this.is_primary_key;
			}
		}


		public DbColumnLocalisation ColumnLocalisation
		{
			get
			{
				return this.localisation;
			}
		}

		public DbColumnClass ColumnClass
		{
			get
			{
				return this.column_class;
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
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot define a new category.", this.Name));
			}

			this.category = category;
		}

		public void DefineRevisionMode(DbRevisionMode revision_mode)
		{
			if (this.revision_mode == revision_mode)
			{
				return;
			}

			if (this.revision_mode != DbRevisionMode.Unknown)
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot define a new revision mode.", this.Name));
			}

			this.revision_mode = revision_mode;
		}

		public void DefineAttributes(params string[] attributes)
		{
			this.attributes.SetFromInitialisationList (attributes);
		}


		internal void DefineTable(DbTable table)
		{
			this.table = table;
		}

		internal void DefineParentTableName(string parent_table_name)
		{
			string current_name = this.ParentTableName;

			if (current_name == parent_table_name)
			{
				return;
			}

			if (current_name != null)
			{
				string message = string.Format ("Column '{0}' cannot change its parent table name from '{1}' to '{2}'.",
					/**/						this.Name, current_name, parent_table_name);
				throw new System.InvalidOperationException (message);
			}

			this.Attributes.SetAttribute (Tags.Parent, parent_table_name);
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

		internal void DefineName(string name)
		{
			if (Druid.IsValidResourceId (name))
			{
				this.captionId = Druid.Parse (name);
				this.name = null;
			}
			else
			{
				this.name = name;
			}
		}

		internal void DefineType(INamedType type)
		{
			if (this.type != null)
			{
				throw new System.InvalidOperationException ("Cannot reinitialise type of column.");
			}

			this.type = type;
		}

		internal void DefineColumnLocalisation(DbColumnLocalisation localisation)
		{
			this.localisation = localisation;
		}

		internal void DefineColumnClass(DbColumnClass column_class)
		{
			this.column_class = column_class;
		}

		internal void DefinePrimaryKey(bool is_primary_key)
		{
			this.is_primary_key = is_primary_key;
		}


		public SqlColumn CreateSqlColumn(ITypeConverter type_converter)
		{
			DbRawType raw_type = TypeConverter.GetRawType (this.type);
			SqlColumn column   = null;

			//	Vérifie que la définition de la colonne est bien correcte. On ne permet ainsi
			//	pas de localiser des colonnes de type référence (ça n'aurait pas de sens).

			switch (this.localisation)
			{
				case DbColumnLocalisation.Default:
				case DbColumnLocalisation.Localised:
					if (this.column_class != DbColumnClass.Data)
					{
						string message = string.Format ("Column '{0}' specifies localisation {1} for class {2}.",
							/**/						this.Name, this.localisation, this.column_class);
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

				return DbSqlStandard.MakeSimpleSqlName (this.Name, DbElementCat.Internal);
			}

			switch (this.column_class)
			{
				case DbColumnClass.Data:
					return DbSqlStandard.MakeSimpleSqlName (this.Name, this.Category);
				case DbColumnClass.KeyId:
					return Tags.ColumnId;
				case DbColumnClass.KeyStatus:
					return Tags.ColumnStatus;
				case DbColumnClass.RefId:
					return DbSqlStandard.MakeSimpleSqlName (this.Name, "REF", "ID");
			}

			throw new System.NotSupportedException (string.Format ("Column '{0}' column class not supported.", this.Name));
		}


		public SqlField CreateSqlField(ITypeConverter type_converter, int value)
		{
			//	TODO: pas une bonne idée d'avoir ces méthodes de création qui ne tiennent
			//	pas compte des types internes supportés par la base... c'est valable pour
			//	toutes les variantes de CreateSqlField.

			SqlField field = SqlField.CreateConstant (value, DbRawType.Int32);
			field.Alias = this.Name;
			return field;
		}

		public SqlField CreateSqlField(ITypeConverter type_converter, long value)
		{
			SqlField field = SqlField.CreateConstant (value, DbRawType.Int64);
			field.Alias = this.Name;
			return field;
		}

		public SqlField CreateSqlField(ITypeConverter type_converter, string value)
		{
			SqlField field = SqlField.CreateConstant (value, DbRawType.String);
			field.Alias = this.Name;
			return field;
		}

		public SqlField CreateSqlField(ITypeConverter type_converter, System.DateTime value)
		{
			SqlField field = SqlField.CreateConstant (value, DbRawType.DateTime);
			field.Alias = this.Name;
			return field;
		}

		public SqlField CreateEmptySqlField(ITypeConverter type_converter)
		{
			DbRawType raw_type = TypeConverter.GetRawType (this.type);
			SqlField field    = SqlField.CreateConstant (null, raw_type);
			field.Alias = this.CreateSqlName ();
			return field;
		}


		public static string SerializeToXml(DbColumn column, bool full)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbColumn.SerializeToXml (buffer, column, full);
			return buffer.ToString ();
		}

		public static void SerializeToXml(System.Text.StringBuilder buffer, DbColumn column, bool full)
		{
			if (column == null)
			{
				buffer.Append ("<null/>");
			}
			else
			{
				column.SerializeXmlDefinition (buffer, full);
			}
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

		protected void SerializeXmlDefinition(System.Text.StringBuilder buffer, bool full)
		{
			buffer.Append (@"<col");

			if (this.IsNullAllowed)
			{
				buffer.Append (@" null=""Y""");
			}

			if (this.IsUnique)
			{
				buffer.Append (@" uniq=""Y""");
			}

			if (this.IsIndexed)
			{
				buffer.Append (@" idx=""Y""");
			}

			if (this.is_primary_key)
			{
				buffer.Append (@" pk=""Y""");
			}

			string arg_cat = DbTools.ElementCategoryToString (this.category);
			string arg_rev = DbTools.RevisionModeToString (this.revision_mode);

			if (arg_cat != null)
			{
				buffer.Append (@" cat=""");
				buffer.Append (arg_cat);
				buffer.Append (@"""");
			}
			if (arg_rev != null)
			{
				buffer.Append (@" rev=""");
				buffer.Append (arg_rev);
				buffer.Append (@"""");
			}

			if (this.column_class != DbColumnClass.Data)
			{
				buffer.Append (@" class=""");
				buffer.Append (InvariantConverter.ToString ((int) this.column_class));
				buffer.Append (@"""");
			}

			if (this.localisation != DbColumnLocalisation.None)
			{
				int num = (int) this.localisation;
				buffer.Append (@" local=""");
				buffer.Append (InvariantConverter.ToString ((int) this.localisation));
				buffer.Append (@"""");
			}

			if (full)
			{
				DbKey.SerializeToXmlAttributes (buffer, this.internal_column_key);
				this.Attributes.SerializeXmlAttributes (buffer);
				buffer.Append (@">");

				DbTypeFactory.SerializeToXml (buffer, this.type, true);

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
			string arg_unique = xml.GetAttribute ("uniq");
			string arg_index  = xml.GetAttribute ("idx");
			string arg_pk     = xml.GetAttribute ("pk");

			this.is_null_allowed = (arg_null   == "Y");
			this.is_unique       = (arg_unique == "Y");
			this.is_indexed      = (arg_index  == "Y");
			this.is_primary_key  = (arg_pk     == "Y");

			string arg_cat = xml.GetAttribute ("cat");
			string arg_rev = xml.GetAttribute ("revs");

			this.category      = DbTools.ParseElementCategory (arg_cat);
			this.revision_mode = DbTools.ParseRevisionMode (arg_rev);

			int column_class_code;
			int localisation_code;

			InvariantConverter.Convert (xml.GetAttribute ("class"), out column_class_code);
			InvariantConverter.Convert (xml.GetAttribute ("local"), out localisation_code);

			this.column_class        = (DbColumnClass) column_class_code;
			this.localisation = (DbColumnLocalisation) localisation_code;
			this.internal_column_key = DbKey.DeserializeFromXmlAttributes (xml);

			this.Attributes.DeserializeXmlAttributes (xml);
		}


		private static readonly Caption nullCaption = new Caption ();

		protected DbAttributes attributes				= new DbAttributes ();
		protected INamedType type;
		protected DbTable table;

		private string name;
		private Druid captionId;
		private Caption caption;
		private DbColumnLocalisation localisation;

		protected bool is_null_allowed;
		protected bool is_unique;
		protected bool is_indexed;
		protected bool is_primary_key;
		protected DbElementCat category;
		protected DbRevisionMode revision_mode;
		protected DbKey internal_column_key;

		protected DbColumnClass column_class			= DbColumnClass.Data;


		internal const int MaxNameLength			= 50;
		internal const int MaxDictKeyLength		= 50;
		internal const int MaxDictValueLength		= 1000;
		internal const int MaxCaptionLength		= 100;
		internal const int MaxDescriptionLength	= 500;
		internal const int MaxInfoXmlLength		= 500;
	}
}
