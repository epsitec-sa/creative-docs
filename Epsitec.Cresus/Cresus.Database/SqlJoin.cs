//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlJoin décrit les jointures pour SqlSelect.
	/// A priori, il y a toujours 2 arguments.
	/// </summary>
	public class SqlJoin
	{
		public SqlJoin(SqlJoinType type, params SqlField[] fields)
		{
			this.type = type;
			
			if (fields.Length != 2)
			{
				throw new System.ArgumentOutOfRangeException (string.Format ("Join ({0}) requires 2 fields, got {1}.", type, fields.Length));
			}
			
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].Type != SqlFieldType.QualifiedName)
				{
					throw new System.ArgumentException (string.Format ("Join argument {0} must be a qualified names (specified {1}).", i, fields[i].Type));
				}
			}
			
			this.a = fields[0];
			this.b = fields[1];
		}
		
		
		public SqlJoinType						Type
		{
			get { return this.type; }
		}
		
		public SqlField							A
		{
			get { return this.a; }
		}
		
		public SqlField							B
		{
			get { return this.b; }
		}
		
		
		
		protected SqlJoinType					type;
		protected SqlField						a, b;
	}
	
	public enum SqlJoinType
	{
		Unsupported,
		
		Inner,									//	A.a, B.b -> A INNER JOIN B ON A.a = B.b
		
		OuterLeft,
		OuterRight
	}
}
