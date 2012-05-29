//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>SqlJoin</c> class describes joins for <c>SqlSelect</c>.
	/// </summary>
	public sealed class SqlJoin
	{


		public SqlJoin(SqlJoinCode code, SqlField table)
			: this (code, table, null)
		{
		}


		public SqlJoin(SqlJoinCode code, SqlField table, SqlFunction condition)
		{
			this.code = code;
			this.table = table;
			this.condition = condition;
		}


		public SqlJoinCode Code
		{
			get
			{
				return this.code;
			}
		}


		public SqlField Table
		{
			get
			{
				return this.table;
			}
		}


		public SqlFunction Condition
		{
			get
			{
				return this.condition;
			}
		}


		public static SqlJoin Create(SqlJoinCode code, SqlField table, SqlField leftColumn, SqlField rightColumn)
		{
			var op = SqlFunctionCode.CompareEqual;
			var condition = new SqlFunction (op, leftColumn, rightColumn);

			return new SqlJoin (code, table, condition);
		}


		private readonly SqlJoinCode code;
		private readonly SqlField table;
		private readonly SqlFunction condition;


	}


}
