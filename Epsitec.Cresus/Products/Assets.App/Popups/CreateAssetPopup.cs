﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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

			foreach (var group in this.accessor.Mandat.GetData (BaseType.Groups))
			{
				bool suggested = ObjectProperties.GetObjectPropertyInt (group, null, ObjectField.GroupSuggestedDuringCreation) == 1;

				if (suggested)
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
						Label                 = this.GetGroupName (group.Guid),
						MultiLabels           = this.GetMultiLabels (group.Guid),
						Width                 = DateController.controllerWidth,
					});

					this.groupsDict.Add (i++, group.Guid);
				}
			}
		}

		private string GetMultiLabels(Guid groupGuid)
		{
			//	Retourne le meta-label permettant de peupler le menu du combo du choix
			//	d'un groupe. Il est composé des noms courts des groupes, séparés par
			//	des fins de lignes.
			var list = new List<string> ();

			foreach (var guid in this.GetChildrensGuid (groupGuid))
			{
				var name = this.GetGroupName (guid);
				list.Add (name);
			}

			return string.Join ("<br/>", list);
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
								&& this.GetRequiredProperties (BaseType.AssetsUserFields).Count () == this.userFieldsCount
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
						var guids = this.GetChildrensGuid (guid).ToArray ();

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

		private IEnumerable<Guid> GetChildrensGuid(Guid groupGuid)
		{
			//	Retourne la liste des groupes fils pour peupler un combo, triée par
			//	ordre alphabétique.
			return this.accessor.Mandat.GetData (BaseType.Groups)
				.Where (x => this.IsChildren (groupGuid, x.Guid) && this.IsFinal (x.Guid))
				.OrderBy (x => this.GetGroupName (x.Guid))
				.Select (x => x.Guid);
		}

		private string GetGroupName(Guid guid)
		{
			//	Retourne le nom court d'un groupe.
			return GroupsLogic.GetShortName (this.accessor, guid);
		}

		private bool IsChildren(Guid parentGuid, Guid guid)
		{
			//	Indique si un groupe est un descendant d'un parent (fils, petit-fils, etc.).
			if (guid == parentGuid)
			{
				return false;
			}

			while (true)
			{
				var group = this.accessor.GetObject (BaseType.Groups, guid);
				guid = ObjectProperties.GetObjectPropertyGuid (group, null, ObjectField.GroupParent);

				if (guid.IsEmpty)
				{
					return false;
				}

				if (guid == parentGuid)
				{
					return true;
				}
			}
		}

		private bool IsFinal(Guid guid)
		{
			//	Indique si un groupe est terminal (donc s'il n'y pas de descendants).
			//	Seuls ces groupes peuvent être choisis dans les combos.
			foreach (var group in this.accessor.Mandat.GetData (BaseType.Groups))
			{
				var parentGuid = ObjectProperties.GetObjectPropertyGuid (group, null, ObjectField.GroupParent);
				if (parentGuid == guid)
				{
					return false;
				}
			}

			return true;
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