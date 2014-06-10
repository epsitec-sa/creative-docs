//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir les paramètres pour l'exportation au format pdf.
	/// </summary>
	public class ExportPdfPopup : StackedPopup
	{
		public ExportPdfPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Exportation des données au format PDF";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Largeur page",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Hauteur page",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge gauche",
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge droite",
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge supérieure",
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marge inférieure",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 6
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Real,
				Label                 = "Taille de police",
			});

			list.Add (new StackedControllerDescription  // 7
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Millimeters,
				Label                 = "Marges des cellules",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 8
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Une ligne sur deux en gris",
			});

			list.Add (new StackedControllerDescription  // 9
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Indentation",
				Width                 = 200,
			});

			list.Add (new StackedControllerDescription  // 10
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Filigrane",
				Width                 = 200,
			});

			this.SetDescriptions (list);
		}


		public PdfExportProfile					Profile
		{
			get
			{
				decimal		pageWidth;
				decimal		pageHeight;
				decimal		leftMargin;
				decimal		rightMargin;
				decimal		topMargin;
				decimal		bottomMargin;
				decimal		fontSize;
				decimal		cellMargins;
				bool		evenOddGrey;
				string		indent;
				string		watermark;

				{
					var controller = this.GetController (0) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					pageWidth = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (1) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					pageHeight = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					leftMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					rightMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (4) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					topMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (5) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					bottomMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (6) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					fontSize = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (7) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					cellMargins = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (8) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					evenOddGrey = controller.Value;
				}

				{
					var controller = this.GetController (9) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					indent = Converters.EditableToInternal (controller.Value);
				}

				{
					var controller = this.GetController (10) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					watermark = controller.Value;
				}

				return new PdfExportProfile (pageWidth, pageHeight, leftMargin, rightMargin, topMargin, bottomMargin, fontSize, cellMargins, evenOddGrey, indent, watermark);
			}
			set
			{
				{
					var controller = this.GetController (0) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.PageWidth;
				}

				{
					var controller = this.GetController (1) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.PageHeight;
				}

				{
					var controller = this.GetController (2) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.LeftMargin;
				}

				{
					var controller = this.GetController (3) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.RightMargin;
				}

				{
					var controller = this.GetController (4) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.TopMargin;
				}

				{
					var controller = this.GetController (5) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.BottomMargin;
				}

				{
					var controller = this.GetController (6) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.FontSize;
				}

				{
					var controller = this.GetController (7) as DecimalStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.CellMargins;
				}

				{
					var controller = this.GetController (8) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.EvenOddGrey;
				}

				{
					var controller = this.GetController (9) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = Converters.InternalToEditable (value.Indent);
				}

				{
					var controller = this.GetController (10) as TextStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Watermark;
				}
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Exporter";
		}
	}
}