//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class LockedPopup : AbstractStackedPopup
	{
		public LockedPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.Locked.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.Locked.Radios.IsDelete.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.Locked.Date.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.Locked.Radios.IsAll.ToString (),
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
				return controller.Value == 1;  // "Tous les objets" si true
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
				return controller.Value == 1;  // "Déverrouiller" si true
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


		public string							Description
		{
			//	Retourne la description de l'opération effectuée.
			//	Par exemple "Verrouiller - Jusqu'au 31.03.2015 - Tous les objets"
			get
			{
				var dd = Res.Strings.Popup.Locked.Radios.IsDelete.ToString ().Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries);
				var d = this.IsDelete ? dd[1] : dd[0];  // "Déverrouiller" ou "Verrouiller"

				var date = string.Concat(Res.Strings.Popup.Locked.Date.ToString (), " ", TypeConverters.DateToString (this.Date.GetValueOrDefault ()));

				var aa = Res.Strings.Popup.Locked.Radios.IsAll.ToString ().Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries);
				var a = this.IsAll ? aa[1] : aa[0];  // "Tous les objets" ou "L'objet sélectionné"

				return UniversalLogic.NiceJoin (d, date, a);
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			//	Cache le champ date en mode "Déverrouiller".
			this.SetVisibility (1, !this.IsDelete);

			//	Grise le bouton radio "L'objet sélectionné" s'il n'y a pas de sélection.
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetRadioEnable (0, this.OneSelectionAllowed);
			}

			//	Nomme le bouton principal, selon les boutons radio.
			if (this.IsDelete)
			{
				this.okButton.Text = this.IsAll ?
					Res.Strings.Popup.Locked.Button.UnlockAll.ToString () :
					Res.Strings.Popup.Locked.Button.UnlockOne.ToString ();
			}
			else
			{
				this.okButton.Text = this.IsAll ?
					Res.Strings.Popup.Locked.Button.LockAll.ToString () :
					Res.Strings.Popup.Locked.Button.LockOne.ToString ();
			}

			this.okButton.Enable = this.IsDelete || (this.Date.HasValue && !this.HasError);
		}

	}
}