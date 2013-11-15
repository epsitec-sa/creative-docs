﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public abstract class AbstractTreeTableFiller2<T>
		where T : struct
	{
		public AbstractTreeTableFiller2(DataAccessor accessor, BaseType baseType, AbstractNodesGetter<T> nodesGetter)
		{
			this.accessor    = accessor;
			this.baseType    = baseType;
			this.nodesGetter = nodesGetter;
		}


		public Timestamp?						Timestamp;
		public DataObject						DataObject;

		public abstract TreeTableColumnDescription[] Columns
		{
			get;
		}

		public abstract TreeTableContentItem GetContent(int firstRow, int count, int selection);


		protected readonly DataAccessor				accessor;
		protected readonly BaseType					baseType;
		protected readonly AbstractNodesGetter<T>	nodesGetter;
	}
}