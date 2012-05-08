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
	public class TopTemporalController
	{
		public TopTemporalController(AbstractController controller)
		{
			this.controller = controller;

			this.compta          = this.controller.ComptaEntity;
			this.dataAccessor    = this.controller.DataAccessor;
			this.businessContext = this.controller.BusinessContext;
			this.data            = this.controller.DataAccessor.TemporalData;
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
				PreferredHeight = TopTemporalController.toolbarHeight,
				DrawFullFrame   = true,
				BackColor       = UIBuilder.TemporalBackColor,
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
			this.topPanelLeftController.CreateUI (topPanelLeftFrame, false, "Panel.Temporal", this.LevelChangedAction);
			this.topPanelLeftController.Specialist = false;

			//	Remplissage de la frame droite.
			this.topPanelRightController = new TopPanelRightController (this.controller);
			this.topPanelRightController.CreateUI (topPanelRightFrame, "Termine le filtre", this.ClearAction, this.controller.MainWindowController.ClosePanelTemporal, this.LevelChangedAction);

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
				PreferredWidth  = 180,
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

			this.editionInfo = new StaticText
			{
				Parent         = this.editionFrame,
				TextBreakMode  = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				PreferredWidth = 60,
				Dock           = DockStyle.Left,
				Margins        = new Margins (10, 0, 0, 0),
			};

			this.prevButton = new GlyphButton
			{
				Parent          = this.mainFrame,
				GlyphShape      = GlyphShape.ArrowLeft,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 1, 0, 0),
			};

			this.nowButton = new Button
			{
				Parent          = this.mainFrame,
				FormattedText   = "Auj.",
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 1, 0, 0),
			};
			this.nowButton.PreferredWidth = this.nowButton.GetBestFitSize ().Width;

			this.nextButton = new GlyphButton
			{
				Parent          = this.mainFrame,
				GlyphShape      = GlyphShape.ArrowRight,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 1, 0, 0),
			};

			this.menuButton = new GlyphButton
			{
				Parent          = this.mainFrame,
				GlyphShape      = GlyphShape.Menu,
				ButtonStyle     = ButtonStyle.Icon,
				AutoFocus       = false,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 20, 0, 0),
			};

			ToolTip.Default.SetToolTip (this.prevButton, "Période précédente");
			ToolTip.Default.SetToolTip (this.nowButton,  "Période incluant aujourd'hui");
			ToolTip.Default.SetToolTip (this.nextButton, "Période suivante");
			ToolTip.Default.SetToolTip (this.menuButton, "Choix d'une autre période...");

			var durationLabel = new StaticText
			{
				Parent          = this.mainFrame,
				FormattedText   = "Durée",
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
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

			TopTemporalController.InitTempoDataDurationCombo (this.durationField);

			this.errorInfo = new StaticText
			{
				Parent         = this.mainFrame,
				TextBreakMode  = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
				Dock           = DockStyle.Fill,
				Margins        = new Margins (10, 0, 0, 0),
			};

			//	Connexion des événements.
			this.durationField.SelectedItemChanged += delegate
			{
				this.data.Duration = TopTemporalController.TempoDataDurationToType (this.durationField.FormattedText);
				this.data.InitDefaultDates (this.data.BeginDate);
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.prevButton.Clicked += delegate
			{
				this.data.Next (-1);
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.nextButton.Clicked += delegate
			{
				this.data.Next (1);
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.nowButton.Clicked += delegate
			{
				this.data.InitDefaultDates (Date.Today);
				this.UpdateButtons ();
				this.searchStartAction ();
			};

			this.menuButton.Clicked += delegate
			{
				this.ShowMenu (this.menuButton);
			};
		}

		private void ValidateDate(EditionData data)
		{
			Validators.ValidateDate (data, emptyAccepted: true);
		}

		private void DateChanged(int line, ColumnType columnType)
		{
			this.data.BeginDate = Converters.ParseDate (this.beginDateController.EditionData.Text);
			this.data.EndDate   = Converters.ParseDate (this.endDateController.EditionData.Text);

			this.UpdateInfos ();
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
			this.nowButton.Visibility    = !this.EditionEnable;
			this.nextButton.Visibility   = !this.EditionEnable;
			this.menuButton.Visibility   = !this.EditionEnable;

			this.topPanelRightController.ClearEnable = !this.data.IsEmpty;
			this.filterEnableButton.ActiveState = this.data.Enable ? ActiveState.Yes : ActiveState.No;
			this.durationField.FormattedText = TopTemporalController.TempoDataDurationToString (this.data.Duration);

			this.beginDateController.EditionData.Text = Converters.DateToString (this.data.BeginDate);
			this.beginDateController.EditionDataToWidget ();
			this.beginDateController.Validate ();

			this.endDateController.EditionData.Text = Converters.DateToString (this.data.EndDate);
			this.endDateController.EditionDataToWidget ();
			this.endDateController.Validate ();

			this.nowButton.Enable = !Dates.DateInRange (Date.Today, this.data.BeginDate, this.data.EndDate);

			this.UpdateInfos ();
		}

		private void UpdateInfos()
		{
			this.staticDates.FormattedText = FormattedText.Concat ("Période ", Dates.GetDescription (this.data.BeginDate, this.data.EndDate)).ApplyBold ();
			this.editionInfo.FormattedText = this.NumberOfDays;
			this.errorInfo.FormattedText = this.ErrorDescription;
		}

		private FormattedText NumberOfDays
		{
			get
			{
				if (this.data.BeginDate.HasValue && this.data.EndDate.HasValue)
				{
					int n = Dates.NumberOfDays (this.data.EndDate.Value, this.data.BeginDate.Value) + 1;

					if (n <= 0)
					{
						return "(0 jour)";
					}
					else if (n == 1)
					{
						return "(1 jour)";
					}
					else
					{
						return string.Format ("({0} jours)", n.ToString ());
					}
				}
				else
				{
					return FormattedText.Empty;
				}
			}
		}

		private FormattedText ErrorDescription
		{
			get
			{
				var période = this.controller.MainWindowController.Période;

				if (Dates.DateInRange (this.data.BeginDate, période.DateDébut, période.DateFin) &&
					Dates.DateInRange (this.data.EndDate,   période.DateDébut, période.DateFin))
				{
					return FormattedText.Empty;
				}
				else
				{
					return FormattedText.Concat ("Attention, la période choisie déborde de la période comptable.").ApplyItalic ();
				}
			}
		}


		private bool EditionEnable
		{
			get
			{
				return this.data.Duration == TemporalDataDuration.Other;
			}
		}


		private static void InitTempoDataDurationCombo(TextFieldCombo combo)
		{
			combo.Items.Clear ();

			foreach (var type in TopTemporalController.TempoDataDurations)
			{
				combo.Items.Add (TopTemporalController.TempoDataDurationToString (type));
			}
		}

		private static TemporalDataDuration TempoDataDurationToType(FormattedText text)
		{
			foreach (var type in TopTemporalController.TempoDataDurations)
			{
				if (TopTemporalController.TempoDataDurationToString (type) == text)
				{
					return type;
				}
			}

			return TemporalDataDuration.Unknown;
		}

		private static FormattedText TempoDataDurationToString(TemporalDataDuration duration)
		{
			//	Texte affiché après "Durée".
			switch (duration)
			{
				case TemporalDataDuration.Daily:
					return "Journalière";

				case TemporalDataDuration.Weekly:
					return "Hebdomadaire";

				case TemporalDataDuration.Monthly:
					return "Mensuelle";

				case TemporalDataDuration.Quarterly:
					return "Trimestrielle";

				case TemporalDataDuration.Biannual:
					return "Semestrielle";

				case TemporalDataDuration.Annual:
					return "Annuelle";

				case TemporalDataDuration.Other:
					return "Quelconque";

				default:
					return "?";
			}
		}

		private static IEnumerable<TemporalDataDuration> TempoDataDurations
		{
			get
			{
				yield return TemporalDataDuration.Other;
				yield return TemporalDataDuration.Daily;
				yield return TemporalDataDuration.Weekly;
				yield return TemporalDataDuration.Monthly;
				yield return TemporalDataDuration.Quarterly;
				yield return TemporalDataDuration.Biannual;
				yield return TemporalDataDuration.Annual;
			}
		}


		private void ShowMenu(Widget parentButton)
		{
			//	Affiche le menu permettant de choisir la période.
			var menu = new VMenu ();

			var périodes = this.Périodes.ToArray ();

			int first = 0;
			int count = périodes.Length;
			int max = 20;

			if (count > max)  // menu trop long ?
			{
				int sel = -1;

				for (int i = 0; i < périodes.Length; i++)
				{
					var période = périodes[i];
					bool select = période.DateDébut == this.data.BeginDate;

					if (select)
					{
						sel = i;
						break;
					}
				}

				if (sel == -1)
				{
					if (this.data.BeginDate > périodes.First ().DateDébut)
					{
						first = périodes.Length-max;
					}
				}
				else
				{
					first = System.Math.Min (sel+max/2, périodes.Length-1);
					first = System.Math.Max (first-max+1, 0);
				}

				count = System.Math.Min (périodes.Length - first, max);
			}

			for (int i = first; i < first+count; i++)
			{
				var période = périodes[i];
				bool select = période.DateDébut == this.data.BeginDate;

				var item = new MenuItem ()
				{
					IconUri       = UIBuilder.GetRadioStateIconUri (select),
					FormattedText = this.GetPériodeDescription (période),
					TabIndex      = i,
				};

				item.Clicked += delegate
				{
					this.data.BeginDate = périodes[item.TabIndex].DateDébut;
					this.data.EndDate   = périodes[item.TabIndex].DateFin;
					this.UpdateButtons ();
					this.searchStartAction ();
				};

				menu.Items.Add (item);
			}

			if (menu.Items.Any ())
			{
				TextFieldCombo.AdjustComboSize (parentButton, menu, false);

				menu.Host = parentButton.Window;
				menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
			}
		}


		private FormattedText GetPériodeDescription(Période période)
		{
			var desc = Dates.GetDescription (période.DateDébut, période.DateFin);
			var rank = FormattedText.Empty;

			if (this.data.Duration == TemporalDataDuration.Monthly)
			{
				rank = période.DateDébut.Month.ToString ("00");  // 01..12
			}
			else if (this.data.Duration == TemporalDataDuration.Quarterly)
			{
				rank = ((période.DateDébut.Month-1)/3+1).ToString ("0");  // 1..4
			}
			else if (this.data.Duration == TemporalDataDuration.Biannual)
			{
				rank = ((période.DateDébut.Month-1)/6+1).ToString ("0");  // 1..2
			}
			else if (this.data.Duration == TemporalDataDuration.Weekly)
			{
				rank = Dates.GetWeekNumber (période.DateDébut).ToString ("00");  // 01..52
			}

			if (!rank.IsNullOrEmpty)
			{
				desc = FormattedText.Concat (rank.ApplyBold (), ": ", desc);
			}

			return desc;
		}

		private IEnumerable<Période> Périodes
		{
			get
			{
				var période = this.controller.MainWindowController.Période;

				var temp = new TemporalData ();
				this.data.CopyTo (temp);
				temp.BeginDate = période.DateDébut;
				temp.InitDefaultDates (temp.BeginDate);

				do
				{
					yield return new Période (temp.BeginDate.Value, temp.EndDate.Value);

					temp.BeginDate = Dates.AddDays (temp.EndDate.Value, 1);
					temp.InitDefaultDates (temp.BeginDate);
				}
				while (temp.BeginDate <= période.DateFin);
			}
		}

		private class Période
		{
			public Période(Date dateDébut, Date dateFin)
			{
				this.DateDébut = dateDébut;
				this.DateFin   = dateFin;
			}

			public Date DateDébut
			{
				get;
				private set;
			}

			public Date DateFin
			{
				get;
				private set;
			}
		}


		private static readonly double					toolbarHeight = 20;

		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly BusinessContext				businessContext;
		private readonly AbstractDataAccessor			dataAccessor;
		private readonly TemporalData					data;

		private System.Action							searchStartAction;
		private TopPanelLeftController					topPanelLeftController;
		private TopPanelRightController					topPanelRightController;
		private FrameBox								toolbar;
		private FrameBox								mainFrame;
		private FrameBox								editionFrame;
		private CheckButton								filterEnableButton;
		private TextFieldCombo							durationField;
		private GlyphButton								prevButton;
		private Button									nowButton;
		private GlyphButton								nextButton;
		private GlyphButton								menuButton;
		private DateFieldController						beginDateController;
		private DateFieldController						endDateController;
		private StaticText								staticDates;
		private StaticText								editionInfo;
		private StaticText								errorInfo;
		private bool									showPanel;
	}
}
