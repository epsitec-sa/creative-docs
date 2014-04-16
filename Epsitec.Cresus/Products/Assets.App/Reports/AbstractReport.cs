//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractReport
	{
		public AbstractReport(DataAccessor accessor, NavigationTreeTableController treeTableController)
		{
			this.accessor = accessor;
			this.treeTableController = treeTableController;
		}

		public virtual void Dispose()
		{
		}

		public virtual void Initialize()
		{
		}


		protected readonly DataAccessor			accessor;
		protected readonly NavigationTreeTableController treeTableController;

		protected int							visibleSelectedRow;
	}
}
