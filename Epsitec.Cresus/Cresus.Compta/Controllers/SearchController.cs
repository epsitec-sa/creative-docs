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
			var header = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = 20,
				Dock            = DockStyle.Top,
			};

			this.searchButtonClear = new GlyphButton
			{
				Parent          = header,
				GlyphShape      = GlyphShape.Close,
				PreferredSize   = new Size (20, 20),
				Dock            = DockStyle.Left,
				Enable          = false,
				Margins         = new Margins (0, 10, 0, 0),
			};

			new StaticText
			{
				Parent          = header,
				Text            = this.isFilter ? "Filtrer" : "Rechercher",
				PreferredWidth  = 64,
				PreferredHeight = 20,
				Dock            = DockStyle.Left,
			};

			this.searchButtonClear.Clicked += delegate
			{
				this.SearchClear ();
			};

			ToolTip.Default.SetToolTip (this.searchButtonClear, this.isFilter ? "Termine le filtre" : "Termine la recherche");
		}

		private void CreateMiddleUI()
		{
			this.middleFrame.Children.Clear ();
			this.tabControllers.Clear ();

			int count = this.data.TabsData.Count;
			for (int i = 0; i < count; i++)
			{
				var controller = new SearchTabController (this.data.TabsData[i], this.columnMappers, this.isFilter);

				var frame = controller.CreateUI (this.middleFrame, this.bigDataInterface, this.searchStartAction, this.AddRemoveAction);
				controller.Index = i;
				controller.AddAction = (i == 0);

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
				var andButton = new RadioButton
				{
					Parent          = this.modeFrame,
					Text            = "Tous",
					PreferredWidth  = 60,
					ActiveState     = this.data.OrMode ? ActiveState.No : ActiveState.Yes,
					Dock            = DockStyle.Left,
				};

				var orButton = new RadioButton
				{
					Parent          = this.modeFrame,
					Text            = "Au moins un",
					PreferredWidth  = 90,
					ActiveState     = this.data.OrMode ? ActiveState.Yes : ActiveState.No,
					Dock            = DockStyle.Left,
				};

				orButton.ActiveStateChanged += delegate
				{
					this.data.OrMode = orButton.ActiveState == ActiveState.Yes;
					this.searchStartAction ();
				};

				ToolTip.Default.SetToolTip (andButton, this.isFilter ? "Filtre les données qui répondent à tous les critères"   : "Cherche les données qui répondent à tous les critères");
				ToolTip.Default.SetToolTip (orButton,  this.isFilter ? "Filtre les données qui répondent à au moins un critère" : "Cherche les données qui répondent à au moins un critère");
			}

			{
				this.searchButtonPrev = new GlyphButton
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

				this.searchButtonNext = new GlyphButton
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

				this.searchResult = new StaticText
				{
					Parent          = footer,
					PreferredWidth  = 120,
					PreferredHeight = 20,
					Dock            = DockStyle.Left,
					Margins         = new Margins (0, 0, 0, 0),
				};

				this.searchButtonPrev.Clicked += delegate
				{
					this.searchNextAction (-1);
				};

				this.searchButtonNext.Clicked += delegate
				{
					this.searchNextAction (1);
				};

				ToolTip.Default.SetToolTip (this.searchButtonPrev, "Cherche en arrière");
				ToolTip.Default.SetToolTip (this.searchButtonNext, "Cherche en avant");
			}
		}


		public void SetFocus()
		{
			this.tabControllers[0].SetFocus ();
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

		public void SetSearchCount(int dataCount, int? count, int? locator)
		{
			this.BigDataInterface = (dataCount >= 1000);  // limite arbitraire au-delà de laquelle les recherches deviennent trop lentes !

			this.UpdateButtons ();

			this.searchButtonNext.Enable = (count > 1);
			this.searchButtonPrev.Enable = (count > 1);

			if (!count.HasValue)
			{
				this.searchResult.Text = null;
			}
			else if (count == 0)
			{
				this.searchResult.Text = "Aucun résultat trouvé";
			}
			else
			{
				int l = locator.GetValueOrDefault () + 1;
				int c = count.Value;
				this.searchResult.Text = string.Format ("{0}/{1} resultat{2}", l.ToString (), c.ToString (), (c == 1) ? "" : "s");
			}
		}

		public void SetFilterCount(int dataCount, int count, int allCount)
		{
			this.BigDataInterface = (dataCount >= 1000);  // limite arbitraire au-delà de laquelle les recherches deviennent trop lentes !

			this.UpdateButtons ();

			if (count == allCount)
			{
				this.searchResult.Text = string.Format ("{0} (tous)", allCount.ToString ());
			}
			else
			{
				this.searchResult.Text = string.Format ("{0} sur {1}", count.ToString (), allCount.ToString ());
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
			this.searchButtonClear.Enable = !this.data.IsEmpty || this.data.TabsData.Count > 1;
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
		private readonly SearchData					data;
		private readonly List<ColumnMapper>				columnMappers;
		private readonly List<SearchTabController>	tabControllers;
		private readonly bool							isFilter;

		private bool									bigDataInterface;
		private System.Action							searchStartAction;
		private System.Action<int>						searchNextAction;

		private FrameBox								middleFrame;
		private GlyphButton								searchButtonClear;
		private GlyphButton								searchButtonNext;
		private GlyphButton								searchButtonPrev;
		private StaticText								searchResult;
		private FrameBox								modeFrame;
	}
}
