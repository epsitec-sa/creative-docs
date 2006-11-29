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
				dim.DrawMark(graphics);
			}

			if (this.hilited != Dimension.Type.None)
			{
				Dimension dim = this.GetDimension(this.hilited);
				dim.DrawHilite(graphics, (this.dragging == Dimension.Type.None));
			}
		}

		public bool Hilite(Point mouse)
		{
			//	Met en �vidence la cote survol�e par la souris.
			Dimension dim = this.Detect(mouse);
			Dimension.Type type = (dim == null) ? Dimension.Type.None : dim.DimensionType;

			if (this.hilited != type)
			{
				this.hilited = type;
				this.editor.Invalidate();
			}

			return (this.hilited != Dimension.Type.None);
		}

		public void Wheel(int direction)
		{
			//	Modifie la cote survol�e selon la molette de la souris.
			if (this.hilited != Dimension.Type.None)
			{
				Dimension dim = this.GetDimension(this.hilited);
				double value = dim.Value;
				value += (direction < 0) ? -1 : 1;
				dim.Value = value;
			}
		}

		public bool DraggingStart(Point mouse)
		{
			//	D�but de la modification interactive d'une cote.
			Dimension dim = this.Detect(mouse);
			if (dim == null)
			{
				this.dragging = Dimension.Type.None;
				return false;
			}
			else
			{
				this.dragging = dim.DimensionType;
				this.startingPos = mouse;
				this.initialValue = dim.Value;
				return true;
			}
		}

		public void DraggingMove(Point mouse, bool isControlPressed, bool isShiftPressed)
		{
			//	Modification interactive d'une cote.
			Dimension dim = this.GetDimension(this.dragging);
			System.Diagnostics.Debug.Assert(dim != null);

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

			dim.Value = this.initialValue+value;
		}

		public void DraggingEnd()
		{
			//	Fin de la modification interactive d'une cote.
			this.dragging = Dimension.Type.None;
		}


		protected void CreateDimensions(Widget obj)
		{
			//	Cr�e toutes les cotes pour un objet donn�.
			Dimension dim;

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
			foreach (Dimension dim in this.list)
			{
				if (dim.Detect(pos))
				{
					return dim;
				}
			}

			return null;
		}

		protected Dimension GetDimension(Dimension.Type type)
		{
			//	Retourne une cote d'apr�s son type.
			foreach (Dimension dim in this.list)
			{
				if (dim.DimensionType == type)
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
		protected Dimension.Type			hilited = Dimension.Type.None;
		protected Dimension.Type			dragging = Dimension.Type.None;
		protected Point						startingPos;
		protected double					initialValue;
	}
}
