using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public sealed class ValueSetComparison : SetComparison
	{


		public ValueSetComparison(EntityField field, SetComparator comparator, IEnumerable<Value> set)
			: base (field, comparator)
		{
			set.ThrowIfNull ("values");
			set.ThrowIf (x => x.Any (v => v == null), "values cannot contain null elements");

			this.Set = set.ToList ();

			if (this.Set.Count () == 0)
			{
				throw new ArgumentException ("values is empty");
			}
		}


		public IEnumerable<Value> Set
		{
			get;
			private set;
		}


		internal override SqlField CreateSqlFieldForSet(SqlFieldBuilder builder)
		{
			var sqlSet = new SqlSet (this.Set.Select (v => v.CreateSqlField (builder)));

			return SqlField.CreateSet (sqlSet);
		}


	}


}
