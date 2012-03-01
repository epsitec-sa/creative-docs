//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

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
	public class SearchController
	{
		public SearchController(AbstractController controller, SearchData data, bool isFilter)
		{
			this.controller = controller;
			this.data       = data;
			this.isFilter   = isFilter;

			this.ignoreChanges = new SafeCounter ();
			this.compta        = this.controller.ComptaEntity;
			this.columnMappers = this.controller.ColumnMappers;

			this.nodeControllers = new List<SearchNodeController> ();
		}


		public FrameBox CreateUI(FrameBox parent, System.Action searchStartAction, System.Action<int> searchNextAction)
		{
			this.searchStartAction = searchStartAction;
			this.searchNextAction  = searchNextAction;

			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.middleFrame = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredHeight     = 20,
				Dock                = DockStyle.Fill,
				Padding             = new Margins (0, 0, 0, 0),
			};

			var levelFrame = new FrameBox
			{
				Parent          = frame,
				DrawFullFrame   = true,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Padding         = new Margins (5, 5, 5, 5),
			};

			var rightFrame = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
				Padding         = new Margins (5, 0, 5, 5),
			};

			this.CreateRightUI  (rightFrame);
			this.CreateMiddleUI ();
			this.CreateLevelUI (levelFrame);

			this.UpdateButtons ();

			return frame;
		}


		public bool Specialist
		{
			get
			{
				return this.data.Specialist;
			}
			set
			{
				if (this.data.Specialist != value)
				{
					this.data.Specialist = value;
					this.levelController.Specialist = value;
					this.data.BeginnerAdjust (this.isFilter);

					this.CreateMiddleUI ();
					this.UpdateButtons ();
				}
			}
		}


		public void UpdateContent()
		{
			this.CreateMiddleUI ();
			this.UpdateButtons ();
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			foreach (var controller in this.nodeControllers)
			{
				controller.UpdateColumns ();
			}
		}


		private void CreateMiddleUI()
		{
			this.middleFrame.Children.Clear ();
			this.nodeControllers.Clear ();

			if (this.data.Specialist)
			{
				this.CreateMiddleSpecialistUI ();
			}
			else
			{
				this.CreateMiddleBeginnerUI ();
			}
		}

		private void CreateMiddleBeginnerUI()
		{
			this.beginnerFrame = new FrameBox
			{
				Parent              = this.middleFrame,
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Fill,
				Padding             = new Margins (5),
			};

			{
				var frame = new FrameBox
				{
					Parent          = this.beginnerFrame,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
				};

				new StaticText
				{
					Parent           = frame,
					Text             = this.isFilter ? "Filtrer" : "Rechercher",
					TextBreakMode    = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					ContentAlignment = ContentAlignment.MiddleRight,
					PreferredWidth   = UIBuilder.LeftLabelWidth-10,
					PreferredHeight  = 20,
					Dock             = DockStyle.Top,
					Margins          = new Margins (0, 10, 0, 0),
				};
			}

			if (this.isFilter)
			{
				if (this.columnMappers.Where (x => x.Column == ColumnType.Catégorie).Any ())
				{
					this.CreateMiddleBeginnerCatégorieUI ();
				}

				if (this.columnMappers.Where (x => x.Column == ColumnType.Profondeur).Any ())
				{
					this.CreateMiddleBeginnerProfondeurUI ();
				}

				if (this.columnMappers.Where (x => x.Column == ColumnType.Solde).Any ())
				{
					this.CreateMiddleBeginnerSoldeUI ();
				}

				if (this.columnMappers.Where (x => x.Column == ColumnType.Date).Any ())
				{
					this.CreateMiddleBeginnerDatesUI ();
				}
			}
			else
			{
				this.CreateMiddleBeginnerSearchUI ();
			}
		}


		private void CreateMiddleBeginnerCatégorieUI()
		{
			var frame = new GroupBox
			{
				Parent          = this.beginnerFrame,
				Text            = "Catégories",
				PreferredHeight = 65,  // pour aider le layout !
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
				Padding         = new Margins (5, 5, 2, 2),
			};

			int buttonWidth = 60;

			var frame1 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 16,
				Dock            = DockStyle.Top,
			};

			var frame2 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 16,
				Dock            = DockStyle.Top,
			};

			var frame3 = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 16,
				Dock            = DockStyle.Top,
			};

			var catégorie = this.data.BeginnerCatégories;

			{
				this.beginnerCatégorieActif = new CheckButton
				{
					Parent          = frame1,
					Text            = Converters.CatégorieToString (CatégorieDeCompte.Actif),
					ActiveState     = ((catégorie & CatégorieDeCompte.Actif) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth  = buttonWidth,
					PreferredHeight = 16,
					Dock            = DockStyle.Left,
				};

				this.beginnerCatégorieCharge = new CheckButton
				{
					Parent          = frame1,
					Text            = Converters.CatégorieToString (CatégorieDeCompte.Charge),
					ActiveState     = ((catégorie & CatégorieDeCompte.Charge) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth  = buttonWidth,
					PreferredHeight = 16,
					Dock            = DockStyle.Left,
				};
			}

			{
				this.beginnerCatégoriePassif = new CheckButton
				{
					Parent          = frame2,
					Text            = Converters.CatégorieToString (CatégorieDeCompte.Passif),
					ActiveState     = ((catégorie & CatégorieDeCompte.Passif) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth  = buttonWidth,
					PreferredHeight = 16,
					Dock            = DockStyle.Left,
				};

				this.beginnerCatégorieProduit = new CheckButton
				{
					Parent          = frame2,
					Text            = Converters.CatégorieToString (CatégorieDeCompte.Produit),
					ActiveState     = ((catégorie & CatégorieDeCompte.Produit) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth  = buttonWidth,
					Dock            = DockStyle.Left,
				};
			}

			{
				this.beginnerCatégorieExploitation = new CheckButton
				{
					Parent          = frame3,
					Text            = Converters.CatégorieToString (CatégorieDeCompte.Exploitation),
					ActiveState     = ((catégorie & CatégorieDeCompte.Exploitation) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth  = buttonWidth+40,
					PreferredHeight = 16,
					Dock            = DockStyle.Left,
				};
			}

			this.beginnerCatégorieActif       .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.beginnerCatégoriePassif      .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.beginnerCatégorieCharge      .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.beginnerCatégorieProduit     .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.beginnerCatégorieExploitation.ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
		}

		private void HandleCheckButtonCatégorie(object sender)
		{
			var catégorie = CatégorieDeCompte.Inconnu;

			if (this.beginnerCatégorieActif.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Actif;
			}

			if (this.beginnerCatégoriePassif.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Passif;
			}

			if (this.beginnerCatégorieCharge.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Charge;
			}

			if (this.beginnerCatégorieProduit.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Produit;
			}

			if (this.beginnerCatégorieExploitation.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Exploitation;
			}

			this.data.BeginnerCatégories = catégorie;
			this.searchStartAction ();
		}


		private void CreateMiddleBeginnerProfondeurUI()
		{
			var frame = new GroupBox
			{
				Parent          = this.beginnerFrame,
				Text            = "Profondeur",
				PreferredWidth  = 20+50,  // pour aider le layout !
				PreferredHeight = 65,  // pour aider le layout !
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
				Padding         = new Margins (5, 5, 2, 2),
			};

			var frame1 = new FrameBox
			{
				Parent         = frame,
				PreferredWidth = 20+50,  // pour aider le layout !
				Dock           = DockStyle.Top,
				Margins        = new Margins (0, 0, 0, 1),
			};

			var frame2 = new FrameBox
			{
				Parent         = frame,
				PreferredWidth = 20+50,  // pour aider le layout !
				Dock           = DockStyle.Top,
			};

			{
				new StaticText
				{
					Parent         = frame1,
					Text           = "De",
					PreferredWidth = 20,
					Dock           = DockStyle.Left,
				};

				this.beginnerFromProfondeurField = new TextFieldCombo
				{
					Parent          = frame1,
					IsReadOnly      = true,
					PreferredWidth  = 50,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					TabIndex        = 1,
				};
			}

			{
				new StaticText
				{
					Parent         = frame2,
					Text           = "À",
					PreferredWidth = 20,
					Dock           = DockStyle.Left,
				};

				this.beginnerToProfondeurField = new TextFieldCombo
				{
					Parent          = frame2,
					IsReadOnly      = true,
					PreferredWidth  = 50,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					TabIndex        = 2,
				};
			}

			this.InitializeProfondeurs ();

			int from, to;
			this.data.GetBeginnerProfondeurs (out from, out to);

			using (this.ignoreChanges.Enter ())
			{
				this.beginnerFromProfondeurField.FormattedText = this.ProfondeurToDescription (from);
				this.beginnerToProfondeurField.FormattedText   = this.ProfondeurToDescription (to);
			}

			this.beginnerFromProfondeurField.TextChanged += delegate
			{
				this.UpdateProfondeur ();
			};

			this.beginnerToProfondeurField.TextChanged += delegate
			{
				this.UpdateProfondeur ();
			};
		}

		private void InitializeProfondeurs()
		{
			int from, to;
			this.data.GetBeginnerProfondeurs (out from, out to);

			this.InitializeProfondeur (this.beginnerFromProfondeurField, 1, to);
			this.InitializeProfondeur (this.beginnerToProfondeurField, from, int.MaxValue);
		}

		private void InitializeProfondeur(TextFieldCombo combo, int min, int max)
		{
			combo.Items.Clear ();

			for (int i = 1; i <= 6; i++)
			{
				if (i >= min && i <= max)
				{
					combo.Items.Add (this.ProfondeurToDescription (i));  // 1..6
				}
			}

			if (max == int.MaxValue)
			{
				combo.Items.Add (this.ProfondeurToDescription (int.MaxValue));  // Tout
			}
		}

		private void UpdateProfondeur()
		{
			if (this.ignoreChanges.IsZero)
			{
				var from = this.DescriptionToProfondeur (this.beginnerFromProfondeurField.FormattedText);
				var to   = this.DescriptionToProfondeur (this.beginnerToProfondeurField.FormattedText);
				this.data.SetBeginnerProfondeurs (from, to);

				this.InitializeProfondeurs ();

				this.searchStartAction ();
			}
		}

		private FormattedText ProfondeurToDescription(int profondeur)
		{
			if (profondeur == int.MaxValue)
			{
				return "Tout";
			}
			else
			{
				return profondeur.ToString ();  // 1..9
			}
		}

		private int DescriptionToProfondeur(FormattedText text)
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


		private void CreateMiddleBeginnerSoldeUI()
		{
			var frame = new GroupBox
			{
				Parent          = this.beginnerFrame,
				Text            = "Soldes",
				PreferredHeight = 65,  // pour aider le layout !
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
				Padding         = new Margins (5, 5, 2, 2),
			};

			var button = new CheckButton
			{
				Parent      = frame,
				Text        = "Soldes nuls",
				ActiveState = this.data.BeginnerSoldesNuls ? ActiveState.Yes : ActiveState.No,
				Dock        = DockStyle.Top,
			};

			button.ActiveStateChanged += delegate
			{
				this.data.BeginnerSoldesNuls = (button.ActiveState == ActiveState.Yes);

				this.searchStartAction ();
			};
		}


		private void CreateMiddleBeginnerDatesUI()
		{
			var frame = new GroupBox
			{
				Parent          = this.beginnerFrame,
				Text            = "Période",
				PreferredWidth  = 100,  // pour aider le layout !
				PreferredHeight = 65,  // pour aider le layout !
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
				Padding         = new Margins (5, 5, 2, 2),
			};

			var frame1 = new FrameBox
			{
				Parent  = frame,
				Dock    = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 1),
			};

			var frame2 = new FrameBox
			{
				Parent = frame,
				Dock   = DockStyle.Top,
			};

			Date? beginDate, endDate;
			this.data.GetBeginnerDates (out beginDate, out endDate);

			{
				new StaticText
				{
					Parent         = frame1,
					Text           = "Du",
					PreferredWidth = 20,
					Dock           = DockStyle.Left,
				};

				var initialDate = beginDate.HasValue ? beginDate.Value.ToString () : null;
				this.beginnerBeginDateController = UIBuilder.CreateDateField (this.controller, frame1, initialDate, "Date initiale incluse", this.BeginnerValidateDate, this.BeginnerDateChanged);
			}

			{
				new StaticText
				{
					Parent         = frame2,
					Text           = "Au",
					PreferredWidth = 20,
					Dock           = DockStyle.Left,
				};

				var initialDate = endDate.HasValue ? endDate.Value.ToString () : null;
				this.beginnerEndDateController = UIBuilder.CreateDateField (this.controller, frame2, initialDate, "Date finale incluse", this.BeginnerValidateDate, this.BeginnerDateChanged);
			}
		}

		private void BeginnerValidateDate(EditionData data)
		{
			Validators.ValidateDate (this.controller.MainWindowController.Période, data, emptyAccepted: true);
		}

		private void BeginnerDateChanged()
		{
			Date? beginDate = Converters.ParseDate (this.beginnerBeginDateController.EditionData.Text);
			Date? endDate   = Converters.ParseDate (this.beginnerEndDateController.EditionData.Text);
			data.SetBeginnerDates (beginDate, endDate);

			this.searchStartAction ();
		}


		private void CreateMiddleSpecialistUI()
		{
			this.beginnerSearchField         = null;
			this.beginnerBeginDateController = null;
			this.beginnerEndDateController   = null;

			int count = this.data.NodesData.Count;
			for (int i = 0; i < count; i++)
			{
				var controller = new SearchNodeController (this.controller, this.data.NodesData[i], this.isFilter);

				var frame = controller.CreateUI (this.middleFrame, this.searchStartAction, this.AddRemoveAction, this.SwapNodeAction);
				controller.SetAddAction (i, count < 10);

				frame.TabIndex = i+1;
				frame.Margins = new Margins (0, 0, 0, (count > 1 && i < count-1) ? -1 : 0);

				this.nodeControllers.Add (controller);
			}
		}


		private void CreateRightUI(FrameBox parent)
		{
			var footer = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
			};

			{
				this.buttonPrev = new GlyphButton
				{
					Parent          = footer,
					GlyphShape      = Common.Widgets.GlyphShape.TriangleLeft,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (0, 0, 0, 0),
					Visibility      = !this.isFilter,
				};

				this.buttonNext = new GlyphButton
				{
					Parent          = footer,
					GlyphShape      = Common.Widgets.GlyphShape.TriangleRight,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (-1, 10, 0, 0),
					Visibility      = !this.isFilter,
				};

				new FrameBox
				{
					Parent          = footer,
					PreferredWidth  = 20+20-1+10,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Visibility      = this.isFilter,
				};

				this.resultLabel = new StaticText
				{
					Parent          = footer,
					TextBreakMode   = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine,
					PreferredWidth  = 110,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 0, 0, 0),
				};

				this.buttonPrev.Clicked += delegate
				{
					this.searchNextAction (-1);
				};

				this.buttonNext.Clicked += delegate
				{
					this.searchNextAction (1);
				};

				ToolTip.Default.SetToolTip (this.buttonPrev, "Cherche en arrière");
				ToolTip.Default.SetToolTip (this.buttonNext, "Cherche en avant");
			}
		}

		private void CreateMiddleBeginnerSearchUI()
		{
			this.beginnerSearchField = new TextField
			{
				Parent          = this.beginnerFrame,
				Text            = this.data.BeginnerSearch,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.beginnerSearchField.TextChanged += delegate
			{
				this.data.BeginnerSearch = this.beginnerSearchField.Text;
				this.searchStartAction ();
			};

			ToolTip.Default.SetToolTip (this.beginnerSearchField, "Texte cherché n'importe où");
		}


		private void CreateLevelUI(FrameBox parent)
		{
			this.levelController = new LevelController (this.controller);
			this.levelController.CreateUI (parent, isFilter ? "Termine le filtre" : "Termine la recherche", this.ClearAction, this.LevelChangedAction);
			this.levelController.Specialist = this.data.Specialist;
		}

		private void ClearAction()
		{
			this.SearchClear ();
			this.SetFocus ();
		}

		private void LevelChangedAction()
		{
			this.data.Specialist = this.levelController.Specialist;
			this.data.BeginnerAdjust (this.isFilter);

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.searchStartAction ();
		}


		private void UpdateOrMode()
		{
			foreach (var controller in this.nodeControllers)
			{
				controller.OrMode = this.data.OrMode;
			}
		}


		public void SetFocus()
		{
			if (this.data.Specialist)
			{
				if (this.nodeControllers.Count != 0)
				{
					this.nodeControllers[0].SetFocus ();
				}
			}
			else
			{
				if (!this.isFilter && this.beginnerSearchField != null)
				{
					this.beginnerSearchField.Focus ();
				}
			}
		}


		public void SearchClear()
		{
			while (this.data.NodesData.Count > 1)
			{
				this.data.NodesData.RemoveAt (1);
			}

			this.nodeControllers[0].SearchClear ();

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.searchStartAction ();
		}

		public void SetSearchCount(int dataCount, int? count, int? locator)
		{
			this.BigDataInterface = (dataCount >= 1000);  // limite arbitraire au-delà de laquelle les recherches deviennent trop lentes !

			this.UpdateButtons ();

			this.buttonNext.Enable = (count > 1);
			this.buttonPrev.Enable = (count > 1);

			if (!count.HasValue)
			{
				this.resultLabel.Text = null;
			}
			else if (count == 0)
			{
				this.resultLabel.Text = "Aucun résultat trouvé";
			}
			else
			{
				int l = locator.GetValueOrDefault () + 1;
				int c = count.Value;
				this.resultLabel.Text = string.Format ("{0}/{1} resultat{2}", l.ToString (), c.ToString (), (c == 1) ? "" : "s");
			}
		}

		public void SetFilterCount(int dataCount, int count, int allCount)
		{
			this.BigDataInterface = (dataCount >= 1000);  // limite arbitraire au-delà de laquelle les recherches deviennent trop lentes !

			this.UpdateButtons ();

			if (count == allCount)
			{
				this.resultLabel.Text = string.Format ("{0} (tous)", allCount.ToString ());
			}
			else
			{
				this.resultLabel.Text = string.Format ("{0} sur {1}", count.ToString (), allCount.ToString ());
			}
		}


		private void AddRemoveAction(int index)
		{
			if (index == 0)
			{
				this.data.NodesData.Add (new SearchNodeData ());
			}
			else
			{
				this.data.NodesData.RemoveAt (index);
			}

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.searchStartAction ();
		}

		private void SwapNodeAction()
		{
			this.data.OrMode = !this.data.OrMode;
			this.UpdateOrMode ();
			this.searchStartAction ();
		}

		private void UpdateButtons()
		{
			this.levelController.ClearEnable = !this.data.IsEmpty;
		}


		private bool BigDataInterface
		{
			get
			{
				return this.bigDataInterface;
			}
			set
			{
				if (this.bigDataInterface != value)
				{
					this.bigDataInterface = value;

					this.CreateMiddleUI ();
					this.UpdateButtons ();
				}
			}
		}


		private readonly AbstractController				controller;
		private readonly ComptaEntity					compta;
		private readonly List<ColumnMapper>				columnMappers;
		private readonly SearchData						data;
		private readonly List<SearchNodeController>		nodeControllers;
		private readonly bool							isFilter;
		private readonly SafeCounter					ignoreChanges;

		private bool									bigDataInterface;
		private System.Action							searchStartAction;
		private System.Action<int>						searchNextAction;

		private FrameBox								middleFrame;
		private GlyphButton								buttonNext;
		private GlyphButton								buttonPrev;
		private StaticText								resultLabel;
		private FrameBox								beginnerFrame;
		private TextField								beginnerSearchField;
		private CheckButton								beginnerCatégorieActif;
		private CheckButton								beginnerCatégoriePassif;
		private CheckButton								beginnerCatégorieCharge;
		private CheckButton								beginnerCatégorieProduit;
		private CheckButton								beginnerCatégorieExploitation;
		private TextFieldCombo							beginnerFromProfondeurField;
		private TextFieldCombo							beginnerToProfondeurField;
		private DateFieldController						beginnerBeginDateController;
		private DateFieldController						beginnerEndDateController;
		private LevelController							levelController;
	}
}
