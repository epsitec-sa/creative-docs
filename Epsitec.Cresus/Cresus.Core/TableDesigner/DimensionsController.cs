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
			this.UpdateDimensionList ();
			this.UpdateSelectedArticle ();
			this.UpdatePointList ();
		}

		public void Update()
		{
		}


		private void CreateArticleUI(Widget parent)
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
				this.UpdateSelectedArticle ();
			};

			this.createDimensionButton.Clicked += delegate
			{
				this.CreateOrDeleteDimension ();
			};
		}

		private void CreateDimensionUI(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Axes</font>",
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
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				Dock = DockStyle.Fill,
			};

			//	Connection des événements.
			this.dimensionScrollList.SelectedItemChanged += delegate
			{
				this.DimensionSelectedItemChanged ();
			};

			this.dimensionNameField.EditionAccepted += delegate
			{
				this.ChangeDimensionName ();
			};
		}

		private void CreatePointUI(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Points sur l'axe</font>",
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
				DefocusAction = DefocusAction.AutoAcceptOrRejectEdition,
				SwallowEscapeOnRejectEdition = true,
				SwallowReturnOnAcceptEdition = true,
				Dock = DockStyle.Fill,
			};

			//	Connection des événements.
			this.pointValueField.EditionAccepted += delegate
			{
				this.ChangePointValue ();
			};
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

		private void UpdateSelectedArticle()
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
				index = this.GetDimensionIndex (code);

				if (index == -1)
				{
					this.createDimensionButton.Text = "Créer l'axe correspondant  >";
				}
				else
				{
					this.createDimensionButton.Text = "Supprimer l'axe correspondant  >";
				}
			}

			this.dimensionScrollList.SelectedItemIndex = index;
		}

		private void UpdateDimensionList()
		{
			int sel = this.dimensionScrollList.SelectedItemIndex;

			this.dimensionScrollList.Items.Clear ();

			foreach (var dimension in this.table.Dimensions)
			{
				string text = dimension.Name;

				this.dimensionScrollList.Items.Add (text);
			}

			this.dimensionScrollList.SelectedItemIndex = sel;
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

				this.pointListController = new Controllers.ListController<string> (list, this.PointListControllerItemToText, this.PointListControllerGetTextInfo, this.PointListControllerCreateItem);
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


		private void CreateOrDeleteDimension()
		{
			int sel = this.articleParameterList.SelectedItemIndex;

			if (sel == -1)
			{
				return;
			}
			else
			{
				string code = this.articleDefinitionEntity.ArticleParameterDefinitions[sel].Code;
				int index = this.GetDimensionIndex (code);

				if (index == -1)  // créer la dimension ?
				{
					var dimension = this.CreateDimension (this.articleDefinitionEntity.ArticleParameterDefinitions[sel]);

					if (dimension != null)
					{
						this.table.InsertDimension (this.table.Dimensions.Count (), dimension);
					}
				}
				else  // supprimer la dimension ?
				{
					this.table.RemoveDimensionAt (index);
				}

				this.UpdateDimensionList ();
				this.UpdateSelectedArticle ();
				this.UpdatePointList ();
			}
		}

		private void DimensionSelectedItemChanged()
		{
			int sel = this.dimensionScrollList.SelectedItemIndex;

			if (sel == -1)
			{
				this.dimensionNameField.Enable = false;
				this.dimensionNameField.Text = null;
			}
			else
			{
				this.dimensionNameField.Enable = true;
				this.dimensionNameField.Text = this.table.Dimensions.ElementAt (sel).Name;
			}

			this.UpdatePointList ();
		}

		private void ChangeDimensionName()
		{
			int sel = this.dimensionScrollList.SelectedItemIndex;

			if (sel != -1)
			{
				this.table.Dimensions.ElementAt(sel).Name = this.dimensionNameField.Text;
				this.UpdateDimensionList ();
			}
		}

		private void ChangePointValue()
		{
		}

		private void PointSelectedItemChanged()
		{
		}

		private void PointItemInserted()
		{
		}


		private AbstractDimension CreateDimension(AbstractArticleParameterDefinitionEntity articleParameter)
		{
			//	Crée une nouvelle dimension d'après le paramètre d'un article.
			if (articleParameter is NumericValueArticleParameterDefinitionEntity)
			{
				var numericArticleParameter = articleParameter as NumericValueArticleParameterDefinitionEntity;

				string[] values = (numericArticleParameter.PreferredValues ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
				var list = new List<decimal> ();

				foreach (var value in values)
				{
					if (!string.IsNullOrWhiteSpace (value))
					{
						decimal d;

						if (decimal.TryParse (value, out d))
						{
							list.Add (d);
						}
					}
				}

				return new NumericDimension (articleParameter.Code, articleParameter.Name.ToString (), RoundingMode.Nearest, list);
			}

			if (articleParameter is EnumValueArticleParameterDefinitionEntity)
			{
				var enumArticleParameter = articleParameter as EnumValueArticleParameterDefinitionEntity;

				string[] values = (enumArticleParameter.Values ?? "").Split (new string[] { AbstractArticleParameterDefinitionEntity.Separator }, System.StringSplitOptions.None);
				var list = new List<string> ();

				foreach (var value in values)
				{
					if (!string.IsNullOrEmpty (value))
					{
						list.Add (value);
					}
				}

				return new CodeDimension (articleParameter.Code, articleParameter.Name.ToString (), list);
			}

			return null;
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

		private List<string> GetPoints
		{
			get
			{
				int sel = this.dimensionScrollList.SelectedItemIndex;

				if (sel != -1)
				{
					var dimension = this.table.Dimensions.ElementAt (sel);
					return dimension.Values.ToList ();
				}

				return null;
			}
		}


		#region ListController callbacks
		private FormattedText PointListControllerItemToText(string value)
		{
			return value;
		}

		private FormattedText PointListControllerGetTextInfo(int count)
		{
			return "";
		}

		private string PointListControllerCreateItem(int sel)
		{
			return "";
		}
		#endregion


		private readonly DimensionTable							table;
		private readonly ArticleDefinitionEntity				articleDefinitionEntity;

		private ScrollList										articleParameterList;
		private Button											createDimensionButton;
		private ScrollList										dimensionScrollList;
		private Widget											pointListParent;
		private Controllers.ListController<string>				pointListController;
		private TextFieldEx										dimensionNameField;
		private TextFieldEx										pointValueField;
	}
}
