using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.PanelEditor
{
	/// <summary>
	/// Gestion de la liste des cotes pour PanelEditor.
	/// </summary>
	public class DimensionsList
	{
		public DimensionsList(Editor editor)
		{
			this.editor = editor;
			this.objectModifier = editor.ObjectModifier;
			this.context = editor.Context;
		}


		public void UpdateSelection()
		{
			//	Met à jour les cotes après un changement de sélection.
			this.hilited = null;
			this.dragging = null;

			this.list.Clear();

			List<Widget> sel = this.editor.SelectedObjects;
			bool slave = false;
			if (sel.Count != 0)  // un ou plusieurs objets sélectionnés ?
			{
				foreach (Widget obj in sel)
				{
					this.CreateDimensions(obj, slave);
					slave = true;
				}
			}
		}

		public bool IsShiftPressed
		{
			//	Etat de la touche shift.
			get
			{
				return this.isShiftPressed;
			}
			set
			{
				if (this.isShiftPressed != value)
				{
					this.isShiftPressed = value;
					this.hilited = null;
					this.editor.Invalidate();  // il faut tout redessiner
				}
			}
		}

		public void Draw(Graphics graphics)
		{
			//	Dessine toutes les cotes.
			foreach (Dimension dim in this.list)
			{
				if (!this.IsSkipping(dim))
				{
					dim.DrawBackground(graphics);
				}
			}

			foreach (Dimension dim in this.list)
			{
				if (!this.IsSkipping(dim))
				{
					dim.DrawDimension(graphics);
				}
			}

			if (this.hilited != null)
			{
				this.hilited.DrawHilite(graphics, (this.dragging == null));
			}
		}

		public bool Hilite(Point mouse)
		{
			//	Met en évidence la cote survolée par la souris.
			Dimension dim = this.Detect(mouse);

			if (this.hilited != dim)
			{
				this.Invalidate(this.hilited);
				this.hilited = dim;
				this.Invalidate(this.hilited);
			}

			return (this.hilited != null);
		}

		public bool Wheel(int direction)
		{
			//	Modifie la cote survolée selon la molette de la souris.
			if (this.hilited != null)
			{
				double value = this.hilited.Value;
				value += (direction < 0) ? -1 : 1;
				this.ChangeValue(this.hilited, value);
				return true;
			}

			return false;
		}

		public bool DraggingStart(Point mouse, bool isControlPressed, bool isShiftPressed)
		{
			//	Début de la modification interactive d'une cote.
			this.dragging = this.Detect(mouse);
			if (this.dragging == null)
			{
				return false;
			}
			else
			{
				if (this.dragging.DimensionType == Dimension.Type.GridColumn||
					this.dragging.DimensionType == Dimension.Type.GridRow   )
				{
					Widget obj = this.dragging.Object;

					GridSelection gs = GridSelection.Get(obj);
					if (gs == null)
					{
						gs = new GridSelection(obj);
						GridSelection.Attach(obj, gs);
					}

					GridSelection.Unit unit = (this.dragging.DimensionType == Dimension.Type.GridColumn) ? GridSelection.Unit.Column : GridSelection.Unit.Row;
					int index = this.dragging.ColumnOrRow;

					int search = gs.Search(unit, index);
					if (search == -1)  // pas encore sélectionné ?
					{
						if (!isControlPressed && !isShiftPressed)
						{
							gs.Clear();
						}

						gs.Add(unit, index);  // sélectionne
					}
					else
					{
						gs.RemoveAt(search);  // désélectionne
					}

					this.editor.UpdateAfterSelectionGridChanged();
					this.editor.Invalidate();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridWidthMode||
						 this.dragging.DimensionType == Dimension.Type.GridHeightMode)
				{
					this.ChangeMode(this.dragging);
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridColumnAddBefore||
						 this.dragging.DimensionType == Dimension.Type.GridColumnAddAfter)
				{
					Widget obj = this.dragging.Object;

					this.objectModifier.GridColumnAdd(obj, this.dragging.ColumnOrRow, (this.dragging.DimensionType == Dimension.Type.GridColumnAddAfter));

					this.editor.UpdateAfterSelectionGridChanged();
					this.editor.Invalidate();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridColumnRemove)
				{
					Widget obj = this.dragging.Object;

					this.objectModifier.GridColumnRemove(obj, this.dragging.ColumnOrRow);

					this.editor.UpdateAfterSelectionGridChanged();
					this.editor.Invalidate();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridRowAddBefore||
						 this.dragging.DimensionType == Dimension.Type.GridRowAddAfter)
				{
					Widget obj = this.dragging.Object;

					this.objectModifier.GridRowAdd(obj, this.dragging.ColumnOrRow, (this.dragging.DimensionType == Dimension.Type.GridRowAddAfter));

					this.editor.UpdateAfterSelectionGridChanged();
					this.editor.Invalidate();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridRowRemove)
				{
					Widget obj = this.dragging.Object;

					this.objectModifier.GridRowRemove(obj, this.dragging.ColumnOrRow);

					this.editor.UpdateAfterSelectionGridChanged();
					this.editor.Invalidate();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridColumnSpanInc)
				{
					Widget obj = this.dragging.Object;
					this.objectModifier.SetGridColumnSpan(obj, this.objectModifier.GetGridColumnSpan(obj)+1);
					this.editor.Invalidate();
					this.UpdateSelection();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridColumnSpanDec)
				{
					Widget obj = this.dragging.Object;
					this.objectModifier.SetGridColumnSpan(obj, this.objectModifier.GetGridColumnSpan(obj)-1);
					this.editor.Invalidate();
					this.UpdateSelection();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridRowSpanInc)
				{
					Widget obj = this.dragging.Object;
					this.objectModifier.SetGridRowSpan(obj, this.objectModifier.GetGridRowSpan(obj)+1);
					this.editor.Invalidate();
					this.UpdateSelection();
				}
				else if (this.dragging.DimensionType == Dimension.Type.GridRowSpanDec)
				{
					Widget obj = this.dragging.Object;
					this.objectModifier.SetGridRowSpan(obj, this.objectModifier.GetGridRowSpan(obj)-1);
					this.editor.Invalidate();
					this.UpdateSelection();
				}
				else if (this.dragging.DimensionType == Dimension.Type.ChildrenPlacement)
				{
					Widget obj = this.dragging.Object;
					ObjectModifier.ChildrenPlacement cp = DimensionsList.NextChildrenPlacement(this.objectModifier.GetChildrenPlacement(obj));

					GeometryCache.FixBounds(obj, this.objectModifier);
					this.objectModifier.SetChildrenPlacement(obj, cp);
					GeometryCache.AdaptBounds(obj, this.objectModifier, cp);

					this.UpdateSelection();
				}
				else
				{
					this.startingPos = this.editor.MapClientToScreen(mouse);
					this.initialValue = this.dragging.Value;
				}

				return true;
			}
		}

		public void DraggingMove(Point mouse, bool isControlPressed, bool isShiftPressed)
		{
			//	Modification interactive d'une cote.
			if (this.dragging != null)
			{
				if (this.dragging.DimensionType != Dimension.Type.GridColumn &&
					this.dragging.DimensionType != Dimension.Type.GridRow)
				{
					mouse = this.editor.MapClientToScreen(mouse);
					double value = mouse.Y - this.startingPos.Y;

					if (isControlPressed)  // moins sensible ?
					{
						value = System.Math.Floor((value+0.5)*0.5);
					}

					if (isShiftPressed)  // grille magnétique ?
					{
						double step = 5;
						value += this.initialValue;
						value = System.Math.Floor((value+step/2)/step) * step;
						value -= this.initialValue;
					}

					this.ChangeValue(this.dragging, this.initialValue+value);
				}
			}
		}

		public void DraggingEnd()
		{
			//	Fin de la modification interactive d'une cote.
			this.dragging = null;
		}

		protected void ChangeValue(Dimension dim, double value)
		{
			//	Change la valeur d'une cote.
			foreach (Dimension d in this.list)
			{
				if (d.DimensionType == dim.DimensionType)
				{
					d.Value = value;
				}
			}
		}

		protected void ChangeMode(Dimension dim)
		{
			//	Change le mode d'une ligne/colonne (passe au mode suivant).
			foreach (Dimension d in this.list)
			{
				if (d.DimensionType == dim.DimensionType)
				{
					Widget obj = d.Object;

					if (dim.DimensionType == Dimension.Type.GridWidthMode)
					{
						ObjectModifier.GridMode mode = this.objectModifier.GetGridColumnMode(obj, d.ColumnOrRow);
						this.objectModifier.SetGridColumnMode(obj, d.ColumnOrRow, DimensionsList.NextGridMode(mode));
					}
					else
					{
						ObjectModifier.GridMode mode = this.objectModifier.GetGridRowMode(obj, d.ColumnOrRow);
						this.objectModifier.SetGridRowMode(obj, d.ColumnOrRow, DimensionsList.NextGridMode(mode));
					}
				}
			}

			this.editor.UpdateAfterSelectionGridChanged();
			this.editor.Invalidate();
		}

		protected static ObjectModifier.GridMode NextGridMode(ObjectModifier.GridMode mode)
		{
			switch (mode)
			{
				case ObjectModifier.GridMode.Auto:          return ObjectModifier.GridMode.Absolute;
				case ObjectModifier.GridMode.Absolute:      return ObjectModifier.GridMode.Proportional;
				case ObjectModifier.GridMode.Proportional:  return ObjectModifier.GridMode.Auto;
				default:                                    return ObjectModifier.GridMode.Auto;
			}
		}

		protected static ObjectModifier.ChildrenPlacement NextChildrenPlacement(ObjectModifier.ChildrenPlacement cp)
		{
			switch (cp)
			{
				case ObjectModifier.ChildrenPlacement.VerticalStacked:    return ObjectModifier.ChildrenPlacement.HorizontalStacked;
				case ObjectModifier.ChildrenPlacement.HorizontalStacked:  return ObjectModifier.ChildrenPlacement.Grid;
				case ObjectModifier.ChildrenPlacement.Grid:               return ObjectModifier.ChildrenPlacement.VerticalStacked;
				default:                                                  return ObjectModifier.ChildrenPlacement.VerticalStacked;
			}
		}


		protected void CreateDimensions(Widget obj, bool slave)
		{
			//	Crée toutes les cotes pour un objet donné.
			Dimension dim;

			//	Crée les cotes de ligne/colonne en premier, pour qu'elles viennent par dessous
			//	toutes les autres.
			if (this.objectModifier.GetChildrenPlacement(obj) == ObjectModifier.ChildrenPlacement.Grid && !slave)
			{
				int columns = this.objectModifier.GetGridColumnsCount(obj);
				for (int i=0; i<columns; i++)
				{
					dim = new Dimension(this.editor, obj, Dimension.Type.GridColumn, i);
					this.list.Add(dim);
				}

				int rows = this.objectModifier.GetGridRowsCount(obj);
				for (int i=0; i<rows; i++)
				{
					dim = new Dimension(this.editor, obj, Dimension.Type.GridRow, i);
					this.list.Add(dim);
				}

				GridSelection gs = GridSelection.Get(obj);
				if (gs != null)
				{
					int indexColumn = -1;
					int indexRow = -1;
					int columnsCount = 0;
					int rowsCount = 0;

					for (int i=0; i<gs.Count; i++)
					{
						GridSelection.OneItem item = gs[i];

						if (item.Unit == GridSelection.Unit.Column)
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridWidth, item.Index);
							dim.Slave = (indexColumn != -1);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridWidthMode, item.Index);
							dim.Slave = (indexColumn != -1);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginLeft, item.Index);
							dim.Slave = (indexColumn != -1);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginRight, item.Index);
							dim.Slave = (indexColumn != -1);
							this.list.Add(dim);

							if (indexColumn == -1)
							{
								indexColumn = i;
							}

							columnsCount++;
						}

						if (item.Unit == GridSelection.Unit.Row)
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridHeight, item.Index);
							dim.Slave = (indexRow != -1);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridHeightMode, item.Index);
							dim.Slave = (indexRow != -1);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginBottom, item.Index);
							dim.Slave = (indexRow != -1);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginTop, item.Index);
							dim.Slave = (indexRow != -1);
							this.list.Add(dim);

							if (indexRow == -1)
							{
								indexRow = i;
							}

							rowsCount++;
						}
					}

					if (columnsCount == 1)
					{
						GridSelection.OneItem item = gs[indexColumn];

						dim = new Dimension(this.editor, obj, Dimension.Type.GridColumnAddBefore, item.Index);
						this.list.Add(dim);

						dim = new Dimension(this.editor, obj, Dimension.Type.GridColumnAddAfter, item.Index);
						this.list.Add(dim);

						if (this.objectModifier.GetGridColumnsCount(obj) > 1 &&
							this.objectModifier.IsGridColumnEmpty(obj, item.Index))
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridColumnRemove, item.Index);
							this.list.Add(dim);
						}
					}

					if (rowsCount == 1)
					{
						GridSelection.OneItem item = gs[indexRow];

						dim = new Dimension(this.editor, obj, Dimension.Type.GridRowAddBefore, item.Index);
						this.list.Add(dim);

						dim = new Dimension(this.editor, obj, Dimension.Type.GridRowAddAfter, item.Index);
						this.list.Add(dim);

						if (this.objectModifier.GetGridRowsCount(obj) > 1 &&
							this.objectModifier.IsGridRowEmpty(obj, item.Index))
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridRowRemove, item.Index);
							this.list.Add(dim);
						}
					}
				}
			}

			if (this.objectModifier.HasWidth(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.Width);
				dim.Slave = slave;
				this.list.Add(dim);
			}

			if (this.objectModifier.HasHeight(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.Height);
				dim.Slave = slave;
				this.list.Add(dim);
			}

			if (this.objectModifier.HasMargins(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.MarginLeft);
				dim.Slave = slave;
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.MarginRight);
				dim.Slave = slave;
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.MarginBottom);
				dim.Slave = slave;
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.MarginTop);
				dim.Slave = slave;
				this.list.Add(dim);
			}

			if (this.objectModifier.HasChildrenPlacement(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.ChildrenPlacement);
				dim.Slave = slave;
				this.list.Add(dim);
			}

			if (this.objectModifier.AreChildrenGrid(obj.Parent))
			{
				int index, span, count;

				index = this.objectModifier.GetGridColumn(obj);
				span = this.objectModifier.GetGridColumnSpan(obj);
				count = this.objectModifier.GetGridColumnsCount(obj.Parent);

				if (index+span < count)
				{
					dim = new Dimension(this.editor, obj, Dimension.Type.GridColumnSpanInc);
					dim.Slave = slave;
					this.list.Add(dim);
				}

				if (span > 1)
				{
					dim = new Dimension(this.editor, obj, Dimension.Type.GridColumnSpanDec);
					dim.Slave = slave;
					this.list.Add(dim);
				}

				index = this.objectModifier.GetGridRow(obj);
				span = this.objectModifier.GetGridRowSpan(obj);
				count = this.objectModifier.GetGridRowsCount(obj.Parent);

				if (index+span < count)
				{
					dim = new Dimension(this.editor, obj, Dimension.Type.GridRowSpanInc);
					dim.Slave = slave;
					this.list.Add(dim);
				}

				if (span > 1)
				{
					dim = new Dimension(this.editor, obj, Dimension.Type.GridRowSpanDec);
					dim.Slave = slave;
					this.list.Add(dim);
				}
			}

			//	Crée les cotes de padding en dernier, pour qu'elles viennent par dessus
			//	toutes les autres.
			if (this.objectModifier.HasPadding(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingLeft);
				dim.Slave = slave;
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingRight);
				dim.Slave = slave;
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingBottom);
				dim.Slave = slave;
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingTop);
				dim.Slave = slave;
				this.list.Add(dim);
			}
		}

		protected Dimension Detect(Point pos)
		{
			//	Retourne la cote contenant une position donnée.
			for (int i=this.list.Count-1; i>=0; i--)  // du dernier (dessus) au premier (dessous)
			{
				Dimension dim = this.list[i];
				if (!this.IsSkipping(dim) && dim.Detect(pos))
				{
					return dim;
				}
			}

			return null;
		}

		protected bool IsSkipping(Dimension dim)
		{
			//	Indique si une cote doit être ignorée. C'est le cas des cotes autres
			//	que celles pour les lignes/colonnes lorsque la touche Shift est pressée.
			if (this.isShiftPressed)
			{
				if (dim.DimensionType != Dimension.Type.GridColumn &&
					dim.DimensionType != Dimension.Type.GridRow)
				{
					return true;
				}
			}

			return false;
		}

		protected void Invalidate(Dimension dim)
		{
			//	Invalide la zone occupée par une cote.
			if (dim != null)
			{
				Rectangle bounds = dim.GetBounds(true);
				bounds.Offset(Dimension.margin, Dimension.margin);
				this.editor.Invalidate(bounds);
			}
		}

		
		protected Editor					editor;
		protected ObjectModifier			objectModifier;
		protected PanelsContext				context;
		protected List<Dimension>			list = new List<Dimension>();
		protected Dimension					hilited;
		protected Dimension					dragging;
		protected Point						startingPos;
		protected double					initialValue;
		protected bool						isShiftPressed;
	}
}
