//	Copyright © 2003-20004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : DD, 19/04/2004

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe SqlJoin décrit les jointures pour les SELECT
	/// à priori il n'y a jamais que 2 arguments
	/// dérivé de SqlField pour pouvoir l'utiliser dans un SqlFieldCollection
	/// </summary>
	public class SqlJoin
	{
		public SqlJoin(SqlJoinType type, params SqlField[] fields)
		{
			this.type = type;
			
			int count_provided  = fields.Length;
			int count_expected = this.ArgumentCount;
			
			if (count_provided != count_expected)
			{
				throw new System.ArgumentOutOfRangeException (string.Format ("{0} requires {1} field(s).", type, count_expected));
			}
			
			switch (count_provided)
			{
//				case 1: this.a = fields[0]; break;
				case 2: this.a = fields[0]; this.b = fields[1]; break;
//				case 3: this.a = fields[0]; this.b = fields[1]; this.c = fields[2]; break;
			}
		}
		
		public SqlJoinType			Type
		{
			get { return this.type; }
		}
		
		public int						ArgumentCount
		{
			get
			{
				return 2;
			}
		}
		
		public SqlField					A
		{
			get { return this.a; }
		}
		
		public SqlField					B
		{
			get { return this.b; }
		}
		
		protected SqlJoinType			type;
		protected SqlField				a, b;
	}
	
	public enum SqlJoinType
	{
		Unsupported,
		
		Inner,							//	A.a, B.b -> A INNER JOIN B ON A.a = B.b
		OuterLeft,
		OuterRight
	}
}
