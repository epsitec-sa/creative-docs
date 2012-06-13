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
			this.specialistFrames = new List<FrameBox> ();
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
				this.container.Visibility = this.showPanel;

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
				return this.topPanelLeftController.Specialist;
			}
			set
			{
				if (this.topPanelLeftController.Specialist != value)
				{
					this.topPanelLeftController.Specialist = value;
					this.LevelChangedAction ();
				}
			}
		}

		public void LinkHilitePanel(bool hilite)
		{
			//?this.container.BackColor = hilite ? UIBuilder.LinkHiliteBackColor : UIBuilder.OptionsBackColor;
			//?this.toolbar.BackColor   = hilite ? UIBuilder.LinkHiliteBackColor : UIBuilder.OptionsBackColor;
			//?this.graphbar.BackColor  = hilite ? UIBuilder.LinkHiliteBackColor : UIBuilder.OptionsBackColor;
		}


		public virtual void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.optionsChanged = optionsChanged;

			this.container = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = UIBuilder.OptionsBackColor,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, 0, -1),
			};

			this.toolbar = new FrameBox
			{
				Parent              = this.container,
				DrawFullFrame       = true,
				BackColor           = UIBuilder.OptionsBackColor,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
			};

			this.graphbar = new FrameBox
			{
				Parent              = this.container,
				DrawFullFrame       = true,
				BackColor           = UIBuilder.OptionsBackColor,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock                = DockStyle.Top,
				Margins             = new Margins (0, 0, -1, 0),
			};

			this.container.Entered += delegate
			{
				this.controller.LinkHiliteOptionsButton (true);
			};

			this.container.Exited += delegate
			{
				this.controller.LinkHiliteOptionsButton (false);
			};

			this.toolbar.Entered += delegate
			{
				this.controller.LinkHiliteOptionsButton (true);
			};

			this.toolbar.Exited += delegate
			{
				this.controller.LinkHiliteOptionsButton (false);
			};

			this.graphbar.Entered += delegate
			{
				this.controller.LinkHiliteOptionsButton (true);
			};

			this.graphbar.Exited += delegate
			{
				this.controller.LinkHiliteOptionsButton (false);
			};

			//	Crée les frames gauche, centrale et droite.
			var topPanelLeftFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Left,
				Padding        = new Margins (5),
			};

			this.mainFrame = new FrameBox
			{
				Parent         = this.toolbar,
				Dock           = DockStyle.Fill,
				Padding        = new Margins (5),
			};

			var topPanelRightFrame = new FrameBox
			{
				Parent         = this.toolbar,
				DrawFullFrame  = true,
				PreferredWidth = 20,
				Dock           = DockStyle.Right,
				Padding        = new Margins (5),
			};

			//	Remplissage de la frame gauche.
			this.topPanelLeftController = new TopPanelLeftController (this.controller);
			this.topPanelLeftController.CreateUI (topPanelLeftFrame, this.HasBeginnerSpecialist, "Panel.Options", this.LevelChangedAction);
			this.topPanelLeftController.Specialist = this.options.Specialist;

			//	Remplissage de la frame droite.
			this.topPanelRightController = new TopPanelRightController (this.controller);
			this.topPanelRightController.CreateUI (topPanelRightFrame, "Remet les options standards", this.ClearAction, this.controller.MainWindowController.ClosePanelOptions, this.LevelChangedAction);

			this.graphOptionsController = new GraphOptionsController (this.controller);
			this.graphOptionsController.CreateUI (this.graphbar, this.optionsChanged);

			this.UpdateGraphWidgets ();
		}

		protected virtual bool HasBeginnerSpecialist
		{
			get
			{
				return false;
			}
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
			this.options.Specialist = this.topPanelLeftController.Specialist;

			foreach (var line in this.specialistFrames)
			{
				line.Visibility = this.topPanelLeftController.Specialist;
			}
		}

		protected virtual void UpdateWidgets()
		{
			this.topPanelRightController.ClearEnable = !this.options.IsEmpty;

			foreach (var line in this.specialistFrames)
			{
				line.Visibility = this.topPanelLeftController.Specialist;
			}
		}


		public virtual void UpdateContent()
		{
			this.ShowArrayOrGraph ();
		}


		#region ZeroFiltered
		protected void CreateZeroFilteredUI(FrameBox parent)
		{
			this.zeroFilteredButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Cacher les comptes dont le solde est nul",
				AutoToggle     = false,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
				TabIndex       = ++this.tabIndex,
			};

			UIBuilder.AdjustWidth (this.zeroFilteredButton);

			this.zeroFilteredButton.Clicked += delegate
			{
				this.options.ZeroFiltered = !this.options.ZeroFiltered;
				this.UpdateZeroFiltered ();
				this.OptionsChanged ();
			};

			this.UpdateZeroFiltered ();
		}

		protected void UpdateZeroFiltered()
		{
			this.zeroFilteredButton.ActiveState = this.options.ZeroFiltered ? ActiveState.Yes : ActiveState.No;
		}
		#endregion


		#region ZeroDisplayedInWhite
		protected void CreateZeroDisplayedInWhiteUI(FrameBox parent)
		{
			this.zeroDisplayedInWhiteButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Afficher en blanc les montants nuls",
				AutoToggle     = false,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
				TabIndex       = ++this.tabIndex,
			};

			UIBuilder.AdjustWidth (this.zeroDisplayedInWhiteButton);

			this.zeroDisplayedInWhiteButton.Clicked += delegate
			{
				this.options.ZeroDisplayedInWhite = !this.options.ZeroDisplayedInWhite;
				this.UpdateZeroDisplayedInWhite ();
				this.OptionsChanged ();
			};

			this.UpdateZeroDisplayedInWhite ();
		}

		protected void UpdateZeroDisplayedInWhite()
		{
			this.zeroDisplayedInWhiteButton.ActiveState = this.options.ZeroDisplayedInWhite ? ActiveState.Yes : ActiveState.No;
		}
		#endregion


		#region HasGraphicColumn
		protected void CreateHasGraphicColumnUI(FrameBox parent)
		{
			this.hasGraphicColumnButton = new CheckButton
			{
				Parent         = parent,
				FormattedText  = "Graphique du solde",
				AutoToggle     = false,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
				TabIndex       = ++this.tabIndex,
			};

			UIBuilder.AdjustWidth (this.hasGraphicColumnButton);

			this.hasGraphicColumnButton.Clicked += delegate
			{
				this.options.HasGraphicColumn = !this.options.HasGraphicColumn;
				this.UpdateHasGraphicColumn ();
				this.OptionsChanged ();
			};

			this.UpdateHasGraphicColumn ();
		}

		protected void UpdateHasGraphicColumn()
		{
			this.hasGraphicColumnButton.ActiveState = this.options.HasGraphicColumn ? ActiveState.Yes : ActiveState.No;
		}
		#endregion


		#region Catégories
		protected void CreateCatégoriesUI(FrameBox parent)
		{
			this.catégorieActifButton = new CheckButton
			{
				Parent      = parent,
				Text        = Converters.CatégorieToString (CatégorieDeCompte.Actif),
				AutoToggle  = false,
				Dock        = DockStyle.Left,
				Margins     = new Margins (0, 10, 0, 0),
				TabIndex    = ++this.tabIndex,
			};

			this.catégoriePassifButton = new CheckButton
			{
				Parent      = parent,
				Text        = Converters.CatégorieToString (CatégorieDeCompte.Passif),
				AutoToggle  = false,
				Dock        = DockStyle.Left,
				Margins     = new Margins (0, 10, 0, 0),
				TabIndex    = ++this.tabIndex,
			};

			this.catégorieChargeButton = new CheckButton
			{
				Parent      = parent,
				Text        = Converters.CatégorieToString (CatégorieDeCompte.Charge),
				AutoToggle  = false,
				Dock        = DockStyle.Left,
				Margins     = new Margins (0, 10, 0, 0),
				TabIndex    = ++this.tabIndex,
			};

			this.catégorieProduitButton = new CheckButton
			{
				Parent      = parent,
				Text        = Converters.CatégorieToString (CatégorieDeCompte.Produit),
				AutoToggle  = false,
				Dock        = DockStyle.Left,
				Margins     = new Margins (0, 10, 0, 0),
				TabIndex    = ++this.tabIndex,
			};

			this.catégorieExploitationButton = new CheckButton
			{
				Parent      = parent,
				Text        = Converters.CatégorieToString (CatégorieDeCompte.Exploitation),
				AutoToggle  = false,
				Dock        = DockStyle.Left,
				Margins     = new Margins (0, 10, 0, 0),
				TabIndex    = ++this.tabIndex,
			};

			UIBuilder.AdjustWidth (this.catégorieActifButton);
			UIBuilder.AdjustWidth (this.catégoriePassifButton);
			UIBuilder.AdjustWidth (this.catégorieChargeButton);
			UIBuilder.AdjustWidth (this.catégorieProduitButton);
			UIBuilder.AdjustWidth (this.catégorieExploitationButton);

			this.catégorieActifButton.Clicked += delegate
			{
				this.options.Catégories ^= CatégorieDeCompte.Actif;
				this.UpdateCatégories ();
				this.OptionsChanged ();
			};

			this.catégoriePassifButton.Clicked += delegate
			{
				this.options.Catégories ^= CatégorieDeCompte.Passif;
				this.UpdateCatégories ();
				this.OptionsChanged ();
			};

			this.catégorieChargeButton.Clicked += delegate
			{
				this.options.Catégories ^= CatégorieDeCompte.Charge;
				this.UpdateCatégories ();
				this.OptionsChanged ();
			};

			this.catégorieProduitButton.Clicked += delegate
			{
				this.options.Catégories ^= CatégorieDeCompte.Produit;
				this.UpdateCatégories ();
				this.OptionsChanged ();
			};

			this.catégorieExploitationButton.Clicked += delegate
			{
				this.options.Catégories ^= CatégorieDeCompte.Exploitation;
				this.UpdateCatégories ();
				this.OptionsChanged ();
			};

			this.UpdateCatégories ();
		}

		protected void UpdateCatégories()
		{
			this.catégorieActifButton       .ActiveState = ((this.options.Catégories & CatégorieDeCompte.Actif)        != 0) ? ActiveState.Yes : ActiveState.No;
			this.catégoriePassifButton      .ActiveState = ((this.options.Catégories & CatégorieDeCompte.Passif)       != 0) ? ActiveState.Yes : ActiveState.No;
			this.catégorieChargeButton      .ActiveState = ((this.options.Catégories & CatégorieDeCompte.Charge)       != 0) ? ActiveState.Yes : ActiveState.No;
			this.catégorieProduitButton     .ActiveState = ((this.options.Catégories & CatégorieDeCompte.Produit)      != 0) ? ActiveState.Yes : ActiveState.No;
			this.catégorieExploitationButton.ActiveState = ((this.options.Catégories & CatégorieDeCompte.Exploitation) != 0) ? ActiveState.Yes : ActiveState.No;
		}
		#endregion


		#region Deep
		protected void CreateDeepUI(FrameBox parent)
		{
			var fromLabel = new StaticText
			{
				Parent         = parent,
				Text           = "Profondeur de",
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
			};
			UIBuilder.AdjustWidth (fromLabel);

			this.deepFromField = new TextFieldCombo
			{
				Parent          = parent,
				IsReadOnly      = true,
				PreferredWidth  = 50,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			var toLabel = new StaticText
			{
				Parent         = parent,
				Text           = "À",
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 0, 0),
			};
			UIBuilder.AdjustWidth (toLabel);

			this.deepToField = new TextFieldCombo
			{
				Parent          = parent,
				IsReadOnly      = true,
				PreferredWidth  = 50,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 1, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.deepClearButton = new IconButton
			{
				Parent          = parent,
				IconUri         = UIBuilder.GetResourceIconUri ("Level.Clear"),
				AutoFocus       = false,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			ToolTip.Default.SetToolTip (this.deepClearButton, "Montre tous les comptes");

			this.InitializeDeep ();
			this.UpdateDeep ();

			this.deepFromField.TextChanged += delegate
			{
				this.DeepChanged ();
			};

			this.deepToField.TextChanged += delegate
			{
				this.DeepChanged ();
			};

			this.deepClearButton.Clicked += delegate
			{
				this.options.DeepFrom = 1;
				this.options.DeepTo   = int.MaxValue;

				this.InitializeDeep ();
				this.OptionsChanged ();
			};
		}

		protected void UpdateDeep()
		{
			using (this.ignoreChanges.Enter ())
			{
				this.deepFromField.FormattedText = this.DeepToDescription (this.options.DeepFrom);
				this.deepToField.FormattedText   = this.DeepToDescription (this.options.DeepTo);
			}
		}

		private void InitializeDeep()
		{
			this.InitializeDeep (this.deepFromField, 1, this.options.DeepTo);
			this.InitializeDeep (this.deepToField, this.options.DeepFrom, int.MaxValue);

			this.deepClearButton.Enable = (this.options.DeepFrom != 1 || this.options.DeepTo != int.MaxValue);
		}

		private void InitializeDeep(TextFieldCombo combo, int min, int max)
		{
			combo.Items.Clear ();

			for (int i = 1; i <= 6; i++)
			{
				if (i >= min && i <= max)
				{
					combo.Items.Add (this.DeepToDescription (i));  // 1..6
				}
			}

			if (max == int.MaxValue)
			{
				combo.Items.Add (this.DeepToDescription (int.MaxValue));  // Tout
			}
		}

		protected void DeepChanged()
		{
			if (this.ignoreChanges.IsZero)
			{
				this.options.DeepFrom = this.DescriptionToDeep (this.deepFromField.FormattedText);
				this.options.DeepTo   = this.DescriptionToDeep (this.deepToField.FormattedText);

				this.InitializeDeep ();
				this.OptionsChanged ();
			}
		}

		private FormattedText DeepToDescription(int deep)
		{
			if (deep == int.MaxValue)
			{
				return "Tout";
			}
			else
			{
				return deep.ToString ();  // 1..9
			}
		}

		private int DescriptionToDeep(FormattedText text)
		{
			var t = text.ToSimpleText ();

			if (string.IsNullOrEmpty (t) || t.Length != 1 || t[0] < '1' || t[0] > '9')
			{
				return int.MaxValue;
			}
			else
			{
				return t[0] - '0';  // 1..n
			}
		}
		#endregion

		#region Comparaison
		protected FrameBox CreateComparisonUI(FrameBox parent, ComparisonShowed possibleMode)
		{
			this.comparisonFrame = this.CreateSpecialistFrameUI(parent);

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
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
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
				this.comparisonFrame.Visibility = this.topPanelLeftController.Specialist;

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


		protected FrameBox CreateSpecialistFrameUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 5, 0),
				TabIndex        = ++this.tabIndex,
				Visibility      = false,
			};

			this.specialistFrames.Add (frame);

			return frame;
		}

		protected void CreateSeparator(FrameBox parent)
		{
			new Separator
			{
				Parent          = parent,
				PreferredWidth  = 1,
				IsVerticalLine  = true,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};
		}


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
			this.viewGraphButton.Margins = new Margins (2, 0, 0, 0);

			this.showCommonGraphButton = new BackIconButton
			{
				Parent          = frame,
				IconUri         = UIBuilder.GetResourceIconUri ("View.Graph.Common"),
				BackColor       = UIBuilder.SelectionColor,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				AutoToggle      = false,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (-1, 0, 0, 0),
			};

			this.showDetailedGraphButton = new BackIconButton
			{
				Parent          = frame,
				IconUri         = UIBuilder.GetResourceIconUri ("View.Graph.Detailed"),
				BackColor       = UIBuilder.SelectionColor,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				AutoToggle      = false,
				AutoFocus       = false,
				Dock            = DockStyle.Left,
				Margins         = new Margins (-1, 0, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.showCommonGraphButton,   "Montre les options graphiques principales");
			ToolTip.Default.SetToolTip (this.showDetailedGraphButton, "Montre les options graphiques détaillés");

			this.viewArrayButton.Clicked += delegate
			{
				this.options.ViewGraph = false;
				this.ShowHideControllers ();
				this.UpdateGraphWidgets ();
				this.OptionsChanged ();
			};

			this.viewGraphButton.Clicked += delegate
			{
				this.options.ViewGraph = true;
				this.ShowHideControllers ();
				this.UpdateGraphWidgets ();
				this.OptionsChanged ();
			};

			this.showCommonGraphButton.Clicked += delegate
			{
				if (this.options.GraphShowLevel == 0)
				{
					this.options.GraphShowLevel = 1;
				}
				else
				{
					this.options.GraphShowLevel = 0;
				}

				this.UpdateGraphWidgets ();
			};

			this.showDetailedGraphButton.Clicked += delegate
			{
				if (this.options.GraphShowLevel == 2)
				{
					this.options.GraphShowLevel = 1;
				}
				else
				{
					this.options.GraphShowLevel = 2;
				}

				this.UpdateGraphWidgets ();
			};

			return frame;
		}

		private void ShowHideControllers()
		{
			if (this.options.ViewGraph)
			{
				this.controller.DataAccessor.UpdateGraphData (force: true);
			}

			this.ShowArrayOrGraph ();
			this.UpdateWidgets ();
		}

		protected void UpdateGraphWidgets()
		{
			if (this.options.ViewGraph && this.options.GraphShowLevel == 1)
			{
				this.graphbar.Visibility = true;
				this.graphOptionsController.Detailed = false;
			}
			else if (this.options.ViewGraph && this.options.GraphShowLevel == 2)
			{
				this.graphbar.Visibility = true;
				this.graphOptionsController.Detailed = true;
			}
			else
			{
				this.graphbar.Visibility = false;
			}

			this.graphOptionsController.Update ();

			using (this.ignoreChanges.Enter ())
			{
				if (this.viewArrayButton != null)
				{
					this.viewArrayButton.ActiveState = this.options.ViewGraph ? ActiveState.No  : ActiveState.Yes;
					this.viewGraphButton.ActiveState = this.options.ViewGraph ? ActiveState.Yes : ActiveState.No;

					this.showCommonGraphButton.Visibility   = this.options.ViewGraph;
					this.showDetailedGraphButton.Visibility = this.options.ViewGraph;

					this.showCommonGraphButton.ActiveState   = this.options.GraphShowLevel >= 1 ? ActiveState.Yes : ActiveState.No;
					this.showDetailedGraphButton.ActiveState = this.options.GraphShowLevel >= 2 ? ActiveState.Yes : ActiveState.No;
				}
			}
		}

		private void ShowArrayOrGraph()
		{
			this.graphbar.Visibility =  this.options.ViewGraph;

			if (this.controller.ArrayController != null)
			{
				this.controller.ArrayController.Show = !this.options.ViewGraph;
			}

			if (this.controller.GraphWidget != null)
			{
				this.controller.GraphWidget.Visibility = this.options.ViewGraph;
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
			};

			ToolTip.Default.SetToolTip (button, description);

			return button;
		}


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							compta;
		protected readonly ComptaPériodeEntity					période;
		protected readonly AbstractOptions						options;
		protected readonly SafeCounter							ignoreChanges;
		protected readonly List<FrameBox>						specialistFrames;

		protected System.Action									optionsChanged;

		protected int											tabIndex;
		protected FrameBox										container;
		protected FrameBox										toolbar;
		protected FrameBox										graphbar;
		protected FrameBox										mainFrame;
		protected FrameBox										comparisonFrame;

		protected CheckButton									zeroFilteredButton;
		protected CheckButton									zeroDisplayedInWhiteButton;
		protected CheckButton									hasGraphicColumnButton;
		protected CheckButton									catégorieActifButton;
		protected CheckButton									catégoriePassifButton;
		protected CheckButton									catégorieChargeButton;
		protected CheckButton									catégorieProduitButton;
		protected CheckButton									catégorieExploitationButton;
		protected TextFieldCombo								deepFromField;
		protected TextFieldCombo								deepToField;
		protected IconButton									deepClearButton;

		protected CheckButton									buttonComparisonEnable;
		protected FrameBox										frameComparisonShowed;
		protected StaticText									fieldComparisonShowed;
		protected GlyphButton									buttonComparisonShowed;
		protected StaticText									labelComparisonDisplayMode;
		protected TextFieldCombo								fieldComparisonDisplayMode;

		protected BackIconButton								viewArrayButton;
		protected BackIconButton								viewGraphButton;
		protected BackIconButton								showCommonGraphButton;
		protected BackIconButton								showDetailedGraphButton;

		protected TopPanelLeftController						topPanelLeftController;
		protected TopPanelRightController						topPanelRightController;
		protected GraphOptionsController						graphOptionsController;

		protected bool											showPanel;
	}
}
