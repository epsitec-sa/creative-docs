//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les 4 valeurs en millimètres d'un Drawing.Margins.
	/// </summary>
	public class MarginsPopup : StackedPopup
	{
		public MarginsPopup(DataAccessor accessor, string title)
			: base (accessor)
		{
			this.title = title;

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge gauche",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge droite",
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge supérieure",
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge inférieure",
			});

			this.SetDescriptions (list);
		}


		public Margins							Value
		{
			get
			{
				double		leftMargin;
				double		rightMargin;
				double		topMargin;
				double		bottomMargin;

				{
					var controller = this.GetController (0) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					leftMargin = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (1) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					rightMargin = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					topMargin = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					bottomMargin = (double) controller.Value.GetValueOrDefault ();
				}

				return new Margins (leftMargin, rightMargin, topMargin, bottomMargin);
			}
			set
			{
				{
					var controller = this.GetController (0) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Left;
				}

				{
					var controller = this.GetController (1) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Right;
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Top;
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Bottom;
				}
			}
		}
	}
}