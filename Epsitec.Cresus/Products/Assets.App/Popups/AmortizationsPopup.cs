//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AmortizationsPopup : AbstractStackedPopup
	{
		private AmortizationsPopup(DataAccessor accessor)
			: base(accessor)
		{
			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.Amortizations.FromDate.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.Amortizations.ToDate.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.Amortizations.Object.ToString (),
			});

			this.SetDescriptions (list);
		}


		private string							Title
		{
			set
			{
				this.title = value;
			}
		}

		private string							ActionOne;
		private string							ActionAll;
		private bool							DateFromAllowed;
		private bool							DateToAllowed;
		private bool							OneSelectionAllowed;

		private bool							IsAll
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

		private System.DateTime?				DateFrom
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

		private System.DateTime?				DateTo
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


		private string							Description
		{
			//	Retourne la description de l'opération effectuée.
			//	Par exemple "Depuis 31.03.2015 - Pour tous les objets"
			get
			{
				string dateFrom = null;
				if (this.DateFromAllowed)
				{
					dateFrom = string.Concat (Res.Strings.Popup.Amortizations.FromDate.ToString (), " ", TypeConverters.DateToString (this.DateFrom));
				}

				string dateTo = null;
				if (this.DateToAllowed)
				{
					dateTo = string.Concat (Res.Strings.Popup.Amortizations.ToDate.ToString (), " ", TypeConverters.DateToString (this.DateFrom));
				}

				var aa = Res.Strings.Popup.Amortizations.Object.ToString ().Split (new string[] { "<br/>" }, System.StringSplitOptions.RemoveEmptyEntries);
				var a = this.IsAll ? aa[1] : aa[0];  // "Pour tous les objets" ou "Pour l'objet sélectionné"

				return UniversalLogic.NiceJoin (dateFrom, dateTo, a);
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

			if (this.DateFromAllowed && this.DateToAllowed)
			{
				this.okButton.Enable = this.DateFrom.HasValue
									&& this.DateTo.HasValue
									&& this.DateFrom < this.DateTo
									&& !this.HasError;
			}
			else if (this.DateFromAllowed)
			{
				this.okButton.Enable = this.DateFrom.HasValue
									&& !this.HasError;
			}
			else
			{
				this.okButton.Enable = !this.HasError;
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, bool fromAllowed, bool toAllowed, string title, string one, string all, bool isAll, DateRange range, System.Action<DateRange, bool, string> action)
		{
			//	Affiche le popup pour l'amortissement.
			var popup = new AmortizationsPopup (accessor)
			{
				Title               = title,
				ActionOne           = one,
				ActionAll           = all,
				DateFromAllowed     = fromAllowed,
				DateToAllowed       = toAllowed,
				OneSelectionAllowed = !isAll,
				IsAll               =  isAll,
				DateFrom            = range.IncludeFrom,
				DateTo              = range.ExcludeTo,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					System.Diagnostics.Debug.Assert (popup.DateFrom.HasValue);
					System.Diagnostics.Debug.Assert (popup.DateTo.HasValue);
					range = new DateRange (popup.DateFrom.Value, popup.DateTo.Value.AddDays (1));

					action (range, popup.IsAll, popup.Description);
				}
			};
		}
		#endregion
	}
}