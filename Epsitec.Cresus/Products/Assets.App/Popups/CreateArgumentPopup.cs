//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'une
	/// nouvelle expression, à savoir le nom de l'expression.
	/// </summary>
	public class CreateArgumentPopup : AbstractStackedPopup
	{
		private CreateArgumentPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.CreateArgument.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.CreateCategory.Name.ToString (),
				Width                 = DateController.controllerWidth,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private string							ObjectName
		{
			get
			{
				var controller = this.GetController (0) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Enable = !string.IsNullOrEmpty (this.ObjectName);
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<string> action)
		{
			//	Affiche le Popup.
			var popup = new CreateArgumentPopup (accessor);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.ObjectName);
				}
			};
		}
		#endregion
	}
}