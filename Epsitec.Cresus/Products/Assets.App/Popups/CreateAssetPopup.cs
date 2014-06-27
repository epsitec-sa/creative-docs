//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouvel objet, à savoir la date d'entrée et le nom de l'objet.
	/// </summary>
	public class CreateAssetPopup : StackedPopup
	{
		public CreateAssetPopup(DataAccessor accessor)
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
				Width                 = DateController.controllerWidth - 3,
			});

			this.SetDescriptions (list);
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

		public string							ObjectName
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


		public override void CreateUI()
		{
			base.CreateUI ();

			var controller = this.GetController (1);
			controller.SetFocus ();
		}

		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.okButton.Text = "Créer";

			this.okButton.Enable = this.ObjectDate.HasValue
								&& !string.IsNullOrEmpty (this.ObjectName)
								&& !this.HasError;
		}

	
		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<System.DateTime, string> action)
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

						action (popup.ObjectDate.Value, popup.ObjectName);
					}
				};
			}
		}
		#endregion
	}
}