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
	/// Popup posant la question avant de quitter le logiciel.
	/// </summary>
	public class QuitPopup : AbstractStackedPopup
	{
		private QuitPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.Quit.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = 200,
				Label                 = Res.Strings.Popup.Quit.Question.ToString (),
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Radio,
				MultiLabels           = Res.Strings.Popup.Quit.Radios.ToString (),
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Quit.MainButton.ToString ();
		}


		private bool							Save
		{
			get
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value.GetValueOrDefault () == 0;
			}
			set
			{
				var controller = this.GetController (1) as RadioStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value ? 0 : 1;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			if (string.IsNullOrEmpty (this.accessor.ComputerSettings.MandatFilename))
			{
				//	S'il n'y a pas de nom de fichier connu pour le mandat, on ne pourra
				//	pas enregistrer au moment de quitter le logiciel.
				this.Save = false;
				this.SetEnable (1, false);
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<bool> action)
		{
			if (target != null)
			{
				var popup = new QuitPopup (accessor)
				{
					Save = true,
				};

				popup.Create (target, leftOrRight: false);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						action (popup.Save);
					}
				};
			}
		}
		#endregion

	}
}