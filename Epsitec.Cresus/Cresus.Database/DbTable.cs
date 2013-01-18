//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
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
		

		// TODO Make sure that this serialization/deserialization index stuff is ok, because it seems
		// a little bit strange.
		// Marc
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DbTable"/> class.
		/// </summary>
		public DbTable()
		{
			this.columns = new Collections.DbColumnList ();

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
			this.DefineDisplayName (name);
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

		// HACK: I added this hack to bypass the test for relation table in the GetSqlName() method.
		// The trouble is that this method adds a suffix after the table name if the table does not
		// represent a Druid. This was done in order to allow several table with the same high level
		// name but with different low level names. Now, this suffix was removed for relation tables
		// which are supposed to have unique names because the name of their table is included in
		// them. So, for the entities, before I made some changes, their relations where stored as
		// relation tables (in the sense that their category was Relation). So their suffix was
		// removed. But now that they are regular tables and are not associated with a Druid, the
		// have the suffix included in their name. We don't want that, because the suffix messes up
		// everything when exporting and importing. I totally overlooked that suffix stuff when
		// designing the importation/exportation stuff. So I create this property in order to remove
		// the suffix in some cases. As the name implies, this is an ugly hack designed to bypass all
		// that stuff. It is not definitive and I will fix all this stuff properly once I come back
		// to work in Crésus.Core. I promise ;-P
		// Marc
		public bool EnableUglyHackInOrderToRemoveSuffixFromTableName
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the display name of the table. If no name is defined, tries to use
		/// the caption name instead.
		/// </summary>
		/// <value>The display name of the table.</value>
		public string							DisplayName
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

		public string Comment
		{
			get;
			set;
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
				if (this.captionId.IsValid)
				{
					return DbTable.GetEntityTableName (this.CaptionId);
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
		/// Gets the columns defined for the table.
		/// </summary>
		/// <value>The columns.</value>
		public Collections.DbColumnList			Columns
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
		public Collections.DbColumnList			PrimaryKeys
		{
			get
			{
				if (this.primaryKeys == null)
				{
					this.primaryKeys = new Collections.DbColumnList ();
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
		/// Gets the table column indexes.
		/// </summary>
		/// <value>The table column indexes.</value>
		public IList<DbIndex>					Indexes
		{
			get
			{
				if (this.serializedIndexTuples != null)
				{
					this.DeserializeIndexes ();
				}
				
				if (this.indexes == null)
				{
					this.indexes = new List<DbIndex> ();
				}

				return this.indexes;
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
		/// Gets the name of the source table if this table is a relation table.
		/// </summary>
		/// <value>The name of the source table.</value>
		public string							RelationSourceTableName
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.category == DbElementCat.Relation);
				return this.relationSourceTableName;
			}
		}

		/// <summary>
		/// Gets the name of the target table if this table is a relation table.
		/// </summary>
		/// <value>The name of the target table.</value>
		public string							RelationTargetTableName
		{
			get
			{
				System.Diagnostics.Debug.Assert (this.category == DbElementCat.Relation);
				return this.relationTargetTableName;
			}
		}

		/// <summary>
		/// Defines the name for this table. A table name may not be changed
		/// after it has been defined.
		/// </summary>
		/// <param name="value">The table name.</param>
		public void DefineDisplayName(string value)
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
		/// Adds an index for the table.
		/// </summary>
		/// <param name="name">The name of the index.</param>
		/// <param name="sortOrder">The sort order.</param>
		/// <param name="columns">The columns.</param>
		public void AddIndex(string name, SqlSortOrder sortOrder, params DbColumn[] columns)
		{
			if (columns.Length > 0)
			{
				this.Indexes.Add (new DbIndex (name, columns, sortOrder));
			}
		}


		/// <summary>
		/// Creates an SQL table definition based on this high level table definition.
		/// </summary>
		/// <param name="converter">The type converter.</param>
		/// <returns>An SQL table definition.</returns>
		public SqlTable CreateSqlTable(ITypeConverter converter)
		{
			SqlTable sqlTable = new SqlTable (this.GetSqlName ());

			sqlTable.Comment = this.Comment;

			foreach (DbColumn dbColumn in this.columns)
			{
				if (dbColumn.Cardinality == DbCardinality.None)
				{
					SqlColumn sqlColumn = dbColumn.CreateSqlColumn (converter);

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

			foreach (DbIndex index in this.Indexes)
			{
				int n = index.Columns.Count;
				SqlColumn[] indexColumns = new SqlColumn[n];

				for (int i = 0; i < n; i++)
				{
					DbColumn dbIndexColumn = index.Columns[i];
					string indexColumnName = dbIndexColumn.GetSqlName ();

					if (sqlTable.Columns.IndexOf (indexColumnName) < 0)
					{
						string message = string.Format ("Index column {0} not found", indexColumnName);
						throw new Exceptions.SyntaxException (DbAccess.Empty, message);
					}

					indexColumns[i] = sqlTable.Columns[indexColumnName];
				}

				sqlTable.AddIndex (index.Name, index.SortOrder, indexColumns);
			}

			return sqlTable;
		}

		/// <summary>
		/// Creates the SQL name for this table.
		/// </summary>
		/// <returns>The SQL name.</returns>
		public string GetSqlName()
		{
			if (this.category == DbElementCat.Relation || this.EnableUglyHackInOrderToRemoveSuffixFromTableName)
			{
				return DbSqlStandard.MakeSqlTableName (this.Name, false, this.Category, this.Key);
			}
			else
			{
				return DbSqlStandard.MakeSqlTableName (this.Name, this.CaptionId.IsEmpty, this.Category, this.Key);
			}
		}

		/// <summary>
		/// Gets the name of the relation table for the specified column.
		/// </summary>
		/// <param name="sourceColumn">The source column.</param>
		/// <returns>The name of the relation table.</returns>
		public string GetRelationTableName(DbColumn sourceColumn)
		{
			System.Diagnostics.Debug.Assert (sourceColumn.Cardinality != DbCardinality.None);
			return DbTable.GetRelationTableName (this.Name, sourceColumn.Name);
		}

		/// <summary>
		/// Gets the name of the relation table for the specified table and column.
		/// </summary>
		/// <param name="sourceTableName">Name of the source table.</param>
		/// <param name="sourceColumnName">Name of the source column.</param>
		/// <returns>The name of the relation table.</returns>
		public static string GetRelationTableName(string sourceTableName, string sourceColumnName)
		{
			return string.Concat (sourceColumnName, ":", sourceTableName);
		}


		public static string GetEntityTableName(Druid entityId)
		{
			return Druid.ToFullString (entityId.ToLong ());
		}

		
		/// <summary>
		/// Extracts the key from the specified row.
		/// </summary>
		/// <param name="row">The row.</param>
		/// <returns>The key found in the row.</returns>
		public DbKey ExtractKeyFromRow(System.Data.DataRow row)
		{
			// TODO Unused method? Should it be deleted?
			// Marc

			DbKey key = DbKey.Empty;

			switch (this.category)
			{
				case DbElementCat.Internal:
				case DbElementCat.ManagedUserData:
					if (this.PrimaryKeys.Count == 1)
					{
						System.Diagnostics.Debug.Assert (this.PrimaryKeys[0].Name == Tags.ColumnId);

						key = new DbKey (DbKey.GetRowId (row));
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
		/// Updates the primary key flags of the table columns.
		/// </summary>
		public void UpdatePrimaryKeyInfo()
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
			DbTools.WriteAttribute (xmlWriter, "typ", DbTools.DruidToString (this.captionId));
			DbTools.WriteAttribute (xmlWriter, "idx", DbTools.StringToString (this.SerializeIndexes (this.indexes)));
			DbTools.WriteAttribute (xmlWriter, "rstn", DbTools.StringToString (this.relationSourceTableName));
			DbTools.WriteAttribute (xmlWriter, "rttn", DbTools.StringToString (this.relationTargetTableName));
			DbTools.WriteAttribute (xmlWriter, "com", DbTools.StringToString (this.Comment));
			DbTools.WriteAttribute (xmlWriter, "uhr", DbTools.BoolDefaultingToFalseToString (this.EnableUglyHackInOrderToRemoveSuffixFromTableName));

			xmlWriter.WriteEndElement ();
		}

		#endregion


		private void DeserializeIndexes()
		{
			string source = this.serializedIndexTuples;
			this.serializedIndexTuples = null;

			if (string.IsNullOrEmpty (source))
			{
				//	Nothing to do...
			}
			else
			{
				string[] sourceTuples = source.Split (';');
				
				for (int i = 0; i < sourceTuples.Length; i++)
				{
					this.DeserializeIndexTuple (sourceTuples[i]);
				}
			}
		}

		private void DeserializeIndexTuple(string source)
		{
			string[] args = source.Split (',');

			SqlSortOrder sortOrder;

			switch (args[0])
			{
				case "A":
					sortOrder = SqlSortOrder.Ascending;
					break;

				case "D":
					sortOrder = SqlSortOrder.Descending;
					break;

				case "N":
					sortOrder = SqlSortOrder.None;
					break;

				default:
					throw new Exceptions.SyntaxException (DbAccess.Empty, "Cannot deserialize index tuple");
			}

			string name = args[1];

			DbColumn[] columns = new DbColumn[args.Length-2];

			for (int i = 2; i < args.Length; i++)
			{
				string columnName = args[i];
				columns[i-2] = this.Columns[columnName];
			}

			this.AddIndex (name, sortOrder, columns);
		}

		private string SerializeIndexes(IEnumerable<DbIndex> indexes)
		{
			if (indexes == null)
			{
				return null;
			}

			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();

			foreach (DbIndex index in indexes)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (";");
				}
				
				this.SerializeIndexTuple (buffer, index);
			}

			return buffer.ToString ();
		}

		private void SerializeIndexTuple(System.Text.StringBuilder buffer, DbIndex index)
		{
			switch (index.SortOrder)
			{
				case SqlSortOrder.Ascending:
					buffer.Append ("A");
					break;
				
				case SqlSortOrder.Descending:
					buffer.Append ("D");
					break;
				
				case SqlSortOrder.None:
					buffer.Append ("N");
					break;

				default:
					throw new System.NotSupportedException ();
			}

			buffer.Append ("," + index.Name);

			foreach (DbColumn column in index.Columns)
			{
				buffer.Append (",");
				buffer.Append (column.Name);
			}
		}


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

				table.category                = DbTools.ParseElementCategory (xmlReader.GetAttribute ("cat"));
				table.captionId               = DbTools.ParseDruid (xmlReader.GetAttribute ("typ"));
				table.relationSourceTableName = DbTools.ParseString (xmlReader.GetAttribute ("rstn"));
				table.relationTargetTableName = DbTools.ParseString (xmlReader.GetAttribute ("rttn"));
				table.serializedIndexTuples   = DbTools.ParseString (xmlReader.GetAttribute ("idx"));
				table.Comment				  = DbTools.ParseString (xmlReader.GetAttribute ("com"));
				table.EnableUglyHackInOrderToRemoveSuffixFromTableName = DbTools.ParseDefaultingToFalseBool (xmlReader.GetAttribute ("uhr"));
				
				// NOTE Here we do not deserialize the indexes but keep them in their serialized
				// form because the columns are not yet deserialized and added to the table.
				// Therefore we cannot deserialize the indexes because the columns are missing. We
				// will deserialize it as soon as it is accessed by the property this.Index.
				// Marc

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

		public void EnsureIsDeserialized()
		{
			var tableCaption = this.Caption;
			var tablePrimaryKeys = this.PrimaryKeys;
			var tableIndexes = this.Indexes;

			foreach (DbColumn column in this.Columns)
			{
				column.EnsureIsDeserialized ();
			}
		}

		/// <summary>
		/// Creates a foreign key reference column pointing to the specified target
		/// table.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="infrastructure">The database infrastructure.</param>
		/// <param name="columnName">Name of the column.</param>
		/// <param name="targetTableName">Name of the target table.</param>
		/// <param name="nullability">The column nullability.</param>
		/// <returns>The column.</returns>
		public static DbColumn CreateRefColumn(DbTransaction transaction, DbInfrastructure infrastructure, string columnName, string targetTableName, DbNullability nullability)
		{
			System.Diagnostics.Debug.Assert (nullability != DbNullability.Undefined);
			System.Diagnostics.Debug.Assert (!string.IsNullOrEmpty (targetTableName));

			DbTypeDef  type = infrastructure.ResolveDbType (transaction, nullability == DbNullability.Yes ? Tags.TypeNullableKeyId : Tags.TypeKeyId);
			DbColumn column = new DbColumn (columnName, type, DbColumnClass.RefId, DbElementCat.ManagedUserData);

			column.DefineTargetTableName (targetTableName);

			return column;
		}

		/// <summary>
		/// Creates the relation column.
		/// </summary>
		/// <param name="columnCaptionId">The column caption id.</param>
		/// <param name="targetTable">The target table.</param>
		/// <param name="cardinality">The cardinality.</param>
		/// <returns>The column.</returns>
		public static DbColumn CreateRelationColumn(Druid columnCaptionId, DbTable targetTable, DbCardinality cardinality)
		{
			System.Diagnostics.Debug.Assert (targetTable != null);
			System.Diagnostics.Debug.Assert (cardinality != DbCardinality.None);

			DbColumn column = new DbColumn (columnCaptionId, null, DbColumnClass.Virtual, DbElementCat.ManagedUserData, null);

			column.DefineCardinality (cardinality);
			column.DefineTargetTableName (targetTable.Name);

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
		/// Creates the relation table for the specified source table and column.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="sourceTable">The source table.</param>
		/// <param name="sourceColumn">The source column.</param>
		/// <returns>The relation table.</returns>
		public static DbTable CreateRelationTable(DbInfrastructure infrastructure, DbTable sourceTable, DbColumn sourceColumn)
		{
			string sourceTableName   = sourceTable.Name;
			string targetTableName   = sourceColumn.TargetTableName;
			string relationTableName = sourceTable.GetRelationTableName (sourceColumn);
			
			DbTable relationTable = new DbTable (relationTableName);

			relationTable.Comment = sourceTable.DisplayName + ":" + sourceColumn.DisplayName;

			relationTable.DefineCategory (DbElementCat.Relation);
			relationTable.relationSourceTableName = sourceTableName;
			relationTable.relationTargetTableName = targetTableName;

			DbTypeDef refIdType  = infrastructure.ResolveDbType (Tags.TypeKeyId);
			DbTypeDef rankType   = infrastructure.ResolveDbType (Tags.TypeCollectionRank);

			relationTable.Columns.Add (new DbColumn (Tags.ColumnId, refIdType, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true,
				AutoIncrementStartValue = 0
			});
			relationTable.Columns.Add (new DbColumn (Tags.ColumnRefSourceId, refIdType, DbColumnClass.RefInternal, DbElementCat.Internal));
			relationTable.Columns.Add (new DbColumn (Tags.ColumnRefTargetId, refIdType, DbColumnClass.RefInternal, DbElementCat.Internal));
			relationTable.Columns.Add (new DbColumn (Tags.ColumnRefRank, rankType, DbColumnClass.Data, DbElementCat.Internal));

			relationTable.PrimaryKeys.Add (relationTable.Columns[Tags.ColumnId]);

			string indexName = "IDX_" + relationTable.GetSqlName ();

			relationTable.AddIndex (indexName, SqlSortOrder.Ascending, relationTable.Columns[Tags.ColumnRefSourceId], relationTable.Columns[Tags.ColumnRefTargetId]);

			relationTable.UpdatePrimaryKeyInfo ();

			return relationTable;
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

		private Collections.DbColumnList		columns;
		private Collections.DbColumnList		primaryKeys;
		private List<DbIndex>					indexes;
		private string							serializedIndexTuples;
		private DbElementCat					category;
		private string							relationSourceTableName;
		private string							relationTargetTableName;
	}
}
