//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Support;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.Finance;
using Epsitec.Cresus.Core.Business.Finance.Comptabilité;
using Epsitec.Cresus.Core.Dialogs;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	public class MainWindow : CoreDialog
	{
		public MainWindow(CoreApp app, BusinessContext businessContext, ComptabilitéEntity comptabilité, TypeDeDocumentComptable type)
			: base (app)
		{
			this.app = app;
			this.businessContext = businessContext;
			this.comptabilité = comptabilité;
			this.selectedType = type;

			this.toolbarShowed = true;
			this.toolbarTypes = this.WindowTypes.ToList ();
			this.toolbarFrames = new List<FrameBox> ();
			this.toolbarIconButtons = new List<IconButton> ();
		}

		public FrameBox MainFrame
		{
			get
			{
				return this.mainFrame;
			}
		}

		protected override void SetupWindow(Window window)
		{
			window.Text = "Crésus Comptabilité";
			window.ClientSize = new Size (800, 500);
			window.Root.WindowStyles = WindowStyles.DefaultDocumentWindow;  // pour avoir les boutons Minimize/Maximize/Close !
		}

		protected override void SetupWidgets(Window window)
		{
			//	Ce bouton est nécessaire pour que le bouton de fermeture [x] en haut à droite
			//	de la fenêtre existe !
			var button = new Button (Epsitec.Common.Dialogs.Res.Commands.Dialog.Generic.Close)
			{
				Parent     = window.Root,
				Anchor     = AnchorStyles.TopLeft,
				Visibility = false,
			};

			//	Crée les frames principales.
			var frame = new FrameBox
			{
				Parent  = window.Root,
				Dock    = DockStyle.Fill,
				Padding = new Margins (10, 10, 5, 10),
			};

			this.toolbarFrame = new FrameBox
			{
				Parent         = frame,
				PreferredWidth = MainWindow.IconSize,
				Dock           = DockStyle.Left,
				Margins        = new Margins (0, 10, 22, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent  = frame,
				Dock    = DockStyle.Fill,
			};

			//	Crée la barre d'icônes à gauche.
			this.CreateToolbarUI (this.toolbarFrame);

			//	Crée le titre en haut.
			this.title = new StaticText
			{
				Parent           = rightFrame,
				ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
				PreferredHeight  = 22,
				Dock             = DockStyle.Top,
			};

			//	Crée la zone éditable.
			this.mainFrame = new FrameBox
			{
				Parent = rightFrame,
				Dock   = DockStyle.Fill,
			};

			this.CreateEditor ();
			this.CreateShowHideButtonUI (frame);
		}


		private void CreateShowHideButtonUI(FrameBox parent)
		{
			this.showHideButton = new GlyphButton
			{
				Parent        = parent,
				Anchor        = AnchorStyles.TopLeft,
				PreferredSize = new Size (16, 16),
				ButtonStyle   = ButtonStyle.Slider,
			};

			this.showHideButton.Clicked += delegate
			{
				this.toolbarShowed = !this.toolbarShowed;
				this.UpdateShowHideButton ();
			};

			this.UpdateShowHideButton ();
		}

		private void UpdateShowHideButton()
		{
			//	Met à jour le bouton pour montrer/cacher la barre d'icône.
			this.showHideButton.GlyphShape = this.toolbarShowed ? GlyphShape.ArrowLeft : GlyphShape.ArrowRight;

			ToolTip.Default.SetToolTip (this.showHideButton, this.toolbarShowed ? "Cache la barre d'icônes" : "Montre la barre d'icônes");

			this.title.Margins = new Margins (this.toolbarShowed ? 0 : 25, 0, 0, 0);
			this.toolbarFrame.Visibility = this.toolbarShowed;
		}


		private void CreateToolbarUI(Widget parent)
		{
			this.toolbarFrames.Clear ();
			this.toolbarIconButtons.Clear ();

			int index = 0;
			foreach (var type in this.toolbarTypes)
			{
				this.CreateToolbarBottonUI (parent, type, index++);
			}

			var add = new IconButton
			{
				Parent        = parent,
				IconUri       = Misc.GetResourceIconUri ("Action.Create"),
				PreferredSize = new Size (MainWindow.IconSize, MainWindow.IconSize),
				Dock          = DockStyle.Top,
			};

			add.Clicked += delegate
			{
				this.OpenNewWindow ();
			};

			ToolTip.Default.SetToolTip (add, "Ouvre une nouvelle fenêtre comptable");
		}

		private void OpenNewWindow()
		{
			var window = new MainWindow (this.app, this.businessContext, this.comptabilité, this.selectedType);

			window.IsModal = false;
			window.OpenDialog ();
		}

		private void CreateToolbarBottonUI(Widget parent, TypeDeDocumentComptable type, int index)
		{
			var frame = new FrameBox
			{
				Parent        = parent,
				Index         = index,
				PreferredSize = new Size (MainWindow.IconSize, MainWindow.IconSize),
				Dock          = DockStyle.Top,
			};

			var button = new IconButton
			{
				Parent        = frame,
				Index         = index,
				IconUri       = Misc.GetResourceIconUri (this.GetToolbarIcon (type)),
				PreferredSize = new Size (MainWindow.IconSize, MainWindow.IconSize),
				Dock          = DockStyle.Fill,
			};

			button.Clicked += delegate
			{
				this.selectedType = this.toolbarTypes[button.Index];
				this.CreateEditor ();
			};

			ToolTip.Default.SetToolTip (button, this.GetToolbarDescription (type));

			this.toolbarFrames.Add (frame);
			this.toolbarIconButtons.Add (button);
		}

		private void UpdateToolbarButtons()
		{
			for (int i = 0; i < this.toolbarTypes.Count; i++)
			{
				if (this.toolbarTypes[i] == this.selectedType)
				{
					this.toolbarFrames[i].BackColor = Color.FromName ("Gold");
					this.toolbarFrames[i].DrawFullFrame = true;
				}
				else
				{
					this.toolbarFrames[i].BackColor = Color.Empty;
					this.toolbarFrames[i].DrawFullFrame = false;
				}
			}
		}

		private void UpdateTitle()
		{
			var title = TextFormatter.FormatText ("Comptabilité", this.comptabilité.GetCompactSummary (), "/", this.GetToolbarDescription (this.selectedType));
			this.title.FormattedText = title.ApplyBold ().ApplyFontSize (13.0);
		}

		private void CreateEditor()
		{
			this.DisposeController ();

			switch (this.selectedType)
			{
				case TypeDeDocumentComptable.Journal:
					this.journalController = new JournalController (this.businessContext, this.comptabilité);
					this.journalController.CreateUI (this.mainFrame);
					break;

				case TypeDeDocumentComptable.PlanComptable:
					this.planComptableController = new PlanComptableController (this.businessContext, this.comptabilité);
					this.planComptableController.CreateUI (this.mainFrame);
					break;

				case TypeDeDocumentComptable.Balance:
					this.balanceController = new BalanceController (this.businessContext, this.comptabilité);
					this.balanceController.CreateUI (this.mainFrame);
					break;

				case TypeDeDocumentComptable.Extrait:
					this.extraitDeCompteController = new ExtraitDeCompteController (this.businessContext, this.comptabilité);
					this.extraitDeCompteController.CreateUI (this.mainFrame);
					break;

				case TypeDeDocumentComptable.Bilan:
					this.bilanController = new BilanController (this.businessContext, this.comptabilité);
					this.bilanController.CreateUI (this.mainFrame);
					break;

				case TypeDeDocumentComptable.PP:
					this.ppController = new PPController (this.businessContext, this.comptabilité);
					this.ppController.CreateUI (this.mainFrame);
					break;

				case TypeDeDocumentComptable.Exploitation:
					this.exploitationController = new ExploitationController (this.businessContext, this.comptabilité);
					this.exploitationController.CreateUI (this.mainFrame);
					break;
			}

			this.UpdateToolbarButtons ();
			this.UpdateTitle ();
		}

		private void DisposeController()
		{
			if (this.journalController != null)
			{
				this.journalController.Dispose ();
				this.journalController = null;
			}

			if (this.planComptableController != null)
			{
				this.planComptableController.Dispose ();
				this.planComptableController = null;
			}

			if (this.balanceController != null)
			{
				this.balanceController.Dispose ();
				this.balanceController = null;
			}

			if (this.extraitDeCompteController != null)
			{
				this.extraitDeCompteController.Dispose ();
				this.extraitDeCompteController = null;
			}

			if (this.bilanController != null)
			{
				this.bilanController.Dispose ();
				this.bilanController = null;
			}

			if (this.ppController != null)
			{
				this.ppController.Dispose ();
				this.ppController = null;
			}

			if (this.exploitationController != null)
			{
				this.exploitationController.Dispose ();
				this.exploitationController = null;
			}

			this.mainFrame.Children.Clear ();
		}



		private string GetToolbarIcon(TypeDeDocumentComptable type)
		{
			switch (type)
			{
				case TypeDeDocumentComptable.Journal:
					return "Comptabilité.Journal";

				case TypeDeDocumentComptable.PlanComptable:
					return "Comptabilité.PlanComptable";

				case TypeDeDocumentComptable.Balance:
					return "Comptabilité.Balance";

				case TypeDeDocumentComptable.Extrait:
					return "Comptabilité.Compte";

				case TypeDeDocumentComptable.Bilan:
					return "Comptabilité.Bilan";

				case TypeDeDocumentComptable.PP:
					return "Comptabilité.PP";

				case TypeDeDocumentComptable.Exploitation:
					return "Comptabilité.Exploitation";

				case TypeDeDocumentComptable.Budgets:
					return "Comptabilité.Budgets";

				case TypeDeDocumentComptable.Change:
					return "Comptabilité.Change";

				case TypeDeDocumentComptable.RésuméPériodique:
					return "Comptabilité.RésuméPériodique";

				case TypeDeDocumentComptable.RésuméTVA:
					return "Comptabilité.RésuméTVA";

				case TypeDeDocumentComptable.DécompteTVA:
					return "Comptabilité.DécompteTVA";
			}

			return null;
		}

		private FormattedText GetToolbarDescription(TypeDeDocumentComptable type)
		{
			switch (type)
			{
				case TypeDeDocumentComptable.Journal:
					return "Journal des écritures";

				case TypeDeDocumentComptable.PlanComptable:
					return "Plan comptable";

				case TypeDeDocumentComptable.Balance:
					return "Balance";

				case TypeDeDocumentComptable.Extrait:
					return "Extrait de compte";

				case TypeDeDocumentComptable.Bilan:
					return "Bilan";

				case TypeDeDocumentComptable.PP:
					return "Pertes et Profits";

				case TypeDeDocumentComptable.Exploitation:
					return "Compte d'exploitation";

				case TypeDeDocumentComptable.Budgets:
					return "Budgets";

				case TypeDeDocumentComptable.Change:
					return "Différences de change";

				case TypeDeDocumentComptable.RésuméPériodique:
					return "Résumé périodique";

				case TypeDeDocumentComptable.RésuméTVA:
					return "Résumé TVA";

				case TypeDeDocumentComptable.DécompteTVA:
					return "Décompte TVA";
			}

			return FormattedText.Empty;
		}

		private IEnumerable<TypeDeDocumentComptable> WindowTypes
		{
			get
			{
				yield return TypeDeDocumentComptable.Journal;
				yield return TypeDeDocumentComptable.PlanComptable;
				yield return TypeDeDocumentComptable.Balance;
				yield return TypeDeDocumentComptable.Extrait;
				yield return TypeDeDocumentComptable.Bilan;
				yield return TypeDeDocumentComptable.PP;
				yield return TypeDeDocumentComptable.Exploitation;
				yield return TypeDeDocumentComptable.Budgets;
				yield return TypeDeDocumentComptable.Change;
				yield return TypeDeDocumentComptable.RésuméPériodique;
				yield return TypeDeDocumentComptable.RésuméTVA;
				yield return TypeDeDocumentComptable.DécompteTVA;
			}
		}

		public enum TypeDeDocumentComptable
		{
			Aucun,
			Journal,
			PlanComptable,
			Balance,
			Extrait,
			Bilan,
			PP,
			Exploitation,
			Budgets,
			Change,
			RésuméPériodique,
			RésuméTVA,
			DécompteTVA,
		};


		[Command (Epsitec.Common.Dialogs.Res.CommandIds.Dialog.Generic.Close)]
		private void ProcessClose()
		{
			this.DisposeController ();

			this.Result = DialogResult.Cancel;
			this.CloseDialog ();
		}


		private readonly static double IconSize = 40;

		private readonly CoreApp app;
		private readonly BusinessContext businessContext;
		private readonly ComptabilitéEntity comptabilité;
		private readonly List<TypeDeDocumentComptable> toolbarTypes;
		private readonly List<FrameBox> toolbarFrames;
		private readonly List<IconButton> toolbarIconButtons;

		private StaticText title;
		private FrameBox toolbarFrame;
		private FrameBox mainFrame;
		private TypeDeDocumentComptable selectedType;
		private GlyphButton						showHideButton;
		private bool							toolbarShowed;

		private JournalController journalController;
		private PlanComptableController planComptableController;
		private BalanceController balanceController;
		private ExtraitDeCompteController extraitDeCompteController;
		private BilanController bilanController;
		private PPController ppController;
		private ExploitationController exploitationController;
	}
}
