//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Expression;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de définir les paramètres d'une expression d'amortissement
	/// pour lancer la simulation des amortissements.
	/// </summary>
	public class AmountExpressionSimulationPopup : AbstractStackedPopup
	{
		private AmountExpressionSimulationPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Paramètres initiaux de la simulation";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "BaseAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ResidualAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "RoundAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Rate",
				DecimalFormat         = DecimalFormat.Rate,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Periodicity",
				MultiLabels           = AmountExpressionSimulationPopup.PeriodicityLabels,
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "YearCount",
				DecimalFormat         = DecimalFormat.Real,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Simulation";  // anglais, ne pas traduire
			this.defaultCancelButtonName = Res.Strings.Popup.Button.Cancel.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private decimal BaseAmount
		{
			get
			{
				var controller = this.GetController (0) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (0) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal ResidualAmount
		{
			get
			{
				var controller = this.GetController (1) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (1) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal RoundAmount
		{
			get
			{
				var controller = this.GetController (2) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (2) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal Rate
		{
			get
			{
				var controller = this.GetController (3) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (3) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private Periodicity Periodicity
		{
			get
			{
				var controller = this.GetController (4) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return AmountExpressionSimulationPopup.RankToPeriodicity (controller.Value.GetValueOrDefault ());
			}
			set
			{
				var controller = this.GetController (4) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = AmountExpressionSimulationPopup.PeriodicityToRank (value);
			}
		}

		private decimal YearCount
		{
			get
			{
				var controller = this.GetController (5) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (5) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private AmortizedAmount Amount
		{
			get
			{
				return new AmortizedAmount (
					AmortizationMethod.Custom, null,
					this.Rate,
					0.0m, this.YearCount,
					this.Periodicity,
					null,
					this.BaseAmount, this.BaseAmount,
					this.BaseAmount,
					null, null,
					this.RoundAmount,
					this.ResidualAmount,
					EntryScenario.None, System.DateTime.Now,
					Guid.Empty, Guid.Empty, Guid.Empty, 0);
			}
			set
			{
					this.BaseAmount     = value.BaseAmount.GetValueOrDefault ();
					this.ResidualAmount = value.ResidualAmount.GetValueOrDefault ();
					this.RoundAmount    = value.RoundAmount.GetValueOrDefault ();
					this.Rate           = value.Rate.GetValueOrDefault ();
					this.Periodicity    = value.Periodicity;
					this.YearCount      = value.YearCount;
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
		}


		private static int PeriodicityToRank(Periodicity periodicity)
		{
			var list = EnumDictionaries.DictPeriodicities.Select (x => x.Key).ToList ();
			return list.IndexOf ((int) periodicity);
		}

		private static Periodicity RankToPeriodicity(int index)
		{
			var list = EnumDictionaries.DictPeriodicities.Select (x => x.Key).ToArray ();

			if (index >= 0 && index < list.Length)
			{
				return (Periodicity) list[index];
			}
			else
			{
				return Periodicity.Unknown;
			}
		}

		private static string PeriodicityLabels
		{
			get
			{
				return string.Join ("<br/>", EnumDictionaries.DictPeriodicities.Select (x => x.Value));
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action action)
		{
			if (target != null)
			{
				var popup = new AmountExpressionSimulationPopup (accessor)
				{
					Amount = LocalSettings.ExpressionSimulationAmount,
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						LocalSettings.ExpressionSimulationAmount = popup.Amount;
						action ();
					}
				};
			}
		}
		#endregion
	}
}