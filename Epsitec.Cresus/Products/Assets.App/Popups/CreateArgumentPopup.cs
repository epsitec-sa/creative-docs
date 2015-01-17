//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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
		public CreateArgumentPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Création d'un nouvel argument";

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


		public string							ObjectName
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
	}
}