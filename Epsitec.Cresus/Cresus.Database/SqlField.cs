//	Copyright � 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier, compl�t� DD 2004.04.19, ajout� CreateJoin, AsJoin...

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
		internal SqlField()
		{
		}

		internal SqlField(string name)
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
		
		internal SqlField(string name, string alias) : this (name)
		{
			this.alias = alias;
		}
		
		internal SqlField(string name, string alias, SqlFieldOrder order) : this (name, alias)
		{
			this.order = order;
		}
		
		
		public SqlFieldType						Type
		{
			get { return this.type; }
		}
		
		public DbRawType						RawType
		{
			get { return this.raw_type; }
		}
		
		public SqlFieldOrder					Order
		{
			get { return this.order; }
			set { this.order = value; }
		}
		
		public string							Alias
		{
			get { return this.alias; }
			set { this.alias = value; }
		}
		
		public object							Value
		{
			get
			{
				return this.value;
			}
		}
		
		
		public object							AsConstant
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
		
		public object							AsParameter
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
		
		public string							AsName
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
		
		public string							AsQualifiedName
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
		
		public string							AsQualifier
		{
			get
			{
				if (this.type == SqlFieldType.Name)
				{
					throw new DbFormatException (string.Format ("{0} is not a qualified name.", this.AsName));
				}
				if (this.type == SqlFieldType.QualifiedName)
				{
					string qualifier;
					string name;
					
					DbSqlStandard.SplitQualifiedName (this.AsQualifiedName, out qualifier, out name);
					
					return qualifier;
				}
				
				return null;
			}
		}

		public SqlAggregate						AsAggregate
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
		
		public object							AsVariable
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
		
		public SqlFunction						AsFunction
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

		public SqlJoin							AsJoin
		{
			get
			{
				if (this.type == SqlFieldType.Join)
				{
					return this.value as SqlJoin;
				}
				
				return null;
			}
		}
		
		public string							AsProcedure
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
		
		public SqlSelect						AsSubQuery
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
		
		
		public bool Validate(ISqlValidator validator)
		{
			//	TODO: valide en fonction du validator et du type du SqlField.
			//
			//	Il faudra compl�ter au fur et � mesure si ISqlValidator sait
			//	valider d'autres types de champs. On pourrait imaginer valider
			//	une proc�dure SQL...
			
			switch (this.Type)
			{
				case SqlFieldType.Name:				return validator.ValidateName (this.AsName);
				case SqlFieldType.QualifiedName:	return validator.ValidateQualifiedName (this.AsQualifiedName);
			}
			
			return true;
		}
		
		
		public void SetParameterOutResult(object raw_value)
		{
			if ((this.type == SqlFieldType.ParameterInOut) ||
				(this.type == SqlFieldType.ParameterOut) ||
				(this.type == SqlFieldType.ParameterResult))
			{
				this.value = raw_value;
			}
			else
			{
				throw new System.InvalidOperationException ("Field must be an 'out' parameter");
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
		
		public static SqlField CreateConstant(object raw_value, DbRawType raw_type)
		{
			//	Cr�e et initialise une instance de SqlField. Dans le cas
			//	d'une constante, la valeur est 'raw_value', et le type de la
			//	valeur est 'raw_type'. On esp�re que l'appelant utilise un
			//	type coh�rent (ex.: raw_value est un objet decimal, raw_type
			//	est DbRawType.SmallDecimal et la valeur est "correcte"),
			//	mais ce serait trop compliqu� � v�rifier ici.
			
			//	Exemple de constantes : 1.5, "X", etc.
			
			//	Une constante n'a g�n�ralement pas de nom d'alias, sauf si la
			//	constante est un param�tre � ins�rer. Dans ce cas, le nom d'alias
			//	est alors le nom de la colonne dans la table.
			
			SqlField field	= new SqlField ();
			
			//	Si on a re�u en entr�e un DbId, on remplace silencieusement celui-
			//	ci par un long :
			
			if (raw_value is DbId)
			{
				raw_value = ((DbId) raw_value).Value;
			}
			
			field.type		= SqlFieldType.Constant;
			field.raw_type	= raw_type;
			field.value		= raw_value;

			return field;
		}
		
		public static SqlField CreateParameterIn(object raw_value, DbRawType raw_type)
		{
			return SqlField.CreateConstant (raw_value, raw_type);
		}
		
		public static SqlField CreateParameterInOut(object raw_value, DbRawType raw_type)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.ParameterInOut;
			field.raw_type	= raw_type;
			field.value		= raw_value;

			return field;
		}
		
		public static SqlField CreateParameterOut(DbRawType raw_type)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.ParameterOut;
			field.raw_type	= raw_type;
//			field.value		= null;

			return field;
		}
		
		public static SqlField CreateParameterResult(DbRawType raw_type)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.ParameterResult;
			field.raw_type	= raw_type;
