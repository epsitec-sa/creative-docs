//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 07/10/2003
//			 DD, 19/04/2004, ajouté 'Joins'

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlSelect décrit une requête SELECT. C'est l'une des requêtes
	/// les plus complexes...
	/// </summary>
	public class SqlSelect
	{
		public SqlSelect()
		{
		}
		

		public void Add(SqlSelect set_query, SqlSelectSetOp set_op)
		{
			if (this.set_query == null)
			{
				if (set_op == SqlSelectSetOp.None)
				{
					throw new System.ArgumentException ("Invalid set operation");
				}
				
				this.set_query = set_query;
				this.set_op    = set_op;
			}
			else
			{
				this.set_query.Add (set_query, set_op);
			}
		}
		
		
		public SqlSelectPredicate		Predicate
		{
			get { return this.predicate; }
			set { this.predicate = value; }
		}
		
		public SqlFieldCollection		Fields
		{
			get { return this.field_coll; }
		}

		public SqlFieldCollection		Tables
		{
			get { return this.table_coll; }
		}
		
		public SqlFieldCollection		Conditions
		{
			get { return this.where_coll; }
		}

		public SqlFieldCollection		Joins
		{
			get { return this.join_coll; }
		}
		
		
		public SqlSelect				SelectSetQuery
		{
			get { return this.set_query; }
		}
		
		public SqlSelectSetOp			SelectSetOp
		{
			get { return this.set_op; }
		}
		
		
		protected SqlSelectPredicate	predicate	= SqlSelectPredicate.All;
		
		protected SqlFieldCollection	field_coll	= new SqlFieldCollection ();
		protected SqlFieldCollection	table_coll	= new SqlFieldCollection ();
		protected SqlFieldCollection	where_coll	= new SqlFieldCollection ();
		protected SqlFieldCollection	join_coll	= new SqlFieldCollection ();
		
		protected SqlSelectSetOp		set_op		= SqlSelectSetOp.None;
		protected SqlSelect				set_query	= null;
	}
	
	public enum SqlSelectPredicate
	{
		All,							//	SELECT ALL ...
		Distinct						//	SELECT DISTINCT ...
	}
	
	public enum SqlSelectSetOp
	{
		None,							//	un seul SELECT, pas de combinaison...
		
		Union,							//	SELECT ... UNION [ALL|DISTINCT] SELECT ...
		Except,							//	SELECT ... EXCEPT [ALL|DISTINCT] SELECT ...
		Intersect						//	SELECT ... INTERSECT [ALL|DISTINCT] SELECT ...
	}
}
