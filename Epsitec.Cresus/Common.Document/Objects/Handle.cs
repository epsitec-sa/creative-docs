using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	// ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	// sous peine de plant�e lors de la d�s�rialisation.
	public enum HandleType
	{
		Primary   = 0,		// poign�e principale
		Secondary = 1,		// poign�e secondaire
		Bezier    = 2,		// poign�e secondaire pour courbe de B�zier
		Starting  = 3,		// poign�e de d�part
		Ending    = 4,		// poign�e d'arriv�e
		Hide      = 5,		// poign�e invisible
		Property  = 6,		// poign�e d'une propri�t�
		Center    = 7,		// poign�e du centre de rotation
		Rotate    = 8,		// poign�e de l'angle de rotation
	}

	public enum HandleConstrainType
	{
		Symmetric = 0,		// sym�trique
		Smooth    = 1,		// lisse
		Corner    = 2,		// anguleux
		Simply    = 3,		// simple (sans coin fantaisie)
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
				if ( this.document.IsSurfaceRotation )
				{
					return Transform.RotatePointDeg(this.document.SurfaceRotationAngle, this.position);
				}
				else
				{
					return this.position;
				}
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

		// Position initiale de la poign�e.
		public Point InitialPosition
		{
			get
			{
				return this.initialPosition;
			}

			set
			{
				this.initialPosition = value;
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
			if ( !this.isVisible )  return;
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		// Copie la poign�e courante dans une poign�e destination.
		public void CopyTo(Handle dst)
		{
			dst.position         = this.position;
			dst.initialPosition  = this.initialPosition;
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
			double handleSize = context.HandleSize*2.0;

			if ( this.type == HandleType.Secondary ||
				 this.type == HandleType.Bezier    )
			{
				handleSize -= 4.0/scaleX;
			}

			if ( this.type == HandleType.Property )
			{
				handleSize -= 2.0/scaleX;
			}

			handleSize += 1.0/scaleX;

			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.position.X - handleSize/2.0;
			rect.Right  = this.position.X + handleSize/2.0;
			rect.Bottom = this.position.Y - handleSize/2.0;
			rect.Top    = this.position.Y + handleSize/2.0;
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
		public void Draw(Graphics graphics, DrawingContext context)
		{
			if ( !this.isVisible )  return;

			double scaleX     = context.ScaleX;
			double scaleY     = context.ScaleY;
			double handleSize = context.HandleSize;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/scaleX;

			Point pos = this.position;
			graphics.Align(ref pos);

			if ( this.isGlobalSelected )
			{
				if ( !context.PreviewActive )
				{
					Drawing.Rectangle rect = new Drawing.Rectangle();
					rect.Left   = pos.X-handleSize*0.25;
					rect.Right  = pos.X+handleSize*0.25;
					rect.Bottom = pos.Y-handleSize*0.25;
					rect.Top    = pos.Y+handleSize*0.25;
					graphics.Align(ref rect);

					if ( this.type == HandleType.Primary  ||
						 this.type == HandleType.Starting ||
						 this.type == HandleType.Ending   )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleGlobal, context));

						rect.Deflate(0.5/scaleX, 0.5/scaleY);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));
					}
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
					color = context.HiliteOutlineColor;
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

				if ( this.type == HandleType.Primary  ||
					 this.type == HandleType.Starting ||
					 this.type == HandleType.Ending   )
				{
					Drawing.Rectangle irect = rect;

					if ( this.constrainType == HandleConstrainType.Smooth )
					{
						rect.Inflate(0.5/scaleX, 0.5/scaleY);
						this.PaintCircle(graphics, rect, DrawingContext.ColorHandleOutline, context);
						rect.Deflate(1.0/scaleX, 1.0/scaleY);
						this.PaintCircle(graphics, rect, color, context);
					}
					else if ( this.constrainType == HandleConstrainType.Corner )
					{
						rect.Inflate(0.5/scaleX, 0.0);
						this.PaintTriangle(graphics, rect, DrawingContext.ColorHandleOutline, context);
						rect.Deflate(1.0/scaleX, 1.0/scaleY);
						this.PaintTriangle(graphics, rect, color, context);
					}
					else
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(this.Adapt(color, context));

						rect.Deflate(0.5/scaleX, 0.5/scaleY);
						graphics.AddRectangle(rect);
						graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));
					}

					if ( this.type == HandleType.Ending )
					{
						irect.Deflate(0.5/scaleX, 0.5/scaleY);
						graphics.AddLine(irect.BottomLeft, irect.TopRight);
						graphics.AddLine(irect.BottomRight, irect.TopLeft);
						graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));
					}
				}

				if ( this.type == HandleType.Secondary ||
					 this.type == HandleType.Bezier    )
				{
					rect.Deflate(2.0/scaleX, 2.0/scaleY);
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.Adapt(color, context));

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));
				}

				if ( this.type == HandleType.Property )
				{
					rect.Deflate(1.0/scaleX, 1.0/scaleY);
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(this.Adapt(color, context));

					rect.Deflate(0.5/scaleX, 0.5/scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));
				}

				if ( this.type == HandleType.Center )
				{
					Drawing.Rectangle r1 = rect;
					Drawing.Rectangle r2 = rect;
					r1.Inflate(1.0/scaleX, -2.0/scaleY);
					r2.Inflate(-2.0/scaleX, 1.0/scaleY);

					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));

					r1.Deflate(1.0/scaleX, 1.0/scaleY);
					r2.Deflate(1.0/scaleX, 1.0/scaleY);
					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(this.Adapt(color, context));
				}

				if ( this.type == HandleType.Rotate )
				{
					rect.Inflate(0.5/scaleX, 0.5/scaleY);
					this.PaintCircle(graphics, rect, DrawingContext.ColorHandleOutline, context);
					rect.Deflate(1.0/scaleX, 1.0/scaleY);
					this.PaintCircle(graphics, rect, color, context);
				}
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine un cercle complet.
		protected void PaintCircle(Graphics graphics, Drawing.Rectangle rect, Color color, DrawingContext context)
		{
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			Path path = new Path();
			path.AppendCircle(rect.Center, rx, ry);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.Adapt(color, context));
		}

		// Dessine un cercle complet.
		protected void PaintTriangle(Graphics graphics, Drawing.Rectangle rect, Color color, DrawingContext context)
		{
			Path path = new Path();
			path.MoveTo((rect.Left+rect.Right)/2, rect.Top);
			path.LineTo(rect.Left, rect.Bottom);
			path.LineTo(rect.Right, rect.Bottom);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.Adapt(color, context));
		}

		// Adapte une couleur au mode d'aper�u avant impression.
		protected Color Adapt(Color color, DrawingContext context)
		{
			if ( context.PreviewActive )
			{
				//?color = Color.FromBrightness(color.GetBrightness());
				color.A *= 0.3;
			}
			return color;
		}


		#region Serialization
		// S�rialise la poign�e.
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Position", this.position);
			info.AddValue("Type", this.type);
			info.AddValue("ConstrainType", this.constrainType);
		}

		// Constructeur qui d�s�rialise la poign�e.
		protected Handle(SerializationInfo info, StreamingContext context)
		{
			this.document = Document.ReadDocument;
			this.position = (Point) info.GetValue("Position", typeof(Point));
			this.type = (HandleType) info.GetValue("Type", typeof(HandleType));
			this.constrainType = (HandleConstrainType) info.GetValue("ConstrainType", typeof(HandleConstrainType));
		}
		#endregion

		
		protected Document					document;
		protected Point						position = new Point(0, 0);
		protected Point						initialPosition = new Point(0, 0);
		protected HandleType				type = HandleType.Primary;
		protected HandleConstrainType		constrainType = HandleConstrainType.Symmetric;
		protected bool						isVisible = false;
		protected bool						isHilited = false;
		protected bool						isGlobalSelected = false;
		protected Properties.Type			propertyType = Properties.Type.None;
		protected int						propertyRank;
	}
}
