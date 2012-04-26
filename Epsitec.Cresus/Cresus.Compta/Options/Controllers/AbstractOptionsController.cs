//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Options.Data;
using Epsitec.Cresus.Compta.Graph;
using Epsitec.Cresus.Compta.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Options.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptionsController
	{
		public AbstractOptionsController(AbstractController controller)
		{
			this.controller = controller;

			this.compta  = this.controller.ComptaEntity;
			this.période = this.controller.PériodeEntity;
			this.options = this.controller.DataAccessor.Options;

			this.ignoreChanges = new SafeCounter ();
		}


		public bool ShowPanel
		{
			get
			{
				return this.showPanel;
			}
			set
			{
				this.showPanel = value;
				this.toolbar.Visibility = this.showPanel;

				if (this.showPanel)
				{
					this.UpdateContent ();
				}
			}
		}

		public bool Specialist
		{
			get
			{
				return this.levelController.Specialist;
			}
			set
			{
				if (this.levelController.Specialist != value)
				{
					this.levelController.Specialist = value;
					this.LevelChangedAction ();
				}
			}
		}


		public virtual void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.optionsChanged = optionsChanged;

			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = UIBuilder.OptionsBackColor,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, 5),
			};

			//	Crée les frames gauche, centrale et droite.
			this.mainFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (5),
			};

			var levelFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			//	Remplissage de la frame gauche.
			//	Remplissage de la frame centrale.
			this.levelController = new LevelController (this.controller);
			this.levelController.CreateUI (levelFrame, "Remet les options standards", this.ClearAction, this.LevelChangedAction);
			this.levelController.Specialist = this.options.Specialist;
		}

		protected virtual void OptionsChanged()
		{
			this.optionsChanged ();
		}

		public void ClearAction()
		{
			this.options.Clear ();
			this.OptionsChanged ();
		}

		protected virtual void LevelChangedAction()
		{
			this.options.Specialist = this.levelController.Specialist;

			if (this.comparisonFrame != null)
			{
				this.comparisonFrame.Visibility = this.levelController.Specialist;
			}
		}

		protected virtual void UpdateWidgets()
		{
			this.levelController.ClearEnable = !this.options.IsEmpty;
		}


		public virtual void UpdateContent()
		{
		}


		#region Comparaison
		protected FrameBox CreateComparisonUI(FrameBox parent, ComparisonShowed possibleMode)
		{
			this.comparisonFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 5, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.buttonComparisonEnable = new CheckButton
			{
				Parent          = this.comparisonFrame,
				PreferredWidth  = 120,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			{
				this.frameComparisonShowed = UIBuilder.CreatePseudoCombo (this.comparisonFrame, out this.fieldComparisonShowed, out this.buttonComparisonShowed);
				this.frameComparisonShowed.PreferredWidth = 150;
				this.frameComparisonShowed.Margins = new Margins (0, 20, 0, 0);
			}

			this.labelComparisonDisplayMode = new StaticText
			{
				Parent         = this.comparisonFrame,
				FormattedText  = "Affichage",
				PreferredWidth = 55,
				Dock           = DockStyle.Left,
			};

			this.fieldComparisonDisplayMode = new TextFieldCombo
			{
				Parent          = this.comparisonFrame,
				IsReadOnly      = true,
				PreferredWidth  = 150,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.ComparisonDisplayModeInitialize (this.fieldComparisonDisplayMode);
			this.UpdateComparison ();

			//	Gestion des événements.
			this.buttonComparisonEnable.ActiveStateChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.options.ComparisonEnable = (this.buttonComparisonEnable.ActiveState == ActiveState.Yes);
					this.OptionsChanged ();
				}
			};

			this.fieldComparisonShowed.Clicked += delegate
			{
				this.ShowComparisonShowedMenu (this.frameComparisonShowed, possibleMode);
			};

			this.buttonComparisonShowed.Clicked += delegate
			{
				this.ShowComparisonShowedMenu (this.frameComparisonShowed, possibleMode);
			};

			this.fieldComparisonDisplayMode.TextChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.options.ComparisonDisplayMode = this.GetComparisonDisplayMode (this.fieldComparisonDisplayMode);
					this.OptionsChanged ();
				}
			};

			return this.comparisonFrame;
		}

		protected void UpdateComparison()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.comparisonFrame.Visibility = this.levelController.Specialist;

				bool enable = this.options.ComparisonEnable;

				this.buttonComparisonEnable.ActiveState = this.options.ComparisonEnable ? ActiveState.Yes : ActiveState.No;
				this.buttonComparisonEnable.Text = enable ? "Comparaison avec" : "Comparaison";

				this.frameComparisonShowed.Visibility = enable;
				this.fieldComparisonShowed.Visibility = enable;
				this.buttonComparisonShowed.Visibility = enable;
				this.fieldComparisonDisplayMode.Visibility = enable;
				this.labelComparisonDisplayMode.Visibility = enable;

				this.fieldComparisonShowed.Text = Converters.GetComparisonShowedListDescription (this.options.ComparisonShowed);
				this.fieldComparisonDisplayMode.Text = Converters.GetComparisonDisplayModeDescription (this.options.ComparisonDisplayMode);
			}
		}

		private void ShowComparisonShowedMenu(Widget parentButton, ComparisonShowed possibleMode)
		{
			//	Affiche le menu permettant de choisir le mode.
			var menu = new VMenu ();

			foreach (var mode in Converters.ComparisonsShowed)
			{
				if ((mode & possibleMode) != 0)
				{
					this.AddComparisonShowedMenu (menu, mode, (this.options.ComparisonShowed & mode) != 0);
				}
			}

			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddComparisonShowedMenu(VMenu menu, ComparisonShowed mode, bool active)
		{
			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetCheckStateIconUri (active),
				FormattedText = Converters.GetComparisonShowedDescription (mode),
				Name          = mode.ToString (),
			};

			item.Clicked += delegate
			{
				var m = (ComparisonShowed) System.Enum.Parse (typeof (ComparisonShowed), item.Name);
				this.options.ComparisonShowed ^= m;
				this.OptionsChanged ();
			};

			menu.Items.Add (item);
		}


		private void ComparisonDisplayModeInitialize(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			combo.Items.Add (Converters.GetComparisonDisplayModeDescription (ComparisonDisplayMode.Montant));
			combo.Items.Add (Converters.GetComparisonDisplayModeDescription (ComparisonDisplayMode.Différence));
			combo.Items.Add (Converters.GetComparisonDisplayModeDescription (ComparisonDisplayMode.Pourcentage));
			combo.Items.Add (Converters.GetComparisonDisplayModeDescription (ComparisonDisplayMode.PourcentageMontant));
		}

		private ComparisonDisplayMode GetComparisonDisplayMode(TextFieldCombo combo)
		{
			foreach (var value in System.Enum.GetValues (typeof (ComparisonDisplayMode)))
			{
				var mode = (ComparisonDisplayMode) value;

				if (combo.Text == Converters.GetComparisonDisplayModeDescription (mode))
				{
					return mode;
				}
			}

			return ComparisonDisplayMode.Montant;
		}
		#endregion


		#region Graph
		protected FrameBox CreateGraphUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent         = parent,
				PreferredWidth = 40,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 20, 0, 0),
			};

			this.viewArrayButton = this.CreateButton (frame, "View.Array", "Montre le tableau");
			this.viewGraphButton = this.CreateButton (frame, "View.Graph", "Montre le graphique");

			this.viewArrayButton.Clicked += delegate
			{
				this.options.ViewGraph = false;
				this.ShowHideControllers ();
				this.UpdateGraph ();
			};

			this.viewGraphButton.Clicked += delegate
			{
				this.options.ViewGraph = true;
				this.ShowHideControllers ();
				this.UpdateGraph ();
			};

			return frame;
		}

		private void ShowHideControllers()
		{
			if (this.options.ViewGraph)
			{
				this.controller.DataAccessor.UpdateGraphData (force: true);
			}

			this.controller.ArrayController.Show = !this.options.ViewGraph;
			this.controller.GraphController.Show =  this.options.ViewGraph;
		}

		protected void UpdateGraph()
		{
			if (this.options.ViewGraph && this.controller.GraphController != null)
			{
				this.controller.GraphController.UpdateController ();
			}

			using (this.ignoreChanges.Enter ())
			{
				this.viewArrayButton.ActiveState = this.options.ViewGraph ? ActiveState.No  : ActiveState.Yes;
				this.viewGraphButton.ActiveState = this.options.ViewGraph ? ActiveState.Yes : ActiveState.No;
			}
		}
		#endregion


		private BackIconButton CreateButton(FrameBox parent, string icon, string description)
		{
			var button = new BackIconButton
			{
				Parent            = parent,
				IconUri           = UIBuilder.GetResourceIconUri (icon),
				PreferredIconSize = new Size (20, 20),
				PreferredSize     = new Size (20, 20),
				Dock              = DockStyle.Left,
				AutoToggle        = false,
				AutoFocus         = false,
				Margins           = new Margins (0, 2, 0, 0),
			};

			ToolTip.Default.SetToolTip (button, description);

			return button;
		}


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							compta;
		protected readonly ComptaPériodeEntity					période;
		protected readonly AbstractOptions						options;
		protected readonly SafeCounter							ignoreChanges;

		protected System.Action									optionsChanged;

		protected int											tabIndex;
		protected FrameBox										toolbar;
		protected FrameBox										mainFrame;
		protected FrameBox										comparisonFrame;

		protected CheckButton									buttonComparisonEnable;
		protected FrameBox										frameComparisonShowed;
		protected StaticText									fieldComparisonShowed;
		protected GlyphButton									buttonComparisonShowed;
		protected StaticText									labelComparisonDisplayMode;
		protected TextFieldCombo								fieldComparisonDisplayMode;

		protected BackIconButton								viewArrayButton;
		protected BackIconButton								viewGraphButton;

		protected LevelController								levelController;

		protected bool											showPanel;
	}
}
