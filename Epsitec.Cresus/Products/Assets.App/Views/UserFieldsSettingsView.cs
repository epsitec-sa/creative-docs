//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class UserFieldsSettingsView : AbstractSettingsView
	{
		public UserFieldsSettingsView(DataAccessor accessor, BaseType baseType)
			: base (accessor)
		{
			this.baseType = baseType;

			this.treeTableController = new UserFieldsToolbarTreeTableController (this.accessor, this.baseType);
		}


		public override void CreateUI(Widget parent)
		{
			this.treeTableController.CreateUI (parent);
		}


		private readonly BaseType baseType;
		private readonly UserFieldsToolbarTreeTableController treeTableController;
	}
}
