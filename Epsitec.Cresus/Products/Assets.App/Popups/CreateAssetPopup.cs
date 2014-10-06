//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
			this.title = Res.Strings.Popup.CreateAsset.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.Date,
				DateRangeCategory     = DateRangeCategory.Mandat,
				Label                 = Res.Strings.Popup.CreateAsset.Date.ToString (),
			});

			this.CreateRequiredUserFields (list, BaseType.AssetsUserFields);
			this.userFieldsCount = list.Count - 1;

			list.Add (new StackedControllerDescription  // userFieldsCount+1
			{
				StackedControllerType = StackedControllerType.Decimal,
				DecimalFormat         = DecimalFormat.Amount,
				Label                 = Res.Strings.Popup.CreateAsset.Value.ToString (),
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // userFieldsCount+2
			{
				StackedControllerType = StackedControllerType.Bool,
				Label                 = Res.Strings.Popup.CreateAsset.Category.ToString (),
			});

			list.Add (new StackedControllerDescription  // userFieldsCount+3
			{
				StackedControllerType = StackedControllerType.CategoryGuid,
				Label                 = "",
				Width                 = DateController.controllerWidth - 4,
				Height                = 180,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 1;
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

		private decimal?						MainValue
		{
			get
			{
				var controller = this.GetController (this.userFieldsCount+1) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (this.userFieldsCount+1) as DecimalStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private bool							UseCategory
		{
			get
			{
				var controller = this.GetController (this.userFieldsCount+2) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (this.userFieldsCount+2) as BoolStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		public Guid								ObjectCategory
		{
			get
			{
				if (this.UseCategory)
				{
					var controller = this.GetController (this.userFieldsCount+3) as CategoryGuidStackedController;
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
				var controller = this.GetController (this.userFieldsCount+3) as CategoryGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			this.SetEnable (this.userFieldsCount+3, this.UseCategory);

			this.okButton.Enable = this.ObjectDate.HasValue
								&& this.GetequiredProperties (BaseType.AssetsUserFields).Count () == this.userFieldsCount
								&& (!this.UseCategory || !this.ObjectCategory.IsEmpty)
								&& !this.HasError;
		}

	
		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, System.Action<System.DateTime, IEnumerable<AbstractDataProperty>, decimal?, Guid> action)
		{
			if (target != null)
			{
				var popup = new CreateAssetPopup (accessor)
				{
					ObjectDate     = LocalSettings.CreateAssetDate,
					UseCategory    = true,
					ObjectCategory = Guid.Empty,
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

						action (popup.ObjectDate.Value, popup.GetequiredProperties (BaseType.AssetsUserFields), popup.MainValue, popup.ObjectCategory);
					}
				};
			}
		}
		#endregion


		private readonly int userFieldsCount;
	}
}