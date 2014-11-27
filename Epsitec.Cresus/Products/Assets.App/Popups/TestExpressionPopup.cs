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
	/// Popup permettant de définir les paramètres d'une expression d'amortissement puis
	/// de l'évaluer pour contrôler le résultat.
	/// </summary>
	public class TestExpressionPopup : AbstractStackedPopup
	{
		private TestExpressionPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.TestExpression.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ForcedAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "BaseAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "InitialAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ResidualAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "RoundAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "Rate",
				DecimalFormat         = DecimalFormat.Rate,
			});

			list.Add (new StackedControllerDescription  // 6
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Periodicity",
				MultiLabels           = TestExpressionPopup.PeriodicityLabels,
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 7
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ProrataNumerator",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 8
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ProrataDenominator",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 9
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "YearCount",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 10
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "YearRank",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 11
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 200,
				Height                = 15*1,  // place pour 1 ligne
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Test";  // anglais, ne pas traduire
			this.defaultCancelButtonName = Res.Strings.Popup.Button.Close.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private decimal? ForcedAmount
		{
			get
			{
				var controller = this.GetController (0) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal BaseAmount
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

		private decimal InitialAmount
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

		private Periodicity Periodicity
		{
			get
			{
				var controller = this.GetController (6) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return TestExpressionPopup.RankToPeriodicity (controller.Value.GetValueOrDefault ());
			}
			set
			{
				var controller = this.GetController (6) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = TestExpressionPopup.PeriodicityToRank (value);
			}
		}

		private decimal ProrataNumerator
		{
			get
			{
				var controller = this.GetController (7) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (7) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal ProrataDenominator
		{
			get
			{
				var controller = this.GetController (8) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (8) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal YearCount
		{
			get
			{
				var controller = this.GetController (9) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (9) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private decimal YearRank
		{
			get
			{
				var controller = this.GetController (10) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (10) as DecimalStackedController;
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
					this.YearRank, this.YearCount,
					this.Periodicity,
					this.ForcedAmount,
					this.InitialAmount, this.InitialAmount,
					this.BaseAmount,
					this.ProrataNumerator, this.ProrataDenominator,
					this.RoundAmount,
					this.ResidualAmount,
					EntryScenario.None, System.DateTime.Now,
					Guid.Empty, Guid.Empty, Guid.Empty, 0);
			}
			set
			{
					this.ForcedAmount       = value.ForcedAmount;
					this.BaseAmount         = value.BaseAmount.GetValueOrDefault ();
					this.InitialAmount      = value.InitialAmount.GetValueOrDefault ();
					this.ResidualAmount     = value.ResidualAmount.GetValueOrDefault ();
					this.RoundAmount        = value.RoundAmount.GetValueOrDefault ();
					this.Rate               = value.Rate.GetValueOrDefault ();
					this.Periodicity        = value.Periodicity;
					this.ProrataNumerator   = value.ProrataNumerator.GetValueOrDefault ();
					this.ProrataDenominator = value.ProrataDenominator.GetValueOrDefault ();
					this.YearCount          = value.YearCount;
					this.YearRank           = value.YearRank;
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Name = "NoClose";
		}

		private void SetResult(decimal? result)
		{
			var controller = this.GetController (11) as LabelStackedController;
			System.Diagnostics.Debug.Assert (controller != null);

			if (result.HasValue)
			{
				controller.SetLabel ("Result = " + TypeConverters.AmountToString (result));
			}
			else
			{
				controller.SetLabel ("Result = Error");  // anglais, ne pas traduire
			}
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
		public static void Show(Widget target, DataAccessor accessor, AmortizationExpression expression)
		{
			if (target != null)
			{
				var popup = new TestExpressionPopup (accessor)
				{
					Amount = LocalSettings.TestExpressionAmount,
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "NoClose")
					{
						LocalSettings.TestExpressionAmount = popup.Amount;
						var result = expression.Evaluate (popup.Amount);
						popup.SetResult (result);
					}
				};
			}
		}
		#endregion
	}
}