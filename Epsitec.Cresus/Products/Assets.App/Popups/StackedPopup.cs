//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups.StackedControllers;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Classe abstraite permettant de créer facilement n'importe quel Popup contenant un
	/// empilement de contrôleurs pour saisir divers types de valeurs (textes, dates,
	/// booléens, énumérations, etc.).
	/// </summary>
	public abstract class StackedPopup : AbstractPopup
	{
		public StackedPopup(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.descriptions = new List<StackedControllerDescription> ();
			this.controllers = new List<AbstractStackedController> ();
			this.controllerVisibleFrames = new List<FrameBox> ();
			this.controllerHiddenFrames = new List<FrameBox> ();
		}


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

				int w = StackedPopup.margin*2 + labelsWidth + this.ControllersWidth;
				int h = AbstractPopup.titleHeight + StackedPopup.margin*2 + this.ControllersHeight + StackedPopup.footerHeight;

				return new Size (w, h);
			}
		}


		public override void CreateUI()
		{
			this.CreateTitle (this.title);
			this.CreateControllersUI (this.mainFrameBox);
			this.CreateButtons ();

			this.UpdateWidgets (null);
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
				Margins = new Margins (StackedPopup.margin, StackedPopup.margin, AbstractPopup.titleHeight + StackedPopup.margin, 0),
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
					Margins         = new Margins (0, 0, 0, description.BottomMargin + StackedPopup.verticalGap),
				};

				var hiddenFrame = new FrameBox
				{
					Parent          = globalFrame,
					PreferredHeight = controller.RequiredHeight,
					Dock            = DockStyle.Top,
					Margins         = new Margins (0, 0, 0, description.BottomMargin + StackedPopup.verticalGap),
					Visibility      = false,
				};

				controller.CreateUI (visibleFrame, labelsWidth, ++tabIndex, description);

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
				return this.controllers.Select (x => x.RequiredHeight + x.Description.BottomMargin + StackedPopup.verticalGap).Sum ();
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


		private void CreateButtons()
		{
			//	Crée les boutons tout en bas du Popup.
			var footer = this.CreateFooter ();

			this.okButton     = this.CreateFooterButton (footer, DockStyle.Left,  "ok",     "D'accord");
			this.cancelButton = this.CreateFooterButton (footer, DockStyle.Right, "cancel", "Annuler");
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

		private readonly DataAccessor						accessor;
		private readonly List<StackedControllerDescription>	descriptions;
		private readonly List<AbstractStackedController>	controllers;
		private readonly List<FrameBox>						controllerVisibleFrames;
		private readonly List<FrameBox>						controllerHiddenFrames;

		protected string									title;
		protected Button									okButton;
		protected Button									cancelButton;
	}
}