using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Layouts;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// La classe ObjectModifier permet de gérer les 'widgets' de Designer.
	/// </summary>
	public class ObjectModifier
	{
		public enum ChildrenPlacement
		{
			[Types.Hidden] None,
			Anchored,
			VerticalStacked,
			HorizontalStacked,
			Grid,
		}

		public enum AnchoredHorizontalAttachment
		{
			[Types.Hidden] None,
			Left,
			Right,
			Fill,
		}

		public enum AnchoredVerticalAttachment
		{
			[Types.Hidden] None,
			Bottom,
			Top,
			Fill,
		}

		public enum StackedHorizontalAttachment
		{
			[Types.Hidden] None,
			Left,
			Right,
			Fill,
		}

		public enum StackedVerticalAttachment
		{
			[Types.Hidden] None,
			Bottom,
			Top,
			Fill,
		}

		public enum StackedHorizontalAlignment
		{
			[Types.Hidden] None,
			Stretch,
			Center,
			Left,
			Right,
		}

		public enum StackedVerticalAlignment
		{
			[Types.Hidden] None,
			Stretch,
			Center,
			Bottom,
			Top,
			BaseLine,
		}

		public enum GridMode
		{
			[Types.Hidden] None,
			Auto,
			Absolute,
			Proportional,
		}


		public ObjectModifier(MyWidgets.PanelEditor panelEditor)
		{
			//	Constructeur unique.
			this.panelEditor = panelEditor;
		}

		protected UI.Panel Container
		{
			get
			{
				return this.panelEditor.Panel;
			}
		}

		#region ChildrenPlacement
		public bool AreChildrenAnchored(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Anchored);
			}
			return false;
		}

		public bool AreChildrenStacked(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked || p == ChildrenPlacement.VerticalStacked);
			}
			return false;
		}

		public bool AreChildrenGrid(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.Grid);
			}
			return false;
		}

		public bool AreChildrenHorizontal(Widget obj)
		{
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				ChildrenPlacement p = this.GetChildrenPlacement(obj);
				return (p == ChildrenPlacement.HorizontalStacked);
			}
			return false;
		}

		public bool HasChildrenPlacement(Widget obj)
		{
			//	Indique s'il existe un mode de placement des enfants de l'objet.
			AbstractGroup group = obj as AbstractGroup;
			return (group != null);
		}

		public ChildrenPlacement GetChildrenPlacement(Widget obj)
		{
			//	Retourne le mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			if (group != null)
			{
				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Anchored)
				{
					return ChildrenPlacement.Anchored;
				}

				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Stacked)
				{
					if (group.ContainerLayoutMode == ContainerLayoutMode.HorizontalFlow)
					{
						return ChildrenPlacement.HorizontalStacked;
					}
					else
					{
						return ChildrenPlacement.VerticalStacked;
					}
				}

				if (group.ChildrenLayoutMode == Widgets.Layouts.LayoutMode.Grid)
				{
					return ChildrenPlacement.Grid;
				}
			}

			return ChildrenPlacement.None;
		}

		public void SetChildrenPlacement(Widget obj, ChildrenPlacement mode)
		{
			//	Choix du mode de placement des enfants de l'objet.
			//	Uniquement pour les objects AbstractGroup.
			AbstractGroup group = obj as AbstractGroup;
			System.Diagnostics.Debug.Assert(group != null);

			switch (mode)
			{
				case ChildrenPlacement.Anchored:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Anchored;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.HorizontalStacked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
					group.ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.VerticalStacked:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Stacked;
					group.ContainerLayoutMode = ContainerLayoutMode.VerticalFlow;
					LayoutEngine.SetLayoutEngine(group, null);
					break;

				case ChildrenPlacement.Grid:
					group.ChildrenLayoutMode = Widgets.Layouts.LayoutMode.Grid;
					this.SetGridColumnsCount(obj, 2);
					this.SetGridRowsCount(obj, 2);
					break;
			}

			this.Invalidate();
		}
		#endregion


		#region Grid
		public int GetGridColumnsCount(Widget obj)
		{
			//	Retourne le nombre total de colonnes de l'objet group.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					return engine.ColumnDefinitions.Count;
				}
			}

			return 0;
		}

		public void SetGridColumnsCount(Widget obj, int columns)
		{
			//	Détermine le nombre total de colonnes de l'objet group.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			if (engine == null)
			{
				engine = new GridLayoutEngine();
				LayoutEngine.SetLayoutEngine(obj, engine);
			}

			columns = System.Math.Max(columns, engine.MaxColumnIndex+1);

			while (columns != engine.ColumnDefinitions.Count)
			{
				int count = engine.ColumnDefinitions.Count;

				if (columns > engine.ColumnDefinitions.Count)
				{
					engine.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(100, GridUnitType.Auto)));
					engine.ColumnDefinitions[count].MinWidth = 20;
				}
				else
				{
					engine.ColumnDefinitions.RemoveAt(count-1);
				}
			}

			this.Invalidate();
		}

		public int GetGridRowsCount(Widget obj)
		{
			//	Retourne le nombre total de colonnes de l'objet group.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					return engine.RowDefinitions.Count;
				}
			}

			return 0;
		}

		public void SetGridRowsCount(Widget obj, int rows)
		{
			//	Détermine le nombre total de colonnes de l'objet group.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			if (engine == null)
			{
				engine = new GridLayoutEngine();
				LayoutEngine.SetLayoutEngine(obj, engine);
			}

			rows = System.Math.Max(rows, engine.MaxRowIndex+1);

			while (rows != engine.RowDefinitions.Count)
			{
				int count = engine.RowDefinitions.Count;

				if (rows > engine.RowDefinitions.Count)
				{
					engine.RowDefinitions.Add(new RowDefinition(new GridLength(100, GridUnitType.Auto)));
					engine.RowDefinitions[count].MinHeight = 20;
				}
				else
				{
					engine.RowDefinitions.RemoveAt(count-1);
				}
			}

			this.Invalidate();
		}


		public GridMode GetGridColumnMode(Widget obj, int column)
		{
			//	Retourne le mode d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					GridLength gl = def.Width;
					return ObjectModifier.GridConvert(gl.GridUnitType);
				}
			}

			return GridMode.None;
		}

		public void SetGridColumnMode(Widget obj, int column, GridMode mode)
		{
			//	Modifie le mode d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			if (this.GetGridColumnMode(obj, column) != mode)
			{
				GridUnitType type = ObjectModifier.GridConvert(mode);
				double value = (type == GridUnitType.Absolute) ? 100 : 100;

				ColumnDefinition def = engine.ColumnDefinitions[column];
				def.Width = new GridLength(value, type);

				this.Invalidate();
			}
		}

		public GridMode GetGridRowMode(Widget obj, int row)
		{
			//	Retourne le mode d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					GridLength gl = def.Height;
					return ObjectModifier.GridConvert(gl.GridUnitType);
				}
			}

			return GridMode.None;
		}

		public void SetGridRowMode(Widget obj, int row, GridMode mode)
		{
			//	Modifie le mode d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			if (this.GetGridRowMode(obj, row) != mode)
			{
				GridUnitType type = ObjectModifier.GridConvert(mode);
				double value = (type == GridUnitType.Absolute) ? 20 : 100;

				RowDefinition def = engine.RowDefinitions[row];
				def.Height = new GridLength(value, type);

				this.Invalidate();
			}
		}

		public double GetGridColumnWidth(Widget obj, int column)
		{
			//	Retourne la largeur d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					GridLength gl = def.Width;
					return gl.Value;
				}
			}

			return 0;
		}

		public void SetGridColumnWidth(Widget obj, int column, double width)
		{
			//	Modifie la largeur d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			def.Width = new GridLength(width, def.Width.GridUnitType);

			this.Invalidate();
		}

		public double GetGridRowHeight(Widget obj, int row)
		{
			//	Retourne la hauteur d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					GridLength gl = def.Height;
					return gl.Value;
				}
			}

			return 0;
		}

		public void SetGridRowHeight(Widget obj, int row, double height)
		{
			//	Modifie la hauteur d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			def.Height = new GridLength(height, def.Height.GridUnitType);

			this.Invalidate();
		}

		static protected GridUnitType GridConvert(GridMode mode)
		{
			switch (mode)
			{
				case GridMode.Proportional:  return GridUnitType.Proportional;
				case GridMode.Absolute:      return GridUnitType.Absolute;
				case GridMode.Auto:          return GridUnitType.Auto;
			}

			return GridUnitType.Proportional;
		}

		static protected GridMode GridConvert(GridUnitType type)
		{
			switch (type)
			{
				case GridUnitType.Proportional:  return GridMode.Proportional;
				case GridUnitType.Absolute:      return GridMode.Absolute;
				case GridUnitType.Auto:          return GridMode.Auto;
			}

			return GridMode.None;
		}


		public double GetGridColumnMinWidth(Widget obj, int column)
		{
			//	Retourne la largeur minimale d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					return def.MinWidth;
				}
			}

			return 0;
		}

		public void SetGridColumnMinWidth(Widget obj, int column, double width)
		{
			//	Modifie la largeur minimale d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.MinWidth != width)
			{
				def.MinWidth = width;
				this.Invalidate();
			}
		}

		public double GetGridColumnMaxWidth(Widget obj, int column)
		{
			//	Retourne la largeur maximale d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					if (System.Double.IsInfinity(def.MaxWidth))  return 9999;  // TODO: à supprimer !
					return def.MaxWidth;
				}
			}

			return 0;
		}

		public void SetGridColumnMaxWidth(Widget obj, int column, double width)
		{
			//	Modifie la largeur maximale d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.MaxWidth != width)
			{
				def.MaxWidth = width;
				this.Invalidate();
			}
		}

		public double GetGridRowMinHeight(Widget obj, int row)
		{
			//	Retourne la hauteur minimale d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					return def.MinHeight;
				}
			}

			return 0;
		}

		public void SetGridRowMinHeight(Widget obj, int row, double height)
		{
			//	Modifie la hauteur minimale d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.MinHeight != height)
			{
				def.MinHeight = height;
				this.Invalidate();
			}
		}

		public double GetGridRowMaxHeight(Widget obj, int row)
		{
			//	Retourne la hauteur maximale d'une ligne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					if (System.Double.IsInfinity(def.MaxHeight))  return 9999;  // TODO: à supprimer !
					return def.MaxHeight;
				}
			}

			return 0;
		}

		public void SetGridRowMaxHeight(Widget obj, int row, double height)
		{
			//	Modifie la hauteur maximale d'une ligne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.MaxHeight != height)
			{
				def.MaxHeight = height;
				this.Invalidate();
			}
		}


		public double GetGridColumnLeftBorder(Widget obj, int column)
		{
			//	Retourne le bord gauche d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					return def.LeftBorder;
				}
			}

			return 0;
		}

		public void SetGridColumnLeftBorder(Widget obj, int column, double value)
		{
			//	Modifie le bord gauche d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.LeftBorder != value)
			{
				def.LeftBorder = value;
				this.Invalidate();
			}
		}

		public double GetGridColumnRightBorder(Widget obj, int column)
		{
			//	Retourne le bord droite d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					ColumnDefinition def = engine.ColumnDefinitions[column];
					return def.RightBorder;
				}
			}

			return 0;
		}

		public void SetGridColumnRightBorder(Widget obj, int column, double value)
		{
			//	Modifie le bord droite d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			ColumnDefinition def = engine.ColumnDefinitions[column];
			if (def.RightBorder != value)
			{
				def.RightBorder = value;
				this.Invalidate();
			}
		}


		public double GetGridRowTopBorder(Widget obj, int row)
		{
			//	Retourne le bord supérieur d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					return def.TopBorder;
				}
			}

			return 0;
		}

		public void SetGridRowTopBorder(Widget obj, int row, double value)
		{
			//	Modifie le bord supérieur d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.TopBorder != value)
			{
				def.TopBorder = value;
				this.Invalidate();
			}
		}

		public double GetGridRowBottomBorder(Widget obj, int row)
		{
			//	Retourne le bord inférieur d'une colonne.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					RowDefinition def = engine.RowDefinitions[row];
					return def.BottomBorder;
				}
			}

			return 0;
		}

		public void SetGridRowBottomBorder(Widget obj, int row, double value)
		{
			//	Modifie le bord inférieur d'une colonne.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj));

			GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
			System.Diagnostics.Debug.Assert(engine != null);

			RowDefinition def = engine.RowDefinitions[row];
			if (def.BottomBorder != value)
			{
				def.BottomBorder = value;
				this.Invalidate();
			}
		}


		public bool IsGridCellEmpty(Widget obj, Widget exclude, int column, int row, int columnCount, int rowCount)
		{
			if (column < 0 || column+columnCount > this.GetGridColumnsCount(obj))
			{
				return false;
			}

			if (row < 0 || row+rowCount > this.GetGridRowsCount(obj))
			{
				return false;
			}

			for (int c=column; c<column+columnCount; c++)
			{
				for (int r=row; r<row+rowCount; r++)
				{
					if (!this.IsGridCellEmpty(obj, exclude, c, r))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool IsGridCellEmpty(Widget obj, Widget exclude, int column, int row)
		{
			//	Indique si une cellule est libre, donc si elle ne contient aucun widget.
			if (column == GridSelection.Invalid || row == GridSelection.Invalid)
			{
				return false;
			}

			return (this.GetGridCellWidget(obj, exclude, column, row) == null);
		}

		public Widget GetGridCellWidget(Widget container, Widget exclude, int column, int row)
		{
			//	Retourne le premier widget occupant une cellule donnée.
			foreach (Widget widget in this.GetGridCellWidgets(container, exclude, column, row))
			{
				return widget;
			}
			
			return null;
		}

		protected IEnumerable<Widget> GetGridCellWidgets(Widget container, Widget exclude, int column, int row)
		{
			//	Retourne tous les widgets occupant une cellule donnée, en tenant
			//	compte de leur span.
			if (this.AreChildrenGrid(container) && container.HasChildren)
			{
				foreach (Widget child in container.Children)
				{
					if (child == exclude)
					{
						continue;
					}

					int c = GridLayoutEngine.GetColumn(child);
					int r = GridLayoutEngine.GetRow(child);

					if (c < 0 || c > column)
					{
						continue;
					}
					if (r < 0 || r > row)
					{
						continue;
					}

					int columnSpan = GridLayoutEngine.GetColumnSpan(child);
					int rowSpan    = GridLayoutEngine.GetRowSpan(child);

					//	Si le widget implémente IGridPermeable, cela implique qu'il a son
					//	mot à dire sur le nombre de lignes et/ou colonnes qu'il utilise :
					IGridPermeable permeable = child as IGridPermeable;
					
					if (permeable != null)
					{
						permeable.UpdateGridSpan(ref columnSpan, ref rowSpan);
					}
					
					if (c <= column && c+columnSpan-1 >= column &&
						r <= row    && r+rowSpan-1    >= row    )
					{
						yield return child;
					}
				}
			}
		}


		public void SetGridClear(Widget obj)
		{
			//	Annule les informations d'appartenance à une cellule, lorsque l'objet n'est
			//	plus dans un tableau.
			GridLayoutEngine.SetColumn(obj, -1);
			GridLayoutEngine.SetRow(obj, -1);

			GridLayoutEngine.SetColumnSpan(obj, 1);
			GridLayoutEngine.SetRowSpan(obj, 1);
		}

		public void SetGridParentColumnRow(Widget obj, Widget parent, int column, int row)
		{
			//	Détermine la cellule dans un tableau à laquelle appartient l'objet.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(parent));

			obj.Anchor = AnchorStyles.None;
			obj.Dock = DockStyle.None;

			GridLayoutEngine.SetColumn(obj, column);
			GridLayoutEngine.SetRow(obj, row);

			if (obj.Parent != parent)
			{
				obj.SetParent(parent);
			}
		}

		public int GetGridColumn(Widget obj)
		{
			//	Retourne la colonne à laquelle appartient l'objet.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetColumn(obj);
			}

			return 0;
		}

		public int GetGridRow(Widget obj)
		{
			//	Retourne la colonne à laquelle appartient l'objet.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetRow(obj);
			}

			return 0;
		}


		public void SetGridColumnSpan(Widget obj, int span)
		{
			//	Détermine le nombre de colonnes occupées.
			//	Empêche le chevauchement de deux objets.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj.Parent));

			int ic = GridLayoutEngine.GetColumn(obj);
			int ir = GridLayoutEngine.GetRow(obj);
			int irs = GridLayoutEngine.GetRowSpan(obj);
			GridLayoutEngine.SetColumnSpan(obj, 1);
			GridLayoutEngine.SetRowSpan(obj, 1);

			span = System.Math.Min(span, this.GetGridColumnsCount(obj.Parent)-ic);

			int maxSpan = span;
			for (int c=0; c<span; c++)
			{
				for (int r=0; r<irs; r++)
				{
					if (!this.IsGridCellEmpty(obj.Parent, obj, ic+c, ir+r))
					{
						maxSpan = System.Math.Min(maxSpan, c);
						break;
					}
				}
			}

			span = System.Math.Min(span, maxSpan);
			GridLayoutEngine.SetColumnSpan(obj, span);
			GridLayoutEngine.SetRowSpan(obj, irs);
			this.Invalidate();
		}

		public int GetGridColumnSpan(Widget obj)
		{
			//	Retourne le nombre de colonnes occupées.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetColumnSpan(obj);
			}

			return 0;
		}

		public void SetGridRowSpan(Widget obj, int span)
		{
			//	Détermine le nombre de lignes occupées.
			//	Empêche le chevauchement de deux objets.
			System.Diagnostics.Debug.Assert(this.AreChildrenGrid(obj.Parent));

			int ic = GridLayoutEngine.GetColumn(obj);
			int ir = GridLayoutEngine.GetRow(obj);
			int ics = GridLayoutEngine.GetColumnSpan(obj);
			GridLayoutEngine.SetColumnSpan(obj, 1);
			GridLayoutEngine.SetRowSpan(obj, 1);

			span = System.Math.Min(span, this.GetGridRowsCount(obj.Parent)-ir);

			int maxSpan = span;
			for (int r=0; r<span; r++)
			{
				for (int c=0; c<ics; c++)
				{
					if (!this.IsGridCellEmpty(obj.Parent, obj, ic+c, ir+r))
					{
						maxSpan = System.Math.Min(maxSpan, r);
						break;
					}
				}
			}

			span = System.Math.Min(span, maxSpan);
			GridLayoutEngine.SetRowSpan(obj, span);
			GridLayoutEngine.SetColumnSpan(obj, ics);
			this.Invalidate();
		}

		public int GetGridRowSpan(Widget obj)
		{
			//	Retourne le nombre de lignes occupées.
			if (this.AreChildrenGrid(obj.Parent))
			{
				return GridLayoutEngine.GetRowSpan(obj);
			}

			return 0;
		}


		public Rectangle GetGridItemArea(Widget obj, GridSelection.OneItem item)
		{
			//	Retourne la zone rectangulaire correspondant à une sélection.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null && item.Index != GridSelection.Invalid)
				{
					if (item.Unit == GridSelection.Unit.Column)
					{
						int x = item.Index;
						double x1 = this.GetGridColumnPosition(obj, x);
						double x2 = this.GetGridColumnPosition(obj, x+1);
						double y1 = this.GetGridRowPosition(obj, engine.RowDefinitions.Count);
						double y2 = this.GetGridRowPosition(obj, 0);
						return new Rectangle(x1, y1, x2-x1, y2-y1);
					}

					if (item.Unit == GridSelection.Unit.Row)
					{
						int y = item.Index;
						double x1 = this.GetGridColumnPosition(obj, 0);
						double x2 = this.GetGridColumnPosition(obj, engine.ColumnDefinitions.Count);
						double y1 = this.GetGridRowPosition(obj, y+1);
						double y2 = this.GetGridRowPosition(obj, y);
						return new Rectangle(x1, y1, x2-x1, y2-y1);
					}
				}
			}

			return Rectangle.Empty;
		}

		public Rectangle GetGridCellArea(Widget obj, int column, int row, int columnCount, int rowCount)
		{
			//	Retourne le rectangle d'une cellule.
			if (this.AreChildrenGrid(obj))
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					double x1 = this.GetGridColumnPosition(obj, column);
					double x2 = this.GetGridColumnPosition(obj, column+columnCount);
					double y1 = this.GetGridRowPosition(obj, row+rowCount);
					double y2 = this.GetGridRowPosition(obj, row);
					return new Rectangle(x1, y1, x2-x1, y2-y1);
				}
			}

			return Rectangle.Empty;
		}

		public double GetGridColumnPosition(Widget obj, int index)
		{
			//	Retourne la position d'une colonne.
			//	Accepte des index < 0 ou > que le nombre de colonnes, en prenant comme
			//	valeur la première ou la dernière colonne.
			if (this.AreChildrenGrid(obj) && index != GridSelection.Invalid)
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					Rectangle rect = this.GetFinalPadding(obj);
					double position = rect.Left;

					while (index < 0)
					{
						ColumnDefinition def = engine.ColumnDefinitions[0];
						position -= def.LeftBorder;
						position -= def.ActualWidth;
						position -= def.RightBorder;
						index++;
					}

					for (int i=0; i<index; i++)
					{
						int ii = System.Math.Min(i, engine.ColumnCount-1);
						ColumnDefinition def = engine.ColumnDefinitions[ii];
						position += def.LeftBorder;
						position += def.ActualWidth;
						position += def.RightBorder;
					}

					return position;
				}
			}

			return 0;
		}

		public double GetGridRowPosition(Widget obj, int index)
		{
			//	Retourne la position d'une ligne.
			//	Accepte des index < 0 ou > que le nombre de lignes, en prenant comme
			//	valeur la première ou la dernière ligne.
			if (this.AreChildrenGrid(obj) && index != GridSelection.Invalid)
			{
				GridLayoutEngine engine = LayoutEngine.GetLayoutEngine(obj) as GridLayoutEngine;
				if (engine != null)
				{
					Rectangle rect = this.GetFinalPadding(obj);
					double position = rect.Top;

					while (index < 0)
					{
						RowDefinition def = engine.RowDefinitions[0];
						position += def.TopBorder;
						position += def.ActualHeight;
						position += def.BottomBorder;
						index++;
					}

					for (int i=0; i<index; i++)
					{
						int ii = System.Math.Min(i, engine.RowCount-1);
						RowDefinition def = engine.RowDefinitions[ii];
						position -= def.TopBorder;
						position -= def.ActualHeight;
						position -= def.BottomBorder;
					}

					return position;
				}
			}

			return 0;
		}
		#endregion


		public void AdaptFromParent(Widget obj, ObjectModifier.StackedHorizontalAttachment ha, ObjectModifier.StackedVerticalAttachment va)
		{
			//	Adapte un objet pour son parent.
			if (this.AreChildrenAnchored(obj.Parent))
			{
				if (this.GetAnchoredVerticalAttachment(obj) == ObjectModifier.AnchoredVerticalAttachment.None)
				{
					this.SetAnchoredHorizontalAttachment(obj, ObjectModifier.AnchoredHorizontalAttachment.Left);
					this.SetAnchoredVerticalAttachment(obj, ObjectModifier.AnchoredVerticalAttachment.Bottom);
				}
			}

			if (this.AreChildrenStacked(obj.Parent))
			{
				if (this.GetStackedHorizontalAttachment(obj) == ObjectModifier.StackedHorizontalAttachment.None||
					this.GetStackedVerticalAttachment(obj) == ObjectModifier.StackedVerticalAttachment.None)
				{
					this.SetMargins(obj, new Margins(5, 5, 5, 5));

					if (this.AreChildrenHorizontal(obj.Parent))
					{
						this.SetStackedHorizontalAttachment(obj, ha);
					}
					else
					{
						this.SetStackedVerticalAttachment(obj, va);
					}
				}
			}

			if (this.AreChildrenGrid(obj.Parent))
			{
				this.SetMargins(obj, new Margins(0, 0, 0, 0));
			}

			if (obj is StaticText)
			{
				obj.PreferredHeight = obj.MinHeight;
			}

			if (obj is Button)
			{
				obj.PreferredHeight = obj.MinHeight;
			}

			if (obj is TextField)
			{
				obj.PreferredHeight = obj.MinHeight;
			}
		}


		public Rectangle GetActualBounds(Widget obj)
		{
			//	Retourne la position et les dimensions actuelles de l'objet.
			//	Le rectangle rendu est toujours valide, quel que soit le mode d'attachement.
			obj.Window.ForceLayout();
			Rectangle bounds = obj.Client.Bounds;

			while (obj != this.Container)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
		}

		public bool HasPreferredBounds(Widget obj)
		{
			//	Indique si l'objet a une position et des dimensions modifiables.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.Anchored || placement == ChildrenPlacement.Grid);
		}

		public Rectangle GetPreferredBounds(Widget obj)
		{
			//	Retourne la position et les dimensions de l'objet.
			//	Le rectangle rendu est toujours valide, quel que soit le mode d'attachement.
#if false
			obj.Window.ForceLayout();
			Rectangle bounds = new Rectangle(Point.Zero, obj.PreferredSize);

			while (obj != this.Container)
			{
				bounds = obj.MapClientToParent(bounds);
				obj = obj.Parent;
			}

			return bounds;
#else
			Point center = this.GetActualBounds(obj).Center;
			Size size = obj.PreferredSize;
			return new Rectangle(center.X-size.Width/2, center.Y-size.Height/2, size.Width, size.Height);
#endif
		}

		public void SetPreferredBounds(Widget obj, Rectangle bounds)
		{
			//	Choix de la position et des dimensions de l'objet.
			//	Uniquement pour les objets Anchored ou Grid.
			System.Diagnostics.Debug.Assert(this.HasPreferredBounds(obj));

			bounds.Normalise();

			if (bounds.Width < obj.MinWidth)
			{
				bounds.Width = obj.MinWidth;
			}

			if (bounds.Height < obj.MinHeight)
			{
				bounds.Height = obj.MinHeight;
			}

			obj.Window.ForceLayout();
			Widget parent = obj.Parent;
			while (parent != this.Container)
			{
				bounds = parent.MapParentToClient(bounds);
				parent = parent.Parent;
			}

			parent = obj.Parent;
			Rectangle box = parent.ActualBounds;
			Margins margins = obj.Margins;
			Margins padding = parent.Padding + parent.GetInternalPadding();
			AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
			AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);

			if (ha == AnchoredHorizontalAttachment.Left || ha == AnchoredHorizontalAttachment.Fill)
			{
				double px = bounds.Left;
				px -= padding.Left;
				px = System.Math.Max(px, 0);
				margins.Left = px;
			}

			if (ha == AnchoredHorizontalAttachment.Right || ha == AnchoredHorizontalAttachment.Fill)
			{
				double px = box.Width - bounds.Right;
				px -= padding.Right;
				px = System.Math.Max(px, 0);
				margins.Right = px;
			}

			if (va == AnchoredVerticalAttachment.Bottom || va == AnchoredVerticalAttachment.Fill)
			{
				double py = bounds.Bottom;
				py -= padding.Bottom;
				py = System.Math.Max(py, 0);
				margins.Bottom = py;
			}

			if (va == AnchoredVerticalAttachment.Top || va == AnchoredVerticalAttachment.Fill)
			{
				double py = box.Height - bounds.Top;
				py -= padding.Top;
				py = System.Math.Max(py, 0);
				margins.Top = py;
			}

			obj.Margins = margins;
			obj.PreferredSize = bounds.Size;

			this.Invalidate();
		}


		public bool HasMargins(Widget obj)
		{
			//	Indique si l'objet a des marges.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.HorizontalStacked || placement == ChildrenPlacement.VerticalStacked || placement == ChildrenPlacement.Grid)
			{
				return true;
			}

			return false;
		}

		public Margins GetMargins(Widget obj)
		{
			//	Retourne les marges de l'objet.
			//	Uniquement pour les objets Stacked et Grid.
			if (this.HasMargins(obj))
			{
				return obj.Margins;
			}

			return Margins.Zero;
		}

		public void SetMargins(Widget obj, Margins margins)
		{
			//	Choix des marges de l'objet.
			//	Uniquement pour les objets Stacked et Grid.
			System.Diagnostics.Debug.Assert(this.HasMargins(obj));

			if (obj.Margins != margins)
			{
				obj.Margins = margins;
				this.Invalidate();
			}
		}


		public bool HasPadding(Widget obj)
		{
			//	Indique si l'objet a des marges internes.
			return (obj is AbstractGroup);
		}

		public Margins GetPadding(Widget obj)
		{
			//	Retourne les marges internes de l'objet.
			//	Uniquement pour les objets AbstractGroup.
			if (this.HasPadding(obj))
			{
				return obj.Padding;
			}

			return Margins.Zero;
		}

		public void SetPadding(Widget obj, Margins padding)
		{
			//	Choix des marges internes de l'objet.
			//	Uniquement pour les objets AbstractGroup.
			System.Diagnostics.Debug.Assert(this.HasPadding(obj));

			if (obj.Padding != padding)
			{
				obj.Padding = padding;
				this.Invalidate();
			}
		}

		public Rectangle GetFinalPadding(Widget obj)
		{
			//	Retourne le rectangle intérieur d'un objet AbstractGroup.
			Rectangle bounds = this.GetActualBounds(obj);

			if (this.HasPadding(obj))
			{
				bounds.Deflate(obj.GetInternalPadding());
				bounds.Deflate(obj.Padding);
			}

			return bounds;
		}


		public bool HasPreferredWidth(Widget obj)
		{
			//	Indique s'il est possible de modifier la largeur d'un objet.
			//	A ne pas confondre avec SetPreferredBounds pour le mode ancré. Un objet ancré
			//	pour lequel on peut faire un SetPreferredBounds n'accepte pas le SetPreferredWidth !
			if (!HandlesList.HasWidthHandles(obj))
			{
				return false;
			}

			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				return true;
			}

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				StackedHorizontalAlignment ha = this.GetStackedHorizontalAlignment(obj);
				return (ha != StackedHorizontalAlignment.Stretch && ha != StackedHorizontalAlignment.None);
			}

			return false;
		}

		public double GetPreferredWidth(Widget obj)
		{
			//	Retourne la largeur de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			if (this.HasPreferredWidth(obj))
			{
				return obj.PreferredWidth;
			}

			return 0;
		}

		public void SetPreferredWidth(Widget obj, double width)
		{
			//	Choix de la largeur de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			System.Diagnostics.Debug.Assert(this.HasPreferredWidth(obj));

			if (obj.PreferredWidth != width)
			{
				obj.PreferredWidth = width;
				this.Invalidate();
			}
		}


		public bool HasPreferredHeight(Widget obj)
		{
			//	Indique s'il est possible de modifier la hauteur d'un objet.
			//	A ne pas confondre avec SetPreferredBounds pour le mode ancré. Un objet ancré
			//	pour lequel on peut faire un SetPreferredBounds n'accepte pas le HasPreferredHeight !
			if (!HandlesList.HasHeightHandles(obj))
			{
				return false;
			}

			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				return true;
			}

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				StackedVerticalAlignment ha = this.GetStackedVerticalAlignment(obj);
				return (ha != StackedVerticalAlignment.Stretch && ha != StackedVerticalAlignment.None);
			}

			return false;
		}

		public double GetPreferredHeight(Widget obj)
		{
			//	Retourne la hauteur de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			if (this.HasPreferredHeight(obj))
			{
				return obj.PreferredHeight;
			}

			return 0;
		}

		public void SetPreferredHeight(Widget obj, double height)
		{
			//	Choix de la hauteur de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			System.Diagnostics.Debug.Assert(this.HasPreferredHeight(obj));

			if (obj.PreferredHeight != height)
			{
				obj.PreferredHeight = height;
				this.Invalidate();
			}
		}


		public int GetZOrder(Widget obj)
		{
			//	Retourne l'ordre de l'objet.
			return obj.ZOrder;
		}

		public void SetZOrder(Widget obj, int order)
		{
			//	Choix de l'ordre de l'objet.
			if (obj.ZOrder != order)
			{
				obj.ZOrder = order;
				this.Invalidate();
			}
		}


		public bool HasAttachmentLeft(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Right);
			}

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Left);
			}

			return false;
		}

		public bool HasAttachmentRight(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredHorizontalAttachment ha = this.GetAnchoredHorizontalAttachment(obj);
				return (ha != AnchoredHorizontalAttachment.Left);
			}

			if (placement == ChildrenPlacement.HorizontalStacked)
			{
				StackedHorizontalAttachment ha = this.GetStackedHorizontalAttachment(obj);
				return (ha == StackedHorizontalAttachment.Right);
			}

			return false;
		}

		public bool HasAttachmentBottom(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Top);
			}

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				StackedVerticalAttachment va = this.GetStackedVerticalAttachment(obj);
				return (va == StackedVerticalAttachment.Bottom);
			}

			return false;
		}

		public bool HasAttachmentTop(Widget obj)
		{
			ChildrenPlacement placement = this.GetParentPlacement(obj);

			if (placement == ChildrenPlacement.Anchored)
			{
				AnchoredVerticalAttachment va = this.GetAnchoredVerticalAttachment(obj);
				return (va != AnchoredVerticalAttachment.Bottom);
			}

			if (placement == ChildrenPlacement.VerticalStacked)
			{
				StackedVerticalAttachment va = this.GetStackedVerticalAttachment(obj);
				return (va == StackedVerticalAttachment.Top);
			}

			return false;
		}


		#region Anchored
		public AnchoredVerticalAttachment GetAnchoredVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool bottom = ((style & AnchorStyles.Bottom) != 0);
				bool top    = ((style & AnchorStyles.Top   ) != 0);

				if (bottom && top)  return AnchoredVerticalAttachment.Fill;
				if (bottom       )  return AnchoredVerticalAttachment.Bottom;
				if (top          )  return AnchoredVerticalAttachment.Top;
			}

			return AnchoredVerticalAttachment.None;
		}

		public void SetAnchoredVerticalAttachment(Widget obj, AnchoredVerticalAttachment attachment)
		{
			//	Choix de l'attachement vertical de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

			AnchorStyles style = obj.Anchor;

			switch (attachment)
			{
				case AnchoredVerticalAttachment.Bottom:
					style |=  AnchorStyles.Bottom;
					style &= ~AnchorStyles.Top;
					break;

				case AnchoredVerticalAttachment.Top:
					style |=  AnchorStyles.Top;
					style &= ~AnchorStyles.Bottom;
					break;

				case AnchoredVerticalAttachment.Fill:
					style |=  AnchorStyles.Bottom;
					style |=  AnchorStyles.Top;
					break;
			}

			if (obj.Anchor != style)
			{
				obj.Anchor = style;
				obj.Dock = DockStyle.None;
				this.Invalidate();
			}
		}


		public AnchoredHorizontalAttachment GetAnchoredHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			if (placement == ChildrenPlacement.Anchored)
			{
				AnchorStyles style = obj.Anchor;
				bool left  = ((style & AnchorStyles.Left ) != 0);
				bool right = ((style & AnchorStyles.Right) != 0);

				if (left && right)  return AnchoredHorizontalAttachment.Fill;
				if (left         )  return AnchoredHorizontalAttachment.Left;
				if (right        )  return AnchoredHorizontalAttachment.Right;
			}

			return AnchoredHorizontalAttachment.None;
		}

		public void SetAnchoredHorizontalAttachment(Widget obj, AnchoredHorizontalAttachment attachment)
		{
			//	Choix de l'attachement horizontal de l'objet.
			//	Uniquement pour les objets Anchored.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			System.Diagnostics.Debug.Assert(placement == ChildrenPlacement.Anchored);

			AnchorStyles style = obj.Anchor;

			switch (attachment)
			{
				case AnchoredHorizontalAttachment.Left:
					style |=  AnchorStyles.Left;
					style &= ~AnchorStyles.Right;
					break;

				case AnchoredHorizontalAttachment.Right:
					style |=  AnchorStyles.Right;
					style &= ~AnchorStyles.Left;
					break;

				case AnchoredHorizontalAttachment.Fill:
					style |=  AnchorStyles.Left;
					style |=  AnchorStyles.Right;
					break;
			}

			if (obj.Anchor != style)
			{
				obj.Anchor = style;
				obj.Dock = DockStyle.None;
				this.Invalidate();
			}
		}
		#endregion


		#region Stacked
		public bool HasStackedVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.VerticalStacked);
		}

		public StackedVerticalAttachment GetStackedVerticalAttachment(Widget obj)
		{
			//	Retourne l'attachement vertical de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			if (this.HasStackedVerticalAttachment(obj))
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.StackFill )  return StackedVerticalAttachment.Fill;
				if (style == DockStyle.StackEnd  )  return StackedVerticalAttachment.Bottom;
				if (style == DockStyle.StackBegin)  return StackedVerticalAttachment.Top;
			}

			return StackedVerticalAttachment.None;
		}

		public void SetStackedVerticalAttachment(Widget obj, StackedVerticalAttachment attachment)
		{
			//	Choix de l'attachement vertical de l'objet.
			//	Uniquement pour les objets VerticalStacked.
			System.Diagnostics.Debug.Assert(this.HasStackedVerticalAttachment(obj));

			DockStyle style = obj.Dock;

			switch (attachment)
			{
				case StackedVerticalAttachment.Bottom:
					style = DockStyle.StackEnd;
					break;

				case StackedVerticalAttachment.Top:
					style = DockStyle.StackBegin;
					break;

				case StackedVerticalAttachment.Fill:
					style = DockStyle.StackFill;
					break;
			}

			if (obj.Dock != style)
			{
				obj.Dock = style;
				obj.Anchor = AnchorStyles.None;
				this.Invalidate();
			}
		}


		public bool HasStackedHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalStacked);
		}

		public StackedHorizontalAttachment GetStackedHorizontalAttachment(Widget obj)
		{
			//	Retourne l'attachement horizontal de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			if (this.HasStackedHorizontalAttachment(obj))
			{
				DockStyle style = obj.Dock;
				if (style == DockStyle.StackFill )  return StackedHorizontalAttachment.Fill;
				if (style == DockStyle.StackBegin)  return StackedHorizontalAttachment.Left;
				if (style == DockStyle.StackEnd  )  return StackedHorizontalAttachment.Right;
			}

			return StackedHorizontalAttachment.None;
		}

		public void SetStackedHorizontalAttachment(Widget obj, StackedHorizontalAttachment attachment)
		{
			//	Choix de l'attachement horizontal de l'objet.
			//	Uniquement pour les objets HorizontalStacked.
			System.Diagnostics.Debug.Assert(this.HasStackedHorizontalAttachment(obj));

			DockStyle style = obj.Dock;

			switch (attachment)
			{
				case StackedHorizontalAttachment.Left:
					style = DockStyle.StackBegin;
					break;

				case StackedHorizontalAttachment.Right:
					style = DockStyle.StackEnd;
					break;

				case StackedHorizontalAttachment.Fill:
					style = DockStyle.StackFill;
					break;
			}

			if (obj.Dock != style)
			{
				obj.Dock = style;
				obj.Anchor = AnchorStyles.None;
				this.Invalidate();
			}
		}


		public bool HasStackedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalStacked || placement == ChildrenPlacement.VerticalStacked || placement == ChildrenPlacement.Grid);
		}

		public StackedHorizontalAlignment GetStackedHorizontalAlignment(Widget obj)
		{
			//	Retourne l'alignement horizontal de l'objet.
			if (this.HasStackedHorizontalAlignment(obj))
			{
				HorizontalAlignment ha = obj.HorizontalAlignment;
				if (ha == HorizontalAlignment.Stretch)  return StackedHorizontalAlignment.Stretch;
				if (ha == HorizontalAlignment.Center )  return StackedHorizontalAlignment.Center;
				if (ha == HorizontalAlignment.Left   )  return StackedHorizontalAlignment.Left;
				if (ha == HorizontalAlignment.Right  )  return StackedHorizontalAlignment.Right;
			}

			return StackedHorizontalAlignment.None;
		}

		public void SetStackedHorizontalAlignment(Widget obj, StackedHorizontalAlignment alignment)
		{
			//	Choix de l'alignement horizontal de l'objet.
			System.Diagnostics.Debug.Assert(this.HasStackedHorizontalAlignment(obj));

			HorizontalAlignment ha = obj.HorizontalAlignment;

			switch (alignment)
			{
				case StackedHorizontalAlignment.Stretch:
					ha = HorizontalAlignment.Stretch;
					break;

				case StackedHorizontalAlignment.Center:
					ha = HorizontalAlignment.Center;
					break;

				case StackedHorizontalAlignment.Left:
					ha = HorizontalAlignment.Left;
					break;

				case StackedHorizontalAlignment.Right:
					ha = HorizontalAlignment.Right;
					break;
			}

			if (obj.HorizontalAlignment != ha)
			{
				obj.HorizontalAlignment = ha;
				this.Invalidate();
			}
		}


		public bool HasStackedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			ChildrenPlacement placement = this.GetParentPlacement(obj);
			return (placement == ChildrenPlacement.HorizontalStacked || placement == ChildrenPlacement.VerticalStacked || placement == ChildrenPlacement.Grid);
		}

		public StackedVerticalAlignment GetStackedVerticalAlignment(Widget obj)
		{
			//	Retourne l'alignement vertical de l'objet.
			if (this.HasStackedVerticalAlignment(obj))
			{
				VerticalAlignment va = obj.VerticalAlignment;
				if (va == VerticalAlignment.Stretch )  return StackedVerticalAlignment.Stretch;
				if (va == VerticalAlignment.Center  )  return StackedVerticalAlignment.Center;
				if (va == VerticalAlignment.Bottom  )  return StackedVerticalAlignment.Bottom;
				if (va == VerticalAlignment.Top     )  return StackedVerticalAlignment.Top;
				if (va == VerticalAlignment.BaseLine)  return StackedVerticalAlignment.BaseLine;
			}

			return StackedVerticalAlignment.None;
		}

		public void SetStackedVerticalAlignment(Widget obj, StackedVerticalAlignment alignment)
		{
			//	Choix de l'alignement vertical de l'objet.
			System.Diagnostics.Debug.Assert(this.HasStackedVerticalAlignment(obj));

			VerticalAlignment va = obj.VerticalAlignment;

			switch (alignment)
			{
				case StackedVerticalAlignment.Stretch:
					va = VerticalAlignment.Stretch;
					break;

				case StackedVerticalAlignment.Center:
					va = VerticalAlignment.Center;
					break;

				case StackedVerticalAlignment.Bottom:
					va = VerticalAlignment.Bottom;
					break;

				case StackedVerticalAlignment.Top:
					va = VerticalAlignment.Top;
					break;

				case StackedVerticalAlignment.BaseLine:
					va = VerticalAlignment.BaseLine;
					break;
			}

			if (obj.VerticalAlignment != va)
			{
				obj.VerticalAlignment = va;
				this.Invalidate();
			}
		}
		#endregion


		#region Min & max size
		public double GetMinWidth(Widget obj)
		{
			return obj.MinWidth;
		}

		public void SetMinWidth(Widget obj, double width)
		{
			if (obj.MinWidth != width)
			{
				obj.MinWidth = width;
				this.Invalidate();
			}
		}

		public double GetMaxWidth(Widget obj)
		{
			if (System.Double.IsInfinity(obj.MaxWidth))  return 9999;  // TODO: à supprimer !
			return obj.MaxWidth;
		}

		public void SetMaxWidth(Widget obj, double width)
		{
			if (obj.MaxWidth != width)
			{
				obj.MaxWidth = width;
				this.Invalidate();
			}
		}

		public double GetMinHeight(Widget obj)
		{
			return obj.MinHeight;
		}

		public void SetMinHeight(Widget obj, double height)
		{
			if (obj.MinHeight != height)
			{
				obj.MinHeight = height;
				this.Invalidate();
			}
		}

		public double GetMaxHeight(Widget obj)
		{
			if (System.Double.IsInfinity(obj.MaxHeight))  return 9999;  // TODO: à supprimer !
			return obj.MaxHeight;
		}

		public void SetMaxHeight(Widget obj, double height)
		{
			if (obj.MaxHeight != height)
			{
				obj.MaxHeight = height;
				this.Invalidate();
			}
		}
		#endregion


		protected ChildrenPlacement GetParentPlacement(Widget obj)
		{
			//	Retourne le mode de placement du parent d'un objet.
			if (obj == this.Container)
			{
				return ChildrenPlacement.None;
			}
			else
			{
				return this.GetChildrenPlacement(obj.Parent);
			}
		}

		public void Invalidate()
		{
			//	Invalide le PanelEditor.
			this.panelEditor.Invalidate();
			this.panelEditor.UpdateGeometry();
		}


		protected MyWidgets.PanelEditor				panelEditor;
	}
}
