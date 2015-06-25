//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'une
	/// nouvelle catégorie, à savoir le nom de la catégorie et son éventuel modèle.
	/// </summary>
	public class CreateCategoryPopup : AbstractStackedPopup
	{
		private CreateCategoryPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.CreateCategory.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.CreateCategory.Name.ToString (),
				Width                 = DateController.controllerWidth,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.CreateCategory.Model.ToString (),
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.CategoryGuid,
				Label                 = "",
				Width                 = DateController.controllerWidth,
				Height                = 180,
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

		private Guid							ObjectModel
		{
			get
			{
				if (this.UseModel)
				{
					var controller = this.GetController (2) as CategoryGuidStackedController;
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
				var controller = this.GetController (2) as CategoryGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
				this.UseModel = !value.IsEmpty;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetEnable (2, this.UseModel);

			this.okButton.Enable = !string.IsNullOrEmpty (this.ObjectName)
								&& (!this.UseModel || !this.ObjectModel.IsEmpty);
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, Guid objectModel, System.Action<string, Guid> action)
		{
			//	Affiche le Popup.
			var popup = new CreateCategoryPopup (accessor)
			{
				ObjectModel = objectModel,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.ObjectName, popup.ObjectModel);
				}
			};
		}
		#endregion
	}
}