using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using Epsitec.Cresus.WebCore.Server.Core.PropertyAccessor;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal class Filter
	{


		public Filter(Column column, EntityColumnFilter condition)
		{
			this.column = column;
			this.condition = condition;
		}


		public Column Column
		{
			get
			{
				return this.column;
			}
		}


		public EntityColumnFilter Condition
		{
			get
			{
				return this.condition;
			}
		}


		public Expression ToCondition(AbstractEntity example)
		{
			var lambda = this.Column.LambdaExpression;
			var name = this.Column.Name;
			var entityColumnMetaData = new EntityColumnMetadata (lambda, name);

			return this.condition.ToCondition (entityColumnMetaData, example);
		}


		protected object ConvertValue(PropertyAccessorCache propertyAccessorCache, object value)
		{
			var propertyAccessor = propertyAccessorCache.Get (this.Column.LambdaExpression);
			var stringPropertyAccessor = (AbstractStringPropertyAccessor) propertyAccessor;

			return stringPropertyAccessor.ConvertValue (value);
		}


		private readonly Column column;


		private readonly EntityColumnFilter condition;


	}


}