//			field.value		= null;

			return field;
		}
		
		public static SqlField CreateAll()
		{
			//	cr�e un champs pour repr�senter une s�lection de tout ( * )
			SqlField field	= new SqlField ();

			field.type = SqlFieldType.All;

			return field;
		}
		
		public static SqlField CreateName(string name)
		{
			if (/*DbSqlStandard.ValidateQualifiedName (name) ||*/
				DbSqlStandard.ValidateName (name) )
			{
				return new SqlField (name);
			}
			
			throw new DbFormatException (string.Format ("{0} is not a valid SQL name.", name));
		}
		
		public static SqlField CreateName(string table_name, string column_name)
		{
			string name = DbSqlStandard.QualifyName (table_name, column_name);
			
			if (DbSqlStandard.ValidateQualifiedName (name))
			{
				return new SqlField (name);
			}
			
			throw new DbFormatException (string.Format ("{0} is not a valid SQL name.", name));
		}
		
		public static SqlField CreateAggregate(SqlAggregate aggregate)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.Aggregate;
			field.value		= aggregate;

			return field;
		}
		
		public static SqlField CreateAggregate(SqlAggregateType aggregate_type, SqlField field)
		{
			return SqlField.CreateAggregate (new SqlAggregate(aggregate_type, field));
		}
		
		public static SqlField CreateVariable()
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.Variable;

			return field;
		}
		
		public static SqlField CreateFunction(SqlFunction sql_function)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.Function;
			field.value		= sql_function;

			return field;
		}

		public static SqlField CreateJoin(SqlJoin sql_join)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.Join;
			field.value		= sql_join;

			return field;
		}
		
		public static SqlField CreateProcedure(string procedure_name)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.Procedure;
			field.value		= procedure_name;

			return field;
		}
		
		public static SqlField CreateSubQuery(SqlSelect sub_query)
		{
			SqlField field	= new SqlField ();
			
			field.type		= SqlFieldType.SubQuery;
			field.value		= sub_query;

			return field;
		}

		
		
		protected SqlFieldType					type		= SqlFieldType.Unsupported;
		protected SqlFieldOrder					order		= SqlFieldOrder.None;
		protected DbRawType						raw_type	= DbRawType.Unknown;
		protected object						value;
		protected string						alias;
	}
	
	
	public enum SqlFieldType
	{
		Unsupported,							//	champ non support� (ou non d�fini)
										
		Null,									//	constante NULL
		All,									//	constante sp�ciale pour aggr�gats: *
		Default,								//	constante sp�ciale pour INSERT INTO...
		Constant,								//	constante (donn�e compatible DbRawType)
										
		ParameterIn = Constant,					//	param�tre en entr�e = comme constante
		ParameterOut,							//	param�tre en sortie
		ParameterInOut,							//	param�tre en entr�e et en sortie
		ParameterResult,						//	param�tre en sortie (r�sultat de proc�dure)
										
		Name,									//	nom simple (nom de colonne, nom de table, nom de type, ...)
		QualifiedName,							//	nom qualifi� (nom de table + nom de colonne)
										
		Aggregate,						
		Variable,								//	variable SQL (?)
		Function,								//	fonction SQL (?)
		Procedure,								//	proc�dure SQL (?)
										
		SubQuery,								//	sous-requ�te
		Join									//	jointure
	}									
										
	public enum SqlFieldOrder			
	{									
		None,									//	pas de tri sur ce champ
		Normal,									//	tri normal (ASC)
		Inverse									//	tri inverse (DESC)
	}
}
