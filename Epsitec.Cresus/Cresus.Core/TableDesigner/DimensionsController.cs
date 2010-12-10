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
		public DimensionsController(ArticleDefinitionEntity articleDefinitionEntity, DesignerTable table)
		{
			this.articleDefinitionEntity = articleDefinitionEntity;
			this.table = table;
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
				Margins = new Margins (0, 10, 10, 10),
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
			this.UpdateDimensionsList ();
			this.UpdateSelectedArticle ();
			this.UpdatePointsList ();
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

			//	Connexion des événements.
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

			this.dimensionsScrollList = new ScrollList
			{
				Parent = tile,
				ScrollListStyle = Common.Widgets.ScrollListStyle.FrameLess,
				Dock = DockStyle.Fill,
			};

			//	Crée le pied de page.
			this.dimensionsRoundingPane = new GroupBox
			{
				Parent = parent,
				Text = "Type d'arrondi",
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, TileArrow.Breadth, 10, 0),
				Padding = new Margins (10),
			};

			this.dimensionsRadioRoundingNone = new RadioButton
			{
				Parent = this.dimensionsRoundingPane,
				Text = "Pas d'arrondi",
				Name = "None",
				Dock = DockStyle.Top,
			};

			this.dimensionsRadioRoundingNearest = new RadioButton
			{
				Parent = this.dimensionsRoundingPane,
				Text = "Valeur la plus proche",
				Name = "Nearest",
				Dock = DockStyle.Top,
			};

			this.dimensionsRadioRoundingDown = new RadioButton
			{
				Parent = this.dimensionsRoundingPane,
				Text = "Valeur inférieure",
				Name = "Down",
				Dock = DockStyle.Top,
			};

			this.dimensionsRadioRoundingUp = new RadioButton
			{
				Parent = this.dimensionsRoundingPane,
				Text = "Valeur supérieure",
				Name = "Up",
				Dock = DockStyle.Top,
			};

			//	Connexion des événements.
			this.dimensionsScrollList.SelectedItemChanged += delegate
			{
				this.DimensionSelectedItemChanged ();
			};

			this.dimensionsRadioRoundingNone   .Clicked += new EventHandler<MessageEventArgs> (this.HandleDimensionsRadioRoundingClicked);
			this.dimensionsRadioRoundingNearest.Clicked += new EventHandler<MessageEventArgs> (this.HandleDimensionsRadioRoundingClicked);
			this.dimensionsRadioRoundingUp     .Clicked += new EventHandler<MessageEventArgs> (this.HandleDimensionsRadioRoundingClicked);
			this.dimensionsRadioRoundingDown   .Clicked += new EventHandler<MessageEventArgs> (this.HandleDimensionsRadioRoundingClicked);
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

			this.CreatePointsToolbarUI (parent);

			this.pointsScrollList = new ScrollList
			{
				Parent = parent,
				Dock = DockStyle.Fill,
			};

			//	Crée le pied de page.
			var footer = new FrameBox
			{
				Parent = parent,
				Dock = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			var label = new StaticText
			{
				Parent = footer,
				Text = "Valeur du point",
				PreferredWidth = 85,
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

			//	Connexion des événements.
			this.pointsScrollList.SelectedItemChanged += delegate
			{
				this.PointSelectedItemChanged ();
			};

			this.pointValueField.EditionAccepted += delegate
			{
				this.ChangePointValue ();
			};
		}

		private void CreatePointsToolbarUI(Widget parent)
		{
			//	Crée la toolbar.
			double buttonSize = 19;

			this.pointsToolbar = UIBuilder.CreateMiniToolbar (parent, buttonSize);
			this.pointsToolbar.Margins = new Margins (0, 0, 0, -1);

			this.addPointsButton = new GlyphButton
			{
				Parent = pointsToolbar,
				PreferredSize = new Size (buttonSize*2+1, buttonSize),
				GlyphShape = GlyphShape.Plus,
				Margins = new Margins (0, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.removePointsButton = new GlyphButton
			{
				Parent = pointsToolbar,
				PreferredSize = new Size (buttonSize, buttonSize),
				GlyphShape = GlyphShape.Minus,
				Margins = new Margins (1, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.upPointsButton = new GlyphButton
			{
				Parent = pointsToolbar,
				PreferredSize = new Size (buttonSize, buttonSize),
				GlyphShape = GlyphShape.ArrowUp,
				Margins = new Margins (10, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			this.downPointsButton = new GlyphButton
			{
				Parent = pointsToolbar,
				PreferredSize = new Size (buttonSize, buttonSize),
				GlyphShape = GlyphShape.ArrowDown,
				Margins = new Margins (1, 0, 0, 0),
				Dock = DockStyle.Left,
			};

			ToolTip.Default.SetToolTip (this.addPointsButton,    "Crée un nouveau point");
			ToolTip.Default.SetToolTip (this.removePointsButton, "Supprime le point sélectionné");
			ToolTip.Default.SetToolTip (this.upPointsButton,     "Monte le point dans la liste");
			ToolTip.Default.SetToolTip (this.downPointsButton,   "Descend le point dans la liste");

			//	Connexion des événements.
			this.addPointsButton.Clicked += delegate
			{
				this.PointItemInserted ();
			};

			this.removePointsButton.Clicked += delegate
			{
				this.PointItemRemoved ();
			};

			this.upPointsButton.Clicked += delegate
			{
				this.PointItemMoved (-1);
			};

			this.downPointsButton.Clicked += delegate
			{
				this.PointItemMoved (1);
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

			this.dimensionsScrollList.SelectedItemIndex = index;
		}

		private void UpdateDimensionsList()
		{
			int sel = this.dimensionsScrollList.SelectedItemIndex;

			this.dimensionsScrollList.Items.Clear ();

			foreach (var dimension in this.table.Dimensions)
			{
				this.dimensionsScrollList.Items.Add (dimension.Name.ToString ());
			}

			this.dimensionsScrollList.SelectedItemIndex = sel;
			this.UpdateAfterDimensionSelected ();

			this.UpdateRoundingPane ();
		}

		private void UpdateRoundingPane()
		{
			var dimension = this.GetDimension;

			if (dimension != null && dimension.HasDecimal)
			{
				this.dimensionsRoundingPane.Visibility = true;

				this.dimensionsRadioRoundingNone   .ActiveState = (dimension.RoundingMode == RoundingMode.None   ) ? ActiveState.Yes : ActiveState.No;
				this.dimensionsRadioRoundingNearest.ActiveState = (dimension.RoundingMode == RoundingMode.Nearest) ? ActiveState.Yes : ActiveState.No;
				this.dimensionsRadioRoundingUp     .ActiveState = (dimension.RoundingMode == RoundingMode.Up     ) ? ActiveState.Yes : ActiveState.No;
				this.dimensionsRadioRoundingDown   .ActiveState = (dimension.RoundingMode == RoundingMode.Down   ) ? ActiveState.Yes : ActiveState.No;
			}
			else
			{
				this.dimensionsRoundingPane.Visibility = false;
			}
		}

		private void UpdatePointsList(int? sel = null)
		{
			if (sel == null)
			{
				sel = this.pointsScrollList.SelectedItemIndex;
			}

			this.pointsScrollList.Items.Clear ();

			var dimension = this.GetDimension;
			var list = this.GetPoints;

			if (list == null)
			{
				this.pointsToolbar.Visibility = false;
			}
			else
			{
				this.pointsToolbar.Visibility = true;

				if (dimension.HasDecimal)
				{
					//	Une liste numérique est intrinsèquement ordonnée. Cela n'a donc pas de sens
					//	de pouvoir modifier l'ordre.
					this.upPointsButton.Visibility = false;
					this.downPointsButton.Visibility = false;
				}
				else
				{
					this.upPointsButton.Visibility = true;
					this.downPointsButton.Visibility = true;
				}

				foreach (var value in list)
				{
					this.pointsScrollList.Items.Add (value);
				}
			}

			this.pointsScrollList.SelectedItemIndex = sel.Value;
			this.UpdateAfterPointSelected ();
		}

		private void UpdateAfterDimensionSelected()
		{
			this.UpdateRoundingPane ();
			this.UpdatePointsList (-1);
		}

		private void UpdateAfterPointSelected()
		{
			int sel = this.pointsScrollList.SelectedItemIndex;
			var dimension = this.GetDimension;

			if (sel == -1)
			{
				this.pointValueField.Enable = false;
				this.pointValueField.Text = null;
			}
			else
			{
				this.pointValueField.Enable = true;
				this.pointValueField.Text = this.GetDimension.Points.ElementAt (sel);
			}

			this.removePointsButton.Enable = sel != -1;
			this.upPointsButton.Enable     = sel > 0;
			this.downPointsButton.Enable   = sel != -1 && sel < dimension.Points.Count-1;
		}


		private void HandleDimensionsRadioRoundingClicked(object sender, MessageEventArgs e)
		{
			var radio = sender as RadioButton;
			var dimension = this.GetDimension;

			switch (radio.Name)
			{
				case "None":
					dimension.RoundingMode = RoundingMode.None;
					break;

				case "Nearest":
					dimension.RoundingMode = RoundingMode.Nearest;
					break;

				case "Up":
					dimension.RoundingMode = RoundingMode.Up;
					break;

				case "Down":
					dimension.RoundingMode = RoundingMode.Down;
					break;
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
					var dimension = new DesignerDimension (this.articleDefinitionEntity.ArticleParameterDefinitions[sel]);
					this.table.Dimensions.Add (dimension);
				}
				else  // supprimer la dimension ?
				{
					this.table.Dimensions.RemoveAt (index);
				}

				this.table.Values.Clear ();

				this.UpdateDimensionsList ();
				this.UpdateSelectedArticle ();
				this.UpdatePointsList (-1);
			}
		}

		private void DimensionSelectedItemChanged()
		{
			this.UpdateAfterDimensionSelected ();
		}


		private void PointSelectedItemChanged()
		{
			this.UpdateAfterPointSelected ();
		}

		private void PointItemInserted()
		{
			int sel = this.pointsScrollList.SelectedItemIndex;
			var dimension = this.GetDimension;

			var data = this.table.ExportValues ();

			if (dimension.HasDecimal)
			{
				dimension.Points.Add ("0");
				sel = dimension.Sort (dimension.Points.Count-1);
			}
			else
			{
				sel++;
				dimension.Points.Insert (sel, "Nouveau");
			}

			this.table.ImportValues (data);

			this.UpdatePointsList (sel);

			this.pointValueField.SelectAll ();
			this.pointValueField.Focus ();
		}

		private void PointItemRemoved()
		{
			int sel = this.pointsScrollList.SelectedItemIndex;
			var dimension = this.GetDimension;

			if (sel != -1)
			{
				var data = this.table.ExportValues ();
				dimension.Points.RemoveAt (sel);
				this.table.ImportValues (data);

				if (sel >= dimension.Points.Count)
				{
					sel = dimension.Points.Count-1;
				}

				this.UpdatePointsList (sel);
			}
		}

		private void PointItemMoved(int direction)
		{
			int sel = this.pointsScrollList.SelectedItemIndex;
			var dimension = this.GetDimension;

			if (sel != -1)
			{
				var data = this.table.ExportValues ();

				var t = dimension.Points[sel];
				dimension.Points.RemoveAt (sel);
				dimension.Points.Insert (sel+direction, t);

				this.table.ImportValues (data);

				this.UpdatePointsList (sel+direction);
			}
		}

		private void ChangePointValue()
		{
			int sel = this.pointsScrollList.SelectedItemIndex;
			var dimension = this.GetDimension;
			string value = this.pointValueField.Text;

			if (sel != -1)
			{
				var data = this.table.ExportValues ();

				if (dimension.HasDecimal)
				{
					decimal dv;
					if (decimal.TryParse (value, out dv))
					{
						dimension.Points[sel] = value;
						sel = dimension.Sort (sel);
					}
				}
				else
				{
					dimension.Points[sel] = value;
				}

				this.table.ImportValues (data);
			}

			this.UpdatePointsList (sel);
		}


		private int GetDimensionIndex(string code)
		{
			for (int i=0; i<this.table.Dimensions.Count (); i++)
			{
				if (this.table.Dimensions[i].Code == code)
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
				var dimension = this.GetDimension;

				if (dimension == null)
				{
					return null;
				}
				else
				{
					return dimension.Points.ToList ();
				}
			}
		}

		private DesignerDimension GetDimension
		{
			get
			{
				int sel = this.dimensionsScrollList.SelectedItemIndex;

				if (sel != -1)
				{
					return this.table.Dimensions[sel];
				}

				return null;
			}
		}


		private readonly ArticleDefinitionEntity			articleDefinitionEntity;
		private readonly DesignerTable						table;

		private ScrollList									articleParameterList;
		private Button										createDimensionButton;

		private ScrollList									dimensionsScrollList;
		private GroupBox									dimensionsRoundingPane;
		private RadioButton									dimensionsRadioRoundingNone;
		private RadioButton									dimensionsRadioRoundingNearest;
		private RadioButton									dimensionsRadioRoundingUp;
		private RadioButton									dimensionsRadioRoundingDown;

		private FrameBox									pointsToolbar;
		private GlyphButton									addPointsButton;
		private GlyphButton									removePointsButton;
		private GlyphButton									upPointsButton;
		private GlyphButton									downPointsButton;
		private ScrollList									pointsScrollList;
		
		private TextFieldEx									pointValueField;
	}
}
