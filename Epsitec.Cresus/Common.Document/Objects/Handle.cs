using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	public enum HandleType
	{
		Primary,		// poign�e principale
		Secondary,		// poign�e secondaire
		Bezier,			// poign�e secondaire pour courbe de B�zier
		Starting,		// poign�e de d�part
		Ending,			// poign�e d'arriv�e
		Hide,			// poign�e invisible
		Property,		// poign�e d'une propri�t�
		Center,			// poign�e du centre de rotation
		Rotate,			// poign�e de l'angle de rotation
	}

	public enum HandleConstrainType
	{
		Symmetric,		// sym�trique
		Smooth,			// lisse
		Corner,			// anguleux
		Simply,			// simple (sans coin fantaisie)
	}

	/// <summary>
	/// La classe Handle repr�sente une poign�e d'un objet graphique.
	/// </summary>
	[System.Serializable()]
	public class Handle : ISerializable
	{
		public Handle(Document document)
		{
			this.document = document;
		}

		// Position de la poign�e.
		public Point Position
		{
			get
			{
				return this.position;
			}

			set
			{
				if ( this.position != value )
				{
					this.NotifyArea();
					this.position = value;
					this.NotifyArea();
				}
			}
		}

		// Type de la poign�e.
		public HandleType Type
		{
			get
			{
				return this.type;
			}
			
			set
			{
				if ( this.type != value )
				{
					this.type = value;
					this.NotifyArea();
				}
			}
		}

		// Type de la contrainte de la poign�e.
		public HandleConstrainType ConstrainType
		{
			get
			{
				return this.constrainType;
			}
			
			set
			{
				if ( this.constrainType != value )
				{
					this.constrainType = value;
					this.NotifyArea();
				}
			}
		}

		// Etat "visible" de la poign�e.
		public bool IsVisible
		{
			get
			{
				return this.isVisible;
			}
			
			set
			{
				if ( this.isVisible != value )
				{
					this.NotifyArea();
					this.isVisible = value;
					this.NotifyArea();
				}
			}
		}

		// Etat "survol�" de la poign�e.
		public bool IsHilited
		{
			get
			{
				return this.isHilited;
			}

			set
			{
				if ( this.isHilited != value )
				{
					this.isHilited = value;
					this.NotifyArea();
				}
			}
		}

		// Etat "s�lectionn� global" de la poign�e.
		public bool IsGlobalSelected
		{
			get
			{
				return this.isGlobalSelected;
			}

			set
			{
				if ( this.isGlobalSelected != value )
				{
					this.isGlobalSelected = value;
					this.NotifyArea();
				}
			}
		}

		// Type de la propri�t� li�e � la poign�e.
		public Properties.Type PropertyType
		{
			get
			{
				return this.propertyType;
			}

			set
			{
				this.propertyType = value;
			}
		}

		// Rang de la poign�e dans la propri�t�.
		public int PropertyRank
		{
			get
			{
				return this.propertyRank;
			}

			set
			{
				this.propertyRank = value;
			}
		}

		// Notifie un changement de la poign�e.
		protected void NotifyArea()
		{
			if ( this.document.Notifier == null )  return;
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		// Copie la poign�e courante dans une poign�e destination.
		public void CopyTo(Handle dst)
		{
			dst.position         = this.position;
			dst.type             = this.type;
			dst.constrainType    = this.constrainType;
			dst.isVisible        = this.isVisible;
			dst.isHilited        = this.isHilited;
			dst.isGlobalSelected = this.isGlobalSelected;
			dst.propertyType     = this.propertyType;
			dst.propertyRank     = this.propertyRank;
		}

		// Permute les informations de s�lections entre 2 poign�es.
		public void SwapSelection(Handle h)
		{
			Misc.Swap(ref this.isVisible,        ref h.isVisible       );
			Misc.Swap(ref this.isGlobalSelected, ref h.isGlobalSelected);
		}


		// D�tecte si la souris est dans la poign�e.
		public bool Detect(Point pos)
		{
			if ( !this.isVisible || this.isGlobalSelected )  return false;
			if ( this.type == HandleType.Hide )  return false;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double scaleX     = context.ScaleX;
			double scaleY     = context.ScaleY;
			double handleSize = context.HandleSize;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.position.X - handleSize/2 - 3.0/scaleX;
			rect.Right  = this.position.X + handleSize/2 + 1.0/scaleX;
			rect.Bottom = this.position.Y - handleSize/2 - 3.0/scaleY;
			rect.Top    = this.position.Y + handleSize/2 + 1.0/scaleY;
			return rect.Contains(pos);
		}


		// Retourne la bbox d'une poign�e.
		// Il n'est pas n�cessaire de tenir compte des dimensions de la poign�e,
		// car la zone � repeindre est toujours engraiss�e en cons�quence juste
		// avant le Invalidate !
		public Drawing.Rectangle BoundingBox
		{
			get
			{
				if ( this.isVisible )
				{
					return new Drawing.Rectangle(this.position, Size.Empty);
				}
				else
				{
					return Drawing.Rectangle.Empty;
				}
			}
		}


		// Dessine la poign�e.
		public void Draw(Graphics graphics, DrawingContext drawingContext)
		{
			if ( !this.isVisible )  return;

			double scaleX     = drawingContext.ScaleX;
			double scaleY     = drawingContext.ScaleY;
			double handleSize = drawingContext.HandleSize;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/scaleX;

			Point pos = this.position;
			graphics.Align(ref pos);

			if ( this.isGlobalSelected )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = pos.X-handleSize*0.25;
				rect.Right  = pos.X+handleSize*0.25;
				rect.Bottom = pos.Y-handleSize*0.25;
				rect.Top    = pos.Y+handleSize*0.25;
				graphics.Align(ref rect);

				if ( this.type == HandleType.Primary )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(DrawingContext.ColorHandleGlobal);

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(DrawingContext.ColorHandleOutline);
				}
			}
			else
			{
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = pos.X-handleSize*0.5;
				rect.Right  = pos.X+handleSize*0.5;
				rect.Bottom = pos.Y-handleSize*0.5;
				rect.Top    = pos.Y+handleSize*0.5;
				graphics.Align(ref rect);

				Color color;
				if ( this.isHilited )
				{
					color = drawingContext.HiliteOutlineColor;
					color.A = 1.0;
				}
				else
				{
					switch ( this.type )
					{
						case HandleType.Starting:  color = DrawingContext.ColorHandleStart;     break;
						case HandleType.Ending:    color = DrawingContext.ColorHandleStart;     break;
						case HandleType.Property:  color = DrawingContext.ColorHandleProperty;  break;
						default:                   color = DrawingContext.ColorHandleMain;      break;
					}
				}

				if ( this.type == HandleType.Primary )
				{
					if ( this.constrainType == HandleConstrainType.Smooth )
					{
						rect.Inflate(0.5/scaleX, 0.5/scaleY);
						this.PaintCircle(graphics, rect, DrawingContext.ColorHandleOutline);
						rect.Deflate(1.0/scaleX, 1.0/scaleY);
						this.PaintCircle(graphics, rect, color);
					}
					else if ( this.constrainType == HandleConstrainType.Corner )
					{
						rect.Inflate(0.5/scaleX, 0.5/scaleY);
						this.PaintTriangle(graphics, rect, DrawingContext.ColorHandleOutline);
						rect.Deflate(1.0/scaleX, 1.0/scaleY);
						this.PaintTriangle(graphics, rect, color);
					}
					else
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(color);

						rect.Deflate(0.5/scaleX, 0.5/scaleY);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(DrawingContext.ColorHandleOutline);
					}
				}

				if ( this.type == HandleType.Secondary ||
					 this.type == HandleType.Bezier    )
				{
					rect.Deflate(2.0/scaleX, 2.0/scaleY);
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(DrawingContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Starting )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(DrawingContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Ending )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(DrawingContext.ColorHandleOutline);

					rect.Inflate(2.0/scaleX, 2.0/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(color);
					rect.Inflate(1.0/scaleX, 1.0/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(color);
				}

				if ( this.type == HandleType.Property )
				{
					rect.Deflate(2.0/scaleX, 2.0/scaleY);
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(DrawingContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Center )
				{
					Drawing.Rectangle r1 = rect;
					Drawing.Rectangle r2 = rect;
					r1.Inflate(1.0/scaleX, -2.0/scaleY);
					r2.Inflate(-2.0/scaleX, 1.0/scaleY);

					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(DrawingContext.ColorHandleOutline);

					r1.Deflate(1.0/scaleX, 1.0/scaleY);
					r2.Deflate(1.0/scaleX, 1.0/scaleY);
					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(color);
				}

				if ( this.type == HandleType.Rotate )
				{
					rect.Inflate(0.5/scaleX, 0.5/scaleY);
					this.PaintCircle(graphics, rect, DrawingContext.ColorHandleOutline);
					rect.Deflate(1.0/scaleX, 1.0/scaleY);
					this.PaintCircle(graphics, rect, color);
				}
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine un cercle complet.
		protected void PaintCircle(Graphics graphics, Drawing.Rectangle rect, Color color)
		{
			Point c = new Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			Path path = new Path();
			path.MoveTo(c.X-rx, c.Y);
			path.CurveTo(c.X-rx, c.Y+ry*0.56, c.X-rx*0.56, c.Y+ry, c.X, c.Y+ry);
			path.CurveTo(c.X+rx*0.56, c.Y+ry, c.X+rx, c.Y+ry*0.56, c.X+rx, c.Y);
			path.CurveTo(c.X+rx, c.Y-ry*0.56, c.X+rx*0.56, c.Y-ry, c.X, c.Y-ry);
			path.CurveTo(c.X-rx*0.56, c.Y-ry, c.X-rx, c.Y-ry*0.56, c.X-rx, c.Y);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(color);
		}

		// Dessine un cercle complet.
		protected void PaintTriangle(Graphics graphics, Drawing.Rectangle rect, Color color)
		{
			Path path = new Path();
			path.MoveTo((rect.Left+rect.Right)/2, rect.Top);
			path.LineTo(rect.Left, rect.Bottom);
			path.LineTo(rect.Right, rect.Bottom);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(color);
		}


		#region Serialization
		// S�rialise la poign�e.
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Position", this.position);
			info.AddValue("Type", this.type);
			info.AddValue("ConstrainType", this.constrainType);
			info.AddValue("PropertyType", this.propertyType);
			info.AddValue("PropertyRank", this.propertyRank);
		}

		// Constructeur qui d�s�rialise la poign�e.
		protected Handle(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.position = (Point) info.GetValue("Position", typeof(Point));
			this.type = (HandleType) info.GetValue("Type", typeof(HandleType));
			this.constrainType = (HandleConstrainType) info.GetValue("ConstrainType", typeof(HandleConstrainType));
			this.propertyType = (Properties.Type) info.GetValue("PropertyType", typeof(Properties.Type));
			this.propertyRank = info.GetInt32("PropertyRank");
		}
		#endregion

		
		protected Document					document;
		protected Point						position = new Point(0, 0);
		protected HandleType				type = HandleType.Primary;
		protected HandleConstrainType		constrainType = HandleConstrainType.Symmetric;
		protected bool						isVisible = false;
		protected bool						isHilited = false;
		protected bool						isGlobalSelected = false;
		protected Properties.Type			propertyType = Properties.Type.None;
		protected int						propertyRank;
	}
}
