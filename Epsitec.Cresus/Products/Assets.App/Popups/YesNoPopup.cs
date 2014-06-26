//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Affiche un dialogue oui/non avec une question.
	/// </summary>
	public class YesNoPopup : StackedPopup
	{
		public YesNoPopup(string question)
			: base (null)
		{
			this.title = "Question";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 200,
				Label                 = question,
			});

			this.SetDescriptions (list);
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton    .Text = "Oui";
			this.cancelButton.Text = "Non";
		}


		#region Helpers
		public static void ShowAssetsDeleteEventQuestion(Widget target, System.Action action)
		{
			YesNoPopup.Show (target, "Voulez-vous supprimer l'événement sélectionné ?", action);
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