using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer
{
	/// <summary>
	/// Gestion de la liste des poignées pour PanelEditor.
	/// </summary>
	public class HandlesList
	{
		public HandlesList(MyWidgets.PanelEditor editor)
		{
			this.editor = editor;
			this.context = editor.Context;
		}


		public void UpdateSelection()
		{
			//	Met à jour les poignées après un changement de sélection.
			//	Les poignées ne sont visibles que s'il existe un seul objet sélectionné.
			//	Dans tous les autres cas, il n'y a aucune poignée.
			List<Widget> sel = this.editor.SelectedObjects;

			if (sel.Count == 1)  // un seul objet sélectionné ?
			{
				this.widget = sel[0];
				this.list.Clear();

				foreach (int i in System.Enum.GetValues(typeof(Handle.Type)))
				{
					Handle handle = this.CreateHandle((Handle.Type) i);
					if (handle != null)
					{
						this.list.Add(handle);
					}
				}

				this.UpdateGeometry();
			}
			else
			{
				this.widget = null;
				this.list.Clear();
			}
		}

		public void UpdateGeometry()
		{
			//	Mise à jour des poignées après une modification géométrique de l'objet.
			foreach (Handle handle in this.list)
			{
				this.HandleUpdatePosition(handle);
			}
		}


		public Handle.Type HandleDetect(Point mouse)
		{
			//	Détecte la poignée visée par la souris.
			for (int i=this.list.Count-1; i>=0; i--)
			{
				Handle handle = this.list[i];
				if (handle.Detect(mouse))
				{
					return handle.HandleType;
				}
			}

			return Handle.Type.None;
		}

		public void DraggingStart(Point pos, Rectangle rect, Size minSize, Handle.Type type)
		{
			//	Débute un déplacement de poignée.
			this.draggingRect = rect;
			this.draggingMinSize = minSize;
			this.draggingType = type;

			Point final = this.GetHandle(this.draggingType).Position;
			this.draggingOffset = final-pos;

			this.editor.ConstrainsList.Starting(new Rectangle(final, final), true);
			this.editor.Invalidate();
		}

		public Rectangle DraggingMove(Point pos)
		{
			//	Effectue un déplacement de poignée.
			pos += this.draggingOffset;
			this.editor.ConstrainsList.Activate(new Rectangle(pos, pos), 0, this.editor.SelectedObjects.ToArray());
			this.editor.Invalidate();
			pos = this.editor.ConstrainsList.Snap(pos);

			switch (this.draggingType)
			{
				case Handle.Type.BottomLeft:
					pos.X = System.Math.Min(pos.X, this.draggingRect.Right-this.draggingMinSize.Width);
					pos.Y = System.Math.Min(pos.Y, this.draggingRect.Top-this.draggingMinSize.Height);
					this.draggingRect.BottomLeft = pos;
					break;

				case Handle.Type.BottomRight:
					pos.X = System.Math.Max(pos.X, this.draggingRect.Left+this.draggingMinSize.Width);
					pos.Y = System.Math.Min(pos.Y, this.draggingRect.Top-this.draggingMinSize.Height);
					this.draggingRect.BottomRight = pos;
					break;

				case Handle.Type.TopRight:
					pos.X = System.Math.Max(pos.X, this.draggingRect.Left+this.draggingMinSize.Width);
					pos.Y = System.Math.Max(pos.Y, this.draggingRect.Bottom+this.draggingMinSize.Height);
					this.draggingRect.TopRight = pos;
					break;

				case Handle.Type.TopLeft:
					pos.X = System.Math.Min(pos.X, this.draggingRect.Right-this.draggingMinSize.Width);
					pos.Y = System.Math.Max(pos.Y, this.draggingRect.Bottom+this.draggingMinSize.Height);
					this.draggingRect.TopLeft = pos;
					break;

				case Handle.Type.Bottom:
					pos.Y = System.Math.Min(pos.Y, this.draggingRect.Top-this.draggingMinSize.Height);
					this.draggingRect.Bottom = pos.Y;
					break;

				case Handle.Type.Top:
					pos.Y = System.Math.Max(pos.Y, this.draggingRect.Bottom+this.draggingMinSize.Height);
					this.draggingRect.Top = pos.Y;
					break;

				case Handle.Type.Left:
					pos.X = System.Math.Min(pos.X, this.draggingRect.Right-this.draggingMinSize.Width);
					this.draggingRect.Left = pos.X;
					break;

				case Handle.Type.Right:
					pos.X = System.Math.Max(pos.X, this.draggingRect.Left+this.draggingMinSize.Width);
					this.draggingRect.Right = pos.X;
					break;
			}

			return this.draggingRect;
		}

		public void DraggingStop(Point pos)
		{
			//	Termine un déplacement de poignée.
			pos += this.draggingOffset;
			this.MoveObject(pos);

			this.draggingType = Handle.Type.None;
			this.UpdateGeometry();
			this.editor.ConstrainsList.Ending();
			this.editor.Invalidate();
		}


		public bool IsDragging
		{
			//	Indique si un déplacement de poignée est en cours.
			get
			{
				return (this.draggingType != Handle.Type.None);
			}
		}


		public void Hilite(Point mouse)
		{
			//	Met à jour la poignée survolée par la souris.
			if (this.IsDragging)
			{
				return;
			}

			this.isFinger = false;

			for (int i=this.list.Count-1; i>=0; i--)
			{
				Handle handle = this.list[i];
				bool hilite = handle.Detect(mouse);

				if (handle.IsHilite != hilite)
				{
					handle.IsHilite = hilite;
					this.editor.Panel.Invalidate(handle.Bounds);
				}

				if (handle.IsHilite)
				{
					this.isFinger = true;
				}
			}
		}

		public bool IsFinger
		{
			//	Indique s'il faut utiliser le doigt comme curseur pour la souris, car
			//	une poignée est survolée.
			get
			{
				return this.IsDragging || this.isFinger;
			}
		}


		public void Draw(Graphics graphics)
		{
			//	Dessine toutes les poignées.
			//	Pendant un drag, aucune poignée n'est affichée.
			if (!this.IsDragging)
			{
				foreach (Handle handle in this.list)
				{
					handle.Draw(graphics);
				}
			}
		}


		protected Handle CreateHandle(Handle.Type type)
		{
			//	Crée une poignée d'un objet, si elle existe.
			//	Selon le type de l'objet, toutes les poignées n'existent pas !
			if (type == Handle.Type.None)
			{
				return null;
			}

			bool w = HandlesList.HasWidthHandles(this.widget);
			bool h = HandlesList.HasHeightHandles(this.widget);

			if (w && h)
			{
				return new Handle(type);
			}

			if (w && !h)
			{
				if (type == Handle.Type.Left || type == Handle.Type.Right)
				{
					return new Handle(type);
				}
			}

			if (!w && h)
			{
				if (type == Handle.Type.Bottom || type == Handle.Type.Top)
				{
					return new Handle(type);
				}
			}

			return null;
		}

		static public bool HasWidthHandles(Widget obj)
		{
			//	Indique s'il est possible de modifier la largeur d'un objet.
			if (obj is TextField)
			{
				return true;
			}
			else if (obj is Button)
			{
				return true;
			}
			else if (obj is Separator)
			{
				if (obj.PreferredHeight == 1)  // séparateur horizontal ?
				{
					return true;
				}
				else  // séparateur vertical ?
				{
					return false;
				}
			}
			else if (obj is StaticText)
			{
				return true;
			}
			else if (obj is GroupBox)
			{
				return true;
			}
			else if (obj is FrameBox)
			{
				return true;
			}

			return false;
		}

		static public bool HasHeightHandles(Widget obj)
		{
			//	Indique s'il est possible de modifier la hauteur d'un objet.
			if (obj is TextField)
			{
				return false;
			}
			else if (obj is Button)
			{
				return false;
			}
			else if (obj is Separator)
			{
				if (obj.PreferredHeight == 1)  // séparateur horizontal ?
				{
					return false;
				}
				else  // séparateur vertical ?
				{
					return true;
				}
			}
			else if (obj is StaticText)
			{
				return false;
			}
			else if (obj is GroupBox)
			{
				return true;
			}
			else if (obj is FrameBox)
			{
				return true;
			}

			return false;
		}


		protected void HandleUpdatePosition(Handle handle)
		{
			//	Met à jour la position et la forme d'une poignée selon l'objet.
			Rectangle bounds = this.editor.GetObjectBounds(this.widget);
			Point center = bounds.Center;

			handle.GlyphType = Handle.Glyph.Square;

			switch (handle.HandleType)
			{
				case Handle.Type.BottomLeft:
					handle.Position = bounds.BottomLeft;

					if (!this.editor.IsObjectWidthChanging(this.widget) || !this.editor.IsObjectHeightChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.BottomRight:
					handle.Position = bounds.BottomRight;

					if (!this.editor.IsObjectWidthChanging(this.widget) || !this.editor.IsObjectHeightChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.TopRight:
					handle.Position = bounds.TopRight;

					if (!this.editor.IsObjectWidthChanging(this.widget) || !this.editor.IsObjectHeightChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.TopLeft:
					handle.Position = bounds.TopLeft;

					if (!this.editor.IsObjectWidthChanging(this.widget) || !this.editor.IsObjectHeightChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.Bottom:
					handle.Position = new Point(center.X, bounds.Bottom);

					if (!this.editor.IsObjectHeightChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.Top:
					handle.Position = new Point(center.X, bounds.Top);

					if (!this.editor.IsObjectHeightChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.Left:
					handle.Position = new Point(bounds.Left, center.Y);

					if (!this.editor.IsObjectWidthChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;

				case Handle.Type.Right:
					handle.Position = new Point(bounds.Right, center.Y);

					if (!this.editor.IsObjectWidthChanging(this.widget))
					{
						handle.GlyphType = Handle.Glyph.Hide;
					}
					break;
			}
		}

		protected void MoveObject(Point pos)
		{
			//	Déplace une poignée d'un objet.
			Rectangle bounds = this.editor.GetObjectBounds(this.widget);

			switch (this.draggingType)
			{
				case Handle.Type.BottomLeft:
					bounds.BottomLeft = pos;
					break;

				case Handle.Type.BottomRight:
					bounds.BottomRight = pos;
					break;

				case Handle.Type.TopRight:
					bounds.TopRight = pos;
					break;

				case Handle.Type.TopLeft:
					bounds.TopLeft = pos;
					break;

				case Handle.Type.Bottom:
					bounds.Bottom = pos.Y;
					break;

				case Handle.Type.Top:
					bounds.Top = pos.Y;
					break;

				case Handle.Type.Left:
					bounds.Left = pos.X;
					break;

				case Handle.Type.Right:
					bounds.Right = pos.X;
					break;
			}

			this.editor.SetObjectBounds(this.widget, bounds);
		}


		protected Handle GetHandle(Handle.Type type)
		{
			//	Retourne une poignée d'après son type.
			foreach (Handle handle in this.list)
			{
				if (handle.HandleType == type)
				{
					return handle;
				}
			}

			return null;
		}
		
		
		protected MyWidgets.PanelEditor		editor;
		protected PanelsContext				context;
		protected Widget					widget;
		protected List<Handle>				list = new List<Handle>();
		protected Handle.Type				draggingType = Handle.Type.None;
		protected Rectangle					draggingRect;
		protected Size						draggingMinSize;
		protected Point						draggingOffset;
		protected bool						isFinger;
	}
}
