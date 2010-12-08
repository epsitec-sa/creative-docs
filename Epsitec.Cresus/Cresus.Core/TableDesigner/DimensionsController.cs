//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business.Finance.PriceCalculators;
using Epsitec.Cresus.Core.Widgets;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.TableDesigner
{
	public class DimensionsController
	{
		public DimensionsController(DimensionTable table, ArticleDefinitionEntity articleDefinitionEntity)
		{
			this.table                   = table;
			this.articleDefinitionEntity = articleDefinitionEntity;
		}

		public void CreateUI(Widget parent)
		{
			var frame = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			var articlePane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 200,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
			};

			var pointPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 200,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 4, 10, 10),
			};

			var dimensionPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 200,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 4, 10, 10),
			};

			var sep = new Separator
			{
				Parent = frame,
				PreferredWidth = 1,
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.CreateArticleUI (articlePane);
			this.CreateDimensionUI (dimensionPane);
			this.CreatePointUI (pointPane);

			this.UpdateArticleParameterList ();
			this.UpdateArticleButton ();
			this.UpdateDimensionList ();
			this.UpdatePointList ();
		}

		public void CreateArticleUI(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Paramètres de l'article</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.articleParameterList = new ScrollList
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			this.createDimensionButton = new Button
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			//	Connection des événements.
			this.articleParameterList.SelectedItemChanged += delegate
			{
				this.UpdateArticleButton ();
			};

			this.createDimensionButton.Clicked += delegate
			{
				this.CreateDimension ();
			};
		}

		public void CreateDimensionUI(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des axes</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			//	Crée la liste.
			var tile = new ArrowedFrame
			{
				Parent = parent,
				ArrowDirection = Direction.Right,
				Dock = DockStyle.Fill,
				Padding = new Margins (0, TileArrow.Breadth, 0, 0),
			};

			this.dimensionScrollList = new ScrollList
			{
				Parent = tile,
				ScrollListStyle = Common.Widgets.ScrollListStyle.FrameLess,
				Dock = DockStyle.Fill,
			};

			//	Crée le pied de page.
			var footer = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, TileArrow.Breadth, 10, 0),
			};

			var label = new StaticText
			{
				Parent = footer,
				Text = "Nom de l'axe",
				PreferredWidth = 80,
				Dock = DockStyle.Left,
			};

			this.dimensionNameField = new TextFieldEx
			{
				Parent = footer,
				Dock = DockStyle.Fill,
			};

			//	Connection des événements.
			this.dimensionScrollList.SelectedItemChanged += delegate
			{
				this.DimensionSelectedItemChanged ();
			};
		}

		public void CreatePointUI(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Liste des points</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			this.pointListParent = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			//	Crée le pied de page.
			var footer = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, TileArrow.Breadth, 10, 0),
			};

			var label = new StaticText
			{
				Parent = footer,
				Text = "Valeur du point",
				PreferredWidth = 80,
				Dock = DockStyle.Left,
			};

			this.pointValueField = new TextFieldEx
			{
				Parent = footer,
				Dock = DockStyle.Fill,
			};

			//	Connection des événements.
		}


		private void UpdateArticleParameterList()
		{
			this.articleParameterList.Items.Clear();

			foreach (var parameter in this.articleDefinitionEntity.ArticleParameterDefinitions)
			{
				string text = parameter.GetSummary ().ToString ();
				this.articleParameterList.Items.Add (text);
			}
		}

		private void UpdateArticleButton()
		{
			int sel = this.articleParameterList.SelectedItemIndex;
			int index = -1;

			if (sel == -1)
			{
				this.createDimensionButton.Enable = false;
				this.createDimensionButton.Text = null;
			}
			else
			{
				this.createDimensionButton.Enable = true;

				string code = this.articleDefinitionEntity.ArticleParameterDefinitions[sel].Code;
				bool exist = this.table.Dimensions.Any (x => x.Code == code);

				if (exist)
				{
					this.createDimensionButton.Text = "Supprimer l'axe correspondant  >";
				}
				else
				{
					this.createDimensionButton.Text = "Créer l'axe correspondant  >";
				}

				index = this.GetDimensionIndex (code);
			}

			this.dimensionScrollList.SelectedItemIndex = index;
		}

		private void UpdateDimensionList()
		{
			this.dimensionScrollList.Items.Clear ();

			foreach (var dimension in this.table.Dimensions)
			{
				string text = dimension.Name;

				this.dimensionScrollList.Items.Add (text);
			}
		}

		private void UpdatePointList()
		{
			this.pointListParent.Children.Clear ();
			this.pointListController = null;

			var list = this.GetPoints;

			if (list == null)
			{
				this.pointListParent.DrawFrameState = FrameState.All;
				this.pointListParent.Margins = new Margins (0, TileArrow.Breadth, 0, 0);
			}
			else
			{
				this.pointListParent.DrawFrameState = FrameState.None;
				this.pointListParent.Margins = new Margins (0, 0, 0, 0);

				this.pointListController = new Controllers.ListController<decimal> (list, this.PointListControllerItemToText, this.PointListControllerGetTextInfo, this.PointListControllerCreateItem);
				this.pointListController.CreateUI (this.pointListParent, Direction.Right, 23);

				ToolTip.Default.SetToolTip (this.pointListController.AddButton,      "Crée un nouveau point");
				ToolTip.Default.SetToolTip (this.pointListController.RemoveButton,   "Supprime le point sélectionné");
				ToolTip.Default.SetToolTip (this.pointListController.MoveUpButton,   "Monte le point dans la liste");
				ToolTip.Default.SetToolTip (this.pointListController.MoveDownButton, "Descend le point dans la liste");

				//	Connection des événements.
				this.pointListController.SelectedItemChanged += delegate
				{
					this.PointSelectedItemChanged ();
				};

				this.pointListController.ItemInserted += delegate
				{
					this.PointItemInserted ();
				};
			}
		}


		private void CreateDimension()
		{
		}

		private void DimensionSelectedItemChanged()
		{
			this.UpdatePointList ();
		}

		private void PointSelectedItemChanged()
		{
		}

		private void PointItemInserted()
		{
		}


		private int GetDimensionIndex(string code)
		{
			for (int i=0; i<this.table.Dimensions.Count (); i++)
			{
				var dimension = this.table.Dimensions.ElementAt (i);

				if (dimension.Code == code)
				{
					return i;
				}
			}

			return -1;
		}

		private List<decimal> GetPoints
		{
			get
			{
				int sel = this.dimensionScrollList.SelectedItemIndex;

				if (sel != -1)
				{
					var dimension = this.table.Dimensions.ElementAt (sel);

					if (dimension is NumericDimension)
					{
						var d = dimension as NumericDimension;
						return d.DecimalValues.ToList ();
					}
				}

				return null;
			}
		}


		#region ListController callbacks
		private FormattedText DimensionListControllerItemToText(AbstractDimension dimension)
		{
			return dimension.Name;
		}

		private FormattedText DimensionListControllerGetTextInfo(int count)
		{
			return "";
		}

		private AbstractDimension DimensionListControllerCreateItem(int sel)
		{
			return new NumericDimension ("xyz", "Vide", new decimal[] { 1, 2, 3 }, RoundingMode.Nearest);
		}


		private FormattedText PointListControllerItemToText(decimal value)
		{
			return value.ToString ();
		}

		private FormattedText PointListControllerGetTextInfo(int count)
		{
			return "";
		}

		private decimal PointListControllerCreateItem(int sel)
		{
			return 0;
		}
		#endregion


		private readonly DimensionTable							table;
		private readonly ArticleDefinitionEntity				articleDefinitionEntity;

		private ScrollList										articleParameterList;
		private Button											createDimensionButton;
		private ScrollList										dimensionScrollList;
		private Widget											pointListParent;
		private Controllers.ListController<decimal>				pointListController;
		private TextFieldEx										dimensionNameField;
		private TextFieldEx										pointValueField;
	}
}
