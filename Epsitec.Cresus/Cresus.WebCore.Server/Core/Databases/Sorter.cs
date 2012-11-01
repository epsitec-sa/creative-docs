using Epsitec.Cresus.DataLayer.Expressions;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.WebCore.Server.Core.Databases
{


	internal sealed class Sorter
	{


		public Sorter(Column column, SortOrder sortOrder)
		{
			this.column = column;
			this.sortOrder = sortOrder;
		}


		public Column Column
		{
			get
			{
				return this.column;
			}
		}


		public SortOrder SortOrder
		{
			get
			{
				return this.sortOrder;
			}
		}


		public Dictionary<string, object> GetDataDictionary(IdCache<string> columnIdCache)
		{
			return new Dictionary<string, object> ()
			{
				{ "name", this.Column.GetId (columnIdCache) },
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
					throw new NotImplementedException ();
			}
		}


		private readonly Column column;


		private readonly SortOrder sortOrder;


	}


}
