//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
			this.title = Res.Strings.Popup.AmountExpressionSimulation.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				Label                 = "Depuis le",
				DateRangeCategory     = DateRangeCategory.Free,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Date,
				Label                 = "Jusqu'au",
				DateRangeCategory     = DateRangeCategory.Free,
				BottomMargin          = 20,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Montant initial",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Valeur résiduelle",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Arrondi",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Taux",
				DecimalFormat         = DecimalFormat.Rate,
			});

			list.Add (new StackedControllerDescription  // 6
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Nombre d'années",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 7
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Périodicité",
				MultiLabels           = AmountExpressionSimulationPopup.PeriodicityLabels,
				Width                 = 240,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Simulation";
			this.defaultCancelButtonName = Res.Strings.Popup.Button.Cancel.ToString ();
			this.defaultControllerRankFocus = 2;
		}


		private DateRange Range
		{
			get
			{
				return new DateRange (this.StartDate, this.ToDate);
			}
			set
			{
				this.StartDate = value.IncludeFrom;
				this.ToDate    = value.ExcludeTo;
			}
		}

		private System.DateTime StartDate
		{
			get
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				if (controller.Value.HasValue)
				{
					return controller.Value.Value;
				}
				else
				{
					return new System.DateTime (2000, 1, 1);
				}
			}
			set
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private System.DateTime ToDate
		{
			get
			{
				var controller = this.GetController (1) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				if (controller.Value.HasValue)
				{
					return controller.Value.Value;
				}
				else
				{
					return new System.DateTime (2100, 1, 1);
				}
			}
			set
			{
				var controller = this.GetController (1) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal BaseAmount
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

		private decimal ResidualAmount
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

		private decimal RoundAmount
		{
			get
			{
				var controller = this.GetController (4) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (4) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal Rate
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

		private decimal YearCount
		{
			get
			{
				var controller = this.GetController (6) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (6) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private Periodicity Periodicity
		{
			get
			{
				var controller = this.GetController (7) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return AmountExpressionSimulationPopup.RankToPeriodicity (controller.Value.GetValueOrDefault ());
			}
			set
			{
				var controller = this.GetController (7) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = AmountExpressionSimulationPopup.PeriodicityToRank (value);
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
					0.0m,
					this.Periodicity,
					null,
					this.BaseAmount, this.BaseAmount,
					this.BaseAmount,
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
					Range  = LocalSettings.ExpressionSimulationRange,
					Amount = LocalSettings.ExpressionSimulationAmount,
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						LocalSettings.ExpressionSimulationRange  = popup.Range;
						LocalSettings.ExpressionSimulationAmount = popup.Amount;
						action ();
					}
				};
			}
		}
		#endregion
	}
}