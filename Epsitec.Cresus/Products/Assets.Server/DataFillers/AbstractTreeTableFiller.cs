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
		public AbstractTreeTableFiller(DataAccessor accessor, AbstractNodesGetter<T> nodesGetter)
		{
			this.accessor    = accessor;
			this.nodesGetter = nodesGetter;
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
				return this.nodesGetter.Count;
			}
		}

		public abstract TreeTableContentItem GetContent(int firstRow, int count, int selection);


		protected readonly DataAccessor				accessor;
		protected readonly AbstractNodesGetter<T>	nodesGetter;
	}
}