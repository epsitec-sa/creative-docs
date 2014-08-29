//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouveau contact, à savoir son nom.
	/// </summary>
	public class CreatePersonPopup : StackedPopup
	{
		public CreatePersonPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.CreatePerson.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.CreatePerson.Name.ToString (),
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.CreatePerson.Model.ToString (),
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.PersonGuid,
				Label                 = "",
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
				Height                = 300,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		public string							PersonName
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

		private bool							UseModel
		{
			get
			{
				var controller = this.GetController (1) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public Guid								PersonModel
		{
			get
			{
				if (this.UseModel)
				{
					var controller = this.GetController (2) as PersonGuidStackedController;
					System.Diagnostics.Debug.Assert (controller != null);
					return controller.Value;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				var controller = this.GetController (2) as PersonGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
				this.UseModel = !value.IsEmpty;
			}
		}



		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetEnable (2, this.UseModel);

			this.okButton.Enable = !string.IsNullOrEmpty (this.PersonName);
		}
	}
}