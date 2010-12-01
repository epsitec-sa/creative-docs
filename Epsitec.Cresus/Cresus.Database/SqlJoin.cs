//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

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
			leftColumn.ThrowIfNull ("leftColumn");
			rightColumn.ThrowIfNull ("rightColumn");
			leftColumn.ThrowIf (c => c.FieldType != SqlFieldType.QualifiedName, "leftColumn must have a qualified name");
			rightColumn.ThrowIf (c => c.FieldType != SqlFieldType.QualifiedName, "rightColumn must have a qualified name");

			// TODO Here we could also check that all the fields within conditions also are qualified
			// names. This would need a recursive function exploring the conditions an telling
			// whether all its names are qualified or not. I don't to it now, because there is so much
			// things to do in order to get strong guarantees on all the SqlStuff classes that this
			// would only be a drop in the ocean.
			// Marc

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
