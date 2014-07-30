//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouvel objet, à savoir la date d'entrée et le nom de l'objet.
	/// </summary>
	public class CreateAssetPopup : StackedPopup
	{
		private CreateAssetPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Création d'un nouvel objet";

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = "Date d'entrée",
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = "Nom",
				Width                 = DateController.controllerWidth - 4,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Label,
				Label                 = "Catégorie d'immobilisation :",
				Width                 = DateController.controllerWidth - 4,
				Height                = 15*1,  // place pour 1 ligne
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.CategoryGuid,
				Label                 = "",
				Width                 = DateController.controllerWidth - 4,
				Height                = 180,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = "Créer";
			this.defaultControllerRankFocus = 1;

			this.ObjectCategory = Guid.Empty;
		}


		public System.DateTime?					ObjectDate
		{
			get
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as DateStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private string							ObjectName
		{
			get
			{
				var controller = this.GetController (1) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (1) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public Guid								ObjectCategory
		{
			get
			{
				var controller = this.GetController (3) as CategoryGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (3) as CategoryGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Enable = this.ObjectDate.HasValue
								&& !string.IsNullOrEmpty (this.ObjectName)
								&& !this.ObjectCategory.IsEmpty
								&& !this.HasError;
		}

	
		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<System.DateTime, string, Guid> action)
		{
			if (target != null)
			{
				var popup = new CreateAssetPopup (accessor)
				{
					ObjectDate = LocalSettings.CreateAssetDate,
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						if (popup.ObjectDate.HasValue)
						{
							LocalSettings.CreateAssetDate = popup.ObjectDate.Value;
						}

						action (popup.ObjectDate.Value, popup.ObjectName, popup.ObjectCategory);
					}
				};
			}
		}
		#endregion
	}
}