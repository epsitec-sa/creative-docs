using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public sealed class SetComparison : Comparison
	{


		public SetComparison(EntityField field, SetComparator comparator, IEnumerable<Value> set)
		{
			field.ThrowIfNull ("field");
			set.ThrowIfNull ("values");
			set.ThrowIf (x => x.Any (v => v == null), "values cannot contain null elements");
			
			this.Field = field;
			this.Comparator = comparator;
			this.Set = set.ToList ();

			if (this.Set.Count () == 0)
			{
				throw new ArgumentException ("values is empty");
			}
		}


		public EntityField Field
		{
			get;
			private set;
		}


		public SetComparator Comparator
		{
			get;
			private set;
		}


		public IEnumerable<Value> Set
		{
			get;
			private set;
		}


		internal override SqlFunction CreateSqlCondition(SqlFieldBuilder builder)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Comparator),
				this.Field.CreateSqlField (builder),
				SqlField.CreateSet (new SqlSet (this.Set.Select (v => v.CreateSqlField (builder))))
			);
		}


		internal override void CheckFields(FieldChecker checker)
		{
			this.Field.CheckField (checker);
		}


		internal override void AddEntities(HashSet<AbstractEntity> entities)
		{
			this.Field.AddEntities (entities);
		}


	}


}
