//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.ComptabilitéControllers
{
	/// <summary>
	/// Ce contrôleur gère les options d'affichage génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractOptionsController<Entity>
		where Entity : class
	{
		public AbstractOptionsController(ComptabilitéEntity comptabilitéEntity, AbstractOptions options)
		{
			this.comptabilitéEntity = comptabilitéEntity;
			this.options            = options;

			this.toolbarShowed = true;
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
			if (this.comptabilitéEntity.ParseDate (field.FormattedText, out date))
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


		public void FinalizeUI(FrameBox parent)
		{
			//	Widgets créés en dernier, pour être par-dessus tout le reste.
			this.showHideButton = new GlyphButton
			{
				Parent        = parent,
				Anchor        = AnchorStyles.TopRight,
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

		public double TopOffset
		{
			set
			{
				if (this.topOffset != value)
				{
					this.topOffset = value;
					this.UpdateShowHideButton ();
				}
			}
		}


		private void UpdateShowHideButton()
		{
			//	Met à jour le bouton pour montrer/cacher la barre d'icône.
			this.showHideButton.GlyphShape = this.toolbarShowed ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			this.showHideButton.Margins = new Margins (0, 0, this.toolbarShowed ? this.topOffset+20 : this.topOffset+0, 0);

			ToolTip.Default.SetToolTip (this.showHideButton, this.toolbarShowed ? "Cache les options" : "Montre les options");

			this.toolbar.Visibility   = this.toolbarShowed;
		}


		protected readonly ComptabilitéEntity					comptabilitéEntity;
		protected readonly AbstractOptions						options;

		protected int											tabIndex;
		protected FrameBox										toolbar;
		protected TextFieldEx									fieldDateDébut;
		protected TextFieldEx									fieldDateFin;
		protected GlyphButton									clearButton;
		protected GlyphButton									showHideButton;
		protected bool											toolbarShowed;
		protected double										topOffset;
	}
}
