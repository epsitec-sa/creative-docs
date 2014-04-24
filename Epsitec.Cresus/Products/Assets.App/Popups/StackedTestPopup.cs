//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class StackedTestPopup : StackedPopup
	{
		public StackedTestPopup(DataAccessor accessor)
			: base (accessor)
		{
			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Date,
				Label = "Depuis",
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Date,
				Label = "Jusqu'au",
			});

			this.SetDescriptions (list);
		}


	}
}