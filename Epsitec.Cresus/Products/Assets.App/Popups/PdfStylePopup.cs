//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir le style d'exportation pdf.
	/// </summary>
	public class PdfStylePopup : AbstractStackedPopup
	{
		private PdfStylePopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.PdfStyle.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.PdfStyle.Predefined.ToString (),
				MultiLabels           = PdfPredefinedStyleHelpers.Labels,
				Width                 = 160,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.PdfStyle.LabelColor.ToString (),
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 160,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.PdfStyle.EvenColor.ToString (),
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 160,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.PdfStyle.OddColor.ToString (),
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 160,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = Res.Strings.Popup.PdfStyle.BorderColor.ToString (),
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 160,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = Res.Strings.Popup.PdfStyle.Thickness.ToString (),
			});

			this.SetDescriptions (list);
		}


		private PdfStyle						Value
		{
			get
			{
				ExportColor	labelColor;
				ExportColor	evenColor;
				ExportColor	oddColor;
				ExportColor	borderColor;
				double		thickness;

				{
					var controller = this.GetController (1) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					labelColor = ExportColorHelpers.IntToColor (controller.Value.GetValueOrDefault (-1));
				}

				{
					var controller = this.GetController (2) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					evenColor = ExportColorHelpers.IntToColor (controller.Value.GetValueOrDefault (-1));
				}

				{
					var controller = this.GetController (3) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					oddColor = ExportColorHelpers.IntToColor (controller.Value.GetValueOrDefault (-1));
				}

				{
					var controller = this.GetController (4) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					borderColor = ExportColorHelpers.IntToColor (controller.Value.GetValueOrDefault (-1));
				}

				{
					var controller = this.GetController (5) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					thickness = (double) controller.Value.GetValueOrDefault ();
				}

				return new PdfStyle (labelColor, evenColor, oddColor, borderColor, thickness);
			}
			set
			{
				{
					var controller = this.GetController (0) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = PdfStyleHelpers.StyleToInt (value);
				}

				{
					var controller = this.GetController (1) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = ExportColorHelpers.ColorToInt (value.LabelColor);
				}

				{
					var controller = this.GetController (2) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = ExportColorHelpers.ColorToInt (value.EvenColor);
				}

				{
					var controller = this.GetController (3) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = ExportColorHelpers.ColorToInt (value.OddColor);
				}

				{
					var controller = this.GetController (4) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = ExportColorHelpers.ColorToInt (value.BorderColor);
				}

				{
					var controller = this.GetController (5) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = (decimal) value.BorderThickness;
				}
			}
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			int rank = this.GetRank (description);

			if (rank == 0)  // modification du style prédéfini ?
			{
				var controller = this.GetController (0) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				var predefined = PdfPredefinedStyleHelpers.IntToPredefined (controller.Value);
				this.Value = PdfStyle.Factory (predefined);
			}
			else  // autre modification ?
			{
				this.Value = this.Value;
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, PdfStyle style, System.Action<PdfStyle> action)
		{
			//	Affiche le Popup.
			var popup = new PdfStylePopup (accessor)
			{
				Value = style,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.Value);
				}
			};
		}
		#endregion
	}
}