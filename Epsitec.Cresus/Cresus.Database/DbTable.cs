//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbTable décrit la structure d'une table dans la base de données.
	/// Cette classe ressemble dans l'esprit à System.Data.DataTable.
	/// </summary>
	public class DbTable : ICaption, IName, IXmlSerializable
	{
		public DbTable()
		{
			this.AttachColumns (new Collections.DbColumns ());
		}

		public DbTable(string name)
			: this ()
		{
			this.name = name;
		}


		public string Name
		{
			get
			{
				return this.name;
			}
		}

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

		public string[] Localizations
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

		public int LocalizationCount
		{
			get
			{
				if (string.IsNullOrEmpty (this.localizations))
				{
					return 1;
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

		public Collections.DbColumns Columns
		{
			get
			{
				return this.columns;
			}
		}

		public bool HasPrimaryKey
		{
			get
			{
				return (this.primary_keys != null) && (this.primary_keys.Count > 0);
			}
		}

		public Collections.DbColumns PrimaryKeys
		{
			get
			{
				//	NB: les clefs primaires spécifiées par PrimaryKeys sont utilisées
				//	pour former un 'tuple' (par exemple une paire de clef). Déclarer
				//	une série de colonnes comme PrimaryKeys implique que les tuples
				//	doivent être uniques !

				if (this.primary_keys == null)
				{
					this.primary_keys = new Collections.DbColumns ();
				}

				return this.primary_keys;
			}
		}

		public DbForeignKey[] ForeignKeys
		{
			get
			{
				System.Collections.ArrayList list = new System.Collections.ArrayList ();

				foreach (DbColumn column in this.Columns)
				{
					switch (column.ColumnClass)
					{
						case DbColumnClass.RefId:
							list.Add (new DbForeignKey (column));
							break;
					}
				}

				DbForeignKey[] keys = new DbForeignKey[list.Count];
				list.CopyTo (keys);
				return keys;
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

		public DbReplicationMode ReplicationMode
		{
			get
			{
				return this.replication_mode;
			}
		}

		public DbKey Key
		{
			get
			{
				return this.key;
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
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot define a new category.", this.Name));
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
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot define a new revision mode.", this.Name));
			}

			this.revision_mode = revision_mode;
		}

		public void DefineReplicationMode(DbReplicationMode replication_mode)
		{
			if (this.replication_mode == replication_mode)
			{
				return;
			}

			if (this.replication_mode != DbReplicationMode.Unknown)
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot define a new replication mode.", this.Name));
			}

			this.replication_mode = replication_mode;
		}

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
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot change its internal key.", this.Name));
			}
		}

		public void DefinePrimaryKey(DbColumn column)
		{
			if (this.primary_keys != null)
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot change its primary key.", this.Name));
			}

			this.PrimaryKeys.Add (column);

			System.Diagnostics.Debug.Assert (this.primary_keys.Count == 1);
			System.Diagnostics.Debug.Assert (this.primary_keys[0] == column);
		}

		public void DefineLocalizations(IEnumerable<string> localizations)
		{
			string[] array = Collection.ToArray<string> (localizations);
			this.localizations = array.Length == 0 ? null : string.Join ("/", array);
		}

		public SqlTable CreateSqlTable(ITypeConverter type_converter)
		{
			System.Diagnostics.Debug.Assert (this.ReplicationMode != DbReplicationMode.Unknown);

			SqlTable sql_table = new SqlTable (this.CreateSqlName ());

			foreach (DbColumn db_column in this.columns)
			{
				if ((db_column.IsPrimaryKey) &&
					(db_column.Localization != DbColumnLocalization.None))
				{
					throw new Exceptions.SyntaxException (DbAccess.Empty, string.Format ("Primary key '{0}' may not be localized", db_column.Name));
				}
				
				//	TODO: handle multiple cultures...
				SqlColumn sql_column = db_column.CreateSqlColumn (type_converter, null);

				//	Vérifions juste que personne n'introduit deux fois la même colonne dans une
				//	définition de table.

				if (sql_table.Columns.IndexOf (sql_column.Name) >= 0)
				{
					string message = string.Format ("Multiple columns with same name ({0}) are forbidden", sql_column.Name);
					throw new Exceptions.SyntaxException (DbAccess.Empty, message);
				}

				sql_table.Columns.Add (sql_column);
			}

			if (this.HasPrimaryKey)
			{
				//	S'il y a des clefs primaires définies, reprend simplement les clefs qui
				//	correspondent (elles doivent être définies dans la collection des colonnes
				//	de la table).

				int n = this.PrimaryKeys.Count;

				SqlColumn[] primary_keys = new SqlColumn[n];

				for (int i = 0; i < n; i++)
				{
					DbColumn db_key   = this.PrimaryKeys[i];
					string key_name = db_key.CreateSqlName ();

					if (sql_table.Columns.IndexOf (key_name) < 0)
					{
						string message = string.Format ("Primary key {0} not found in columns", key_name);
						throw new Exceptions.SyntaxException (DbAccess.Empty, message);
					}

					SqlColumn sql_key  = sql_table.Columns[key_name];

					if (sql_key.IsNullable)
					{
						string message = string.Format ("Primary key {0} may not be nullable", key_name);
						throw new Exceptions.SyntaxException (DbAccess.Empty, message);
					}

					primary_keys[i] = sql_key;
				}

				sql_table.PrimaryKey = primary_keys;
			}

			return sql_table;
		}

		public string CreateSqlName()
		{
			return DbSqlStandard.MakeSqlTableName (this.Name, this.Category, this.Key);
		}

		public DbKey CreateKeyFromRow(System.Data.DataRow row)
		{
			DbKey key = DbKey.Empty;

			switch (this.category)
			{
				case DbElementCat.Internal:
				case DbElementCat.ManagedUserData:
					if (this.PrimaryKeys.Count == 1)
					{
						System.Diagnostics.Debug.Assert (this.PrimaryKeys[0].Name.ToUpper () == Tags.ColumnId);

						long id = (long) row[Tags.ColumnId];

						key = new DbKey (id);
					}
					break;

				default:
					if (this.PrimaryKeys.Count == 1)
					{
						long id = (long) row[this.PrimaryKeys[0].Name];

						key = new DbKey (id);
					}
					break;
			}

			if (key.IsEmpty)
			{
				throw new Exceptions.GenericException (DbAccess.Empty, string.Format ("Table {0} uses unsupported key format.", this.Name));
			}

			return key;
		}

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

		internal void UpdatePrimaryKeyInfo()
		{
			if (this.HasPrimaryKey)
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

		public void Serialize(System.Xml.XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("table");

			DbTools.WriteAttribute (xmlWriter, "cat", DbTools.ElementCategoryToString (this.category));
			DbTools.WriteAttribute (xmlWriter, "rev", DbTools.RevisionModeToString (this.revision_mode));
			DbTools.WriteAttribute (xmlWriter, "rep", DbTools.ReplicationModeToString (this.replication_mode));
			DbTools.WriteAttribute (xmlWriter, "l10n", DbTools.StringToString (this.localizations));

			xmlWriter.WriteEndElement ();
		}

		#endregion

		public static DbTable Deserialize(System.Xml.XmlTextReader xmlReader)
		{
			if ((xmlReader.NodeType == System.Xml.XmlNodeType.Element) &&
				(xmlReader.Name == "table"))
			{
				DbTable table = new DbTable ();
				bool isEmptyElement = xmlReader.IsEmptyElement;

				table.category         = DbTools.ParseElementCategory (xmlReader.GetAttribute ("cat"));
				table.revision_mode    = DbTools.ParseRevisionMode (xmlReader.GetAttribute ("rev"));
				table.replication_mode = DbTools.ParseReplicationMode (xmlReader.GetAttribute ("rep"));
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


		private void AttachColumns(Collections.DbColumns columns)
		{
			this.columns = columns;

			this.columns.ItemInserted += this.HandleColumnInserted;
			this.columns.ItemRemoved += this.HandleColumnRemoved;

			foreach (DbColumn column in this.columns)
			{
				column.DefineTable (this);
			}
		}


		private void HandleColumnInserted(object sender, ValueEventArgs e)
		{
			DbColumn column = e.Value as DbColumn;
			column.DefineTable (this);
		}

		private void HandleColumnRemoved(object sender, ValueEventArgs e)
		{
			DbColumn column = e.Value as DbColumn;
			column.DefineTable (null);
		}

		private static readonly Caption nullCaption = new Caption ();

		private string name;
		private Druid captionId;
		private Caption caption;
		private string localizations;

		protected Collections.DbColumns columns;
		protected Collections.DbColumns primary_keys;
		protected DbElementCat category;
		protected DbRevisionMode revision_mode;
		protected DbReplicationMode replication_mode;
		protected DbKey key;

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
	}
}
