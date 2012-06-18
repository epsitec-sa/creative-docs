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
using Epsitec.Cresus.Compta.Widgets;
using Epsitec.Cresus.Compta.Helpers;
using Epsitec.Cresus.Compta.Search.Data;
using Epsitec.Cresus.Compta.Fields.Controllers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Search.Controllers
{
	/// <summary>
	/// Contrôleur gérant la saisie d'un critère complet SearchData pour les recherches ou les filtres.
	/// Ce contrôleur utilise SearchNodeController, qui utilise lui-même SearchTabController.
	/// </summary>
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


		public SearchData Data
		{
			get
			{
				return this.data;
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

			var topPanelLeftFrame = new FrameBox
			{
				Parent          = frame,
				DrawFullFrame   = true,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Padding         = new Margins (5, 5, 5, 5),
			};

			this.middleFrame = new FrameBox
			{
				Parent              = frame,
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				PreferredHeight     = 20,
				Dock                = DockStyle.Fill,
				Padding             = new Margins (-1, 0, 0, 0),
			};

			var topPanelRightFrame = new FrameBox
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

			this.CreateTopPanelLeftUI (topPanelLeftFrame);
			this.CreateRightUI (rightFrame);
			this.CreateMiddleUI ();
			this.CreateTopPanelRightUI (topPanelRightFrame);

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
					this.topPanelLeftController.Specialist = value;
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
			this.UpdateSummary ();
		}

		public void UpdatePériode()
		{
			if (this.temporalController != null)
			{
				this.temporalController.UpdatePériode ();
			}
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

			if (this.isFilter)
			{
				this.filterEnableButton = new CheckButton
				{
					Parent           = this.middleFrame,
					PreferredWidth   = 20,
					PreferredHeight  = 20,
					AutoToggle       = false,
					Anchor           = AnchorStyles.TopLeft,
					Margins          = new Margins (9, 0, 6, 0),
				};

				ToolTip.Default.SetToolTip (this.filterEnableButton, "Active ou désactive le filtre");

				this.filterEnableButton.Clicked += delegate
				{
					this.data.Enable = !this.data.Enable;
					this.UpdateButtons ();
					this.SearchStartAction ();
				};
			}
		}

		private void CreateMiddleBeginnerUI()
		{
			this.beginnerFrame = new FrameBox
			{
				Parent              = this.middleFrame,
				ContainerLayoutMode = this.isFilter ? ContainerLayoutMode.VerticalFlow : ContainerLayoutMode.HorizontalFlow,
				Dock                = DockStyle.Fill,
				Padding             = new Margins (5, 0, this.isFilter ? 0 : 5, this.isFilter ? 0 : 5),
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
					PreferredWidth   = UIBuilder.LeftLabelWidth+1-10,
					PreferredHeight  = 20,
					Dock             = DockStyle.Top,
					Margins          = new Margins (0, 10, this.isFilter ? 5 : 0, 0),
				};
			}

			if (this.isFilter)
			{
				var stackFrame = new FrameBox
				{
					Parent = this.beginnerFrame,
					Dock   = DockStyle.Fill,
				};

				this.CreateMiddleBeginnerFreeTextUI (stackFrame);

				if (this.columnMappers.Where (x => x.Column == ColumnType.Date).Any ())
				{
					this.CreateMiddleBeginnerDatesUI (stackFrame);
				}
			}
			else
			{
				this.CreateMiddleBeginnerSearchUI ();
			}
		}


		private void CreateMiddleBeginnerFreeTextUI(FrameBox parent)
		{
			var frame = this.CreateBeginnerFrame (parent);

			this.beginnerFreeTextField = new TextField
			{
				Parent          = frame,
				FormattedText   = this.data.BeginnerFreeText,
				PreferredWidth  = 250,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.beginnerFreeTextButton = new IconButton
			{
				Parent          = frame,
				IconUri         = UIBuilder.GetResourceIconUri ("Level.Clear"),
				AutoFocus       = false,
				PreferredWidth  = 20,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
				Margins         = new Margins (1, 0, 0, 0),
			};

			this.UpdateMiddleBeginnerFreeText ();

			this.beginnerFreeTextField.TextChanged += delegate
			{
				this.data.BeginnerFreeText = this.beginnerFreeTextField.FormattedText;
				this.UpdateMiddleBeginnerFreeText ();
				this.SearchStartAction ();
			};

			this.beginnerFreeTextButton.Clicked += delegate
			{
				this.beginnerFreeTextField.FormattedText = null;
			};

			ToolTip.Default.SetToolTip (this.beginnerFreeTextField, "Texte cherché n'importe où");
			ToolTip.Default.SetToolTip (this.beginnerFreeTextButton, "Efface le texte");
		}

		private void UpdateMiddleBeginnerFreeText()
		{
			this.beginnerFreeTextButton.Enable = !this.beginnerFreeTextField.FormattedText.IsNullOrEmpty ();
		}


		private void CreateMiddleBeginnerDatesUI(FrameBox parent)
		{
			var frame = this.CreateBeginnerFrame (parent);

			this.temporalController = new TemporalController (this.data.TemporalData);
			this.temporalController.CreateUI (frame, this.GetPériode, this.BeginnerDateChanged);
		}

		private TemporalData GetPériode()
		{
			var temporalData = this.controller.MainWindowController.TemporalData;

			Date? beginDate = this.controller.MainWindowController.Période.DateDébut;
			Date? endDate   = this.controller.MainWindowController.Période.DateFin;

			temporalData.MergeDates(ref beginDate, ref endDate);

			return new TemporalData
			{
				BeginDate = beginDate,
				EndDate   = endDate,
			};
		}

		private void BeginnerDateChanged()
		{
			data.SetBeginnerDates (this.data.TemporalData.BeginDate, this.data.TemporalData.EndDate);
			this.SearchStartAction ();
		}


		private FrameBox CreateBeginnerFrame(FrameBox parent)
		{
			return new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 5+20+5,
				DrawFullFrame   = true,
				Dock            = DockStyle.Top,
				Margins         = new Margins (0, 0, 0, -1),
				Padding         = new Margins (5),
			};
		}


		private void CreateMiddleSpecialistUI()
		{
			this.beginnerSearchField         = null;
			this.beginnerFreeTextField       = null;
			this.beginnerBeginDateController = null;
			this.beginnerEndDateController   = null;

			int count = this.data.NodesData.Count;
			for (int i = 0; i < count; i++)
			{
				var controller = new SearchNodeController (this.controller, this, this.data.NodesData[i], this.isFilter);

				var frame = controller.CreateUI (this.middleFrame, this.SearchStartAction, this.AddRemoveAction, this.SwapNodeAction);
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

			this.summaryLabel = new StaticText
			{
				Parent           = parent,
				ContentAlignment = ContentAlignment.TopLeft,
				Dock             = DockStyle.Fill,
			};

			ToolTip.Default.SetToolTip (this.summaryLabel, "Résumé");

			{
				bool hasNextPrev = !this.isFilter;

				this.buttonPrev = new GlyphButton
				{
					Parent          = footer,
					GlyphShape      = Common.Widgets.GlyphShape.TriangleLeft,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (0, 0, 0, 0),
					Visibility      = hasNextPrev,
				};

				this.buttonNext = new GlyphButton
				{
					Parent          = footer,
					GlyphShape      = Common.Widgets.GlyphShape.TriangleRight,
					PreferredWidth  = 20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Enable          = false,
					Margins         = new Margins (-1, 0, 0, 0),
					Visibility      = hasNextPrev,
				};

				new FrameBox
				{
					Parent          = footer,
					PreferredWidth  = 20+20-1,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Visibility      = !hasNextPrev,
				};

				bool hasQuickFilter = !this.isFilter && this.controller.DataAccessor.FilterData != null;

				this.buttonQuickFilter = new BackIconButton
				{
					Parent          = footer,
					IconUri         = UIBuilder.GetResourceIconUri ("Search.QuickFilter"),
					PreferredWidth  = 20,
					PreferredHeight = 20,
					BackColor       = UIBuilder.SelectionColor,
					Dock            = DockStyle.Left,
					Margins         = new Margins (1, 0, 0, 0),
					AutoFocus       = false,
					Visibility      = hasQuickFilter,
				};

				new FrameBox
				{
					Parent          = footer,
					PreferredWidth  = 1+20,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Visibility      = !hasQuickFilter,
				};

				new FrameBox
				{
					Parent          = footer,
					PreferredWidth  = 10,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
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

				this.buttonQuickFilter.Clicked += delegate
				{
					this.data.QuickFilter = !this.data.QuickFilter;
					this.UpdateButtons ();
					this.SearchStartAction ();
				};

				ToolTip.Default.SetToolTip (this.buttonPrev,        "Cherche en arrière");
				ToolTip.Default.SetToolTip (this.buttonNext,        "Cherche en avant");
				ToolTip.Default.SetToolTip (this.buttonQuickFilter, "Filtre instantané");
			}
		}

		private void CreateMiddleBeginnerSearchUI()
		{
			this.beginnerSearchField = new TextField
			{
				Parent          = this.beginnerFrame,
				Text            = this.data.BeginnerSearch,
				PreferredWidth  = 250+5,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.beginnerSearchField.TextChanged += delegate
			{
				this.data.BeginnerSearch = this.beginnerSearchField.Text;
				this.SearchStartAction ();
			};

			ToolTip.Default.SetToolTip (this.beginnerSearchField, "Texte cherché n'importe où");
		}


		private void CreateTopPanelLeftUI(FrameBox parent)
		{
			System.Action closeAction;

			if (this.isFilter)
			{
				closeAction = this.controller.MainWindowController.ClosePanelFilter;
			}
			else
			{
				closeAction = this.controller.MainWindowController.ClosePanelSearch;
			}

			this.topPanelLeftController = new TopPanelLeftController (this.controller);
			this.topPanelLeftController.CreateUI (parent, true, this.isFilter ? "Panel.Filter" : "Panel.Search", this.LevelChangedAction);
			this.topPanelLeftController.Specialist = this.data.Specialist;
		}

		private void CreateTopPanelRightUI(FrameBox parent)
		{
			System.Action closeAction;

			if (this.isFilter)
			{
				closeAction = this.controller.MainWindowController.ClosePanelFilter;
			}
			else
			{
				closeAction = this.controller.MainWindowController.ClosePanelSearch;
			}

			this.topPanelRightController = new TopPanelRightController (this.controller);
			this.topPanelRightController.CreateUI (parent, this.isFilter ? "Termine le filtre" : "Termine la recherche", this.MiscAction, this.ClearAction, closeAction, this.LevelChangedAction);
		}

		private void MiscAction()
		{
			this.ShowMiscMenu (this.topPanelRightController.ButtonMisc);
		}

		private void ClearAction()
		{
			this.SearchClear ();
			this.SetFocus ();
		}

		private void LevelChangedAction()
		{
			this.data.Specialist = this.topPanelLeftController.Specialist;
			this.data.BeginnerAdjust (this.isFilter);

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.SearchStartAction ();
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

				if (this.isFilter && this.beginnerFreeTextField != null)
				{
					this.beginnerFreeTextField.Focus ();
				}
			}
		}


		public void SearchClear()
		{
#if false
			if (this.data.Specialist)
			{
				while (this.data.NodesData.Count > 1)
				{
					this.data.NodesData.RemoveAt (1);
				}

				if (this.nodeControllers.Count != 0)
				{
					this.nodeControllers[0].SearchClear ();
				}
			}
			else
			{
				this.data.NodesData[0].TabsData[0].Clear ();
			}

			if (this.temporalController != null)
			{
				this.data.TemporalData.Clear ();
			}
#else
			this.data.Clear ();
#endif

			this.CreateMiddleUI ();
			this.UpdateButtons ();
			this.SearchStartAction ();
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
			this.SearchStartAction ();
		}

		private void SwapNodeAction()
		{
			this.data.OrMode = !this.data.OrMode;
			this.UpdateButtons ();
			this.SearchStartAction ();
		}

		private void UpdateButtons()
		{
			this.topPanelRightController.ClearEnable = !this.data.IsEmpty;

			if (this.filterEnableButton != null)
			{
				this.filterEnableButton.ActiveState = this.data.Enable ? ActiveState.Yes : ActiveState.No;
			}

			if (this.buttonQuickFilter != null)
			{
				this.buttonQuickFilter.ActiveState = this.data.QuickFilter ? ActiveState.Yes : ActiveState.No;
			}

			foreach (var controller in this.nodeControllers)
			{
				controller.UpdateButtons ();
			}
		}


		private void SearchStartAction()
		{
			//	Appelé lorsque le critère de recherche a changé et qu'il faut démarrer une nouvelle recherche.
			this.UpdateSummary ();
			this.searchStartAction ();
		}

		private void UpdateSummary()
		{
			//	Met à jour le résumé du critère, qui n'est visible que si la place le permet.
#if false
			int count = this.data.DeepCount;

			if (count <= 1)
			{
				this.summaryLabel.Visibility = false;
			}
			else
			{
				this.summaryLabel.Visibility = true;
				this.summaryLabel.FormattedText = this.data.GetSummary (this.columnMappers);

				if (count == 2)  // résumé sur une seule ligne ?
				{
					this.summaryLabel.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				}
				else
				{
					this.summaryLabel.TextBreakMode = TextBreakMode.Ellipsis | TextBreakMode.Split;
				}
			}
#else
			this.summaryLabel.Visibility = false;
#endif
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


		#region Misc menu
		private void ShowMiscMenu(Widget parentButton)
		{
			//	Affiche le menu des actions supplémentaires.
			var collection = this.MenuCollection;

			bool selectEnable = collection.SelectedIndex != -1;
			bool collectionEnable = selectEnable;

			if (collectionEnable)
			{
				collectionEnable = !collection.DataList[collection.SelectedIndex].CompareTo (this.MenuData);

				if (collection.DataList[collection.SelectedIndex].Specialist != this.Specialist)
				{
					collectionEnable = true;
				}
			}

			bool moveBeginEnable = selectEnable;
			bool moveEndEnable   = selectEnable;

			if (moveBeginEnable && collection.SelectedIndex == 0)
			{
				moveBeginEnable = false;
			}

			if (moveEndEnable && collection.SelectedIndex == collection.DataList.Count-1)
			{
				moveEndEnable = false;
			}

			//	Met toutes les données connues dans le menu.
			var menu = new VMenu ();

			for (int i = 0; i< collection.DataList.Count; i++)
			{
				var data = collection.DataList[i];

				var icon = (i == collection.SelectedIndex) ? "MarkYes" : "MarkNo";
				var text = data.GetSummary (this.controller.ColumnMappers);

				if (collectionEnable && i == collection.SelectedIndex)
				{
					text = text.ApplyBold ();
				}

				this.AddToMenu (menu, true, icon, text, this.MenuUseAction, i);
			}

			if (collection.DataList.Any ())
			{
				menu.Items.Add (new MenuSeparator ());
			}

			//	Met les commandes standards dans le menu.
			this.AddToMenu (menu, true,         "Edit.Add",    "Ajouter",   this.MenuAddAction);
			this.AddToMenu (menu, selectEnable, "Edit.Delete", "Supprimer", this.MenuDeleteAction);

			menu.Items.Add (new MenuSeparator ());

			this.AddToMenu (menu, collectionEnable, "ViewSettings.Reload", "Réutiliser les réglages initiaux", this.MenuReloadAction);
			this.AddToMenu (menu, collectionEnable, "ViewSettings.Save",   "Enregistrer les réglages actuels", this.MenuSaveAction);

			menu.Items.Add (new MenuSeparator ());

			this.AddToMenu (menu, moveBeginEnable, "Edit.Tab.Begin", "Déplacer en tête",  this.MenuMoveBeginAction);
			this.AddToMenu (menu, moveEndEnable,   "Edit.Tab.End",   "Déplacer en queue", this.MenuMoveEndAction);

			//	Affiche le menu.
			TextFieldCombo.AdjustComboSize (parentButton, menu, false);

			menu.Host = parentButton.Window;
			menu.ShowAsComboList (parentButton, Point.Zero, parentButton);
		}

		private void AddToMenu(VMenu menu, bool enable, string icon, FormattedText text, System.Action<MenuItem> action, int index = -1)
		{
			var item = new MenuItem ()
			{
				IconUri       = UIBuilder.GetResourceIconUri (icon),
				FormattedText = text,
				Enable        = enable,
				TabIndex      = index,
			};

			item.Clicked += delegate
			{
				action (item);
			};

			menu.Items.Add (item);
		}

		private void MenuUseAction(MenuItem item)
		{
			var index = item.TabIndex;

			var collection = this.MenuCollection;
			collection.SelectedIndex = index;
			collection.DataList[index].CopyTo (this.MenuData);
			this.Specialist = collection.DataList[index].Specialist;

			this.SearchStartAction ();
			this.controller.UpdateAfterChanged ();
		}

		private void MenuAddAction(MenuItem item)
		{
			var collection = this.MenuCollection;
			var data = this.MenuData.CopyFrom ();
			data.Specialist = this.Specialist;
			collection.DataList.Add (data);
			collection.SelectedIndex = collection.DataList.Count-1;
		}

		private void MenuDeleteAction(MenuItem item)
		{
			var collection = this.MenuCollection;
			this.MenuCollection.DataList.RemoveAt (collection.SelectedIndex);
			collection.SelectedIndex = -1;
		}

		private void MenuReloadAction(MenuItem item)
		{
			var collection = this.MenuCollection;
			var index = collection.SelectedIndex;
			collection.SelectedIndex = index;
			collection.DataList[index].CopyTo (this.MenuData);
			this.Specialist = collection.DataList[index].Specialist;

			this.SearchStartAction ();
			this.controller.UpdateAfterChanged ();
		}

		private void MenuSaveAction(MenuItem item)
		{
			var collection = this.MenuCollection;
			var index = collection.SelectedIndex;
			collection.SelectedIndex = index;
			this.MenuData.CopyTo (collection.DataList[index]);
			collection.DataList[index].Specialist = this.Specialist;
		}

		private void MenuMoveBeginAction(MenuItem item)
		{
			var collection = this.MenuCollection;
			var index = collection.SelectedIndex;
			var data = collection.DataList[index];
			this.MenuCollection.DataList.RemoveAt (collection.SelectedIndex);
			index = 0;
			collection.DataList.Insert (index, data);
			collection.SelectedIndex = index;
		}

		private void MenuMoveEndAction(MenuItem item)
		{
			var collection = this.MenuCollection;
			var index = collection.SelectedIndex;
			var data = collection.DataList[index];
			this.MenuCollection.DataList.RemoveAt (collection.SelectedIndex);
			index = collection.DataList.Count;
			collection.DataList.Insert (index, data);
			collection.SelectedIndex = index;
		}

		private SearchDataCollection MenuCollection
		{
			get
			{
				if (this.isFilter)
				{
					return this.controller.DataAccessor.FilterDataCollection;
				}
				else
				{
					return this.controller.DataAccessor.SearchDataCollection;
				}
			}
		}

		private SearchData MenuData
		{
			get
			{
				if (this.isFilter)
				{
					return this.controller.DataAccessor.FilterData;
				}
				else
				{
					return this.controller.DataAccessor.SearchData;
				}
			}
		}
		#endregion


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
		private CheckButton								filterEnableButton;
		private GlyphButton								buttonNext;
		private GlyphButton								buttonPrev;
		private BackIconButton							buttonQuickFilter;
		private StaticText								summaryLabel;
		private StaticText								resultLabel;
		private FrameBox								beginnerFrame;
		private TextField								beginnerSearchField;
		private TextField								beginnerFreeTextField;
		private IconButton								beginnerFreeTextButton;
		private CheckButton								beginnerCatégorieActif;
		private CheckButton								beginnerCatégoriePassif;
		private CheckButton								beginnerCatégorieCharge;
		private CheckButton								beginnerCatégorieProduit;
		private CheckButton								beginnerCatégorieExploitation;
		private TextFieldCombo							beginnerFromProfondeurField;
		private TextFieldCombo							beginnerToProfondeurField;
		private DateFieldController						beginnerBeginDateController;
		private DateFieldController						beginnerEndDateController;
		private TopPanelLeftController					topPanelLeftController;
		private TopPanelRightController					topPanelRightController;
		private TemporalController						temporalController;
	}
}
