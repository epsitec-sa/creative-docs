//	Copyright � 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

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
			//	Comme on ne sait pas � priori si l'appelant passe un nom qualifi� ou un nom
			//	simple, on doit bien l'analyser. On utiliser l'analyse SQL g�n�rique, dans
			//	l'espoir qu'aucune impl�mentation de SQL n'utilise d'autres r�gles pour
			//	d�finir un nom qualifi�.
			
			if (DbSqlStandard.ValidateQualifiedName (name))
			{
				this.type = SqlFieldType.QualifiedName;
			}
			else
			{
				this.type = SqlFieldType.Name;
			}
			
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
		
		
		public bool Validate(ISqlValidator validator)
		{
			//	TODO: valide en fonction du validator et du type du SqlField.
			
			switch (this.Type)
			{
				case SqlFieldType.Name:				return validator.ValidateName (this.AsName);
				case SqlFieldType.QualifiedName:	return validator.ValidateQualifiedName (this.AsQualifiedName);
			}
			
			return false;
		}
		
		
		public SqlFieldType				Type
		{
			get { return this.type; }
		}
		
		public DbRawType				RawType
		{
			get { return this.raw_type; }
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
		
		public object					AsParameter
		{
			get
			{
				if ((this.type == SqlFieldType.ParameterIn) ||
					(this.type == SqlFieldType.ParameterInOut) ||
					(this.type == SqlFieldType.ParameterOut) ||
					(this.type == SqlFieldType.ParameterResult))
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
				if (this.type == SqlFieldType.QualifiedName)
				{
					string qualifier;
					string name;
					
					DbSqlStandard.SplitQualifiedName (this.AsQualifiedName, out qualifier, out name);
					
					return name;
				}
				
				return null;
			}
		}
		
		public string					AsQualifiedName
		{
			get
			{
				if (this.type == SqlFieldType.QualifiedName)
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
		
		public SqlFunction				AsFunction
		{
			get
			{
				if (this.type == SqlFieldType.Function)
				{
					return this.value as SqlFunction;
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
		
		
		public void SetParameterOutResult(object raw_value)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
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
		
		public static SqlField CreateConstant(object raw_value, DbRawType raw_type)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateParameterIn(object raw_value, DbRawType raw_type)
		{
			return SqlField.CreateConstant (raw_value, raw_type);
		}
		
		public static SqlField CreateParameterInOut(object raw_value, DbRawType raw_type)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateParameterOut(DbRawType raw_type)
		{
			//	TODO: cr�e et initialise une instance de SqlField.
			return null;
		}
		
		public static SqlField CreateParameterResult(DbRawType raw_type)
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
		protected DbRawType				raw_type = DbRawType.Unknown;
		protected object				value = null;
		protected string				alias = null;
	}
	
	
	public enum SqlFieldType
	{
		Unsupported,					//	champ non support� (ou non d�fini)
		
		Null,							//	constante NULL
		All,							//	constante sp�ciale pour aggr�gats: *
		Default,						//	constante sp�ciale pour INSERT INTO...
		Constant,						//	constante (donn�e compatible DbRawType)
		
		ParameterIn = Constant,			//	param�tre en entr�e = comme constante
		ParameterOut,					//	param�tre en sortie
		ParameterInOut,					//	param�tre en entr�e et en sortie
		ParameterResult,				//	param�tre en sortie (r�sultat de proc�dure)
		
		Name,							//	nom simple (nom de colonne, nom de table, ...)
		QualifiedName,					//	nom qualifi� (nom de table + nom de colonne)
		
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
