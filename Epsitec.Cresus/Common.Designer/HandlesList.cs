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


		public void DraggingStart(Point pos)
		{
			//	Débute éventuellement un déplacement de poignée.
			this.draggingType = this.HandleDetect(pos);

			if (this.IsDragging)
			{
				Point final = this.GetHandle(this.draggingType).Position;
				this.draggingOffset = pos-final;

				this.editor.ConstrainsList.Starting(new Rectangle(final, final), true);
				this.editor.Invalidate();
			}
		}

		public void DraggingMove(Point pos)
		{
			//	Effectue un déplacement de poignée.
			pos -= this.draggingOffset;
			this.editor.ConstrainsList.Activate(new Rectangle(pos, pos), 0, this.editor.SelectedObjects.ToArray());
			pos = this.editor.ConstrainsList.Snap(pos);

			this.MoveObject(pos);
			this.UpdateGeometry();
			this.Hilite(pos-this.draggingOffset);
			this.editor.Invalidate();
		}

		public void DraggingStop()
		{
			//	Termine un déplacement de poignée.
			this.draggingType = Handle.Type.None;
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
			//	Pendant un drag, seule la poignée déplacée est affichée.
			foreach (Handle handle in this.list)
			{
				if (this.IsDragging)
				{
					if (handle.HandleType != this.draggingType)
					{
						continue;
					}
				}

				handle.Draw(graphics);
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

			if (this.widget is TextField)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (this.widget is Button)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (this.widget is Separator)
			{
				if (this.widget.PreferredHeight == 1)  // séparateur horizontal ?
				{
					if (type != Handle.Type.Left && type != Handle.Type.Right)
					{
						return null;
					}
				}
				else  // séparateur vertical ?
				{
					if (type != Handle.Type.Bottom && type != Handle.Type.Top)
					{
						return null;
					}
				}
			}
			else if (this.widget is StaticText)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (this.widget is GroupBox)
			{
				//	Tous les types
			}
			else
			{
				return null;
			}

			return new Handle(type);
		}

		protected void HandleUpdatePosition(Handle handle)
		{
			//	Met à jour la position d'une poignée selon l'objet.
			Rectangle bounds = this.editor.GetObjectBounds(this.widget);
			Point center = bounds.Center;

			switch (handle.HandleType)
			{
				case Handle.Type.BottomLeft:
					handle.Position = bounds.BottomLeft;
					break;

				case Handle.Type.BottomRight:
					handle.Position = bounds.BottomRight;
					break;

				case Handle.Type.TopRight:
					handle.Position = bounds.TopRight;
					break;

				case Handle.Type.TopLeft:
					handle.Position = bounds.TopLeft;
					break;

				case Handle.Type.Bottom:
					handle.Position = new Point(center.X, bounds.Bottom);
					break;

				case Handle.Type.Top:
					handle.Position = new Point(center.X, bounds.Top);
					break;

				case Handle.Type.Left:
					handle.Position = new Point(bounds.Left, center.Y);
					break;

				case Handle.Type.Right:
					handle.Position = new Point(bounds.Right, center.Y);
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


		protected Handle.Type HandleDetect(Point mouse)
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
		protected Point						draggingOffset;
		protected bool						isFinger;
	}
}
