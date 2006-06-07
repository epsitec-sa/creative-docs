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

			if (Handle.IsMarginType(type))
			{
				if (!this.editor.IsLayoutDocking)
				{
					return null;
				}
			}
			else
			{
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
			}

			return new Handle(type);
		}

		protected void HandleUpdatePosition(Handle handle)
		{
			//	Met à jour la position et la forme d'une poignée selon l'objet.
			Rectangle bounds = this.editor.GetObjectBounds(this.widget);
			Point center = bounds.Center;

			if (handle.IsMargin)
			{
				Point m = Point.Scale(bounds.BottomLeft, bounds.TopRight, 0.25);
				Margins margins = this.editor.GetObjectMargins(this.widget);

				switch (handle.HandleType)
				{
					case Handle.Type.MarginBottom:
						if (this.editor.IsObjectAttachmentTop(this.widget))
						{
							handle.Position = new Point(m.X, bounds.Bottom-margins.Bottom);
							handle.GlyphType = Handle.Glyph.ArrowDown;
						}
						else
						{
							handle.Position = new Point(m.X, bounds.Bottom);
							handle.GlyphType = Handle.Glyph.ArrowUp;
						}
						break;

					case Handle.Type.MarginTop:
						if (this.editor.IsObjectAttachmentTop(this.widget))
						{
							handle.Position = new Point(m.X, bounds.Top);
							handle.GlyphType = Handle.Glyph.ArrowDown;
						}
						else
						{
							handle.Position = new Point(m.X, bounds.Top+margins.Top);
							handle.GlyphType = Handle.Glyph.ArrowUp;
						}
						break;

					case Handle.Type.MarginLeft:
						if (this.editor.IsObjectAttachmentRight(this.widget))
						{
							handle.Position = new Point(bounds.Left-margins.Left, m.Y);
							handle.GlyphType = Handle.Glyph.ArrowLeft;
						}
						else
						{
							handle.Position = new Point(bounds.Left, m.Y);
							handle.GlyphType = Handle.Glyph.ArrowRight;
						}
						break;

					case Handle.Type.MarginRight:
						if (this.editor.IsObjectAttachmentRight(this.widget))
						{
							handle.Position = new Point(bounds.Right, m.Y);
							handle.GlyphType = Handle.Glyph.ArrowLeft;
						}
						else
						{
							handle.Position = new Point(bounds.Right+margins.Right, m.Y);
							handle.GlyphType = Handle.Glyph.ArrowRight;
						}
						break;
				}
			}
			else
			{
				handle.GlyphType = Handle.Glyph.Square;

				switch (handle.HandleType)
				{
					case Handle.Type.BottomLeft:
						handle.Position = bounds.BottomLeft;

						if (this.editor.IsLayoutDocking)
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.BottomRight:
						handle.Position = bounds.BottomRight;

						if (this.editor.IsLayoutDocking)
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.TopRight:
						handle.Position = bounds.TopRight;

						if (this.editor.IsLayoutDocking)
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.TopLeft:
						handle.Position = bounds.TopLeft;

						if (this.editor.IsLayoutDocking)
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.Bottom:
						handle.Position = new Point(center.X, bounds.Bottom);

						if (this.editor.IsLayoutDocking && !this.editor.IsObjectAttachmentTop(this.widget))
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.Top:
						handle.Position = new Point(center.X, bounds.Top);

						if (this.editor.IsLayoutDocking && !this.editor.IsObjectAttachmentBottom(this.widget))
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.Left:
						handle.Position = new Point(bounds.Left, center.Y);

						if (this.editor.IsLayoutDocking && !this.editor.IsObjectAttachmentRight(this.widget))
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;

					case Handle.Type.Right:
						handle.Position = new Point(bounds.Right, center.Y);

						if (this.editor.IsLayoutDocking && !this.editor.IsObjectAttachmentLeft(this.widget))
						{
							handle.GlyphType = Handle.Glyph.Hide;
						}
						break;
				}
			}
		}

		protected void MoveObject(Point pos)
		{
			//	Déplace une poignée d'un objet.
			Rectangle bounds = this.editor.GetObjectBounds(this.widget);
			Margins margins = this.editor.GetObjectMargins(this.widget);

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

				case Handle.Type.MarginBottom:
					if (this.editor.IsObjectAttachmentTop(this.widget))
					{
						margins.Bottom = System.Math.Max(bounds.Bottom-pos.Y, 0);
					}
					else
					{
						margins.Bottom = System.Math.Max(pos.Y-(bounds.Bottom-margins.Bottom), 0);
					}
					break;

				case Handle.Type.MarginTop:
					if (this.editor.IsObjectAttachmentTop(this.widget))
					{
						margins.Top = System.Math.Max((bounds.Top+margins.Top)-pos.Y, 0);
					}
					else
					{
						margins.Top = System.Math.Max(pos.Y-bounds.Top, 0);
					}
					break;

				case Handle.Type.MarginLeft:
					if (this.editor.IsObjectAttachmentRight(this.widget))
					{
						margins.Left = System.Math.Max(bounds.Left-pos.X, 0);
					}
					else
					{
						margins.Left = System.Math.Max(pos.X-(bounds.Left-margins.Left), 0);
					}
					break;

				case Handle.Type.MarginRight:
					if (this.editor.IsObjectAttachmentRight(this.widget))
					{
						margins.Right = System.Math.Max((bounds.Right+margins.Right)-pos.X, 0);
					}
					else
					{
						margins.Right = System.Math.Max(pos.X-bounds.Right, 0);
					}
					break;
			}

			this.editor.SetObjectBounds(this.widget, bounds);
			this.editor.SetObjectMargins(this.widget, margins);
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
