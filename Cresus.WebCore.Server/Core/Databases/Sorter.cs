//	Copyright © 2012-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.DataLayer.Expressions;

using System.Collections.Generic;

namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{
	/// <summary>
	/// A sorter represents a sort clause that can be used to sort a Database, according to the
	/// values of a column in a give direction.
	/// </summary>
	public sealed class Sorter
	{
		public Sorter(Column column, SortOrder sortOrder)
		{
			this.column = column;
			this.sortOrder = sortOrder;
		}


		public Column							Column
		{
			get
			{
				return this.column;
			}
		}

		public SortOrder						SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}


		public Dictionary<string, object> GetDataDictionary(Caches caches)
		{
			return new Dictionary<string, object> ()
			{
				{ "name", this.Column.GetId (caches) },
				{ "sortDirection", this.GetSortOrderData () },
			};
		}


		private string GetSortOrderData()
		{
			switch (this.sortOrder)
			{
				case SortOrder.Ascending:
					return "ASC";

				case SortOrder.Descending:
					return "DESC";

				default:
					throw new System.NotImplementedException ();
			}
		}


		private readonly Column					column;
		private readonly SortOrder				sortOrder;
	}
}
