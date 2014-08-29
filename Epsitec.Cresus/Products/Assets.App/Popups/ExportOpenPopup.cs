//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant d'ouvrir le fichier ou l'emplacement résultant d'une exportation.
	/// </summary>
	public class ExportOpenPopup : StackedPopup
	{
		public ExportOpenPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.ExportOpen.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.ExportOpen.Radios.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Open.ToString ();
			this.defaultCancelButtonName = Res.Strings.Popup.Button.Close.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public bool								OpenLocation
		{
			get
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value == 1;
			}
			set
			{
				var controller = this.GetController (0) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 1 : 0;
			}
		}
	}
}