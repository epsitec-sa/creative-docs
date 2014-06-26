//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class StackedTestPopup : StackedPopup
	{
		public StackedTestPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "StackedPopup de test";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label = "Depuis",
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label = "Jusqu'au",
				BottomMargin = 10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels = "Ajouter<br/>Supprimer",
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
				MultiLabels = "Rouge<br/>Vert<br/>Bleu",
				BottomMargin = 10,
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Bool,
				Label = "Avec des exemples",
			});

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Label,
				Width = 200,
				Label = "Message quelconque assez long, pour tester le comportement sur plusieurs lignes.<br/>Début d'un deuxième groupe de texte...",
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

		public string FieldName
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

		public bool Samples
		{
			get
			{
				var controller = this.GetController (6) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (6) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

	}
}