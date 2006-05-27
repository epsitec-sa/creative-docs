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


		public void DraggingStart(Widget obj, Point pos)
		{
			//	Débute éventuellement un déplacement de poignée.
			this.draggingType = this.HandlesDetect(obj, pos);
		}

		public void DraggingMove(Widget obj, Point pos)
		{
			//	Effectue un déplacement de poignée.
			this.HandleMove(obj, pos);
		}

		public void DraggingStop()
		{
			//	Termine un déplacement de poignée.
			this.draggingType = Handle.Type.None;
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


		public void Draw(Graphics graphics, Widget obj)
		{
			//	Dessine toutes les poignées d'un objet.
			foreach (int i in System.Enum.GetValues(typeof(Handle.Type)))
			{
				Handle handle = this.GetHandle(obj, (Handle.Type) i);
				if (handle != null)
				{
					if (this.draggingType != Handle.Type.None)
					{
						if (handle.HandleType != this.draggingType)
						{
							continue;
						}
					}

					handle.Draw(graphics);
				}
			}
		}


		protected Handle GetHandle(Widget obj, Handle.Type type)
		{
			//	Retourne une poignée d'un objet.
			if (type == Handle.Type.None)
			{
				return null;
			}

			if (obj is TextField)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (obj is Button)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (obj is Separator)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (obj is StaticText)
			{
				if (type != Handle.Type.Left && type != Handle.Type.Right)
				{
					return null;
				}
			}
			else if (obj is GroupBox)
			{
				//	Tous les types
			}
			else
			{
				return null;
			}

			Rectangle bounds = this.editor.GetObjectBounds(obj);
			Point center = bounds.Center;

			switch (type)
			{
				case Handle.Type.BottomLeft:
					return new Handle(type, bounds.BottomLeft);

				case Handle.Type.BottomRight:
					return new Handle(type, bounds.BottomRight);

				case Handle.Type.TopRight:
					return new Handle(type, bounds.TopRight);

				case Handle.Type.TopLeft:
					return new Handle(type, bounds.TopLeft);

				case Handle.Type.Bottom:
					return new Handle(type, new Point(center.X, bounds.Bottom));

				case Handle.Type.Top:
					return new Handle(type, new Point(center.X, bounds.Top));

				case Handle.Type.Left:
					return new Handle(type, new Point(bounds.Left, center.Y));

				case Handle.Type.Right:
					return new Handle(type, new Point(bounds.Right, center.Y));
			}

			return null;
		}

		protected void HandleMove(Widget obj, Point pos)
		{
			//	Déplace une poignée d'un objet.
			Rectangle bounds = this.editor.GetObjectBounds(obj);

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

			this.editor.SetObjectBounds(obj, bounds);
		}

		protected Handle.Type HandlesDetect(Widget obj, Point mouse)
		{
			//	Détecte la poignée visée par la souris.
			foreach (int i in System.Enum.GetValues(typeof(Handle.Type)))
			{
				Handle handle = this.GetHandle(obj, (Handle.Type) i);
				if (handle != null)
				{
					if (handle.Detect(mouse))
					{
						return (Handle.Type) i;
					}
				}
			}
			return Handle.Type.None;
		}
		
		
		protected MyWidgets.PanelEditor		editor;
		protected PanelsContext				context;
		protected Handle.Type				draggingType = Handle.Type.None;
	}
}
