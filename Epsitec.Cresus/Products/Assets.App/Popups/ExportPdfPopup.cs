//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Format paysage",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = "Une ligne sur deux en gris",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Marge gauche",
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Marge droite",
			});

			list.Add (new StackedControllerDescription  // 4
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Marge supérieure",
			});

			list.Add (new StackedControllerDescription  // 5
			{
				StackedControllerType = StackedControllerType.Int,
				Label                 = "Marge inférieure",
			});

			this.SetDescriptions (list);
		}


		public PdfExportProfile					Profile
		{
			get
			{
				bool		landscape;
				bool		evenOddGrey;
				int			leftMargin;
				int			rightMargin;
				int			topMargin;
				int			bottomMargin;

				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					landscape = controller.Value;
				}

				{
					var controller = this.GetController (1) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					evenOddGrey = controller.Value;
				}

				{
					var controller = this.GetController (2) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					leftMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (3) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					rightMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (4) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					topMargin = controller.Value.GetValueOrDefault ();
				}

				{
					var controller = this.GetController (5) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					bottomMargin = controller.Value.GetValueOrDefault ();
				}

				return new PdfExportProfile (landscape, evenOddGrey, leftMargin, rightMargin, topMargin, bottomMargin);
			}
			set
			{
				{
					var controller = this.GetController (0) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.Landscape;
				}

				{
					var controller = this.GetController (1) as BoolStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.EvenOddGrey;
				}

				{
					var controller = this.GetController (2) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.LeftMargin;
				}

				{
					var controller = this.GetController (3) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.RightMargin;
				}

				{
					var controller = this.GetController (4) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.TopMargin;
				}

				{
					var controller = this.GetController (5) as IntStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					controller.Value = value.BottomMargin;
				}
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Exporter";
		}
	}
}