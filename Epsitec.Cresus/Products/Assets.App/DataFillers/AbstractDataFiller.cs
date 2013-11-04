//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public abstract class AbstractDataFiller
	{
		public AbstractDataFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller, AbstractNodeFiller nodeFiller)
		{
			this.accessor   = accessor;
			this.baseType   = baseType;
			this.controller = controller;
			this.nodeFiller = nodeFiller;
		}


		public Timestamp?						Timestamp;
		public DataObject						DataObject;

		public virtual void UpdateColumns(int dockToLeftCount)
		{
		}

		public virtual void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
		}


		protected readonly DataAccessor						accessor;
		protected readonly NavigationTreeTableController	controller;
		protected readonly AbstractNodeFiller				nodeFiller;
		protected readonly BaseType							baseType;
	}
}
