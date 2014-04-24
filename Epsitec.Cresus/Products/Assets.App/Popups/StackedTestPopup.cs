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
			this.title = "StackedTestPopup";

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
				BottomMargin = 10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Radio,
				Label = "Ajouter<br/>Supprimer",
				BottomMargin = 10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Text,
				Label = "Nom",
				Width = 100,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Int,
				Label = "Quantité",
				BottomMargin = 10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Radio,
				Label = "Rouge<br/>Vert<br/>Bleu",
			});

			this.SetDescriptions (list);
		}


		public System.DateTime? DateFrom
		{
			get
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public System.DateTime? DateTo
		{
			get
			{
				var controller = this.GetController (1) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public int? Operation
		{
			get
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public string Name
		{
			get
			{
				var controller = this.GetController (3) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (3) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public int? Quantity
		{
			get
			{
				var controller = this.GetController (4) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (4) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public int? Color
		{
			get
			{
				var controller = this.GetController (5) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (5) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

	}
}