using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Expressions
{


	public abstract class SetComparison : Comparison
	{


		public SetComparison(EntityField field, SetComparator comparator)
		{
			field.ThrowIfNull ("field");
			
			this.Field = field;
			this.Comparator = comparator;
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


		internal override SqlFunction CreateSqlCondition(SqlFieldBuilder builder)
		{
			return new SqlFunction
			(
				EnumConverter.ToSqlFunctionCode (this.Comparator),
				this.Field.CreateSqlField (builder),
				this.CreateSqlFieldForSet (builder)
			);
		}


		internal abstract SqlField CreateSqlFieldForSet(SqlFieldBuilder builder);


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
