//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database.Collections;

using System.Linq;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>SqlJoin</c> class describes joins for <c>SqlSelect</c>.
	/// </summary>
	public sealed class SqlJoin
	{


		public SqlJoin(SqlField leftColumn, SqlField rightColumn, SqlJoinCode code)
			: this (leftColumn, rightColumn, code, new SqlFieldList ())
		{
		}

		public SqlJoin(SqlField leftColumn, SqlField rightColumn, SqlJoinCode code, SqlFieldList conditions)
		{
			if (leftColumn == null)
			{
				throw new System.ArgumentNullException ("leftColumn");
			}

			if (rightColumn == null)
			{
				throw new System.ArgumentNullException ("rightColumn");
			}

			if (conditions == null)
			{
				throw new System.ArgumentNullException ("conditions");
			}

			foreach (SqlField field in new SqlField[] { leftColumn, rightColumn }.Concat (conditions))
			{
				if (field.FieldType != SqlFieldType.QualifiedName)
				{
					throw new System.ArgumentException ("Fields must have qualified names");
				}
			}

			this.LeftColumn = leftColumn;
			this.RightColumn = rightColumn;
			this.Code = code;
			this.Conditions = conditions;
		}

		public SqlField LeftColumn
		{
			get;
			private set;
		}


		public SqlField RightColumn
		{
			get;
			private set;
		}


		public SqlJoinCode Code
		{
			get;
			private set;
		}


		public SqlFieldList Conditions
		{
			get;
			private set;
		}


	}


}
