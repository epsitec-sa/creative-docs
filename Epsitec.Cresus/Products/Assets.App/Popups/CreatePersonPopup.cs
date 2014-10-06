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
	public class CreatePersonPopup : StackedPopup
	{
		public CreatePersonPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.CreatePerson.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			this.CreateRequiredUserFields (list, BaseType.PersonsUserFields);
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
				Width                 = 200 + (int) AbstractScroller.DefaultBreadth,
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

		public Guid								PersonModel
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

			this.okButton.Enable = this.GetequiredProperties (BaseType.PersonsUserFields).Count () == this.userFieldsCount;
		}


		private readonly int userFieldsCount;
	}
}