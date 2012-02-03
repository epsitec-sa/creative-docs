//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptionsController
	{
		public AbstractOptionsController(AbstractController controller)
		{
			this.controller = controller;

			this.comptaEntity  = this.controller.ComptaEntity;
			this.périodeEntity = this.controller.PériodeEntity;
			this.options       = this.controller.DataAccessor.AccessorOptions;
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


		public virtual void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
			this.optionsChanged = optionsChanged;

			this.toolbar = new FrameBox
			{
				Parent              = parent,
				DrawFullFrame       = true,
				BackColor           = Color.FromHexa ("d2f0ff"),  // bleu pastel
				ContainerLayoutMode = Common.Widgets.ContainerLayoutMode.VerticalFlow,
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

		private void ClearAction()
		{
			this.options.Clear ();
			this.OptionsChanged ();
		}

		protected virtual void LevelChangedAction()
		{
			this.options.Specialist = this.levelController.Specialist;

			if (this.budgtetFrame != null)
			{
				this.budgtetFrame.Visibility = this.levelController.Specialist;
			}
		}

		protected virtual void UpdateWidgets()
		{
			this.levelController.ClearEnable = !this.options.IsEmpty;
		}


		public virtual void UpdateContent()
		{
		}


		#region Budget
		protected FrameBox CreateBudgetUI(FrameBox parent)
		{
			this.budgtetFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 5, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.buttonBudgetEnable = new CheckButton
			{
				Parent          = this.budgtetFrame,
				PreferredWidth  = 120,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			this.fieldBudgetShowed = new TextFieldCombo
			{
				Parent          = this.budgtetFrame,
				IsReadOnly      = true,
				PreferredWidth  = 150,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.frameBudgetDisplayMode = new StaticText
			{
				Parent         = this.budgtetFrame,
				FormattedText  = "Affichage",
				PreferredWidth = 55,
				Dock           = DockStyle.Left,
			};

			this.fieldBudgetDisplayMode = new TextFieldCombo
			{
				Parent          = this.budgtetFrame,
				IsReadOnly      = true,
				PreferredWidth  = 200,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.BudgetShowedInitialize (this.fieldBudgetShowed);
			this.BudgetDisplayModeInitialize (this.fieldBudgetDisplayMode);

			this.UpdateBudget ();

			//	Gestion des événements.
			this.buttonBudgetEnable.ActiveStateChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.options.BudgetEnable = (this.buttonBudgetEnable.ActiveState == ActiveState.Yes);
					this.OptionsChanged ();
				}
			};

			this.fieldBudgetShowed.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.options.BudgetShowed = this.GetBudgetShowed (this.fieldBudgetShowed);
					this.OptionsChanged ();
				}
			};

			this.fieldBudgetDisplayMode.TextChanged += delegate
			{
				if (!this.ignoreChange)
				{
					this.options.BudgetDisplayMode = this.GetBudgetDisplayMode (this.fieldBudgetDisplayMode);
					this.OptionsChanged ();
				}
			};

			return this.budgtetFrame;
		}

		protected void UpdateBudget()
		{
			this.ignoreChange = true;

			this.budgtetFrame.Visibility = this.levelController.Specialist;

			bool enable = this.options.BudgetEnable;

			this.buttonBudgetEnable.ActiveState = this.options.BudgetEnable ? ActiveState.Yes : ActiveState.No;
			this.buttonBudgetEnable.Text = enable ? "Comparaison avec" : "Comparaison";

			this.fieldBudgetShowed     .Visibility = enable;
			this.frameBudgetDisplayMode.Visibility = enable;
			this.fieldBudgetDisplayMode.Visibility = enable;

			this.fieldBudgetShowed     .Text = this.GetBudgetDescription (this.options.BudgetShowed);
			this.fieldBudgetDisplayMode.Text = this.GetBudgetDescription (this.options.BudgetDisplayMode);

			this.ignoreChange = false;
		}


		private void BudgetShowedInitialize(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			combo.Items.Add (this.GetBudgetDescription (BudgetShowed.Budget));
			combo.Items.Add (this.GetBudgetDescription (BudgetShowed.Prorata));
			combo.Items.Add (this.GetBudgetDescription (BudgetShowed.Futur));
			combo.Items.Add (this.GetBudgetDescription (BudgetShowed.FuturProrata));
			combo.Items.Add (this.GetBudgetDescription (BudgetShowed.Précédent));
		}

		private BudgetShowed GetBudgetShowed(TextFieldCombo combo)
		{
			foreach (var value in System.Enum.GetValues (typeof (BudgetShowed)))
			{
				var budget = (BudgetShowed) value;

				if (combo.Text == this.GetBudgetDescription (budget))
				{
					return budget;
				}
			}

			return BudgetShowed.Budget;
		}

		private string GetBudgetDescription(BudgetShowed budget)
		{
			switch (budget)
			{
				case BudgetShowed.Budget:
					return "Budget";

				case BudgetShowed.Prorata:
					return "Budget au prorata";

				case BudgetShowed.Futur:
					return "Budget futur";

				case BudgetShowed.FuturProrata:
					return "Budget futur au prorata";

				case BudgetShowed.Précédent:
					return "Année précédente";

				default:
					return "?";
			}
		}


		private void BudgetDisplayModeInitialize(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			combo.Items.Add (this.GetBudgetDescription (BudgetDisplayMode.Montant));
			combo.Items.Add (this.GetBudgetDescription (BudgetDisplayMode.Différence));
			combo.Items.Add (this.GetBudgetDescription (BudgetDisplayMode.Pourcent));
			combo.Items.Add (this.GetBudgetDescription (BudgetDisplayMode.PourcentMontant));
			combo.Items.Add (this.GetBudgetDescription (BudgetDisplayMode.Graphique));
		}

		private BudgetDisplayMode GetBudgetDisplayMode(TextFieldCombo combo)
		{
			foreach (var value in System.Enum.GetValues (typeof (BudgetDisplayMode)))
			{
				var budget = (BudgetDisplayMode) value;

				if (combo.Text == this.GetBudgetDescription (budget))
				{
					return budget;
				}
			}

			return BudgetDisplayMode.Montant;
		}

		private string GetBudgetDescription(BudgetDisplayMode budget)
		{
			switch (budget)
			{
				case BudgetDisplayMode.Montant:
					return "Montant";

				case BudgetDisplayMode.Différence:
					return "Différence en francs";

				case BudgetDisplayMode.Pourcent:
					return "Comparaison en %";

				case BudgetDisplayMode.PourcentMontant:
					return "Comparaison en % avec montant";

				case BudgetDisplayMode.Graphique:
					return "Graphique";

				default:
					return "?";
			}
		}
		#endregion


		protected readonly AbstractController					controller;
		protected readonly ComptaEntity							comptaEntity;
		protected readonly ComptaPériodeEntity					périodeEntity;
		protected readonly AbstractOptions						options;

		protected System.Action									optionsChanged;

		protected int											tabIndex;
		protected FrameBox										toolbar;
		protected FrameBox										mainFrame;
		protected FrameBox										budgtetFrame;

		protected TextFieldCombo								fieldProfondeur;
		protected CheckButton									buttonBudgetEnable;
		protected TextFieldCombo								fieldBudgetShowed;
		protected StaticText									frameBudgetDisplayMode;
		protected TextFieldCombo								fieldBudgetDisplayMode;
		protected LevelController								levelController;

		protected bool											showPanel;
		protected bool											ignoreChange;
	}
}
