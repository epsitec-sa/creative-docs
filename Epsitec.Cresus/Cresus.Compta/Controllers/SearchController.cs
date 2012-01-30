//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Controllers
{
	public class SearchController
	{
		public SearchController(ComptaEntity comptaEntity, SearchData data, List<ColumnMapper> columnMappers, bool isFilter)
		{
			this.comptaEntity  = comptaEntity;
			this.data          = data;
			this.columnMappers = columnMappers;
			this.isFilter      = isFilter;

			this.tabControllers = new List<SearchTabController> ();

			if (this.data.TabsData.Count == 0)
			{
				this.data.TabsData.Add (new SearchTabData (this.comptaEntity));
			}
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

			var leftFrame = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.middleFrame = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
				Margins         = new Margins (0, 20, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent          = frame,
				PreferredHeight = 20,
				Dock            = DockStyle.Right,
			};

			this.CreateLeftUI   (leftFrame);
			this.CreateRightUI  (rightFrame);
			this.CreateMiddleUI ();

			this.UpdateButtons ();

			return frame;
		}


		public void UpdateColumns()
		{
			//	Met à jour les widgets en fonction de la liste des colonnes présentes.
			foreach (var controller in this.tabControllers)
			{
				controller.UpdateColumns ();
			}
		}

	
		private void CreateLeftUI(FrameBox parent)
		{
			var frame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.buttonClear = new GlyphButton
			{
				Parent          = frame,
				GlyphShape      = GlyphShape.Close,
				PreferredSize   = new Size (20, 20),
				Dock            = DockStyle.Left,
				Enable          = false,
				Margins         = new Margins (0, 1, 0, 0),
			};

			this.buttonSpecific = new GlyphButton
			{
				Parent          = frame,
				GlyphShape      = GlyphShape.TriangleRight,
				PreferredSize   = new Size (20, 20),
				Dock            = DockStyle.Left,
				Margins         = new Margins (0, 10, 0, 0),
			};

			new StaticText
			{
				Parent          = frame,
				Text            = this.isFilter ? "Filtrer" : "Rechercher",
				PreferredWidth  = 64,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.buttonClear.Clicked += delegate
			{
				this.SearchClear ();
			};

			this.buttonSpecific.Clicked += delegate
			{
				this.SpecificInvert ();
			};

			ToolTip.Default.SetToolTip (this.buttonClear,    this.isFilter ? "Termine le filtre" : "Termine la recherche");
			ToolTip.Default.SetToolTip (this.buttonSpecific, this.isFilter ? "Filtre standard ou spécial" : "Recherche standard ou spéciale");
		}

		private void CreateMiddleUI()
		{
			this.middleFrame.Children.Clear ();
			this.tabControllers.Clear ();

			if (this.data.Specific)
			{
				this.CreateMiddleSpecificUI ();
			}
			else
			{
				this.CreateMiddleGeneralUI ();
			}
		}

		private void CreateMiddleSpecificUI()
		{
			this.middleFrame.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;

			if (this.isFilter)
			{
				this.CreateMiddleSpecificCatégorieUI ();
				this.CreateMiddleSpecificDatesUI ();
			}
			else
			{
				this.CreateMiddleSpecificSearchUI ();
			}

			this.modeFrame.Visibility = false;
		}

		private void CreateMiddleSpecificCatégorieUI()
		{
			var frame = new GroupBox
			{
				Parent  = this.middleFrame,
				Text    = "Catégories",
				Dock    = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
				Padding = new Margins (5, 5, 2, 2),
			};

			int buttonWidth = 65;

			var frame1 = new FrameBox
			{
				Parent = frame,
				Dock   = DockStyle.Top,
			};

			var frame2 = new FrameBox
			{
				Parent = frame,
				Dock   = DockStyle.Top,
			};

			var catégorie = this.data.Catégorie;

			{
				this.specificCatégorieActif = new CheckButton
				{
					Parent         = frame1,
					Text           = "Actif",
					ActiveState    = ((catégorie & CatégorieDeCompte.Actif) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth = buttonWidth,
					Dock           = DockStyle.Left,
				};

				this.specificCatégorieCharge = new CheckButton
				{
					Parent         = frame1,
					Text           = "Charge",
					ActiveState    = ((catégorie & CatégorieDeCompte.Charge) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth = buttonWidth,
					Dock           = DockStyle.Left,
				};

				this.specificCatégorieExploitation = new CheckButton
				{
					Parent         = frame1,
					Text           = "Exploitation",
					ActiveState    = ((catégorie & CatégorieDeCompte.Exploitation) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth = buttonWidth+20,
					Dock           = DockStyle.Left,
				};
			}

			{
				this.specificCatégoriePassif = new CheckButton
				{
					Parent         = frame2,
					Text           = "Passif",
					ActiveState    = ((catégorie & CatégorieDeCompte.Passif) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth = buttonWidth,
					Dock           = DockStyle.Left,
				};

				this.specificCatégorieProduit = new CheckButton
				{
					Parent         = frame2,
					Text           = "Produit",
					ActiveState    = ((catégorie & CatégorieDeCompte.Produit) != 0) ? ActiveState.Yes : ActiveState.No,
					PreferredWidth = buttonWidth,
					Dock           = DockStyle.Left,
				};
			}

			this.specificCatégorieActif       .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.specificCatégoriePassif      .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.specificCatégorieCharge      .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.specificCatégorieProduit     .ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
			this.specificCatégorieExploitation.ActiveStateChanged += new Common.Support.EventHandler (this.HandleCheckButtonCatégorie);
		}

		private void HandleCheckButtonCatégorie(object sender)
		{
			var catégorie = CatégorieDeCompte.Inconnu;

			if (this.specificCatégorieActif.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Actif;
			}

			if (this.specificCatégoriePassif.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Passif;
			}

			if (this.specificCatégorieCharge.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Charge;
			}

			if (this.specificCatégorieProduit.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Produit;
			}

			if (this.specificCatégorieExploitation.ActiveState == ActiveState.Yes)
			{
				catégorie |= CatégorieDeCompte.Exploitation;
			}

			this.data.Catégorie = catégorie;
			this.searchStartAction ();
		}

		private void CreateMiddleSpecificDatesUI()
		{
			var frame = new GroupBox
			{
				Parent  = this.middleFrame,
				Text    = "Dates",
				Dock    = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
				Padding = new Margins (5, 5, 2, 2),
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
			this.data.GetIntervalDates (out beginDate, out endDate);

			{
				new StaticText
				{
					Parent         = frame1,
					Text           = "Depuis le",
					PreferredWidth = 55,
					Dock           = DockStyle.Left,
				};

				this.specificBeginDateField = new TextField
				{
					Parent          = frame1,
					Text            = beginDate.HasValue ? beginDate.Value.ToString() : null,
					PreferredWidth  = 80,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					TabIndex        = 1,
				};
			}

			{
				new StaticText
				{
					Parent         = frame2,
					Text           = "Jusqu'au",
					PreferredWidth = 55,
					Dock           = DockStyle.Left,
				};

				this.specificEndDateField = new TextField
				{
					Parent          = frame2,
					Text            = endDate.HasValue ? endDate.Value.ToString () : null,
					PreferredWidth  = 80,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					TabIndex        = 2,
				};
			}

			this.specificBeginDateField.TextChanged += delegate
			{
				var data = this.data.GetIntervalDatesData ();
				data.SearchText.FromText = this.specificBeginDateField.Text;
				this.searchStartAction ();
			};

			this.specificEndDateField.TextChanged += delegate
			{
				var data = this.data.GetIntervalDatesData ();
				data.SearchText.ToText = this.specificEndDateField.Text;
				this.searchStartAction ();
			};

			ToolTip.Default.SetToolTip (this.specificBeginDateField, "Date initiale incluse");
			ToolTip.Default.SetToolTip (this.specificEndDateField,   "Date finale incluse");
		}

		private void CreateMiddleGeneralUI()
		{
			this.middleFrame.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;

			this.specificSearchField    = null;
			this.specificBeginDateField = null;
			this.specificEndDateField   = null;

			int count = this.data.TabsData.Count;
			for (int i = 0; i < count; i++)
			{
				var controller = new SearchTabController (this.data.TabsData[i], this.columnMappers, this.isFilter);

				var frame = controller.CreateUI (this.middleFrame, this.bigDataInterface, this.searchStartAction, this.AddRemoveAction);
				controller.Index = i;
				controller.SetAddAction (i == 0, count < 10);

				frame.TabIndex = i+1;
				frame.Margins = new Margins (0, 0, 0, (count > 1 && i < count-1) ? 1 : 0);

				this.tabControllers.Add (controller);
			}

			this.modeFrame.Visibility = (this.data.TabsData.Count > 1);
		}

		private void CreateRightUI(FrameBox parent)
		{
			this.modeFrame = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			var footer = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Bottom,
			};

			{
				this.andButton = new RadioButton
				{
					Parent          = this.modeFrame,
					Text            = "Tous",
					PreferredWidth  = 60,
					ActiveState     = this.data.OrMode ? ActiveState.No : ActiveState.Yes,
					Dock            = DockStyle.Left,
				};

				this.orButton = new RadioButton
				{
					Parent          = this.modeFrame,
					Text            = "Au moins un",
					PreferredWidth  = 90,
					ActiveState     = this.data.OrMode ? ActiveState.Yes : ActiveState.No,
					Dock            = DockStyle.Left,
				};

				this.orButton.ActiveStateChanged += delegate
				{
					if (!this.ignoreChange)
					{
						this.data.OrMode = this.orButton.ActiveState == ActiveState.Yes;
						this.searchStartAction ();
					}
				};

				ToolTip.Default.SetToolTip (this.andButton, this.isFilter ? "Filtre les données qui répondent à tous les critères"   : "Cherche les données qui répondent à tous les critères");
				ToolTip.Default.SetToolTip (this.orButton,  this.isFilter ? "Filtre les données qui répondent à au moins un critère" : "Cherche les données qui répondent à au moins un critère");
			}

			{
				this.buttonPrev = new GlyphButton
				{
					Parent          = footer,
					GlyphShape      = Common.Widgets.GlyphShape.TriangleLeft,
					PreferredWidth  = 30,
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
					PreferredWidth  = 30,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (-1, 10, 0, 0),
					Visibility      = !this.isFilter,
				};

				new FrameBox
				{
					Parent          = footer,
					PreferredWidth  = 30+30-1+10,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Visibility      = this.isFilter,
				};

				this.resultLabel = new StaticText
				{
					Parent          = footer,
					PreferredWidth  = 120,
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

		private void UpdateOrMode()
		{
			this.ignoreChange = true;
			this.andButton.ActiveState = this.data.OrMode ? ActiveState.No  : ActiveState.Yes;
			this.orButton.ActiveState  = this.data.OrMode ? ActiveState.Yes : ActiveState.No;
			this.ignoreChange = false;
		}

		private void CreateMiddleSpecificSearchUI()
		{
			this.specificSearchField = new TextField
			{
				Parent          = this.middleFrame,
				Text            = this.data.TabsData[0].SearchText.FromText,
				PreferredHeight = 20,
				Dock            = DockStyle.Fill,
			};

			this.specificSearchField.TextChanged += delegate
			{
				this.data.TabsData[0].SearchText.FromText = this.specificSearchField.Text;
				this.searchStartAction ();
			};

			ToolTip.Default.SetToolTip (this.specificSearchField, "Texte cherché n'importe où");
		}


		public void SetFocus()
		{
			if (this.data.Specific)
			{
				if (!this.isFilter && this.specificSearchField != null)
				{
					this.specificSearchField.Focus ();
				}
			}
			else
			{
				if (this.tabControllers.Count != 0)
				{
					this.tabControllers[0].SetFocus ();
				}
			}
		}


		public void SearchClear()
		{
			while (this.data.TabsData.Count > 1)
			{
				this.data.TabsData.RemoveAt (1);
			}

			this.data.TabsData[0].Clear ();

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.searchStartAction ();
		}

		private void SpecificInvert()
		{
			this.data.Specific = !this.data.Specific;

			//	Adapte les données.
			{
				var d1 = this.data.GetCatégorieData ();
				var d2 = this.data.GetIntervalDatesData ();

				if (string.IsNullOrEmpty (d1.SearchText.FromText))
				{
					d1 = null;
				}

				if (string.IsNullOrEmpty (d2.SearchText.FromText) && string.IsNullOrEmpty (d2.SearchText.ToText))
				{
					d2 = null;
				}

				this.data.TabsData.Clear ();

				if (d1 != null)
				{
					this.data.TabsData.Add (d1);
				}

				if (d2 != null)
				{
					this.data.TabsData.Add (d2);
				}

				if (this.data.TabsData.Count == 0)
				{
					this.data.TabsData.Add (new SearchTabData (null));
				}

				if (this.data.TabsData.Count > 1)
				{
					this.data.OrMode = false;
				}
			}

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.UpdateOrMode ();
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
				this.data.TabsData.Add (new SearchTabData (this.comptaEntity));
			}
			else
			{
				this.data.TabsData.RemoveAt (index);
			}

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.searchStartAction ();
		}

		private void UpdateButtons()
		{
			this.buttonClear.Enable = !this.data.IsEmpty || this.data.TabsData.Count > 1;
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


		private readonly ComptaEntity					comptaEntity;
		private readonly SearchData						data;
		private readonly List<ColumnMapper>				columnMappers;
		private readonly List<SearchTabController>		tabControllers;
		private readonly bool							isFilter;

		private bool									bigDataInterface;
		private System.Action							searchStartAction;
		private System.Action<int>						searchNextAction;

		private FrameBox								middleFrame;
		private GlyphButton								buttonClear;
		private GlyphButton								buttonSpecific;
		private GlyphButton								buttonNext;
		private GlyphButton								buttonPrev;
		private StaticText								resultLabel;
		private FrameBox								modeFrame;
		private TextField								specificSearchField;
		private CheckButton								specificCatégorieActif;
		private CheckButton								specificCatégoriePassif;
		private CheckButton								specificCatégorieCharge;
		private CheckButton								specificCatégorieProduit;
		private CheckButton								specificCatégorieExploitation;
		private TextField								specificBeginDateField;
		private TextField								specificEndDateField;
		private RadioButton								andButton;
		private RadioButton								orButton;
		private bool									ignoreChange;
	}
}
