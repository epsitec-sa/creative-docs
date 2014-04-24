//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class LockedPopup2 : StackedPopup
	{
		public LockedPopup2(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Gestion des verrous";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				Label                 = "Verrouiller<br/>Déverrouiller",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				Label                 = "Jusqu'au",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Radio,
				Label                 = "L'objet sélectionné<br/>Tous les objets",
			});

			this.SetDescriptions (list);
		}


		public bool								OneSelectionAllowed;

		public bool								IsAll
		{
			get
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value == 1;
			}
			set
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 1 : 0;
			}
		}

		public bool								IsDelete
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value == 1;
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 1 : 0;
			}
		}

		public System.DateTime?					Date
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


		protected override void UpdateWidgets()
		{
			this.SetVisibility (1, !this.IsDelete);

			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetRadioEnable (0, this.OneSelectionAllowed);
			}

			if (this.IsDelete)
			{
				this.okButton.Text = this.IsAll ? "Tout déverrouiller" : "Déverrouiller un";
			}
			else
			{
				this.okButton.Text = this.IsAll ? "Tout verrouiller" : "Verrouiller un";
			}

			this.okButton.Enable = this.IsDelete || this.Date.HasValue;
		}

	}
}