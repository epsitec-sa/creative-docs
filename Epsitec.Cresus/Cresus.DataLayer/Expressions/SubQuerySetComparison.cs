using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Expressions
{

	
	public sealed class SubQuerySetComparison : SetComparison
	{


		public SubQuerySetComparison(EntityField field, SetComparator comparator, SubQuery subQuery)
			: base (field, comparator)
		{
			subQuery.ThrowIfNull ("subQuery");

			this.SubQuery = subQuery;
		}


		public SubQuery SubQuery
		{
			get;
			private set;
		}


		internal override SqlField CreateSqlFieldForSet(SqlFieldBuilder builder)
		{
			return this.SubQuery.CreateSqlField (builder);
		}


	}


}
