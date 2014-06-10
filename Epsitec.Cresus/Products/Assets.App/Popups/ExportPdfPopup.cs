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

			this.SetDescriptions (list);
		}


		public PdfExportProfile					Profile
		{
			get
			{
				bool		landscape;
				bool		evenOddGrey;

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

				return new PdfExportProfile (landscape, evenOddGrey);
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
			}
		}


		protected override void UpdateWidgets()
		{
			this.okButton.Text = "Exporter";
		}
	}
}