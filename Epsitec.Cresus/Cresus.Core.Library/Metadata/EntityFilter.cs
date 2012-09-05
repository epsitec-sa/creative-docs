//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.Metadata
{
	public class EntityFilter : IFilter
	{
		public EntityFilter(Druid entityId)
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

		public Expression GetExpression(Expression parameter)
		{
			return Filter.GetExpression (this.GetColumnFilters (parameter), FilterCombineMode.And);
		}

		#endregion


		public XElement Save(string xmlNodeName)
		{
			var xml = new XElement (xmlNodeName,
				new XAttribute (Strings.EntityId, this.entityId.ToCompactString ()),
				new XElement (Strings.ColumnList,
					this.columns.Select (x => x.Save (Strings.ColumnItem))));

			return xml;
		}

		public static EntityFilter Restore(XElement xml)
		{
			var list     = xml.Element (Strings.ColumnList).Elements ();
			var entityId = Druid.Parse (xml.Attribute (Strings.EntityId));
			var filter   = new EntityFilter (entityId);

			filter.columns.AddRange (list.Select (x => ColumnRef.Restore<EntityColumnFilter> (x)));

			return filter;
		}


		private IEnumerable<Expression> GetColumnFilters(Expression parameter)
		{
			foreach (var column in this.columns)
			{
				var entityColumn = column.Resolve (this.entityId);
				var columnLambda = ExpressionAnalyzer.ReplaceParameter (entityColumn.Expression, parameter as ParameterExpression);

				yield return column.Value.GetExpression (columnLambda);
			}
		}


		#region Strings Class

		private static class Strings
		{
			public const string EntityId = "id";
			public const string ColumnList = "C";
			public const string ColumnItem = "c";
		}

		#endregion


		private readonly List<ColumnRef<EntityColumnFilter>> columns;
		private readonly Druid entityId;
	}
}
