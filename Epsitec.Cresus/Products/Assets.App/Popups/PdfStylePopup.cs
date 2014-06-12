//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.Core.Helpers;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir le style d'exportation pdf.
	/// </summary>
	public class PdfStylePopup : StackedPopup
	{
		public PdfStylePopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Style";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Style prédéfini",
				MultiLabels           = PdfPredefinedStyleHelpers.Labels,
				Width                 = 150,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Couleur en-tête",
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 150,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Couleur lignes paires",
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 150,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Couleur lignes impaires",
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 150,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Combo,
				Label                 = "Couleur traits",
				MultiLabels           = ExportColorHelpers.Labels,
				Width                 = 150,
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Epaisseur traits",
			});

			this.SetDescriptions (list);
		}


		public PdfStyle							Value
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

			if (rank == 0)  // modification du style ?
			{
				var controller = this.GetController (0) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				var style = PdfPredefinedStyleHelpers.IntToPredefined (controller.Value);
				this.Value = PdfStyle.Factory (style);
			}
			else  // autre modification ?
			{
				this.Value = this.Value;
			}
		}
	}
}