//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouveau contact, à savoir son nom.
	/// </summary>
	public class CreatePersonPopup : AbstractStackedPopup
	{
		private CreatePersonPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.CreatePerson.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			this.CreateRequiredUserFields (list, BaseType.PersonsUserFields, CreatePersonPopup.fieldWidth);
			this.userFieldsCount = list.Count;

			list.Add (new StackedControllerDescription  // userFieldsCount + 0
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.CreatePerson.Model.ToString (),
			});

			list.Add (new StackedControllerDescription  // userFieldsCount + 1
			{
				StackedControllerType = StackedControllerType.PersonGuid,
				Label                 = "",
				Width                 = CreatePersonPopup.fieldWidth - (int) AbstractScroller.DefaultBreadth - 3,
				Height                = 250,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private bool							UseModel
		{
			get
			{
				var controller = this.GetController (this.userFieldsCount+0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (this.userFieldsCount+0) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private Guid							PersonModel
		{
			get
			{
				if (this.UseModel)
				{
					var controller = this.GetController (this.userFieldsCount+1) as PersonGuidStackedController;
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
				var controller = this.GetController (this.userFieldsCount+1) as PersonGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
				this.UseModel = !value.IsEmpty;
			}
		}



		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetEnable (this.userFieldsCount+1, this.UseModel);

			this.okButton.Enable = this.GetRequiredProperties (BaseType.PersonsUserFields).Count () == this.userFieldsCount;
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<IEnumerable<AbstractDataProperty>, Guid> action)
		{
			//	Affiche le Popup.
			var popup = new CreatePersonPopup (accessor);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.GetRequiredProperties (BaseType.PersonsUserFields), popup.PersonModel);
				}
			};
		}
		#endregion


		private const int fieldWidth = 400;

		private readonly int userFieldsCount;
	}
}