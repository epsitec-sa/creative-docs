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
		public EntityFilter()
		{
			this.columns = new List<ColumnRef<EntityColumnFilter>> ();
		}


		public IList<ColumnRef<EntityColumnFilter>> Columns
		{
			get
			{
				return this.columns;
			}
		}

		#region IFilter Members

		public bool IsValid
		{
			get
			{
				return this.columns.All (x => this.GetExpression (x) != null);
			}
		}

		public Expression GetExpression(Expression parameter)
		{
			throw new System.NotImplementedException ();
		}

		#endregion


		public Expression GetExpression(ColumnRef<EntityColumnFilter> column)
		{
			var columnRef = column.Id;
			var columnFilter = column.Value;
			
			
			
			return null;
		}


		public XElement Save(string xmlNodeName)
		{
			var xml = new XElement (xmlNodeName,
				new XElement (Strings.ColumnList,
					this.columns.Select (x => x.Save (Strings.ColumnItem))));

			return xml;
		}

		public static EntityFilter Restore(XElement xml)
		{
			var list = xml.Element (Strings.ColumnList).Elements ();
			var filter = new EntityFilter ();

			filter.columns.AddRange (list.Select (x => ColumnRef.Restore<EntityColumnFilter> (x)));

			return filter;
		}


		#region Strings Class

		private static class Strings
		{
			public const string ColumnList = "C";
			public const string ColumnItem = "c";
		}

		#endregion


		private readonly List<ColumnRef<EntityColumnFilter>> columns;
	}
}
