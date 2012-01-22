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
		public AbstractOptionsController(ComptaEntity comptaEntity, AbstractOptions options)
		{
			this.comptaEntity = comptaEntity;
			this.options      = options;
		}


		public AbstractOptions Options
		{
			get
			{
				return this.options;
			}
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
			}
		}


		public virtual void CreateUI(FrameBox parent, System.Action optionsChanged)
		{
		}


		protected FrameBox CreateProfondeurUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				PreferredWidth  = 140,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			new StaticText
			{
				Parent         = frame,
				FormattedText  = "Profondeur",
				PreferredWidth = 64,
				Dock           = DockStyle.Left,
			};

			var field = new TextFieldCombo
			{
				Parent          = frame,
				IsReadOnly      = true,
				PreferredHeight = 20,
				FormattedText   = this.ProfondeurToDescription (this.options.Profondeur),
				Dock            = DockStyle.Fill,
			};

			for (int i = 1; i <= 6; i++)
            {
				field.Items.Add (this.ProfondeurToDescription (i));  // 1..6
            }
			field.Items.Add (this.ProfondeurToDescription (null));  // Tout

			field.TextChanged += delegate
			{
				this.options.Profondeur = this.DescriptionToProfondeur (field.FormattedText);
				optionsChanged ();
			};

			return frame;
		}

		private FormattedText ProfondeurToDescription(int? profondeur)
		{
			if (profondeur.HasValue)
			{
				return profondeur.ToString ();  // 1..9
			}
			else
			{
				return "Tout";
			}
		}

		private int? DescriptionToProfondeur(FormattedText text)
		{
			var t = text.ToSimpleText();

			if (string.IsNullOrEmpty (t)|| t.Length != 1 || t[0] < '1' || t[0] < '9')
			{
				return t[0] - '0';
			}
			else
			{
				return null;
			}
		}


		#region Dates
		protected FrameBox CreateDatesUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 5, 0),
				TabIndex        = ++this.tabIndex,
			};

			new StaticText
			{
				Parent         = frame,
				FormattedText  = "Depuis le",
				PreferredWidth = 64,
				Dock           = DockStyle.Left,
			};

			this.fieldDateDébut = new TextFieldEx
			{
				Parent                       = frame,
				PreferredWidth               = 100,
				PreferredHeight              = 20,
				FormattedText                = this.options.DateDébut.HasValue ? this.options.DateDébut.ToString () : FormattedText.Empty,
				Dock                         = DockStyle.Left,
				Margins                      = new Margins (0, 20, 0, 0),
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex                     = ++this.tabIndex,
			};

			new StaticText
			{
				Parent         = frame,
				FormattedText  = "Jusqu'au",
				PreferredWidth = 64,
				Dock           = DockStyle.Left,
			};

			this.fieldDateFin = new TextFieldEx
			{
				Parent                       = frame,
				PreferredWidth               = 100,
				PreferredHeight              = 20,
				FormattedText                = this.options.DateFin.HasValue ? this.options.DateFin.ToString () : FormattedText.Empty,
				Dock                         = DockStyle.Left,
				Margins                      = new Margins (0, 20, 0, 0),
				DefocusAction                = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				TabIndex                     = ++this.tabIndex,
			};

			this.clearButton = new GlyphButton
			{
				Parent        = frame,
				GlyphShape    = GlyphShape.Close,
				PreferredSize = new Size (20, 20),
				Dock          = DockStyle.Left,
				TabIndex      = ++this.tabIndex,
			};

			this.fieldDateDébut.EditionAccepted += delegate
			{
				this.CheckDate (this.fieldDateDébut, x => this.options.DateDébut = x, optionsChanged);
			};

			this.fieldDateFin.EditionAccepted += delegate
			{
				this.CheckDate (this.fieldDateFin, x => this.options.DateFin = x, optionsChanged);
			};

			this.clearButton.Clicked += delegate
			{
				this.fieldDateDébut.FormattedText = null;
				this.fieldDateFin.FormattedText   = null;
				this.options.DateDébut = null;
				this.options.DateFin   = null;
				this.UpdateClearButton ();
				optionsChanged ();
			};

			ToolTip.Default.SetToolTip (this.fieldDateDébut, "Filtre depuis cette date (inclus)");
			ToolTip.Default.SetToolTip (this.fieldDateFin,   "Filtre jusqu'à cette date (inclus)");
			ToolTip.Default.SetToolTip (this.clearButton,    "Annule le filtre");

			this.UpdateClearButton ();

			return frame;
		}

		private delegate void SetDate(Date? date);

		private void CheckDate(TextFieldEx field, SetDate setDate, System.Action optionsChanged)
		{
			Date? date;
			if (this.comptaEntity.ParseDate (field.FormattedText, out date))
			{
				setDate (date);
				field.FormattedText = date.HasValue ? date.ToString () : FormattedText.Empty;
				field.SetError (false);
				this.UpdateClearButton ();
				optionsChanged ();
			}
			else
			{
				field.SetError (true);
			}
		}

		private void UpdateClearButton()
		{
			this.clearButton.Enable = this.options.DateDébut.HasValue || this.options.DateFin.HasValue;
		}
		#endregion


		#region Budget
		protected FrameBox CreateBudgetUI(FrameBox parent, System.Action optionsChanged)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 5, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.buttonBudgetEnable = new CheckButton
			{
				Parent          = frame,
				PreferredWidth  = 120,
				PreferredHeight = 20,
				ActiveState     = this.options.BudgetEnable ? ActiveState.Yes : ActiveState.No,
				Dock            = DockStyle.Left,
				TabIndex        = ++this.tabIndex,
			};

			this.fieldBudgetShowed = new TextFieldCombo
			{
				Parent          = frame,
				IsReadOnly      = true,
				PreferredWidth  = 150,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.frameBudgetDisplayMode = new StaticText
			{
				Parent         = frame,
				FormattedText  = "Affichage",
				PreferredWidth = 55,
				Dock           = DockStyle.Left,
			};

			this.fieldBudgetDisplayMode = new TextFieldCombo
			{
				Parent          = frame,
				IsReadOnly      = true,
				PreferredWidth  = 200,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
				TabIndex        = ++this.tabIndex,
			};

			this.BudgetShowedInitialize (this.fieldBudgetShowed);
			this.BudgetDisplayModeInitialize (this.fieldBudgetDisplayMode);

			this.SetBudgetShowed (this.fieldBudgetShowed, this.options.BudgetShowed);
			this.SetBudgetDisplayMode (this.fieldBudgetDisplayMode, this.options.BudgetDisplayMode);

			//	Gestion des événements.
			this.buttonBudgetEnable.ActiveStateChanged += delegate
			{
				this.options.BudgetEnable = (this.buttonBudgetEnable.ActiveState == ActiveState.Yes);
				this.UpdateBudget ();
				optionsChanged ();
			};

			this.fieldBudgetShowed.TextChanged += delegate
			{
				this.options.BudgetShowed = this.GetBudgetShowed (this.fieldBudgetShowed);
				optionsChanged ();
			};

			this.fieldBudgetDisplayMode.TextChanged += delegate
			{
				this.options.BudgetDisplayMode = this.GetBudgetDisplayMode (this.fieldBudgetDisplayMode);
				optionsChanged ();
			};

			this.UpdateBudget ();

			return frame;
		}

		private void UpdateBudget()
		{
			bool enable = this.options.BudgetEnable;

			this.buttonBudgetEnable.Text = enable ? "Comparaison avec" : "Comparaison";
			this.fieldBudgetShowed.Visibility = enable;
			this.frameBudgetDisplayMode.Visibility = enable;
			this.fieldBudgetDisplayMode.Visibility = enable;
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

		private void SetBudgetShowed(TextFieldCombo combo, BudgetShowed budget)
		{
			combo.Text = this.GetBudgetDescription (budget);
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

		private void SetBudgetDisplayMode(TextFieldCombo combo, BudgetDisplayMode budget)
		{
			combo.Text = this.GetBudgetDescription (budget);
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


		protected readonly ComptaEntity							comptaEntity;
		protected readonly AbstractOptions						options;

		protected int											tabIndex;
		protected FrameBox										toolbar;

		protected TextFieldEx									fieldDateDébut;
		protected TextFieldEx									fieldDateFin;
		protected GlyphButton									clearButton;

		protected CheckButton									buttonBudgetEnable;
		protected TextFieldCombo								fieldBudgetShowed;
		protected StaticText									frameBudgetDisplayMode;
		protected TextFieldCombo								fieldBudgetDisplayMode;

		protected bool											showPanel;
	}
}
