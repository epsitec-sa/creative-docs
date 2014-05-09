//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class RateCalculatorPopup : StackedPopup
	{
		public RateCalculatorPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Calcul du taux d'amortissement linéaire";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Nombre d'années",
			});

			this.SetDescriptions (list);
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


		public override void CreateUI()
		{
			base.CreateUI ();

			var controller = this.GetController (0);
			controller.SetFocus ();

			this.okButton.Text = "Calculer";
		}

		protected override void UpdateWidgets()
		{
			this.okButton.Enable = this.TotalYears.HasValue;
		}
	}
}