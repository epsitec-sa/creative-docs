//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
	/// <summary>
	/// La classe DbTable décrit la structure d'une table dans la base de données.
	/// Cette classe ressemble dans l'esprit à System.Data.DataTable.
	/// </summary>
	public class DbTable : IDbAttributesHost, Common.Types.INameCaption
	{
		public DbTable()
		{
		}
		
		public DbTable(string name)
		{
			this.Attributes.SetAttribute (Tags.Name, name);
		}
		
		public DbTable(System.Xml.XmlElement xml)
		{
			this.ProcessXmlDefinition (xml);
		}
		
		
		public static DbTable CreateTable(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml (xml);
			
			return DbTable.CreateTable (doc.DocumentElement);
		}
		
		public static DbTable CreateTable(System.Xml.XmlElement xml)
		{
			return (xml.Name == "null") ? null : new DbTable (xml);
		}
		
		
		public string							Name
		{
			get { return this.Attributes[Tags.Name, ResourceLevel.Default]; }
		}
		
		public string							Caption
		{
			get { return this.Attributes[Tags.Caption]; }
		}
		
		public string							Description
		{
			get { return this.Attributes[Tags.Description]; }
		}
		
		
		public DbAttributes						Attributes
		{
			get
			{
				return this.attributes;
			}
		}
		
		
		public Collections.DbColumns			Columns
		{
			get { return this.columns; }
		}
		
		public bool								HasPrimaryKey
		{
			get { return (this.primary_keys != null) && (this.primary_keys.Count > 0); }
		}
		
		public Collections.DbColumns			PrimaryKeys
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
		
		public DbElementCat						Category
		{
			get { return this.category; }
		}
		
		public DbKey							InternalKey
		{
			get { return this.internal_table_key; }
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
		
		public void DefineInternalKey(DbKey key)
		{
			if (this.internal_table_key == key)
			{
				return;
			}
			
			if (this.internal_table_key != null)
			{
				throw new System.InvalidOperationException (string.Format ("Table '{0}' cannot change its internal key.", this.Name));
			}
			
			this.internal_table_key = key.Clone () as DbKey;
		}
		
		
		public SqlTable CreateSqlTable(ITypeConverter type_converter)
		{
			SqlTable sql_table = new SqlTable (this.CreateSqlName ());
			
			foreach (DbColumn db_column in this.columns)
			{
				SqlColumn sql_column = db_column.CreateSqlColumn (type_converter);
				
				//	Vérifions juste que personne n'introduit deux fois la même colonne dans une
				//	définition de table.
				
				if (sql_table.Columns.IndexOf (sql_column.Name) >= 0)
				{
					string message = string.Format ("Multiple columns with same name ({0}) are forbidden", sql_column.Name);
					throw new DbSyntaxException (DbAccess.Empty, message);
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
					string   key_name = db_key.CreateSqlName ();
					
					if (sql_table.Columns.IndexOf (key_name) < 0)
					{
						string message = string.Format ("Primary key {0} not found in columns", key_name);
						throw new DbSyntaxException (DbAccess.Empty, message);
					}
					
					SqlColumn sql_key  = sql_table.Columns[key_name];
					
					if (sql_key.IsNullAllowed)
					{
						string message = string.Format ("Primary key {0} may not be nullable", key_name);
						throw new DbSyntaxException (DbAccess.Empty, message);
					}
					
					primary_keys[i] = sql_key;
				}
				
				sql_table.PrimaryKey = primary_keys;
			}
			
			return sql_table;
		}
		
		public string   CreateSqlName()
		{
			return DbSqlStandard.MakeSqlTableName (this.Name, this.Category, this.InternalKey);
		}
		
		public DbKey    CreateKeyFromRow(System.Data.DataRow row)
		{
			DbKey key = null;
			
			switch (this.category)
			{
				case DbElementCat.Internal:
				case DbElementCat.UserDataManaged:
					if (this.PrimaryKeys.Count == 1)
					{
						System.Diagnostics.Debug.Assert (this.PrimaryKeys[0].Name.ToUpper () == Tags.ColumnId);
						
						long id = (long) row[Tags.ColumnId];
						
						key = new DbKey (id);
					}
					else if (this.PrimaryKeys.Count == 2)
					{
						System.Diagnostics.Debug.Assert (this.PrimaryKeys[0].Name.ToUpper () == Tags.ColumnId);
						System.Diagnostics.Debug.Assert (this.PrimaryKeys[1].Name.ToUpper () == Tags.ColumnRevision);
						
						long id   = (long) row[Tags.ColumnId];
						int  rev  = (int)  row[Tags.ColumnRevision];
						int  stat = (int)  row[Tags.ColumnStatus];
						
						key = new DbKey (id, rev, DbKey.ConvertFromIntStatus (stat));
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
			
			if (key == null)
			{
				throw new DbException (DbAccess.Empty, string.Format ("Table {0} uses unsupported key format.", this.Name));
			}
			
			return key;
		}
		
		
		public static string SerializeToXml(DbTable table, bool full)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			DbTable.SerializeToXml (buffer, table, full);
			return buffer.ToString ();
		}
		
		public static void   SerializeToXml(System.Text.StringBuilder buffer, DbTable table, bool full)
		{
			if (table == null)
			{
				buffer.Append ("<null/>");
			}
			else
			{
				table.SerializeXmlDefinition (buffer, full);
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
		
		
		protected void SerializeXmlDefinition(System.Text.StringBuilder buffer, bool full)
		{
			this.UpdatePrimaryKeyInfo ();
			
			buffer.Append (@"<table");
			
			string arg_cat = DbTools.ElementCategoryToString (this.category);
			
			if (arg_cat != null)
			{
				buffer.Append (@" cat=""");
				buffer.Append (arg_cat);
				buffer.Append (@"""");
			}
			
			if (full)
			{
				DbKey.SerializeToXmlAttributes (buffer, this.internal_table_key);
				this.Attributes.SerializeXmlAttributes (buffer);
				buffer.Append (@">");
				
				Collections.DbColumns.SerializeToXml (buffer, this.primary_keys, "keys");
				Collections.DbColumns.SerializeToXml (buffer, this.columns, "cols");
				
				buffer.Append (@"</table>");
			}
			else
			{
				buffer.Append (@"/>");
			}
		}
		
		protected void ProcessXmlDefinition(System.Xml.XmlElement xml)
		{
			if (xml.Name != "table")
			{
				throw new System.FormatException (string.Format ("Expected root element named <table>, but found <{0}>.", xml.Name));
			}
			
			string arg_cat = xml.GetAttribute ("cat");
			this.category  = DbTools.ParseElementCategory (arg_cat);
			
			this.internal_table_key = DbKey.DeserializeFromXmlAttributes (xml);
			this.Attributes.DeserializeXmlAttributes (xml);
			
			for (int i = 0; i < xml.ChildNodes.Count; i++)
			{
				System.Xml.XmlElement node = xml.ChildNodes[i] as System.Xml.XmlElement;
				
				if ((node == null) ||
					(node.GetAttribute ("id") == ""))
				{
					new System.FormatException (string.Format ("Expected nodes with id in {0}.", xml.InnerXml));
				}
				
				string id = node.GetAttribute ("id");
				
				switch (id)
				{
					case "keys":
						this.primary_keys = Collections.DbColumns.CreateCollection (node);
						break;
					case "cols":
						this.columns = Collections.DbColumns.CreateCollection (node);
						break;
					
					default:
						throw new System.FormatException (string.Format ("Expected id not found, '{0}' is not recognized.", id));
				}
			}
			
			this.UpdatePrimaryKeyInfo ();
		}
		
		
		
		protected DbAttributes					attributes	= new DbAttributes ();
		protected Collections.DbColumns			columns		= new Collections.DbColumns ();
		protected Collections.DbColumns			primary_keys;
		protected DbElementCat					category;
		protected DbKey							internal_table_key;
	}
}
