//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 26/10/2003

namespace Epsitec.Cresus.Database
{
	using Tags = Epsitec.Common.Support.Tags;
	using ResourceLevel = Epsitec.Common.Support.ResourceLevel;
	
	/// <summary>
	/// La classe DbTable décrit la structure d'une table dans la base de données.
	/// Cette classe ressemble dans l'esprit à System.Data.DataTable.
	/// </summary>
	public class DbTable : IDbAttributesHost
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
		
		
		public static DbTable NewTable(string xml)
		{
			System.Xml.XmlDocument doc = new System.Xml.XmlDocument ();
			
			doc.LoadXml (xml);
			
			return DbTable.NewTable (doc.DocumentElement);
		}
		
		public static DbTable NewTable(System.Xml.XmlElement xml)
		{
			return new DbTable (xml);
		}

		
		protected void SerialiseXmlDefinition(System.Text.StringBuilder buffer)
		{
			buffer.Append (@"<table");
			
			//	TODO: ajoute les réglages propres à la table
			
			buffer.Append (@"/>");
		}
		
		protected void ProcessXmlDefinition(System.Xml.XmlElement xml)
		{
			if (xml.Name != "table")
			{
				throw new System.ArgumentException (string.Format ("Expected root element named <table>, but found <{0}>.", xml.Name));
			}
			
			//	TODO: extrait les réglages propres à la table
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
		
		public DbColumnCollection		Columns
		{
			get { return this.columns; }
		}
		
		public bool						HasPrimaryKeys
		{
			get { return (this.primary_key != null) && (this.primary_key.Count > 0); }
		}
		
		public DbColumn[]				PrimaryKey
		{
			get
			{
				//	NB: les clefs primaires spécifiées par PrimaryKey sont utilisées
				//	pour former un 'tuple' (par exemple une paire de clef). Déclarer
				//	une série de colonnes comme PrimaryKey implique que les tuples
				//	doivent être uniques !
				
				if (this.primary_key == null)
				{
					return new DbColumn[0];
				}
				
				DbColumn[] columns = new DbColumn[this.primary_key.Count];
				this.primary_key.CopyTo (columns, 0);
				return columns;
			}
			set
			{
				//	Il n'est pas nécessaire de marquer les colonnes ajoutées ici comme
				//	étant indexées (SqlColumn.IsIndexed). Si l'appelant le spécifie
				//	néanmoins, des index supplémentaires seront créés pour les colonnes
				//	spécifiées. Cela permet par exemple d'avoir l'indexage automatique
				//	selon le tuple des clefs primaires, et l'indexage de chaque clef
				//	individuellement.
				
				if (this.primary_key == null)
				{
					if (value == null)
					{
						return;
					}
					
					this.primary_key = new DbColumnCollection ();
				}
				
				this.primary_key.Clear ();
				this.primary_key.AddRange (value);
			}
		}
		
		public DbElementCat				Category
		{
			get { return this.category; }
		}
		
		public DbKey					InternalKey
		{
			get { return this.internal_table_key; }
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
			
			if (this.HasPrimaryKeys)
			{
				//	S'il y a des clefs primaires définies, reprend simplement les clefs qui
				//	correspondent (elles doivent être définies dans la collection des colonnes
				//	de la table).
				
				int n = this.primary_key.Count;
				
				SqlColumn[] primary_keys = new SqlColumn[n];
				
				for (int i = 0; i < n; i++)
				{
					DbColumn db_key   = this.primary_key[i];
					string   key_name = db_key.Name;
					
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
		
		public string CreateSqlName()
		{
			return DbSqlStandard.CreateSimpleSqlName (this.Name);
		}
		
		
		public DbKey CreateKeyFromRow(System.Data.DataRow row)
		{
			DbColumn[] primary_key = this.PrimaryKey;
			DbKey      key = null;
			
			switch (this.category)
			{
				case DbElementCat.Internal:
				case DbElementCat.UserDataManaged:
					if (primary_key.Length == 1)
					{
						System.Diagnostics.Debug.Assert (primary_key[0].Name.ToUpper () == DbColumn.TagId);
						
						long id = (long) row[DbColumn.TagId];
						
						key = new DbKey (id);
					}
					else if (primary_key.Length == 2)
					{
						System.Diagnostics.Debug.Assert (primary_key[0].Name.ToUpper () == DbColumn.TagId);
						System.Diagnostics.Debug.Assert (primary_key[1].Name.ToUpper () == DbColumn.TagRevision);
						
						long id   = (long) row[DbColumn.TagId];
						int  rev  = (int)  row[DbColumn.TagRevision];
						int  stat = (int)  row[DbColumn.TagStatus];
						
						key = new DbKey (id, rev, stat);
					}
					break;
				
				default:
					if (primary_key.Length == 1)
					{
						long id = (long) row[primary_key[0].Name];
						
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
		
		
		protected DbAttributes			attributes = new DbAttributes ();
		protected DbColumnCollection	columns = new DbColumnCollection ();
		protected DbColumnCollection	primary_key = null;
		protected DbElementCat			category;
		protected DbKey					internal_table_key;
		
		
		internal const string			TagTableDef			= "CR_TABLE_DEF";
		internal const string			TagColumnDef		= "CR_COLUMN_DEF";
		internal const string			TagTypeDef			= "CR_TYPE_DEF";
		internal const string			TagEnumValDef		= "CR_ENUMVAL_DEF";
		internal const string			TagRefDef			= "CR_REF_DEF";
	}
}
