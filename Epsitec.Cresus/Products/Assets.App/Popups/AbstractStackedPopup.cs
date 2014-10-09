//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.DataProperties;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Classe abstraite permettant de créer facilement n'importe quel Popup contenant un
	/// empilement de contrôleurs pour saisir divers types de valeurs (textes, dates,
	/// booléens, énumérations, etc.).
	/// </summary>
	public abstract class AbstractStackedPopup : AbstractPopup
	{
		public AbstractStackedPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.descriptions = new List<StackedControllerDescription> ();
			this.controllers = new List<AbstractStackedController> ();
			this.controllerVisibleFrames = new List<FrameBox> ();
			this.controllerHiddenFrames = new List<FrameBox> ();
			this.userFieldsControllersIndexes = new List<int> ();

			this.defaultAcceptButtonName = Res.Strings.Popup.Button.Ok.ToString ();
			this.defaultCancelButtonName = Res.Strings.Popup.Button.Cancel.ToString ();
			this.defaultControllerRankFocus = 0;
		}


		#region Required user fields logic
		protected void CreateRequiredUserFields(List<StackedControllerDescription> list, BaseType baseType)
		{
			//	Pour les Popup de création (Asset ou Person), on crée tous les contrôleurs
			//	permettant de remplir les champs obligatoires (UserField.Required).
			this.userFieldsControllersIndexes.Clear ();
			var userFields = this.GetRequiredUserFields (baseType);

			for (int i=0; i<userFields.Count; i++)
			{
				var userField = userFields[i];
				bool last = (i == userFields.Count-1);
				int bottomMargin = last ? 10 : 0;
				int width = System.Math.Min (userField.LineWidth.HasValue ? userField.LineWidth.GetValueOrDefault () : 1000, DateController.controllerWidth);

				switch (userField.Type)
				{
					case FieldType.String:
						this.userFieldsControllersIndexes.Add (list.Count);
						list.Add (new StackedControllerDescription
						{
							StackedControllerType = StackedControllerType.Text,
							Label                 = userField.Name,
							Width                 = width,
							BottomMargin          = bottomMargin,
						});
						break;

					case FieldType.Int:
						this.userFieldsControllersIndexes.Add (list.Count);
						list.Add (new StackedControllerDescription
						{
							StackedControllerType = StackedControllerType.Int,
							Label                 = userField.Name,
							BottomMargin          = bottomMargin,
						});
						break;

					case FieldType.Decimal:
						this.userFieldsControllersIndexes.Add (list.Count);
						list.Add (new StackedControllerDescription
						{
							StackedControllerType = StackedControllerType.Decimal,
							Label                 = userField.Name,
							DecimalFormat         = DecimalFormat.Real,
							BottomMargin          = bottomMargin,
						});
						break;

					case FieldType.ComputedAmount:
						this.userFieldsControllersIndexes.Add (list.Count);
						list.Add (new StackedControllerDescription
						{
							StackedControllerType = StackedControllerType.Decimal,
							Label                 = userField.Name,
							DecimalFormat         = DecimalFormat.Amount,
							BottomMargin          = bottomMargin,
						});
						break;

					case FieldType.Date:
						this.userFieldsControllersIndexes.Add (list.Count);
						list.Add (new StackedControllerDescription
						{
							StackedControllerType = StackedControllerType.Date,
							Label                 = userField.Name,
							DateRangeCategory     = DateRangeCategory.Mandat,
							BottomMargin          = bottomMargin,
						});
						break;

					case FieldType.GuidPerson:
						this.userFieldsControllersIndexes.Add (list.Count);
						list.Add (new StackedControllerDescription
						{
							StackedControllerType = StackedControllerType.PersonGuid,
							Label                 = userField.Name,
							Height                = 100,
							BottomMargin          = bottomMargin,
						});
						break;
				}
			}
		}

		public IEnumerable<AbstractDataProperty> GetRequiredProperties(BaseType baseType)
		{
			//	Pour les Popup de création (Asset ou Person), retourne les propriétés
			//	correspondant aux valeurs entrées dans champs obligatoires (UserField.Required).
			var userFields = this.GetRequiredUserFields (baseType);

			int i = 0;
			foreach (var userField in userFields)
			{
				switch (userField.Type)
				{
					case FieldType.String:
						{
							var controller = this.GetController (this.userFieldsControllersIndexes[i++]) as TextStackedController;
							if (!string.IsNullOrEmpty (controller.Value))
							{
								yield return new DataStringProperty (userField.Field, controller.Value);
							}
						}
						break;

					case FieldType.Int:
						{
							var controller = this.GetController (this.userFieldsControllersIndexes[i++]) as IntStackedController;
							if (controller.Value.HasValue)
							{
								yield return new DataIntProperty (userField.Field, controller.Value.Value);
							}
						}
						break;

					case FieldType.Decimal:
						{
							var controller = this.GetController (this.userFieldsControllersIndexes[i++]) as DecimalStackedController;
							if (controller.Value.HasValue)
							{
								yield return new DataDecimalProperty (userField.Field, controller.Value.Value);
							}
						}
						break;

					case FieldType.ComputedAmount:
						{
							var controller = this.GetController (this.userFieldsControllersIndexes[i++]) as DecimalStackedController;
							if (controller.Value.HasValue)
							{
								var ca = new ComputedAmount (controller.Value);
								yield return new DataComputedAmountProperty (userField.Field, ca);
							}
						}
						break;

					case FieldType.Date:
						{
							var controller = this.GetController (this.userFieldsControllersIndexes[i++]) as DateStackedController;
							if (controller.Value.HasValue)
							{
								yield return new DataDateProperty (userField.Field, controller.Value.Value);
							}
						}
						break;

					case FieldType.GuidPerson:
						{
							var controller = this.GetController (this.userFieldsControllersIndexes[i++]) as PersonGuidStackedController;
							if (!controller.Value.IsEmpty)
							{
								yield return new DataGuidProperty (userField.Field, controller.Value);
							}
						}
						break;
				}
			}
		}

		private List<UserField> GetRequiredUserFields(BaseType baseType)
		{
			return this.accessor.GlobalSettings.GetUserFields (baseType)
				.Where (x => x.Required)
				.ToList ();
		}
		#endregion


		protected bool							HasError
		{
			get
			{
				return this.controllers.Where (x => x.HasError).Any ();
			}
		}

		protected void SetDescriptions(IEnumerable<StackedControllerDescription> descriptions)
		{
			this.descriptions.Clear ();
			this.descriptions.AddRange (descriptions);

			this.CreateControllers ();
		}

		protected AbstractStackedController GetController(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.controllers.Count);
			System.Diagnostics.Debug.Assert (this.descriptions.Count == this.controllers.Count);
			return this.controllers[rank];
		}

		protected void SetVisibility(int rank, bool visibility)
		{
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.controllerVisibleFrames.Count);
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.controllerHiddenFrames.Count);
			System.Diagnostics.Debug.Assert (this.descriptions.Count == this.controllerVisibleFrames.Count);
			System.Diagnostics.Debug.Assert (this.descriptions.Count == this.controllerHiddenFrames.Count);
			this.controllerVisibleFrames[rank].Visibility = visibility;
			this.controllerHiddenFrames[rank].Visibility = !visibility;
		}

		protected void SetEnable(int rank, bool enable)
		{
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.controllers.Count);
			this.controllers[rank].Enable = enable;
		}


		protected override Size					DialogSize
		{
			//	Retourne les dimensions du popup, en prenant en compte l'ensemble des
			//	*StackedController contenus. La largeur est celle du contrôleur le plus
			//	large, et la hauteur est la somme des hauteurs des contrôleurs.
			get
			{
				int labelsWidth = this.LabelsWidth;

				if (labelsWidth > 0)
				{
					//	Si la partie droite pour les labels existe, on ajoute une petite
					//	marge de séparation entre les parties droite et gauche.
					labelsWidth += 10;
				}

				int w = AbstractStackedPopup.margin*2 + labelsWidth + this.ControllersWidth;
				int h = AbstractPopup.titleHeight + AbstractStackedPopup.margin*2 + this.ControllersHeight + AbstractStackedPopup.footerHeight;

				return new Size (w, h);
			}
		}


		protected override void CreateUI()
		{
			this.CreateTitle (this.title);
			this.CreateControllersUI (this.mainFrameBox);
			this.CreateButtons ();

			this.UpdateWidgets (null);

			var controller = this.GetController (this.defaultControllerRankFocus);
			controller.SetFocus ();  // met le focus dans le champ requis
		}

		private void CreateControllers()
		{
			//	Crée l'ensemble des *StackedController requis, sans créer les UI correspondantes.
			this.controllers.Clear ();

			foreach (var description in this.descriptions)
			{
				var controller = StackedControllerDescription.CreateController (this.accessor, description);
				this.controllers.Add (controller);
			}
		}

		private void CreateControllersUI(Widget parent)
		{
			//	Crée toutes les UI des contrôleurs.
			this.controllerVisibleFrames.Clear ();
			this.controllerHiddenFrames.Clear ();

			var globalFrame = new FrameBox
			{
				Parent  = parent,
				Anchor  = AnchorStyles.All,
				Margins = new Margins (AbstractStackedPopup.margin, AbstractStackedPopup.margin, AbstractPopup.titleHeight + AbstractStackedPopup.margin, 0),
			};

			int labelsWidth = this.LabelsWidth;
			int tabIndex = 0;

			System.Diagnostics.Debug.Assert (this.descriptions.Count == this.controllers.Count);
			for (int i=0; i<this.descriptions.Count; i++)
			{
				var description = this.descriptions[i];
				var controller  = this.controllers[i];

				var visibleFrame = new FrameBox
				{
					Parent          = globalFrame,
					PreferredHeight = controller.RequiredHeight,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, description.BottomMargin + AbstractStackedPopup.verticalGap),
				};

				var hiddenFrame = new FrameBox
				{
					Parent          = globalFrame,
					PreferredHeight = controller.RequiredHeight,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, description.BottomMargin + AbstractStackedPopup.verticalGap),
					Visibility      = false,
				};

				controller.CreateUI (visibleFrame, labelsWidth, ref tabIndex);

				controller.ValueChanged += delegate (object sender, StackedControllerDescription d)
				{
					this.OnValueChanged (d);
				};

				this.controllerVisibleFrames.Add (visibleFrame);
				this.controllerHiddenFrames.Add (hiddenFrame);
			}
		}


		private int ControllersHeight
		{
			//	Retourne la hauteur totale nécessaire pour les contrôleurs inclus.
			get
			{
				return this.controllers.Select (x => x.RequiredHeight + x.Description.BottomMargin + AbstractStackedPopup.verticalGap).Sum ();
			}
		}

		private int ControllersWidth
		{
			//	Retourne la largeur nécessaire pour les contrôleurs inclus.
			//	C'est le plus large qui fait sa loi.
			get
			{
				return this.controllers.Select (x => x.RequiredControllerWidth).Max ();
			}
		}

		private int LabelsWidth
		{
			//	Retourne la hauteur nécessaire pour les labels à gauche des contrôleurs inclus.
			//	C'est le plus large qui fait sa loi.
			get
			{
				return this.controllers.Select (x => x.RequiredLabelsWidth).Max ();
			}
		}


		protected override bool IsAcceptEnable
		{
			get
			{
				return this.okButton.Enable;
			}
		}


		private void CreateButtons()
		{
			//	Crée les boutons tout en bas du Popup.
			var footer = this.CreateFooter ();

			this.okButton     = this.CreateFooterAcceptButton (footer, "ok",     this.defaultAcceptButtonName);
			this.cancelButton = this.CreateFooterCancelButton (footer, "cancel", this.defaultCancelButtonName);
		}


		protected virtual void UpdateWidgets(StackedControllerDescription description)
		{
			//	Met à jour les contrôleurs, après le changement d'une valeur.
		}

		protected int GetRank(StackedControllerDescription description)
		{
			return this.descriptions.IndexOf (description);
		}


		#region Events handler
		protected void OnValueChanged(StackedControllerDescription description)
		{
			this.UpdateWidgets (description);
			this.ValueChanged.Raise (this, description);
		}

		public event EventHandler<StackedControllerDescription> ValueChanged;
		#endregion


		private const int margin       = 20;
		private const int verticalGap  = 4;
		private const int footerHeight = 30;

		protected readonly DataAccessor						accessor;
		private readonly List<StackedControllerDescription>	descriptions;
		private readonly List<AbstractStackedController>	controllers;
		private readonly List<FrameBox>						controllerVisibleFrames;
		private readonly List<FrameBox>						controllerHiddenFrames;
		private readonly List<int>							userFieldsControllersIndexes;

		protected string									defaultAcceptButtonName;
		protected string									defaultCancelButtonName;
		protected int										defaultControllerRankFocus;
		protected string									title;
		protected ColoredButton								okButton;
		protected ColoredButton								cancelButton;
	}
}