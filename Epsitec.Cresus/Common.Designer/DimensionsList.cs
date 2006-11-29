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
			//	Met à jour les cotes après un changement de sélection.
			this.list.Clear();

			List<Widget> sel = this.editor.SelectedObjects;
			if (sel.Count != 0)  // un ou plusieurs objets sélectionnés ?
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
				dim.DrawMark(graphics);
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
				this.hilited = dim;
				this.editor.Invalidate();
			}

			return (this.hilited != null);
		}

		public void Wheel(int direction)
		{
			//	Modifie la cote survolée selon la molette de la souris.
			if (this.hilited != null)
			{
				double value = this.hilited.Value;
				value += (direction < 0) ? -1 : 1;
				this.hilited.Value = value;
			}
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
				if (this.dragging.DimensionType == Dimension.Type.GridColumn ||
					this.dragging.DimensionType == Dimension.Type.GridRow    )
				{
					List<Widget> sel = this.editor.SelectedObjects;
					Widget obj = sel[0];  // premier objet sélectionné (celui qui a les cotes)

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

				if (isShiftPressed)  // grille magnétique ?
				{
					double step = 5;
					value += this.initialValue;
					value = System.Math.Floor((value+step/2)/step) * step;
					value -= this.initialValue;
				}

				this.dragging.Value = this.initialValue+value;
			}
		}

		public void DraggingEnd()
		{
			//	Fin de la modification interactive d'une cote.
			this.dragging = null;
		}


		protected void CreateDimensions(Widget obj)
		{
			//	Crée toutes les cotes pour un objet donné.
			Dimension dim;

			//	Crée les cotes de ligne/colonne en premier, pour qu'elles viennent par dessous
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
					for (int i=0; i<gs.Count; i++)
					{
						GridSelection.OneItem item = gs[i];

						if (item.Unit == GridSelection.Unit.Column &&
							this.objectModifier.GetGridColumnMode(obj, item.Index) != ObjectModifier.GridMode.Auto)
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridWidth, item.Index);
							this.list.Add(dim);
						}

						if (item.Unit == GridSelection.Unit.Row &&
							this.objectModifier.GetGridRowMode(obj, item.Index) != ObjectModifier.GridMode.Auto)
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridHeight, item.Index);
							this.list.Add(dim);
						}
					}

					if (gs.Count == 1)
					{
						GridSelection.OneItem item = gs[0];

						if (item.Unit == GridSelection.Unit.Column)
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginLeft, item.Index);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginRight, item.Index);
							this.list.Add(dim);
						}

						if (item.Unit == GridSelection.Unit.Row)
						{
							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginBottom, item.Index);
							this.list.Add(dim);

							dim = new Dimension(this.editor, obj, Dimension.Type.GridMarginTop, item.Index);
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

			//	Crée les cotes de padding en dernier, pour qu'elles viennent par dessus
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
			//	Retourne la cote contenant une position donnée.
			foreach (Dimension dim in this.list)
			{
				if (dim.Detect(pos))
				{
					return dim;
				}
			}

			return null;
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
