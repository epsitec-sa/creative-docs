//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AmortizationsPopup : StackedPopup
	{
		public AmortizationsPopup(DataAccessor accessor)
			: base(accessor)
		{
			this.title = "Création d'un nouveau mandat";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Depuis",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "jusqu'au",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = "Pour l'objet sélectionné<br/>Pour tous les objets",
			});

			this.SetDescriptions (list);
		}


		public string							Title
		{
			set
			{
				this.title = value;
			}
		}

		public string							ActionOne;
		public string							ActionAll;
		public bool								DateFromAllowed;
		public bool								DateToAllowed;
		public bool								OneSelectionAllowed;

		public bool								IsAll
		{
			get
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value == 1;  // "Tous les objets" si true
			}
			set
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 1 : 0;
			}
		}

		public System.DateTime?					DateFrom
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

		public System.DateTime?					DateTo
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


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetVisibility (0, this.DateFromAllowed);
			this.SetVisibility (1, this.DateToAllowed);

			//	Grise le bouton radio "Pour l'objet sélectionné" s'il n'y a pas de sélection.
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetRadioEnable (0, this.OneSelectionAllowed);
			}

			//	Nomme le bouton principal, selon les boutons radio.
			this.okButton.Text = this.IsAll ? this.ActionAll : this.ActionOne;

			this.okButton.Enable = this.DateFrom.HasValue
								&& this.DateTo.HasValue
								&& this.DateFrom < this.DateTo
								&& !this.HasError;
		}
	}
}