//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbColumn</c> class describes a column in a database table.
	/// This is our version of the column metadata wrapper (compare with the
	/// ADO.NET <see cref="System.Data.DataColumn"/> class).
	/// </summary>
	public class DbColumn : IName, ICaption, IXmlSerializable, System.IEquatable<DbColumn>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		public DbColumn()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		public DbColumn(string name, DbTypeDef type)
		{
			this.DefineName (name);
			this.DefineType (type);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		/// <param name="type">The type.</param>
		public DbColumn(Druid captionId, DbTypeDef type)
		{
			this.DefineType (type);
			this.DefineCaptionId (captionId);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="field">The structured type field definition.</param>
		public DbColumn(StructuredTypeField field)
			: this (field.CaptionId, new DbTypeDef (field.Type))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="columnClass">The column class.</param>
		public DbColumn(string name, DbTypeDef type, DbColumnClass columnClass)
			: this (name, type)
		{
			this.DefineColumnClass (columnClass);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="columnClass">The column class.</param>
		/// <param name="category">The category.</param>
		public DbColumn(string name, DbTypeDef type, DbColumnClass columnClass, DbElementCat category)
			: this (name, type, columnClass)
		{
			this.DefineCategory (category);
		}



		#region IName Members

		public string Name
		{
			get
			{
				if (this.name == null)
				{
					Caption caption = this.Caption;
					return caption == null ? null : caption.Name;
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


		public DbKey Key
		{
			get
			{
				return this.key;
			}
		}


		public string TargetTableName
		{
			//	Lorsqu'une colonne appartient à l'une des classes DbColumnClass.RefXyz, cela signifie
			//	que le colonne pointe sur une autre table (foreign key); le nom de cette table est défini
			//	par la propriété TargetTableName :

			get
			{
				return this.targetTableName;
			}
		}

		public string TargetColumnName
		{
			get
			{
				switch (this.columnClass)
				{
					case DbColumnClass.RefId:
						return Tags.ColumnId;
				}

				throw new System.ArgumentException (string.Format ("Column of invalid class {0}.", this.columnClass));
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


		public DbTypeDef Type
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
				return System.Math.Max (this.type.Length, 1);
			}
		}

		public bool IsFixedLength
		{
			get
			{
				return this.type.IsFixedLength;
			}
		}

		public bool IsMultilingual
		{
			get
			{
				return this.type.IsMultilingual;
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


		public bool IsNullable
		{
			get
			{
				return this.type.IsNullable;
			}
		}

		public bool IsUnique
		{
			get
			{
				return this.is_unique;
			}
		}

		public bool IsIndexed
		{
			get
			{
				return this.is_indexed;
			}
		}

		public bool IsPrimaryKey
		{
			get
			{
				return this.is_primary_key;
			}
		}


		public DbColumnLocalisation Localisation
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
				return this.columnClass;
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

		internal void DefineTable(DbTable table)
		{
			this.table = table;
		}

		internal void DefineTargetTableName(string targetTableName)
		{
			if (this.targetTableName == targetTableName)
			{
				return;
			}

			if (string.IsNullOrEmpty (this.targetTableName))
			{
				this.targetTableName = targetTableName;
			}
			else
			{
				string message = string.Format ("Column '{0}' cannot change its target table name from '{1}' to '{2}'.",
					/**/						this.Name, this.TargetTableName, targetTableName);
				throw new System.InvalidOperationException (message);
			}
		}

		internal void DefineKey(DbKey key)
		{
			if (this.key == key)
			{
				return;
			}

			if (this.key.IsEmpty)
			{
				this.key = key;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its key", this.Name));
			}
		}

		internal void DefineName(string name)
		{
			if (Druid.IsValidResourceId (name))
			{
				this.DefineCaptionId (Druid.Parse (name));
			}
			else
			{
				this.name = name;
			}
		}

		internal void DefineCaptionId(Druid captionId)
		{
			this.captionId = captionId;
			this.caption = null;
			this.name = null;
		}



		internal void DefineType(DbTypeDef value)
		{
			if (this.type == value)
			{
				return;
			}

			if (this.type == null)
			{
				this.type = value;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its type", this.Name));
			}
		}

		internal void DefineLocalisation(DbColumnLocalisation value)
		{
			this.localisation = value;
		}

		internal void DefineColumnClass(DbColumnClass value)
		{
			this.columnClass = value;
		}

		internal void DefinePrimaryKey(bool value)
		{
			this.is_primary_key = value;
		}


		public SqlColumn CreateSqlColumn(ITypeConverter typeConverter)
		{
			DbRawType rawType = this.type.RawType;
			SqlColumn column  = null;

			//	Vérifie que la définition de la colonne est bien correcte. On ne permet ainsi
			//	pas de localiser des colonnes de type référence (ça n'aurait pas de sens).

			switch (this.localisation)
			{
				case DbColumnLocalisation.Default:
				case DbColumnLocalisation.Localised:
					if (this.columnClass != DbColumnClass.Data)
					{
						string message = string.Format ("Column '{0}' specifies localisation {1} for class {2}",
							/**/						this.Name, this.localisation, this.columnClass);
						throw new System.InvalidOperationException (message);
					}
					if (this.type.IsMultilingual == false)
					{
						string message = string.Format ("Column '{0}' specifies localisation {1} but type is not multilingual",
							/**/						this.Name, this.localisation);
						throw new System.InvalidOperationException (message);
					}
					break;

				case DbColumnLocalisation.None:
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Column '{0}' specifies invalid localisation", this.Name));
			}

			switch (this.columnClass)
			{
				case DbColumnClass.KeyId:
				case DbColumnClass.KeyStatus:
					if (this.Category != DbElementCat.Internal)
					{
						throw new System.InvalidOperationException (string.Format ("Column '{0}' category should be internal, but is {1}", this.Name, this.Category));
					}
					break;
				
				default:
					break;
			}

			IRawTypeConverter rawConverter;

			if (typeConverter.CheckNativeSupport (rawType))
			{
				column = new SqlColumn ();
				column.SetType (rawType, this.Length, this.IsFixedLength);
			}
			else if (typeConverter.GetRawTypeConverter (rawType, out rawConverter))
			{
				column = new SqlColumn ();
				column.SetRawConverter (rawConverter);
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Conversion column '{0}' to SqlColumn not possible", this.Name));
			}

			if (column != null)
			{
				column.Name       = this.CreateSqlName ();
				column.IsNullable = this.IsNullable;
				column.IsUnique   = this.IsUnique;
				column.IsIndexed  = this.IsIndexed;
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

			switch (this.columnClass)
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

			throw new System.NotSupportedException (string.Format ("Column '{0}' has an unsupported class", this.Name));
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
			DbRawType raw_type = this.type.RawType;
			SqlField  field    = SqlField.CreateConstant (null, raw_type);
			field.Alias = this.CreateSqlName ();
			return field;
		}




		#region IEquatable<DbColumn> Members

		public bool Equals(DbColumn other)
		{
			//	ATTENTION: L'égalité se base uniquement sur le nom des colonnes, pas sur les
			//	détails internes...
			
			if (object.ReferenceEquals (other, null))
			{
				return false;
			}
			else
			{
				return this.Name == other.Name;
			}
		}

		#endregion


		#region Equals and GetHashCode support
		
		public override bool Equals(object obj)
		{
			return this.Equals (obj as DbColumn);
		}

		public override int GetHashCode()
		{
			string name = this.Name;
			return (name == null) ? 0 : name.GetHashCode ();
		}

		#endregion

		public static DbColumn Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.LocalName == "column"))
			{
				DbColumn column = new DbColumn ();
				bool isEmptyElement = xmlReader.IsEmptyElement;

				//	TODO: deserialize contents

				column.captionId = DbTools.ParseDruid (xmlReader.GetAttribute ("capt"));
				column.category  = DbTools.ParseElementCategory (xmlReader.GetAttribute ("cat"));
				column.revision_mode = DbTools.ParseRevisionMode (xmlReader.GetAttribute ("rev"));
				column.columnClass = DbTools.ParseColumnClass (xmlReader.GetAttribute ("class"));
				column.localisation = DbTools.ParseLocalisation (xmlReader.GetAttribute ("loc"));

				column.is_unique = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("un"));
				column.is_indexed = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("idx"));
				column.is_primary_key = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("pk"));

				if (!isEmptyElement)
				{
					xmlReader.ReadEndElement ();
				}

				return column;
			}
			else
			{
				throw new System.Xml.XmlException (string.Format ("Unexpected element {0}", xmlReader.LocalName), null, xmlReader.LineNumber, xmlReader.LinePosition);
			}
		}


		#region IXmlSerializable Members

		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("column");

			DbTools.WriteAttribute (xmlWriter, "capt", DbTools.DruidToString (this.CaptionId));
			DbTools.WriteAttribute (xmlWriter, "cat", DbTools.ElementCategoryToString (this.Category));
			DbTools.WriteAttribute (xmlWriter, "rev", DbTools.RevisionModeToString (this.RevisionMode));
			DbTools.WriteAttribute (xmlWriter, "class", DbTools.ColumnClassToString (this.ColumnClass));
			DbTools.WriteAttribute (xmlWriter, "loc", DbTools.ColumnLocalisationToString (this.Localisation));

			DbTools.WriteAttribute (xmlWriter, "un", DbTools.BoolDefaultingToFalseToString (this.IsUnique));
			DbTools.WriteAttribute (xmlWriter, "idx", DbTools.BoolDefaultingToFalseToString (this.IsIndexed));
			DbTools.WriteAttribute (xmlWriter, "pk", DbTools.BoolDefaultingToFalseToString (this.IsPrimaryKey));
			
			xmlWriter.WriteEndElement ();
		}

		#endregion



		private static readonly Caption nullCaption = new Caption ();

		private DbTypeDef type;
		private DbTable table;

		private string name;
		private string targetTableName;
		private Druid captionId;
		private Caption caption;
		private DbColumnLocalisation localisation;

		private bool is_unique;
		private bool is_indexed;
		private bool is_primary_key;
		private DbElementCat category;
		private DbRevisionMode revision_mode;
		private DbKey key;

		private DbColumnClass columnClass			= DbColumnClass.Data;


		internal const int MaxNameLength			= 50;
		internal const int MaxDictKeyLength		= 50;
		internal const int MaxDictValueLength		= 1000;
		internal const int MaxCaptionLength		= 100;
		internal const int MaxDescriptionLength	= 500;
		internal const int MaxInfoXmlLength		= 500;
	}
}
