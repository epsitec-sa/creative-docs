//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public abstract class AbstractStackedController
	{
		public AbstractStackedController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}


		public virtual void CreateUI(Widget parent, int labelWidth, int tabIndex, StackedControllerDescription description)
		{
		}

		protected virtual void UpdateWidgets()
		{
		}


		protected DataAccessor					accessor;
	}
}