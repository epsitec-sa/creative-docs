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
	public sealed class DbColumn : IName, ICaption, IXmlSerializable, System.IEquatable<DbColumn>
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
			: this ()
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
			: this ()
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

		/// <summary>
		/// Initializes a new instance of the <see cref="DbColumn"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="type">The type.</param>
		/// <param name="columnClass">The column class.</param>
		/// <param name="category">The category.</param>
		/// <param name="revisionMode">The revision mode.</param>
		public DbColumn(string name, DbTypeDef type, DbColumnClass columnClass, DbElementCat category, DbRevisionMode revisionMode)
			: this (name, type, columnClass, category)
		{
			this.DefineRevisionMode (revisionMode);
		}


		#region IName Members

		/// <summary>
		/// Gets the name of the column. If no name is defined, uses the caption
		/// name instead.
		/// </summary>
		/// <value>The name of the column.</value>
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

		/// <summary>
		/// Gets the caption id for the column.
		/// </summary>
		/// <value>The caption DRUID.</value>
		public Druid CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		#endregion

		/// <summary>
		/// Gets the caption for the column.
		/// </summary>
		/// <value>The caption or <c>null</c> if the <c>CaptionId</c> is not valid.</value>
		public Caption							Caption
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


		/// <summary>
		/// Gets the key for the column, used internally to identify the column
		/// metadata.
		/// </summary>
		/// <value>The key.</value>
		public DbKey							Key
		{
			get
			{
				return this.key;
			}
		}


		/// <summary>
		/// Gets the name of the target table, if the column is a reference to
		/// another table (foreign key).
		/// </summary>
		/// <value>The name of the target table.</value>
		public string							TargetTableName
		{
			get
			{
				return this.targetTableName;
			}
		}

		/// <summary>
		/// Gets the name of the target column, if the column is a reference to
		/// another table. The target column contains the key in the target table
		/// which is used to create the relation.
		/// </summary>
		/// <value>The name of the target column.</value>
		public string							TargetColumnName
		{
			get
			{
				if (this.columnClass == DbColumnClass.RefId)
				{
					return Tags.ColumnId;
				}
				else
				{
					return null;
				}
			}
		}

		/// <summary>
		/// Gets the containing table.
		/// </summary>
		/// <value>The containing table.</value>
		public DbTable							Table
		{
			get
			{
				return this.table;
			}
		}

		/// <summary>
		/// Gets the index of the column in the containing column.
		/// </summary>
		/// <value>The index of the column or <c>-1</c> if the column does not
		/// belong to a table.</value>
		public int								TableColumnIndex
		{
			get
			{
				if (this.table != null)
				{
					return this.table.Columns.IndexOf (this);
				}
				else
				{
					return -1;
				}
			}
		}


		/// <summary>
		/// Gets the type of the column.
		/// </summary>
		/// <value>The type definition.</value>
		public DbTypeDef						Type
		{
			get
			{
				return this.type;
			}
		}


		/// <summary>
		/// Gets the category of the column (internal column or user data).
		/// </summary>
		/// <value>The category.</value>
		public DbElementCat						Category
		{
			get
			{
				return this.category;
			}
		}

		/// <summary>
		/// Gets the revision mode for the column.
		/// </summary>
		/// <value>The revision mode.</value>
		public DbRevisionMode					RevisionMode
		{
			get
			{
				return this.revisionMode;
			}
		}


		/// <summary>
		/// Gets a value indicating whether this column is a primary key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column is a primary key; otherwise, <c>false</c>.
		/// </value>
		public bool								IsPrimaryKey
		{
			get
			{
				return this.isPrimaryKey;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this column defines a foreign key.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this column defines a foreign key; otherwise, <c>false</c>.
		/// </value>
		public bool								IsForeignKey
		{
			get
			{
				return (string.IsNullOrEmpty (this.TargetTableName) == false)
					&& (string.IsNullOrEmpty (this.TargetColumnName) == false);
			}
		}

		/// <summary>
		/// Gets the column localization.
		/// </summary>
		/// <value>The column localization.</value>
		public DbColumnLocalization				Localization
		{
			get
			{
				return this.localization;
			}
		}

		/// <summary>
		/// Gets the column class (data, reference, key, etc.).
		/// </summary>
		/// <value>The column class.</value>
		public DbColumnClass					ColumnClass
		{
			get
			{
				return this.columnClass;
			}
		}


		/// <summary>
		/// Defines the column category. A column category may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="category">The category.</param>
		internal void DefineCategory(DbElementCat category)
		{
			if (this.category == category)
			{
				return;
			}

			if (this.category == DbElementCat.Unknown)
			{
				this.category = category;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot define a new category", this.Name));
			}
		}

		/// <summary>
		/// Defines the revision mode. A column revision mode may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="revisionMode">The revision mode.</param>
		internal void DefineRevisionMode(DbRevisionMode revisionMode)
		{
			if (this.revisionMode == revisionMode)
			{
				return;
			}

			if (this.revisionMode == DbRevisionMode.Unknown)
			{
				this.revisionMode = revisionMode;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot define a new revision mode", this.Name));
			}
		}

		/// <summary>
		/// Defines the containing table.
		/// </summary>
		/// <param name="table">The table.</param>
		internal void DefineTable(DbTable table)
		{
			this.table = table;
		}

		/// <summary>
		/// Defines the name of the target table. A target table may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="targetTableName">Name of the target table.</param>
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

		/// <summary>
		/// Defines the column key. A column key may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="key">The key.</param>
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

		/// <summary>
		/// Defines the column name.
		/// </summary>
		/// <param name="name">The name or a caption DRUID (like <c>"[1234]"</c>).</param>
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

		/// <summary>
		/// Defines the caption id for the column. This clears the name, as it
		/// will be derived automatically from the caption.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		internal void DefineCaptionId(Druid captionId)
		{
			this.captionId = captionId;
			this.caption   = null;
			this.name      = null;
		}



		/// <summary>
		/// Defines the column type. The column type may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="value">The type definition.</param>
		internal void DefineType(DbTypeDef value)
		{
			if (this.type == value)
			{
				return;
			}

			if (this.type == null)
			{
				this.type = value;
				this.localization = this.type.IsMultilingual ? DbColumnLocalization.Localized : DbColumnLocalization.None;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot change its type", this.Name));
			}
		}

		/// <summary>
		/// Defines the localization for the column.
		/// </summary>
		/// <param name="value">The localization mode.</param>
		internal void DefineLocalization(DbColumnLocalization value)
		{
			this.localization = value;
		}

		/// <summary>
		/// Defines the column class (data, reference, key, etc.).
		/// </summary>
		/// <param name="value">The column class.</param>
		internal void DefineColumnClass(DbColumnClass value)
		{
			this.columnClass = value;
		}

		/// <summary>
		/// Defines whether the column is used as part of the primary key.
		/// </summary>
		/// <param name="value">if set to <c>true</c>, the column is part of the primary key.</param>
		internal void DefinePrimaryKey(bool value)
		{
			this.isPrimaryKey = value;
		}


		/// <summary>
		/// Creates the SQL column for this column definition.
		/// </summary>
		/// <param name="typeConverter">The type converter.</param>
		/// <param name="localizationSuffix">The localization suffix.</param>
		/// <returns>The SQL column.</returns>
		public SqlColumn CreateSqlColumn(ITypeConverter typeConverter, string localizationSuffix)
		{
			DbRawType rawType = this.type.RawType;
			SqlColumn column;
			
			//	Verify that we do not attempt to create an SQL column based on
			//	incorrect settings; check localization related constraints :

			if (this.localization == DbColumnLocalization.None)
			{
				if (!string.IsNullOrEmpty (localizationSuffix))
				{
					string message = string.Format ("Column '{0}' does not specify localization, but caller provides suffix '{1}'", this.Name, localizationSuffix);
					throw new System.InvalidOperationException (message);
				}
				if (this.type.IsMultilingual)
				{
					string message = string.Format ("Column '{0}' does not specify localization, but type is multilingual", this.Name);
					throw new System.InvalidOperationException (message);
				}
			}
			else if (this.localization == DbColumnLocalization.Localized)
			{
				if (string.IsNullOrEmpty (localizationSuffix))
				{
					string message = string.Format ("Column '{0}' specifies localization, but caller provides no suffix", this.Name);
					throw new System.InvalidOperationException (message);
				}
				if (!this.type.IsMultilingual)
				{
					string message = string.Format ("Column '{0}' specifies localization, but type is not multilingual", this.Name);
					throw new System.InvalidOperationException (message);
				}
				if (this.columnClass != DbColumnClass.Data)
				{
					string message = string.Format ("Column '{0}' specifies localization {1} for wrong class {2}", this.Name, this.localization, this.columnClass);
					throw new System.InvalidOperationException (message);
				}
			}
			else
			{
				throw new System.NotSupportedException (string.Format ("Column '{0}' specifies unsupported localization", this.Name));
			}

			if ((this.columnClass == DbColumnClass.KeyId) ||
				(this.columnClass == DbColumnClass.KeyStatus))
			{
				if (this.Category != DbElementCat.Internal)
				{
					throw new System.InvalidOperationException (string.Format ("Column '{0}' category should be internal, but is {1}", this.Name, this.Category));
				}
			}

			//	OK. The column is properly formed and we can now attempt to
			//	generate the SQL column definition for it :
			
			IRawTypeConverter rawConverter;

			if (typeConverter.CheckNativeSupport (rawType))
			{
				column = new SqlColumn ();
				column.SetType (rawType, this.Type.Length, this.Type.IsFixedLength, DbCharacterEncoding.Unicode);
			}
			else if (typeConverter.GetRawTypeConverter (rawType, out rawConverter))
			{
				column = new SqlColumn ();
				column.SetType (rawConverter.InternalType, rawConverter.Length, rawConverter.IsFixedLength, rawConverter.Encoding);
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Column '{0}' cannot be translated to an SQL column", this.Name));
			}

			column.Name         = this.MakeLocalizedSqlName (localizationSuffix);
			column.IsNullable   = this.Type.IsNullable;
			column.IsForeignKey = this.IsForeignKey;
			
			return column;
		}

		/// <summary>
		/// Creates the display name of the column.
		/// </summary>
		/// <returns>The display name.</returns>
		public string GetDisplayName()
		{
			return this.Name;
		}

		/// <summary>
		/// Creates the SQL name of the column. This name will most certainly
		/// be different from the high level <c>DbColumn.Name</c>.
		/// </summary>
		/// <returns>The SQL name.</returns>
		public string GetSqlName()
		{
			if (this.Category == DbElementCat.Internal)
			{
				//	Make sure the name of the internal column is properly prefixed
				//	with "CR_" or "CREF_" :

				return DbSqlStandard.MakeSimpleSqlName (this.Name, DbElementCat.Internal);
			}

			switch (this.columnClass)
			{
				case DbColumnClass.Data:		return DbSqlStandard.MakeSimpleSqlName (this.Name, this.Category);
				case DbColumnClass.KeyId:		return Tags.ColumnId;
				case DbColumnClass.KeyStatus:	return Tags.ColumnStatus;
				case DbColumnClass.RefId:		return DbSqlStandard.MakeSimpleSqlName (this.Name, "REF", "ID");
			}

			throw new System.NotSupportedException (string.Format ("Column '{0}' has an unsupported class", this.Name));
		}

		#region IEquatable<DbColumn> Members

		/// <summary>
		/// Compares the column with another column.
		/// </summary>
		/// <param name="other">Column to compare with.</param>
		/// <returns><c>true</c> if both columns have the same name.</returns>
		public bool Equals(DbColumn other)
		{
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
			return string.IsNullOrEmpty (name) ? 0 : name.GetHashCode ();
		}

		#endregion

		/// <summary>
		/// Deserializes a column from the specified XML reader.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The column.</returns>
		public static DbColumn Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.LocalName == "column"))
			{
				DbColumn column = new DbColumn ();
				bool isEmptyElement = xmlReader.IsEmptyElement;

				column.captionId    = DbTools.ParseDruid (xmlReader.GetAttribute ("capt"));
				column.category     = DbTools.ParseElementCategory (xmlReader.GetAttribute ("cat"));
				column.revisionMode = DbTools.ParseRevisionMode (xmlReader.GetAttribute ("rev"));
				column.columnClass  = DbTools.ParseColumnClass (xmlReader.GetAttribute ("class"));
				column.localization = DbTools.ParseLocalization (xmlReader.GetAttribute ("loc"));
				column.isPrimaryKey = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("pk"));

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

		/// <summary>
		/// Serializes the column using the specified XML writer.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("column");

			DbTools.WriteAttribute (xmlWriter, "capt", DbTools.DruidToString (this.CaptionId));
			DbTools.WriteAttribute (xmlWriter, "cat", DbTools.ElementCategoryToString (this.Category));
			DbTools.WriteAttribute (xmlWriter, "rev", DbTools.RevisionModeToString (this.revisionMode == DbRevisionMode.Unknown ? DbRevisionMode.Disabled : this.revisionMode));
			DbTools.WriteAttribute (xmlWriter, "class", DbTools.ColumnClassToString (this.ColumnClass));
			DbTools.WriteAttribute (xmlWriter, "loc", DbTools.ColumnLocalizationToString (this.Localization));
			DbTools.WriteAttribute (xmlWriter, "pk", DbTools.BoolDefaultingToFalseToString (this.IsPrimaryKey));
			
			xmlWriter.WriteEndElement ();
		}

		#endregion

		/// <summary>
		/// Creates a localized column name.
		/// </summary>
		/// <param name="localizationSuffix">The localization suffix.</param>
		/// <returns>The localized column name.</returns>
		internal string MakeLocalizedName(string localizationSuffix)
		{
			return string.Concat (this.Name, " (", localizationSuffix, ")");
		}

		/// <summary>
		/// Creates a localized SQL column name.
		/// </summary>
		/// <param name="localizationSuffix">The localization suffix.</param>
		/// <returns>The localized SQL column name.</returns>
		internal string MakeLocalizedSqlName(string localizationSuffix)
		{
			if (string.IsNullOrEmpty (localizationSuffix))
			{
				return this.GetSqlName ();
			}
			else
			{
				return DbTools.MakeCompositeName (this.GetSqlName (), DbSqlStandard.MakeSimpleSqlName (localizationSuffix));
			}
		}

		private static readonly Caption nullCaption = new Caption ();

		private DbTypeDef						type;
		private DbTable							table;

		private string							name;
		private string							targetTableName;
		private Druid							captionId;
		private Caption							caption;

		private DbKey							key;
		
		private bool							isPrimaryKey;
		private DbElementCat					category;
		private DbRevisionMode					revisionMode;
		private DbColumnClass					columnClass;
		private DbColumnLocalization			localization;
	}
}
