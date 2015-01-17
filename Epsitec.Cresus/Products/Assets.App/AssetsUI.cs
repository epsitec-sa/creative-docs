//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App
{
	public class AssetsUI
	{
		public AssetsUI(DataAccessor accessor, CommandDispatcher commandDispatcher, CommandContext commandContext)
		{
			this.accessor          = accessor;
			this.commandDispatcher = commandDispatcher;
			this.commandContext    = commandContext;
		}

		public void CreateUI(Widget parent)
		{
			var mainView = new MainView (this.accessor, this.commandDispatcher, this.commandContext);
			mainView.CreateUI (parent);
		}


		private readonly DataAccessor			accessor;
		private readonly CommandDispatcher		commandDispatcher;
		private readonly CommandContext			commandContext;
	}
}
