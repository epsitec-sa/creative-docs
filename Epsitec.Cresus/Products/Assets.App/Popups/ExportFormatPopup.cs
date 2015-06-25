//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.Export;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de choisir le format d'exportation d'un TreeTable.
	/// </summary>
	public class ExportFormatPopup : AbstractStackedPopup
	{
		private ExportFormatPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.ExportInstructions.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = ExportInstructionsHelpers.MultiLabels,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Export.ToString ();
		}


		private ExportFormat					Format
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return ExportInstructionsHelpers.GetFormat (controller.Value.GetValueOrDefault ());
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = ExportInstructionsHelpers.GetRank (value);
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, ExportFormat format, System.Action<ExportFormat> action)
		{
			//	Affiche le popup pour choisir un type d'exportation.
			var popup = new ExportFormatPopup (accessor)
			{
				Format = format,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.Format);
				}
			};
		}
		#endregion
	}
}