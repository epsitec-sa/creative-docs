//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Core.Helpers;
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
				Label                 = "Unifiée",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Gauche",
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Droite",
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Supérieure",
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Inférieure",
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
					var controller = this.GetController (1) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					leftMargin = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					rightMargin = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					topMargin = (double) controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (4) as DecimalStackedController;
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

					if (MarginsPopup.IsUnified (value))
					{
						controller.Value = (decimal) value.Left;
					}
					else
					{
						controller.Value = null;
					}
				}

				{
					var controller = this.GetController (1) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Left;
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Right;
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Top;
				}

				{
					var controller = this.GetController (4) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.Bottom;
				}
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			int rank = this.GetRank (description);

			if (rank == 0)  // modification de la marge unifiée ?
			{
				var controller = this.GetController (0) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				this.Value = new Margins ((double) controller.Value.GetValueOrDefault ());
			}
			else  // modification d'une marge distincte ?
			{
				this.Value = this.Value;
			}
		}


		public static string GetDescription(Margins margins)
		{
			if (MarginsPopup.IsUnified (margins))
			{
				var l = TypeConverters.DecimalToString ((decimal) margins.Left);

				return string.Format ("{0} mm", l);
			}
			else
			{
				var l = TypeConverters.DecimalToString ((decimal) margins.Left);
				var r = TypeConverters.DecimalToString ((decimal) margins.Right);
				var t = TypeConverters.DecimalToString ((decimal) margins.Top);
				var b = TypeConverters.DecimalToString ((decimal) margins.Bottom);

				return string.Format ("{0} / {1} / {2} / {3} mm", l, r, t, b);
			}
		}

		private static bool IsUnified(Margins margins)
		{
			return margins.Left == margins.Right
				&& margins.Left == margins.Top
				&& margins.Left == margins.Bottom;
		}
	}
}