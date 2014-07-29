//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public abstract class AbstractTreeTableFiller<T>
		where T : struct
	{
		public AbstractTreeTableFiller(DataAccessor accessor, INodeGetter<T> nodeGetter)
		{
			this.accessor   = accessor;
			this.nodeGetter = nodeGetter;
		}


		public Timestamp?						Timestamp;
		public DataObject						DataObject;
		public string							Title;

		public abstract SortingInstructions		DefaultSorting
		{
			get;
		}

		public abstract int						DefaultDockToLeftCount
		{
			get;
		}

		public abstract TreeTableColumnDescription[] Columns
		{
			get;
		}

		public int								Count
		{
			get
			{
				return this.nodeGetter.Count;
			}
		}

		public abstract TreeTableContentItem GetContent(int firstRow, int count, int selection);


		protected readonly DataAccessor			accessor;
		protected readonly INodeGetter<T>		nodeGetter;
	}
}