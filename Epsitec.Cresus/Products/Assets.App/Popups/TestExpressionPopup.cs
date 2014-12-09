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
				Label                 = "BaseAmount",
				DecimalFormat         = DecimalFormat.Amount,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "StartYearAmount",
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
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "YearCount",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 7
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Periodicity",
				MultiLabels           = TestExpressionPopup.PeriodicityLabels,
				Width                 = 240,
			});

			list.Add (new StackedControllerDescription  // 8
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "YearRank",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 9
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "PeriodRank",
				DecimalFormat         = DecimalFormat.Real,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 10
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ProrataNumerator",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 11
			{
				StackedControllerType = StackedControllerType.Decimal,
				Label                 = "ProrataDenominator",
				DecimalFormat         = DecimalFormat.Real,
			});

			list.Add (new StackedControllerDescription  // 12
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 230,
				Height                = 15*5,  // place pour 5 lignes (arbitrairement)
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Test";  // anglais, ne pas traduire
			this.defaultCancelButtonName = Res.Strings.Popup.Button.Close.ToString ();
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

		private decimal StartYearAmount
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
				return TestExpressionPopup.RankToPeriodicity (controller.Value.GetValueOrDefault ());
			}
			set
			{
				var controller = this.GetController (7) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = TestExpressionPopup.PeriodicityToRank (value);
			}
		}

		private decimal YearRank
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

		private decimal PeriodRank
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

		private decimal ProrataNumerator
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

		private decimal ProrataDenominator
		{
			get
			{
				var controller = this.GetController (11) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (11) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private AmortizationDetails Details
		{
			get
			{
				var def = new AmortizationDefinition (DateRange.Empty, System.DateTime.MinValue, null, null, this.Periodicity, this.StartYearAmount);
				var history = new HistoryDetails (this.BaseAmount, this.InitialAmount, this.YearRank, this.PeriodRank);

				return new AmortizationDetails (def, history);
			}
			set
			{
				this.BaseAmount         = value.History.BaseAmount;
				this.StartYearAmount    = value.Def.StartYearAmount;
				this.InitialAmount      = value.History.InitialAmount;
				this.Periodicity        = value.Def.Periodicity;
				this.YearRank           = value.History.YearRank;
				this.PeriodRank         = value.History.PeriodRank;
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Name = "NoClose";
		}


		private void SetResult(AbstractCalculator.Result result)
		{
			var builder = new System.Text.StringBuilder ();

			if (!string.IsNullOrEmpty (result.Trace))
			{
				builder.Append (result.Trace);
			}

			if (result.IsEmpty)
			{
				builder.Append ("Result = Error");  // anglais, ne pas traduire
			}
			else
			{
				builder.Append ("Result = ");
				builder.Append (TypeConverters.AmountToString (result.Value));
			}

			this.SetResult (builder.ToString ());
		}

		private void SetResult(string message)
		{
			var controller = this.GetController (12) as LabelStackedController;
			System.Diagnostics.Debug.Assert (controller != null);

			controller.SetLabel (message);
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
		public static void Show(Widget target, DataAccessor accessor, string arguments, string expression)
		{
			if (target != null)
			{
				var popup = new TestExpressionPopup (accessor)
				{
					Details = LocalSettings.TestExpressionDetails,
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "NoClose")
					{
						LocalSettings.TestExpressionDetails = popup.Details;

						var details = AmortizationDetails.SetMethod (LocalSettings.TestExpressionDetails, arguments, expression);
						var result = Amortizations.ComputeAmortization (accessor, details);
						popup.SetResult (result);
					}
				};
			}
		}
		#endregion
	}
}