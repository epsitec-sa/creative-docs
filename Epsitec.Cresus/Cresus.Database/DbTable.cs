//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbTable</c> class describes a table in a database. This is our
	/// version of the table metadata wrapper (compare with the ADO.NET
	/// <see cref="System.Data.DataTable"/> class).
	/// </summary>
	public sealed class DbTable : ICaption, IName, IXmlSerializable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTable"/> class.
		/// </summary>
		public DbTable()
		{
			this.columns = new Collections.DbColumns ();

			this.columns.ItemInserted += this.HandleColumnInserted;
			this.columns.ItemRemoved  += this.HandleColumnRemoved;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTable"/> class.
		/// </summary>
		/// <param name="name">The table name.</param>
		public DbTable(string name)
			: this ()
		{
			this.DefineName (name);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbTable"/> class.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		public DbTable(Druid captionId)
			: this ()
		{
			this.DefineCaptionId (captionId);
		}

		#region IName Members

		/// <summary>
		/// Gets the name of the table.
		/// </summary>
		/// <value>The name of the table.</value>
		public string							Name
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
		/// Gets the caption id for the table.
		/// </summary>
		/// <value>The caption DRUID.</value>
		public Druid							CaptionId
		{
			get
			{
				return this.captionId;
			}
		}

		#endregion

		/// <summary>
		/// Gets the caption for the table.
		/// </summary>
		/// <value>The caption or <c>null</c> if the <c>CaptionId</c> is not valid.</value>
		public Caption							Caption
		{
			get
			{
				if (this.caption == null)
				{
					this.caption = DbContext.Current.ResourceManager.GetCaption (this.captionId) ?? DbTable.nullCaption;
				}

				if (this.caption == DbTable.nullCaption)
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
		/// Gets the table localizations.
		/// </summary>
		/// <value>The table localizations or an empty array if there are no localizations.</value>
		public string[]							Localizations
		{
			get
			{
				if (string.IsNullOrEmpty (this.localizations))
				{
					return new string[0];
				}
				else
				{
					return this.localizations.Split ('/');
				}
			}
		}

		/// <summary>
		/// Gets the number of localizations for this table.
		/// </summary>
		/// <value>The localization count or <c>0</c> if there are no localizations.</value>
		public int								LocalizationCount
		{
			get
			{
				if (string.IsNullOrEmpty (this.localizations))
				{
					return 0;
				}
				else
				{
					int count = 1;
					
					for (int i = 0; i < this.localizations.Length; i++)
					{
						if (this.localizations[i] == '/')
						{
							count++;
						}
					}

					return count;
				}
			}
		}

		/// <summary>
		/// Gets the columns defined for the table.
		/// </summary>
		/// <value>The columns.</value>
		public Collections.DbColumns			Columns
		{
			get
			{
				return this.columns;
			}
		}

		/// <summary>
		/// Gets a value indicating whether this table has primary keys.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this table has primary keys; otherwise, <c>false</c>.
		/// </value>
		public bool								HasPrimaryKeys
		{
			get
			{
				return (this.primaryKeys != null) && (this.primaryKeys.Count > 0);
			}
		}

		/// <summary>
		/// Gets the primary keys. The tuples formed by the columns must
		/// be unique.
		/// </summary>
		/// <value>The primary keys.</value>
		public Collections.DbColumns			PrimaryKeys
		{
			get
			{
				if (this.primaryKeys == null)
				{
					this.primaryKeys = new Collections.DbColumns ();
				}

				return this.primaryKeys;
			}
		}

		/// <summary>
		/// Gets the foreign keys defined in this table.
		/// </summary>
		/// <value>The foreign keys.</value>
		public IEnumerable<DbForeignKey>		ForeignKeys
		{
			get
			{
				foreach (DbColumn column in this.Columns)
				{
					switch (column.ColumnClass)
					{
						case DbColumnClass.RefId:
							yield return new DbForeignKey (column);
							break;
					}
				}
			}
		}

		/// <summary>
		/// Gets the category for this table.
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
		/// Gets the revision mode for this table.
		/// </summary>
		/// <value>The revision mode.</value>
		public DbRevisionMode					RevisionMode
		{
			get
			{
				if (this.revisionMode == DbRevisionMode.Unknown)
				{
					this.UpdateRevisionMode ();
				}
				
				return this.revisionMode;
			}
		}

		/// <summary>
		/// Gets the replication mode for this table.
		/// </summary>
		/// <value>The replication mode.</value>
		public DbReplicationMode				ReplicationMode
		{
			get
			{
				return this.replicationMode;
			}
		}

		/// <summary>
		/// Gets the key for the table, used internally to identify the table
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
		/// Updates the revision mode based on the column definitions. This method
		/// overwrites the <c>RevisionMode</c> defined by <c>DefineRevisionMode</c>.
		/// </summary>
		public void UpdateRevisionMode()
		{
			foreach (DbColumn column in this.columns)
			{
				if (column.RevisionMode == DbRevisionMode.Enabled)
				{
					this.revisionMode = DbRevisionMode.Enabled;
					return;
				}
			}

			this.revisionMode = DbRevisionMode.Disabled;
		}

		/// <summary>
		/// Defines the name for this table. A table name may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="value">The table name.</param>
		public void DefineName(string value)
		{
			if (this.name == value)
			{
				return;
			}
			if (this.name == null)
			{
				this.name = value;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot be renamed to '{1}'", this.name, value));
			}
		}

		/// <summary>
		/// Defines the caption id for the table. This clears the name, as it
		/// will be derived automatically from the caption.
		/// </summary>
		/// <param name="captionId">The caption DRUID.</param>
		public void DefineCaptionId(Druid captionId)
		{
			this.captionId = captionId;
			this.caption   = null;
			this.name      = null;
		}

		/// <summary>
		/// Defines the category for this table. A table category may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="category">The category.</param>
		public void DefineCategory(DbElementCat category)
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
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot define a new category", this.Name));
			}
		}

		/// <summary>
		/// Defines the revision mode for this table. A revision mode may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="revisionMode">The revision mode.</param>
		public void DefineRevisionMode(DbRevisionMode revisionMode)
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
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot define a new revision mode", this.Name));
			}
		}

		/// <summary>
		/// Defines the replication mode for the table. A repliaction mode may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="replicationMode">The replication mode.</param>
		public void DefineReplicationMode(DbReplicationMode replicationMode)
		{
			if (this.replicationMode == replicationMode)
			{
				return;
			}

			if (this.replicationMode == DbReplicationMode.Unknown)
			{
				this.replicationMode = replicationMode;
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot define a new replication mode", this.Name));
			}
		}

		/// <summary>
		/// Defines the key for the table metadata. A key may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="key">The key.</param>
		public void DefineKey(DbKey key)
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
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot change its internal key", this.Name));
			}
		}

		/// <summary>
		/// Defines the primary key for this table.
		/// </summary>
		/// <param name="column">The column used as primary key.</param>
		public void DefinePrimaryKey(DbColumn column)
		{
			if (this.primaryKeys != null)
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' may not change its primary key", this.Name));
			}

			this.PrimaryKeys.Add (column);

			System.Diagnostics.Debug.Assert (this.primaryKeys.Count == 1);
			System.Diagnostics.Debug.Assert (this.primaryKeys[0] == column);
		}

		/// <summary>
		/// Defines the localizations for this table.
		/// </summary>
		/// <param name="localizations">The localizations.</param>
		public void DefineLocalizations(IEnumerable<string> localizations)
		{
			if (string.IsNullOrEmpty (this.localizations))
			{
				string[] array = Collection.ToArray<string> (localizations);
				this.localizations = array.Length == 0 ? null : string.Join ("/", array);
			}
			else
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' may not change its localization", this.Name));
			}
		}


		/// <summary>
		/// Creates an SQL table definition based on this high level table definition.
		/// </summary>
		/// <param name="converter">The type converter.</param>
		/// <returns>An SQL table definition.</returns>
		public SqlTable CreateSqlTable(ITypeConverter converter)
		{
			System.Diagnostics.Debug.Assert (this.ReplicationMode != DbReplicationMode.Unknown);

			SqlTable sqlTable = new SqlTable (this.GetSqlName ());

			foreach (DbColumn dbColumn in this.columns)
			{
				if ((dbColumn.IsPrimaryKey) &&
					(dbColumn.Localization != DbColumnLocalization.None))
				{
					throw new Exceptions.SyntaxException (DbAccess.Empty, string.Format ("Primary key '{0}' may not be localized", dbColumn.Name));
				}

				foreach (SqlColumn sqlColumn in this.CreateSqlColumns (converter, dbColumn))
				{
					//	Make sure we don't try to create an SQL column more than once.

					if (sqlTable.Columns.IndexOf (sqlColumn.Name) >= 0)
					{
						string message = string.Format ("Multiple columns with same name ({0}) are forbidden", sqlColumn.Name);
						throw new Exceptions.SyntaxException (DbAccess.Empty, message);
					}

					sqlTable.Columns.Add (sqlColumn);
				}
			}

			if (this.HasPrimaryKeys)
			{
				//	If there are primary keys for this table, simple map them to the
				//	already created SQL columns.

				int n = this.PrimaryKeys.Count;

				SqlColumn[] primaryKeys = new SqlColumn[n];

				for (int i = 0; i < n; i++)
				{
					DbColumn dbPrimaryKey   = this.PrimaryKeys[i];
					string   primaryKeyName = dbPrimaryKey.GetSqlName ();

					if (sqlTable.Columns.IndexOf (primaryKeyName) < 0)
					{
						string message = string.Format ("Primary key {0} not found in columns", primaryKeyName);
						throw new Exceptions.SyntaxException (DbAccess.Empty, message);
					}

					SqlColumn sqlPrimaryKey = sqlTable.Columns[primaryKeyName];

					if (sqlPrimaryKey.IsNullable)
					{
						string message = string.Format ("Primary key {0} may not be nullable", primaryKeyName);
						throw new Exceptions.SyntaxException (DbAccess.Empty, message);
					}

					primaryKeys[i] = sqlPrimaryKey;
				}

				sqlTable.PrimaryKey = primaryKeys;
			}

			return sqlTable;
		}

		/// <summary>
		/// Creates the SQL columns for a given column.
		/// </summary>
		/// <param name="converter">The type converter.</param>
		/// <param name="column">The column.</param>
		/// <returns>The SQL columns.</returns>
		internal IEnumerable<SqlColumn> CreateSqlColumns(ITypeConverter converter, DbColumn column)
		{
			if (column.Localization == DbColumnLocalization.Localized)
			{
				System.Diagnostics.Debug.Assert (string.IsNullOrEmpty (this.localizations) == false);

				foreach (string localizationSuffix in this.Localizations)
				{
					yield return column.CreateSqlColumn (converter, localizationSuffix);
				}
			}
			else
			{
				yield return column.CreateSqlColumn (converter, null);
			}
		}

		/// <summary>
		/// Creates the SQL name for this table.
		/// </summary>
		/// <returns>The SQL name.</returns>
		public string GetSqlName()
		{
			return DbSqlStandard.MakeSqlTableName (this.Name, this.Category, this.Key);
		}

		/// <summary>
		/// Gets the name of the corresponding revision table.
		/// </summary>
		/// <returns>The name of the corresponding revision table or <c>null</c> if
		/// the table is not revisioned.</returns>
		public string GetRevisionTableName()
		{
			if (this.RevisionMode == DbRevisionMode.Enabled)
			{
				return DbSqlStandard.MakeSqlTableName (this.Name, DbElementCat.RevisionHistory, this.Key);
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Extracts the key from the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The key found in the row.</returns>
		public DbKey ExtractKeyFromRow(System.Data.DataRow row)
		{
			DbKey key = DbKey.Empty;

			switch (this.category)
			{
				case DbElementCat.Internal:
				case DbElementCat.ManagedUserData:
					if (this.PrimaryKeys.Count == 1)
					{
						System.Diagnostics.Debug.Assert (this.PrimaryKeys[0].Name == Tags.ColumnId);

						key = new DbKey (DbKey.GetRowId (row), DbKey.GetRowStatus (row));
					}
					break;

				default:
					if (this.PrimaryKeys.Count == 1)
					{
						key = new DbKey ((long) row[this.PrimaryKeys[0].Name]);
					}
					break;
			}

			if (key.IsEmpty)
			{
				throw new Exceptions.GenericException (DbAccess.Empty, string.Format ("Table {0} uses unsupported key format", this.Name));
			}

			return key;
		}

		/// <summary>
		/// Gets the number of SQL columns. This may be different from the number
		/// of columns defined in the table (localized columns need several columns
		/// to represent their data, for instance).
		/// </summary>
		/// <returns>The number of SQL columns.</returns>
		public int GetSqlColumnCount()
		{
			int localizationCount = this.LocalizationCount;

			if (localizationCount > 1)
			{
				int count = 0;

				foreach (DbColumn column in this.columns)
				{
					if (column.Localization == DbColumnLocalization.Localized)
					{
						count += localizationCount;
					}
					else
					{
						count += 1;
					}
				}
				
				return count;
			}
			else
			{
				return this.columns.Count;
			}
		}

		/// <summary>
		/// Updates the primary key flags of the table columns.
		/// </summary>
		internal void UpdatePrimaryKeyInfo()
		{
			if (this.HasPrimaryKeys)
			{
				foreach (DbColumn column in this.Columns)
				{
					column.DefinePrimaryKey (false);
				}
				foreach (DbColumn column in this.PrimaryKeys)
				{
					column.DefinePrimaryKey (true);
				}
			}
		}

		#region IXmlSerializable Members

		/// <summary>
		/// Serializes the instance using the specified XML writer.
		/// </summary>
		/// <param name="xmlWriter">The XML writer.</param>
		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("table");

			DbTools.WriteAttribute (xmlWriter, "cat", DbTools.ElementCategoryToString (this.category));
			DbTools.WriteAttribute (xmlWriter, "rev", DbTools.RevisionModeToString (this.RevisionMode));
			DbTools.WriteAttribute (xmlWriter, "rep", DbTools.ReplicationModeToString (this.replicationMode));
			DbTools.WriteAttribute (xmlWriter, "l10n", DbTools.StringToString (this.localizations));

			xmlWriter.WriteEndElement ();
		}

		#endregion

		/// <summary>
		/// Deserializes a <c>DbTable</c> from its XML representation.
		/// </summary>
		/// <param name="xmlReader">The XML reader.</param>
		/// <returns>The <c>DbTable</c>.</returns>
		public static DbTable Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.Name == "table"))
			{
				DbTable table = new DbTable ();
				bool isEmptyElement = xmlReader.IsEmptyElement;

				table.category         = DbTools.ParseElementCategory (xmlReader.GetAttribute ("cat"));
				table.revisionMode    = DbTools.ParseRevisionMode (xmlReader.GetAttribute ("rev"));
				table.replicationMode = DbTools.ParseReplicationMode (xmlReader.GetAttribute ("rep"));
				table.localizations    = DbTools.ParseString (xmlReader.GetAttribute ("l10n"));

				if (!isEmptyElement)
				{
					xmlReader.ReadEndElement ();
				}

				return table;
			}
			else
			{
				throw new System.Xml.XmlException (string.Format ("Unexpected element {0}", xmlReader.LocalName), null, xmlReader.LineNumber, xmlReader.LinePosition);
			}
		}

		/// <summary>
		/// Creates a foreign key reference column pointing to the specified target
		/// table.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="targetTableName">Name of the target table.</param>
		/// <param name="nullability">The column nullability.</param>
		/// <returns>The column.</returns>
		public static DbColumn CreateRefColumn(string columnName, string targetTableName, DbNullability nullability)
		{
			System.Diagnostics.Debug.Assert (nullability != DbNullability.Undefined);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (targetTableName));

			DbTypeDef  type = new DbTypeDef (nullability == DbNullability.Yes ? Res.Types.Num.NullableKeyId : Res.Types.Num.KeyId);
			DbColumn column = new DbColumn (columnName, type, DbColumnClass.RefId, DbElementCat.ManagedUserData);

			column.DefineTargetTableName (targetTableName);

			return column;
		}

		/// <summary>
		/// Creates a column for user data.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="type">The type.</param>
		/// <returns>The column.</returns>
		public static DbColumn CreateUserDataColumn(string columnName, DbTypeDef type)
		{
			System.Diagnostics.Debug.Assert (type != null);

			return new DbColumn (columnName, type, DbColumnClass.Data, DbElementCat.ManagedUserData);
		}

		/// <summary>
		/// Creates a column for user data.
		/// </summary>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="type">The type.</param>
		/// <param name="revisionMode">The revision mode.</param>
		/// <returns>The column.</returns>
		public static DbColumn CreateUserDataColumn(string columnName, DbTypeDef type, DbRevisionMode revisionMode)
		{
			System.Diagnostics.Debug.Assert (type != null);

			return new DbColumn (columnName, type, DbColumnClass.Data, DbElementCat.ManagedUserData, revisionMode);
		}

		#region Private Methods

		private void HandleColumnInserted(object sender, ValueEventArgs e)
		{
			//	When a column is added to the table, the column must be made aware
			//	of the relation with its containing table.
			
			DbColumn column = e.Value as DbColumn;
			column.DefineTable (this);
		}

		private void HandleColumnRemoved(object sender, ValueEventArgs e)
		{
			DbColumn column = e.Value as DbColumn;
			column.DefineTable (null);
		}

		#endregion

		private static readonly Caption			nullCaption = new Caption ();

		private DbKey							key;
		private string							name;
		private Druid							captionId;
		private Caption							caption;
		private string							localizations;

		private Collections.DbColumns			columns;
		private Collections.DbColumns			primaryKeys;
		private DbElementCat					category;
		private DbRevisionMode					revisionMode;
		private DbReplicationMode				replicationMode;
	}
}
