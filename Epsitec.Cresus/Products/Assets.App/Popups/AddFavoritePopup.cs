//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class AddFavoritePopup : AbstractStackedPopup
	{
		private AddFavoritePopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.AddFavorite.Title.ToString ();

			var list = new List<StackedControllerDescription> ();


			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.AddFavorite.Radios.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Ok.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public bool								CreateOperation
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


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<bool> action)
		{
			if (target != null)
			{
				var popup = new AddFavoritePopup (accessor)
				{
					CreateOperation = false,
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action (popup.CreateOperation);
					}
				};
			}
		}
		#endregion
	}
}