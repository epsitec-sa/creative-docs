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
	public class CreateAssetPopup : AbstractStackedPopup
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
				Width                 = DateController.controllerWidth,
				Height                = 180,
			});

			this.groupsDict = new Dictionary<int, Guid> ();
			this.CreateSuggestedGroups (list);

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 1;
		}


		private void CreateSuggestedGroups(List<StackedControllerDescription> list)
		{
			//	Ajoute des contrôleurs de type combo pour choisir les groupes définis
			//	comme GroupSuggestedDuringCreation dans les UserFields.
			this.groupsDict.Clear ();
			bool first = true;
			int i = this.userFieldsCount+5;

			foreach (var group in GroupsLogic.GetSuggestedGroups (this.accessor))
			{
				if (first)
				{
					//	S'il s'agit du premier combo, on le précède d'une ligne de titre.
					list.Add (new StackedControllerDescription
					{
						StackedControllerType = StackedControllerType.Label,
						Width                 = DateController.controllerWidth,
						Label                 = "<br/>" + Res.Strings.Popup.CreateAsset.Groups.ToString (),
					});

					first = false;
				}

				list.Add (new StackedControllerDescription
				{
					StackedControllerType = StackedControllerType.Combo,
					Label                 = GroupsLogic.GetShortName (this.accessor, group.Guid),
					MultiLabels           = this.GetMultiLabels (group.Guid),
					Width                 = DateController.controllerWidth,
				});

				this.groupsDict.Add (i++, group.Guid);
			}
		}

		private string GetMultiLabels(Guid groupGuid)
		{
			//	Retourne le meta-label permettant de peupler le menu du combo du choix
			//	d'un groupe. Il est composé des noms courts des groupes, séparés par
			//	des fins de lignes.
			var list = new List<string> ();

			foreach (var childrenGuid in GroupsLogic.GetChildrensGuids (this.accessor, groupGuid))
			{
				var name = GroupsLogic.GetShortName (this.accessor, childrenGuid);
				list.Add (name);
			}

			return string.Join ("<br/>", list);
		}

		private void InitializeDefaultGroups()
		{
			//	Initialise les combos selon les derniers LocalSettings sauvegardés.
			//	combos <- LocalSettings
			foreach (var pair in this.groupsDict)
			{
				var rank = pair.Key;
				var guid = pair.Value;

				var selectedGuid = LocalSettings.GetCreateGroup (guid);
				if (!selectedGuid.IsEmpty)
				{
					var controller = this.GetController (rank) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);

					var childrenGuids = GroupsLogic.GetChildrensGuids (this.accessor, guid).ToList ();

					int index = childrenGuids.IndexOf (selectedGuid);
					if (index != -1)
					{
						controller.Value = index;
					}
				}
			}
		}

		private void MemorizeDefaultGroups()
		{
			//	Sauvegarde les choix effectués dans les combos dans les LocalSettings.
			//	combos -> LocalSettings
			foreach (var pair in this.groupsDict)
			{
				var rank = pair.Key;
				var guid = pair.Value;

				var controller = this.GetController (rank) as ComboStackedController;
				System.Diagnostics.Debug.Assert (controller != null);

				if (controller.Value.HasValue)
				{
					var guids = GroupsLogic.GetChildrensGuids (this.accessor, guid).ToArray ();

					int sel = controller.Value.Value;
					if (sel >= 0 && sel < guids.Length)
					{
						LocalSettings.AddCreateGroup (guid, guids[sel]);
					}
				}
			}
		}


		private System.DateTime?				ObjectDate
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

		private Guid							ObjectCategory
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
								&& this.RequiredProperties.Count == this.userFieldsCount + this.groupsDict.Count
								&& (!this.UseCategory || !this.ObjectCategory.IsEmpty)
								&& !this.HasError;
		}


		private List<AbstractDataProperty> RequiredProperties
		{
			//	Retourne la liste des propriétés pour le nouvel objet d'immobilisation qui
			//	sera créé. Il y a d'abord les champs UserFields définis comme requis (tel
			//	que le nom de l'objet). Viennent ensuite les groupes choisis dans les combos.
			get
			{
				var properties = new List<AbstractDataProperty> ();

				//	Champs UserFields définis comme requis.
				properties.AddRange (this.GetRequiredProperties (BaseType.AssetsUserFields));

				//	Groupes choisis dans les combos.
				int i = 0;
				foreach (var pair in this.groupsDict)
				{
					var rank = pair.Key;
					var guid = pair.Value;

					var controller = this.GetController (rank) as ComboStackedController;
					System.Diagnostics.Debug.Assert (controller != null);

					if (controller.Value.HasValue)
					{
						var guids = GroupsLogic.GetChildrensGuids (this.accessor, guid).ToArray ();

						int sel = controller.Value.Value;
						if (sel >= 0 && sel < guids.Length)
						{
							var gr = new GuidRatio (guids[sel], null);
							var property = new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+(i++), gr);
							properties.Add (property);
						}
					}
				}

				return properties;
			}
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
				popup.InitializeDefaultGroups ();

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "ok")
					{
						if (popup.ObjectDate.HasValue)
						{
							LocalSettings.CreateAssetDate = popup.ObjectDate.Value;
						}

						popup.MemorizeDefaultGroups ();
						action (popup.ObjectDate.Value, popup.RequiredProperties, popup.MainValue, popup.ObjectCategory);
					}
				};
			}
		}
		#endregion


		private readonly int					userFieldsCount;
		private readonly Dictionary<int, Guid>	groupsDict;
	}
}