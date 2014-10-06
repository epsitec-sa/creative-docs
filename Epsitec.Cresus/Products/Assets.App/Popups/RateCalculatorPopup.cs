//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class RateCalculatorPopup : AbstractStackedPopup
	{
		public RateCalculatorPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.RateCalculator.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = Res.Strings.Popup.RateCalculator.TotalYears.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 80,
				Height                = 15*1,  // place pour 1 ligne
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Compute.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public decimal?							Rate
		{
			get
			{
				var years = this.TotalYears;

				if (years.HasValue && years != 0.0m)
				{
					return 1.0m / years;
				}
				else
				{
					return null;
				}
			}
			set
			{
				if (value.HasValue && value.Value != 0.0m)
				{
					this.TotalYears = (int) System.Math.Floor ((1.0m / value.Value) + 0.5m);
				}
				else
				{
					this.TotalYears = null;
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


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			{
				var controller = this.GetController (1) as LabelStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetLabel (this.Result);
			}

			this.okButton.Enable = this.TotalYears.HasValue;
		}

		private string Result
		{
			get
			{
				if (this.Rate.HasValue)
				{
					var rate = TypeConverters.RateToString (this.Rate);
					return string.Format (Res.Strings.Popup.RateCalculator.Result.ToString (), rate);
				}
				else
				{
					return null;
				}
			}
		}
	}
}