//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	//TODO: Hack this clone of EntityFilter for creating a more complexe Expression Tree
	public class DatasetQueryFilter : IFilter
	{
		public DatasetQueryFilter(Druid entityId)
		{
			this.columns = new List<ColumnRef<EntityColumnFilter>> ();
			this.entityId = entityId;
		}


		public IList<ColumnRef<EntityColumnFilter>> Columns
		{
			get
			{
				return this.columns;
			}
		}

		public Druid							EntityId
		{
			get
			{
				return this.entityId;
			}
		}

		#region IFilter Members

		public bool								IsValid
		{
			get
			{
				return this.columns.All (x => x.Value.IsValid);
			}
		}

		public Expression GetExpression(AbstractEntity example, Expression parameter)
		{
			return Filter.GetExpression (this.GetColumnFilters (example, parameter), FilterCombineMode.And);
		}

		#endregion


		private IEnumerable<Expression> GetColumnFilters(AbstractEntity example, Expression parameter)
		{
			foreach (var column in this.columns)
			{
				var entityColumn = column.Resolve (this.entityId);

				if (entityColumn == null)
				{
					var fieldPath = EntityFieldPath.CreateAbsolutePath (this.entityId, column.Id);
					var fieldExpr = fieldPath.CreateLambda ();

					entityColumn = new EntityColumnMetadata (fieldExpr, "");
				}

				entityColumn.GetLeafEntity (example, NullNodeAction.CreateMissing);

				var expression = entityColumn.Expression;
				var columnLambda = ExpressionAnalyzer.ReplaceParameter (expression, (ParameterExpression) parameter);

				yield return column.Value.GetExpression (example, columnLambda);
			}
		}

		private readonly List<ColumnRef<EntityColumnFilter>> columns;
		private readonly Druid entityId;
	}
}
