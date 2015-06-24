//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class YearsCalculatorPopup : AbstractStackedPopup
	{
		private YearsCalculatorPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.YearsCalculator.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = Res.Strings.Popup.YearsCalculator.TotalYears.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = Res.Strings.Popup.YearsCalculator.TotalMonths.ToString (),
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 80,
				Height                = 15*1,  // place pour 1 ligne
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Compute.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private decimal?						Years
		{
			get
			{
				var years  = (decimal) this.TotalYears .GetValueOrDefault (1);
				var months = (decimal) this.TotalMonths.GetValueOrDefault (0);

				return years + months/12.0m;
			}
			set
			{
				if (value.HasValue && value.Value != 0.0m)
				{
					var years  = (decimal) ((int) value.Value);
					var months = System.Math.Floor (((value.Value - years) * 12.0m) + 0.5m);

					this.TotalYears  = (int) years;
					this.TotalMonths = (int) months;
				}
				else
				{
					this.TotalYears  = null;
					this.TotalMonths = null;
				}
			}
		}

		private int?							TotalYears
		{
			get
			{
				var controller = this.GetController (0) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private int?							TotalMonths
		{
			get
			{
				var controller = this.GetController (1) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as IntStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			{
				var controller = this.GetController (2) as LabelStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetLabel (this.Result);
			}

			this.okButton.Enable = this.TotalYears.HasValue;
		}

		private string Result
		{
			get
			{
				if (this.Years.HasValue)
				{
					var s = TypeConverters.DecimalToString (this.Years, 10);

					//	Supprime les chiffres non significatifs.
					while (s.Length > 1 && s.Last () == '0')
					{
						s = s.Substring (0, s.Length-1);
					}

					if (s.Length > 1 && s.Last () == '.')
					{
						s = s.Substring (0, s.Length-1);
					}

					return s;
				}
				else
				{
					return null;
				}
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, decimal? years, System.Action<decimal?> action)
		{
			//	Affiche le Popup.
			var popup = new YearsCalculatorPopup (accessor)
			{
				Years = years,
			};

			popup.Create (target, leftOrRight: false);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.Years);
				}
			};
		}
		#endregion
	}
}