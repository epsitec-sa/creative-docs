using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Gestion de la liste des cotes pour PanelEditor.
	/// </summary>
	public class DimensionsList
	{
		public DimensionsList(MyWidgets.PanelEditor editor)
		{
			this.editor = editor;
			this.objectModifier = editor.ObjectModifier;
			this.context = editor.Context;
		}


		public void UpdateSelection()
		{
			//	Met � jour les cotes apr�s un changement de s�lection.
			this.hilited = null;
			this.dragging = null;

			this.list.Clear();

			List<Widget> sel = this.editor.SelectedObjects;
			if (sel.Count != 0)  // un ou plusieurs objets s�lectionn�s ?
			{
				this.CreateDimensions(sel[0]);
			}
		}

		public void Draw(Graphics graphics)
		{
			//	Dessine toutes les cotes.
			foreach (Dimension dim in this.list)
			{
				dim.DrawBackground(graphics);
			}

			foreach (Dimension dim in this.list)
			{
				dim.DrawDimension(graphics);
			}

			if (this.hilited != null)
			{
				this.hilited.DrawHilite(graphics, (this.dragging == null));
			}
		}

		public bool Hilite(Point mouse)
		{
			//	Met en �vidence la cote survol�e par la souris.
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
			//	Modifie la cote survol�e selon la molette de la souris.
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
			//	D�but de la modification interactive d'une cote.
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
					if (search == -1)  // pas encore s�lectionn� ?
					{
						if (!isControlPressed && !isShiftPressed)
						{
							gs.Clear();
						}

						gs.Add(unit, index);  // s�lectionne
					}
					else
					{
						gs.RemoveAt(search);  // d�s�lectionne
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
				else
				{
					this.startingPos = mouse;
					this.initialValue = this.dragging.Value;
				}

				return true;
			}
		}

		public void DraggingMove(Point mouse, bool isControlPressed, bool isShiftPressed)
		{
			//	Modification interactive d'une cote.
			if (this.dragging.DimensionType != Dimension.Type.GridColumn &&
				this.dragging.DimensionType != Dimension.Type.GridRow    )
			{
				double value = mouse.Y - this.startingPos.Y;

				if (isControlPressed)  // moins sensible ?
				{
					value = System.Math.Floor((value+0.5)*0.5);
				}

				if (isShiftPressed)  // grille magn�tique ?
				{
					double step = 5;
					value += this.initialValue;
					value = System.Math.Floor((value+step/2)/step) * step;
					value -= this.initialValue;
				}

				this.ChangeValue(this.dragging, this.initialValue+value);
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


		protected void CreateDimensions(Widget obj)
		{
			//	Cr�e toutes les cotes pour un objet donn�.
			Dimension dim;

			//	Cr�e les cotes de ligne/colonne en premier, pour qu'elles viennent par dessous
			//	toutes les autres.
			if (this.objectModifier.GetChildrenPlacement(obj) == ObjectModifier.ChildrenPlacement.Grid)
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
				this.list.Add(dim);
			}

			if (this.objectModifier.HasHeight(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.Height);
				this.list.Add(dim);
			}

			if (this.objectModifier.HasMargins(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.MarginLeft);
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.MarginRight);
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.MarginBottom);
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.MarginTop);
				this.list.Add(dim);
			}

			//	Cr�e les cotes de padding en dernier, pour qu'elles viennent par dessus
			//	toutes les autres.
			if (this.objectModifier.HasPadding(obj))
			{
				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingLeft);
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingRight);
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingBottom);
				this.list.Add(dim);

				dim = new Dimension(this.editor, obj, Dimension.Type.PaddingTop);
				this.list.Add(dim);
			}
		}

		protected Dimension Detect(Point pos)
		{
			//	Retourne la cote contenant une position donn�e.
			for (int i=this.list.Count-1; i>=0; i--)  // du dernier (dessus) au premier (dessous)
			{
				Dimension dim = this.list[i];
				if (dim.Detect(pos))
				{
					return dim;
				}
			}

			return null;
		}

		protected void Invalidate(Dimension dim)
		{
			//	Invalide la zone occup�e par une cote.
			if (dim != null)
			{
				Rectangle bounds = dim.GetBounds(true);
				bounds.Offset(Dimension.margin, Dimension.margin);
				this.editor.Invalidate(bounds);
			}
		}

		
		protected MyWidgets.PanelEditor		editor;
		protected ObjectModifier			objectModifier;
		protected PanelsContext				context;
		protected List<Dimension>			list = new List<Dimension>();
		protected Dimension					hilited;
		protected Dimension					dragging;
		protected Point						startingPos;
		protected double					initialValue;
	}
}
