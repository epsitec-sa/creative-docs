//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant de déverrouiller l'édition de la valeur finale.
	/// </summary>
	public class FinalUnlockPopup : AbstractStackedPopup
	{
		private FinalUnlockPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Déverrouillage de la valeur finale";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 300,
				Label                 = "Il existe des amortissements postérieurs.",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 300,
				Label                 = "Que voulez-vous en faire ?",
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = "Les recalculer<br/>Les supprimer",
				BottomMargin          = 10,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Ok.ToString ();
		}


		private int								Oper
		{
			get
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault ();
			}
			set
			{
				var controller = this.GetController (2) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, int oper, System.Action<int> action)
		{
			if (target != null)
			{
				var popup = new FinalUnlockPopup (accessor)
				{
					Oper = oper,
				};

				popup.Create (target, leftOrRight: false);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action (popup.Oper);
					}
				};
			}
		}
		#endregion

	}
}