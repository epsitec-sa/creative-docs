//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.DataFillers
{
	public abstract class AbstractTreeTableFiller<T>
		where T : struct
	{
		public AbstractTreeTableFiller(DataAccessor accessor, AbstractNodeGetter<T> nodeGetter)
		{
			this.accessor    = accessor;
			this.nodeGetter = nodeGetter;
		}


		public Timestamp?						Timestamp;
		public DataObject						DataObject;

		public abstract IEnumerable<ObjectField> Fields
		{
			get;
		}

		public abstract TreeTableColumnDescription[] Columns
		{
			get;
		}

		public int Count
		{
			get
			{
				return this.nodeGetter.Count;
			}
		}

		public abstract TreeTableContentItem GetContent(int firstRow, int count, int selection);


		protected readonly DataAccessor				accessor;
		protected readonly AbstractNodeGetter<T>	nodeGetter;
	}
}