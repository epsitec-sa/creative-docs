using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Metadata;

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal abstract class Filter
	{


		public Filter(Column column)
		{
			this.column = column;
		}


		public Column Column
		{
			get
			{
				return this.column;
			}
		}


		public DataExpression ToCondition(AbstractEntity example)
		{
			var lambda = this.Column.LambdaExpression;
			var name = this.Column.Name;
			var entityColumnMetaData = new EntityColumnMetadata (lambda, name);

			return this.ToCondition (entityColumnMetaData, example);
		}


		protected abstract DataExpression ToCondition(EntityColumnMetadata column, AbstractEntity example);


		private readonly Column column;


	}


}
