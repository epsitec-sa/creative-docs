//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Popup permettant la saisir des informations nécessaires à la création d'un
	/// nouveau groupe, à savoir le nom du groupe et son parent.
	/// </summary>
	public class CreateGroupPopup : AbstractStackedPopup
	{
		private CreateGroupPopup(DataAccessor accessor)
			: base (accessor)
		{
			this.title = Res.Strings.Popup.CreateGroup.Title.ToString ();

			var list = new List<StackedControllerDescription> ();

			list.Add (new StackedControllerDescription  // 0
			{
				StackedControllerType = StackedControllerType.GroupGuid,
				Label                 = Res.Strings.Popup.CreateGroup.Parent.ToString (),
				Width                 = GroupGuidStackedController.ControllerWidth,
				Height                = 260,
			});

			list.Add (new StackedControllerDescription  // 1
			{
				StackedControllerType = StackedControllerType.Label,
				Width                 = GroupGuidStackedController.ControllerWidth,
				Height                = 15*1,  // place pour 1 ligne
			});

			list.Add (new StackedControllerDescription  // 2
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.CreateGroup.Number.ToString (),
				Width                 = 100,
				BottomMargin          = 10,
			});

			list.Add (new StackedControllerDescription  // 3
			{
				StackedControllerType = StackedControllerType.Text,
				Label                 = Res.Strings.Popup.CreateGroup.Name.ToString (),
				Width                 = GroupGuidStackedController.ControllerWidth,
				BottomMargin          = 10,
			});

			this.SetDescriptions (list);

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Create.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		private IEnumerable<AbstractDataProperty> Properties
		{
			get
			{
				yield return new DataStringProperty (ObjectField.Name,   this.ObjectName);
				yield return new DataStringProperty (ObjectField.Number, this.ObjectNumber);
			}
		}

		private Guid							ObjectParent
		{
			get
			{
				var controller = this.GetController (0) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (0) as GroupGuidStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private string							ObjectNumber
		{
			get
			{
				var controller = this.GetController (2) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (2) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}

		private string							ObjectName
		{
			get
			{
				var controller = this.GetController (3) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				return controller.Value;
			}
			set
			{
				var controller = this.GetController (3) as TextStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.Value = value;
			}
		}


		protected override void UpdateWidgets(StackedControllerDescription description)
		{
			{
				var controller = this.GetController (1) as LabelStackedController;
				System.Diagnostics.Debug.Assert (controller != null);
				controller.SetLabel (this.CurrentNumber);
			}

			this.okButton.Enable = !string.IsNullOrEmpty (this.ObjectNumber) &&
								   !string.IsNullOrEmpty (this.ObjectName) &&
								   !this.ObjectParent.IsEmpty;
		}

		private string CurrentNumber
		{
			get
			{
				string root = GroupsLogic.GetFullNumber (this.accessor, this.ObjectParent);

				if (string.IsNullOrEmpty (root))
				{
					return null;
				}
				else
				{
					root = root.Color (Color.FromBrightness (0.6));
					string edit = this.ObjectNumber;

					if (string.IsNullOrEmpty (edit))
					{
						return root;
					}
					else
					{
						return string.Concat (root, ".", edit);
					}
				}
			}
		}


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, Guid objectParent, System.Action<Guid, IEnumerable<AbstractDataProperty>> action)
		{
			//	Affiche le Popup.
			var popup = new CreateGroupPopup (accessor)
			{
				ObjectParent = objectParent,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					action (popup.ObjectParent, popup.Properties);
				}
			};
		}
		#endregion
	}
}