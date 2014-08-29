//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Affiche un dialogue oui/non avec une question.
	/// </summary>
	public class YesNoPopup : StackedPopup
	{
		private YesNoPopup(string question)
			: base (null)
		{
			this.title = Res.Strings.Popup.YesNo.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 200,
				Label                 = question,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Yes.ToString ();
			this.defaultCancelButtonName = Res.Strings.Popup.Button.No.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		#region Helpers
		public static void ShowAssetsDeleteEventQuestion(Widget target, System.Action action)
		{
			YesNoPopup.Show (target, Res.Strings.Popup.YesNo.DeleteEventQuestion.ToString (), action);
		}

		public static void Show(Widget target, string question, System.Action action)
		{
			if (target != null)
			{
				var popup = new YesNoPopup (question);

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action ();
					}
				};
			}
		}
		#endregion
	}
}