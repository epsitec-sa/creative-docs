//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.DataFillers
{
	public abstract class AbstractTreeTableFiller
	{
		public AbstractTreeTableFiller(DataAccessor accessor, BaseType baseType, NavigationTreeTableController controller)
		{
			this.accessor   = accessor;
			this.baseType   = baseType;
			this.controller = controller;
		}


		public Timestamp?						Timestamp;
		public DataObject						DataObject;

		public virtual void UpdateColumns()
		{
		}

		public virtual void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
		}


		protected readonly DataAccessor						accessor;
		protected readonly BaseType							baseType;
		protected readonly NavigationTreeTableController	controller;
	}
}
