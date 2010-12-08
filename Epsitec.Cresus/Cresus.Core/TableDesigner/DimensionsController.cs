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
				Margins = new Margins (10),
			};

			var dimensionPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 200,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 4, 0, 0),
			};

			var pointPane = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 200,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 4, 0, 0),
			};

			var parameterPane = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.CreateDimensionUI (dimensionPane);
			this.CreatePointUI (pointPane);
			this.CreateParameterUI (parameterPane);

			this.UpdateDimensionList ();
			this.UpdatePointList ();
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

			this.dimensionListController = new Controllers.ListController<AbstractDimension> (this.table.Dimensions.ToList (), this.DimensionListControllerItemToText, this.DimensionListControllerGetTextInfo, this.DimensionListControllerCreateItem);
			this.dimensionListController.CreateUI (parent, Direction.Right, 23);

			ToolTip.Default.SetToolTip (this.dimensionListController.AddButton,      "Crée un nouvel axe");
			ToolTip.Default.SetToolTip (this.dimensionListController.RemoveButton,   "Supprime l'axe sélectionné");
			ToolTip.Default.SetToolTip (this.dimensionListController.MoveUpButton,   "Monte l'axe dans la liste");
			ToolTip.Default.SetToolTip (this.dimensionListController.MoveDownButton, "Descend l'axe dans la liste");

			//	Connection des événements.
			this.dimensionListController.SelectedItemChanged += delegate
			{
				this.DimensionSelectedItemChanged ();
			};

			this.dimensionListController.ItemInserted += delegate
			{
				this.DimensionItemInserted ();
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
		}

		public void CreateParameterUI(Widget parent)
		{
			new StaticText
			{
				Parent = parent,
				PreferredHeight = 20,
				Dock = DockStyle.Top,
				Text = "<font size=\"16\">Paramètres</font>",
				Margins = new Margins (0, 0, 0, 10),
			};

			var pane = new FrameBox
			{
				Parent = parent,
				DrawFullFrame = true,
				Dock = DockStyle.Fill,
				Padding = new Margins (10),
			};


			int tabIndex = 1;

			{
				new StaticText
				{
					Parent = pane,
					Text = "Nom de l'axe :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.dimensionNameField = new TextField
				{
					Parent = pane,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};
			}

			{
				new StaticText
				{
					Parent = pane,
					Text = "Valeur du point :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
				};

				this.pointValueField = new TextField
				{
					Parent = pane,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderTextField),
					TabIndex = ++tabIndex,
				};
			}
		}


		private void UpdateDimensionList()
		{
			int sel = this.dimensionListController.SelectedIndex;
			this.dimensionListController.UpdateList (sel);
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


		private void DimensionSelectedItemChanged()
		{
			this.UpdatePointList ();
		}

		private void DimensionItemInserted()
		{
		}

		private void PointSelectedItemChanged()
		{
		}

		private void PointItemInserted()
		{
		}


		private List<decimal> GetPoints
		{
			get
			{
				int sel = this.dimensionListController.SelectedIndex;

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

		private Controllers.ListController<AbstractDimension>	dimensionListController;
		private Widget											pointListParent;
		private Controllers.ListController<decimal>				pointListController;
		private TextField										dimensionNameField;
		private TextField										pointValueField;
	}
}
