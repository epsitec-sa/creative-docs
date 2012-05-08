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

			var periodLabel = new StaticText
			{
				Parent          = this.mainFrame,
				FormattedText   = "Durée",
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			periodLabel.PreferredWidth = periodLabel.GetBestFitSize ().Width;

			this.periodField = new TextFieldCombo
			{
				Parent          = this.mainFrame,
				PreferredWidth  = 100,
				PreferredHeight = 20,
				MenuButtonWidth = UIBuilder.ComboButtonWidth,
				IsReadOnly      = true,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			TopTempoController.InitTempoDataPeriodCombo (this.periodField);

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

			{
				var label = new StaticText
				{
					Parent         = this.mainFrame,
					Text           = "Du",
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 10, 0, 0),
				};

				label.PreferredWidth = label.GetBestFitSize ().Width;

				var initialDate = Converters.DateToString (this.data.BeginDate);
				this.beginDateController = UIBuilder.CreateDateField (this.controller, this.mainFrame, initialDate, "Date initiale incluse", this.ValidateDate, this.DateChanged);
				this.beginDateController.Box.Dock = DockStyle.Left;
			}

			{
				var label = new StaticText
				{
					Parent         = this.mainFrame,
					Text           = "Au",
					Dock           = DockStyle.Left,
					Margins        = new Margins (10, 10, 0, 0),
				};

				label.PreferredWidth = label.GetBestFitSize ().Width;

				var initialDate = Converters.DateToString (this.data.EndDate);
				this.endDateController = UIBuilder.CreateDateField (this.controller, this.mainFrame, initialDate, "Date finale incluse", this.ValidateDate, this.DateChanged);
				this.endDateController.Box.Dock = DockStyle.Left;
			}

			this.periodField.SelectedItemChanged += delegate
			{
				this.data.Period = TopTempoController.TempoDataPeriodToType (this.periodField.FormattedText);
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
			this.topPanelRightController.ClearEnable = !this.data.IsEmpty;
			this.filterEnableButton.ActiveState = this.data.Enable ? ActiveState.Yes : ActiveState.No;
			this.periodField.FormattedText = TopTempoController.TempoDataPeriodToString (this.data.Period);

			this.beginDateController.EditionData.Text = Converters.DateToString (this.data.BeginDate);
			this.endDateController.EditionData.Text = Converters.DateToString (this.data.EndDate);
			this.beginDateController.EditionDataToWidget ();
			this.endDateController.EditionDataToWidget ();
		}


		private static void InitTempoDataPeriodCombo(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var type in TopTempoController.TempoDataPeriods)
			{
				combo.Items.Add (TopTempoController.TempoDataPeriodToString (type));
			}
		}

		private static TempoDataPeriod TempoDataPeriodToType(FormattedText text)
		{
			foreach (var type in TopTempoController.TempoDataPeriods)
			{
				if (TopTempoController.TempoDataPeriodToString (type) == text)
				{
					return type;
				}
			}

			return TempoDataPeriod.Unknown;
		}

		private static  FormattedText TempoDataPeriodToString(TempoDataPeriod period)
		{
			switch (period)
			{
				case TempoDataPeriod.Daily:
					return "Journalière";

				case TempoDataPeriod.Weekly:
					return "Hebdomadaire";

				case TempoDataPeriod.Monthly:
					return "Mensuelle";

				case TempoDataPeriod.Quarterly:
					return "Trimestrielle";

				case TempoDataPeriod.Biannual:
					return "Semestrielle";

				case TempoDataPeriod.Annual:
					return "Annuelle";

				case TempoDataPeriod.Other:
					return "Autre";

				default:
					return "?";
			}
		}

		private static IEnumerable<TempoDataPeriod> TempoDataPeriods
		{
			get
			{
				yield return TempoDataPeriod.Daily;
				yield return TempoDataPeriod.Weekly;
				yield return TempoDataPeriod.Monthly;
				yield return TempoDataPeriod.Quarterly;
				yield return TempoDataPeriod.Biannual;
				yield return TempoDataPeriod.Annual;
				yield return TempoDataPeriod.Other;
			}
		}


		private static readonly double					toolbarHeight = 20;

		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly TempoData						data;

		private System.Action							searchStartAction;
		private FrameBox								toolbar;
		private FrameBox								mainFrame;
		private TopPanelLeftController					topPanelLeftController;
		private TopPanelRightController					topPanelRightController;
		private CheckButton								filterEnableButton;
		private TextFieldCombo							periodField;
		private GlyphButton								prevButton;
		private GlyphButton								nextButton;
		private DateFieldController						beginDateController;
		private DateFieldController						endDateController;
		private bool									showPanel;
	}
}
