//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesView : AbstractView
	{
		public CategoriesView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.listController = new CategoriesToolbarTreeTableController (this.accessor);

			this.ignoreChanges = new SafeCounter ();

			this.listController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
			{
				//?this.OnStartEdit (eventType, timestamp);
			};
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
		}


		public override void CreateUI(Widget parent)
		{
			this.listController.CreateUI (parent);
		}




		private readonly CategoriesToolbarTreeTableController	listController;
		private readonly SafeCounter						ignoreChanges;

	}
}
