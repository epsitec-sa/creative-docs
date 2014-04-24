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
				BottomMargin = 4,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Date,
				Label = "Jusqu'au",
				BottomMargin = 4+10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Text,
				Label = "Nom",
				BottomMargin = 4,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Int,
				Label = "Quantité",
				BottomMargin = 4+10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Radio,
				Label = "Rouge<br/>Vert<br/>Bleu",
				BottomMargin = 4,
			});

			this.SetDescriptions (list);
		}


	}
}