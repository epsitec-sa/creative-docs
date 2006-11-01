//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlField représente un champ SQL. Un champ peut être, en fonction
	/// du contexte, un nom de colonne qualifié, une variable automatique SQL (?),
	/// une constante, une fonction, une procédure ou une sous-requête SQL (subquery).
	/// 
	/// La représentation dans SqlField est indépendante de tout dialecte SQL. C'est
	/// ISqlBuilder qui sait convertir un SqlField en sa représentation SQL.
	/// </summary>
	public sealed class SqlField : Epsitec.Common.Types.IName
	{
		internal SqlField()
		{
		}

		internal SqlField(string name)
		{
			//	Comme on ne sait pas à priori si l'appelant passe un nom qualifié ou un nom
			//	simple, on doit bien l'analyser. On utiliser l'analyse SQL générique, dans
			//	l'espoir qu'aucune implémentation de SQL n'utilise d'autres règles pour
			//	définir un nom qualifié.
			
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
			get
			{
				return this.type;
			}
		}
		
		public DbRawType						RawType
		{
			get
			{
				return this.raw_type;
			}
		}
		
		public SqlFieldOrder					Order
		{
			get
			{
				return this.order;
			}
			set
			{
				this.order = value;
			}
		}
		
		public string							Alias
		{
			get
			{
				return this.alias;
			}
			set
			{
				this.alias = value;
			}
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
					throw new Exceptions.FormatException (string.Format ("{0} is not a qualified name.", this.AsName));
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


		#region IName Members

		string Epsitec.Common.Types.IName.Name
		{
			get
			{
				return this.alias;
			}
		}

		#endregion
		
		public bool Validate(ISqlValidator validator)
		{
			//	TODO: valide en fonction du validator et du type du SqlField.
			//
			//	Il faudra compléter au fur et à mesure si ISqlValidator sait
			//	valider d'autres types de champs. On pourrait imaginer valider
			//	une procédure SQL...
			
			switch (this.Type)
			{
				case SqlFieldType.Name:				return validator.ValidateName (this.AsName);
				case SqlFieldType.QualifiedName:	return validator.ValidateQualifiedName (this.AsQualifiedName);
			}
			
			return true;
		}

		public SqlField Clone()
		{
			SqlField copy = new SqlField ();
			
			copy.type = this.type;
			copy.order = this.order;
			copy.raw_type = this.raw_type;
			copy.value = this.value;
			copy.alias = this.alias;
			
			return copy;
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
		
		public void Overwrite(SqlField field)
		{
			this.type     = field.type;
			this.order    = field.order;
			this.raw_type = field.raw_type;
			this.alias    = field.alias;
			this.value    = field.value;
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
			//	Crée et initialise une instance de SqlField. Dans le cas
			//	d'une constante, la valeur est 'raw_value', et le type de la
			//	valeur est 'raw_type'. On espère que l'appelant utilise un
			//	type cohérent (ex.: raw_value est un objet decimal, raw_type
			//	est DbRawType.SmallDecimal et la valeur est "correcte"),
			//	mais ce serait trop compliqué à vérifier ici.
			
			//	Exemple de constantes : 1.5, "X", etc.
			
			//	Une constante n'a généralement pas de nom d'alias, sauf si la
			//	constante est un paramètre à insérer. Dans ce cas, le nom d'alias
			//	est alors le nom de la colonne dans la table.
			
			SqlField field	= new SqlField ();
			
			//	Si on a reçu en entrée un DbId, on remplace silencieusement celui-
			//	ci par un long :
			
			if (raw_value is DbId)
			{
				raw_value = ((DbId) raw_value).Value;
			}
			if (raw_value == null)
			{
				raw_value = System.DBNull.Value;
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
			//	crée un champs pour représenter une sélection de tout ( * )
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
			
			throw new Exceptions.FormatException (string.Format ("{0} is not a valid SQL name.", name));
		}
		
		public static SqlField CreateName(string table_name, string column_name)
		{
			string name = DbSqlStandard.QualifyName (table_name, column_name);
			
			if (DbSqlStandard.ValidateQualifiedName (name))
			{
				return new SqlField (name);
			}
			
			throw new Exceptions.FormatException (string.Format ("{0} is not a valid SQL name.", name));
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

		
		
		private SqlFieldType					type		= SqlFieldType.Unsupported;
		private SqlFieldOrder order		= SqlFieldOrder.None;
		private DbRawType raw_type	= DbRawType.Unknown;
		private object value;
		private string alias;
	}
}
