//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Controllers;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Ce contrôleur gère la barre d'outil supérieure de filtre pour la comptabilité.
	/// </summary>
	public class TopTempoController
	{
		public TopTempoController(AbstractController controller)
		{
			this.controller = controller;

			this.compta          = this.controller.ComptaEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.businessContext = this.controller.BusinessContext;
			this.data            = this.controller.DataAccessor.TempoData;
		}


		public bool ShowPanel
		{
			get
			{
				return this.showPanel;
			}
			set
			{
				if (this.showPanel != value)
				{
					this.showPanel = value;
					this.toolbar.Visibility = this.showPanel;
				}
			}
		}

		public bool Specialist
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		public void SearchClear()
		{
			this.data.Clear ();
			this.UpdateButtons ();
		}


		public void CreateUI(FrameBox parent, System.Action searchStartAction)
		{
			this.searchStartAction = searchStartAction;

			this.toolbar = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = TopTempoController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = UIBuilder.TempoBackColor,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, -1),
				Visibility      = false,
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

			this.CreateMainUI ();
			this.CreateFilterEnableButtonUI ();

			//	Remplissage de la frame gauche.
			this.topPanelLeftController = new TopPanelLeftController (this.controller);
			this.topPanelLeftController.CreateUI (topPanelLeftFrame, false, "Panel.Tempo", this.LevelChangedAction);
			this.topPanelLeftController.Specialist = false;

			//	Remplissage de la frame droite.
			this.topPanelRightController = new TopPanelRightController (this.controller);
			this.topPanelRightController.CreateUI (topPanelRightFrame, "Termine le filtre", this.ClearAction, this.controller.MainWindowController.ClosePanelTempo, this.LevelChangedAction);

			this.UpdateButtons ();
		}

		private void CreateMainUI()
		{
			new StaticText
			{
				Parent           = this.mainFrame,
				Text             = "Filtrer",
				TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				ContentAlignment = ContentAlignment.MiddleRight,
				PreferredWidth   = UIBuilder.LeftLabelWidth-10,
				PreferredHeight  = 20,
				Dock             = DockStyle.Left,
				Margins          = new Margins (0, 10, 0, 0),
			};

			this.editionFrame = new FrameBox
			{
				Parent          = this.mainFrame,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			this.staticDates = new StaticText
			{
				Parent          = this.mainFrame,
				PreferredWidth  = 160,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			{
				var label = new StaticText
				{
					Parent         = this.editionFrame,
					Text           = "Du",
					Dock           = DockStyle.Left,
					Margins        = new Margins (0, 10, 0, 0),
				};

				label.PreferredWidth = label.GetBestFitSize ().Width;

				var initialDate = Converters.DateToString (this.data.BeginDate);
				this.beginDateController = UIBuilder.CreateDateField (this.controller, this.editionFrame, initialDate, "Date initiale incluse", this.ValidateDate, this.DateChanged);
				this.beginDateController.Box.Dock = DockStyle.Left;
			}

			{
				var label = new StaticText
				{
					Parent         = this.editionFrame,
					Text           = "Au",
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 10, 0, 0),
				};

				label.PreferredWidth = label.GetBestFitSize ().Width;

				var initialDate = Converters.DateToString (this.data.EndDate);
				this.endDateController = UIBuilder.CreateDateField (this.controller, this.editionFrame, initialDate, "Date finale incluse", this.ValidateDate, this.DateChanged);
				this.endDateController.Box.Dock = DockStyle.Left;
			}

			this.prevButton = new GlyphButton
			{
				Parent          = this.mainFrame,
				GlyphShape      = GlyphShape.Minus,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.nextButton = new GlyphButton
			{
				Parent          = this.mainFrame,
				GlyphShape      = GlyphShape.Plus,
				ButtonStyle     = ButtonStyle.ToolItem,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.prevButton, "Période précédente");
			ToolTip.Default.SetToolTip (this.nextButton, "Période suivante");

			var durationLabel = new StaticText
			{
				Parent          = this.mainFrame,
				FormattedText   = "Durée",
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (10, 10, 0, 0),
			};

			durationLabel.PreferredWidth = durationLabel.GetBestFitSize ().Width;

			this.durationField = new TextFieldCombo
			{
				Parent          = this.mainFrame,
				PreferredWidth  = 100,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			TopTempoController.InitTempoDataDurationCombo (this.durationField);

			//	Connexion des événements.
			this.durationField.SelectedItemChanged += delegate
			{
				this.data.Duration = TopTempoController.TempoDataDurationToType (this.durationField.FormattedText);
				this.InitDefaultDates ();
				this.UpdateButtons ();
			};

			this.prevButton.Clicked += delegate
			{
				this.data.Next (-1);
				this.UpdateButtons ();
			};

			this.nextButton.Clicked += delegate
			{
				this.data.Next (1);
				this.UpdateButtons ();
			};
		}

		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (this.controller.MainWindowController.Période, data, emptyAccepted: true);
		}

		private void DateChanged(int line, ColumnType columnType)
		{
			this.data.BeginDate = Converters.ParseDate (this.beginDateController.EditionData.Text);
			this.data.EndDate   = Converters.ParseDate (this.endDateController.EditionData.Text);

			this.searchStartAction ();
		}


		private void CreateFilterEnableButtonUI()
		{
			this.filterEnableButton = new CheckButton
			{
				Parent           = this.mainFrame,
				PreferredWidth   = 20,
				PreferredHeight  = 20,
				AutoToggle       = false,
				Anchor           = AnchorStyles.TopLeft,
				Margins          = new Margins (3, 0, 1, 0),
			};

			ToolTip.Default.SetToolTip (this.filterEnableButton, "Active ou désactive le filtre temporel");

			this.filterEnableButton.Clicked += delegate
			{
				this.data.Enable = !this.data.Enable;
				this.UpdateButtons ();
				this.searchStartAction ();
			};
		}



		public void UpdateContent()
		{
			this.UpdateButtons ();
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
		}


		public void SetFilterCount(int dataCount, int count, int allCount)
		{
		}


		private void LevelChangedAction()
		{
			this.UpdateButtons ();
		}

		private void ClearAction()
		{
			this.data.Clear ();
			this.UpdateButtons ();
			this.searchStartAction ();
		}


		private void UpdateButtons()
		{
			this.editionFrame.Visibility =  this.EditionEnable;
			this.staticDates.Visibility  = !this.EditionEnable;
			this.prevButton.Visibility   = !this.EditionEnable;
			this.nextButton.Visibility   = !this.EditionEnable;

			this.topPanelRightController.ClearEnable = !this.data.IsEmpty;
			this.filterEnableButton.ActiveState = this.data.Enable ? ActiveState.Yes : ActiveState.No;
			this.durationField.FormattedText = TopTempoController.TempoDataDurationToString (this.data.Duration);

			this.beginDateController.EditionData.Text = Converters.DateToString (this.data.BeginDate);
			this.beginDateController.EditionDataToWidget ();
			this.beginDateController.Validate ();

			this.endDateController.EditionData.Text = Converters.DateToString (this.data.EndDate);
			this.endDateController.EditionDataToWidget ();
			this.endDateController.Validate ();

			this.staticDates.FormattedText = string.Format ("Du <b>{0}</b> au <b>{1}</b>", Converters.DateToString (this.data.BeginDate), Converters.DateToString (this.data.EndDate));
		}


		private void InitDefaultDates()
		{
			this.data.InitDefaultDates (this.controller.MainWindowController.Période);
		}

		private bool EditionEnable
		{
			get
			{
				return this.data.Duration == TempoDataDuration.Other;
			}
		}


		private static void InitTempoDataDurationCombo(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var type in TopTempoController.TempoDataDurations)
			{
				combo.Items.Add (TopTempoController.TempoDataDurationToString (type));
			}
		}

		private static TempoDataDuration TempoDataDurationToType(FormattedText text)
		{
			foreach (var type in TopTempoController.TempoDataDurations)
			{
				if (TopTempoController.TempoDataDurationToString (type) == text)
				{
					return type;
				}
			}

			return TempoDataDuration.Unknown;
		}

		private static FormattedText TempoDataDurationToString(TempoDataDuration duration)
		{
			//	Texte affiché après "Durée".
			switch (duration)
			{
				case TempoDataDuration.Daily:
					return "Journalière";

				case TempoDataDuration.Weekly:
					return "Hebdomadaire";

				case TempoDataDuration.Monthly:
					return "Mensuelle";

				case TempoDataDuration.Quarterly:
					return "Trimestrielle";

				case TempoDataDuration.Biannual:
					return "Semestrielle";

				case TempoDataDuration.Annual:
					return "Annuelle";

				case TempoDataDuration.Other:
					return "Quelconque";

				default:
					return "?";
			}
		}

		private static IEnumerable<TempoDataDuration> TempoDataDurations
		{
			get
			{
				yield return TempoDataDuration.Other;
				yield return TempoDataDuration.Daily;
				yield return TempoDataDuration.Weekly;
				yield return TempoDataDuration.Monthly;
				yield return TempoDataDuration.Quarterly;
				yield return TempoDataDuration.Biannual;
				yield return TempoDataDuration.Annual;
			}
		}


		private static readonly double					toolbarHeight = 20;

		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly TempoData						data;

		private System.Action							searchStartAction;
		private TopPanelLeftController					topPanelLeftController;
		private TopPanelRightController					topPanelRightController;
		private FrameBox								toolbar;
		private FrameBox								mainFrame;
		private FrameBox								editionFrame;
		private CheckButton								filterEnableButton;
		private TextFieldCombo							durationField;
		private GlyphButton								prevButton;
		private GlyphButton								nextButton;
		private DateFieldController						beginDateController;
		private DateFieldController						endDateController;
		private StaticText								staticDates;
		private bool									showPanel;
	}
}
