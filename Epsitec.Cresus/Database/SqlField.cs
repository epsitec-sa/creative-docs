namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlField repr�sente un champ SQL. Un champ peut �tre, en fonction
	/// du contexte, un nom de colonne qualifi�, une variable automatique SQL (?),
	/// une constante, une fonction, une proc�dure ou une sous-requ�te SQL (subquery).
	/// 
	/// La repr�sentation dans SqlField est ind�pendante de tout dialecte SQL. C'est
	/// ISqlBuilder qui sait convertir un SqlField en sa repr�sentation SQL.
	/// </summary>
	public class SqlField
	{
		public SqlField()
		{
		}

		public SqlField(string name)
		{
			this.type = SqlFieldType.Name;
			this.value = name;
		}
		
		public SqlField(string name, string alias) : this (name)
		{
			this.alias = alias;
		}
		
		public SqlField(string name, string alias, SqlFieldOrder order) : this (name, alias)
		{
			this.order = order;
		}
		
		
		public SqlFieldType				Type
		{
			get { return this.type; }
		}
		
		public SqlFieldOrder			Order
		{
			get { return this.order; }
			set { this.order = value; }
		}
		
		public string					Alias
		{
			get { return this.alias; }
			set { this.alias = value; }
		}
		
		
		public object					AsConstant
		{
			get
			{
				if (this.type == SqlFieldType.Constant)
				{
					return this.value;
				}
				
				return null;
			}
		}
		
		public string					AsName
		{
			get
			{
				if (this.type == SqlFieldType.Name)
				{
					return this.value as string;
				}
				
				return null;
			}
		}
		
		public SqlAggregate				AsAggregate
		{
			get
			{
				if (this.type == SqlFieldType.Aggregate)
				{
					return this.value as SqlAggregate;
				}
				
				return null;
			}
		}
		
		public object					AsVariable
		{
			get
			{
				if (this.type == SqlFieldType.Variable)
				{
					return this.value;
				}
				
				return null;
			}
		}
		
		public object					AsFunction
		{
			get
			{
				if (this.type == SqlFieldType.Function)
				{
					return this.value;
				}
				
				return null;
			}
		}
		
		public string					AsProcedure
		{
			get
			{
				if (this.type == SqlFieldType.Procedure)
				{
					return this.value as string;
				}
				
				return null;
			}
		}
		
		public SqlSelect				AsSubQuery
		{
			get
			{
				if (this.type == SqlFieldType.SubQuery)
				{
					return this.value as SqlSelect;
				}
				
				return null;
			}
		}
		
		
		public static SqlField CreateNull()
		{
			SqlField field = new SqlField ();
			
			field.type = SqlFieldType.Null;
			
			return field;
		}
		
		public static SqlField CreateDefault()
		{
			SqlField field = new SqlField ();
			
			field.type = SqlFieldType.Default;
			
			return field;
		}
		
		public static SqlField CreateConstant(object raw_value)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateAll()
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateName(string name)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateName(string table_name, string column_name)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateAggregate(SqlAggregate aggregate)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateAggregate(SqlAggregateType aggregate_type, SqlField field)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateVariable()
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateFunction()
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateProcedure(string procedure_name)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateSubQuery(SqlSelect sub_query)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		
		
		protected SqlFieldType			type = SqlFieldType.Unsupported;
		protected SqlFieldOrder			order = SqlFieldOrder.None;
		protected object				value = null;
		protected string				alias = null;
	}
	
	
	public enum SqlFieldType
	{
		Unsupported,					//	champ non support� (ou non d�fini)
		
		Null,							//	constante NULL
		Constant,						//	constante (donn�e compatible DbRawType)
		All,							//	constante sp�ciale pour aggr�gats: *
		Default,						//	constante sp�ciale pour INSERT INTO...
		
		Name,							//	nom (nom de colonne, nom de table, ...)
		
		Aggregate,
		Variable,						//	variable SQL (?)
		Function,						//	fonction SQL (?)
		Procedure,						//	proc�dure SQL (?)
		
		SubQuery						//	sous-requ�te
	}
	
	public enum SqlFieldOrder
	{
		None,							//	pas de tri sur ce champ
		Normal,							//	tri normal (ASC)
		Inverse							//	tri inverse (DESC)
	}
}
