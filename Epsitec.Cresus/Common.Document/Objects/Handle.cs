using System.Collections.Generic;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	//	ATTENTION: Ne jamais modifier les valeurs existantes de cette liste,
	//	sous peine de plant�e lors de la d�s�rialisation.
	public enum HandleType
	{
		Primary      = 0,		// poign�e principale
		Secondary    = 1,		// poign�e secondaire
		Bezier       = 2,		// poign�e secondaire pour courbe de B�zier
		Starting     = 3,		// poign�e de d�part
		Ending       = 4,		// poign�e d'arriv�e
		Hide         = 5,		// poign�e invisible
		Property     = 6,		// poign�e d'une propri�t�
		Center       = 7,		// poign�e du centre de rotation
		Rotate       = 8,		// poign�e de l'angle de rotation
		Add          = 9,		// poign�e � ajouter
		PropertyZoom = 10,		// poign�e d'une propri�t� pour le zoom
		PropertyMove = 11,		// poign�e d'une propri�t� pour d�placer
	}

	public enum HandleConstrainType
	{
		Symmetric  = 0,		// sym�trique
		Smooth     = 1,		// lisse
		Corner     = 2,		// anguleux
		Simply     = 3,		// simple (sans coin fantaisie)
		SharpRound = 4,		// free: sommet anguleux-arrondi
		RoundSharp = 5,		// free: sommet arrondi-anguleux
		SharpSharp = 6,		// free: sommet anguleux-anguleux
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

		public Point Position
		{
			//	Position de la poign�e.
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

		public Point InitialPosition
		{
			//	Position initiale de la poign�e.
			get
			{
				return this.initialPosition;
			}

			set
			{
				this.initialPosition = value;
			}
		}

		public HandleType Type
		{
			//	Type de la poign�e.
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

		public HandleConstrainType ConstrainType
		{
			//	Type de la contrainte de la poign�e.
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

		public void Modify(bool isVisible, bool isGlobalSelected, bool isManySelected, bool isShaperDeselected)
		{
			//	Modifie l'�tat d'une poign�e.
			if ( this.isVisible          != isVisible          ||
				 this.isGlobalSelected   != isGlobalSelected   ||
				 this.isManySelected     != isManySelected     ||
				 this.isShaperDeselected != isShaperDeselected )
			{
				this.NotifyArea();
				this.isVisible          = isVisible;
				this.isGlobalSelected   = isGlobalSelected;
				this.isManySelected     = isManySelected;
				this.isShaperDeselected = isShaperDeselected;
				this.NotifyArea();
			}
		}

		public bool IsVisible
		{
			//	Etat "visible" de la poign�e.
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

		public bool IsHilited
		{
			//	Etat "survol�" de la poign�e.
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

		public bool IsGlobalSelected
		{
			//	Etat "s�lectionn� global" de la poign�e.
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

		public bool IsManySelected
		{
			//	Etat "s�lectionn� global avec beaucoup d'objets" de la poign�e (cach�).
			get
			{
				return this.isManySelected;
			}

			set
			{
				if ( this.isManySelected != value )
				{
					this.isManySelected = value;
					this.NotifyArea();
				}
			}
		}

		public bool IsShaperDeselected
		{
			//	Etat "modeleur d�s�lectionn�" de la poign�e.
			get
			{
				return this.isShaperDeselected;
			}

			set
			{
				if ( this.isShaperDeselected != value )
				{
					this.isShaperDeselected = value;
					this.NotifyArea();
				}
			}
		}

		public Properties.Type PropertyType
		{
			//	Type de la propri�t� li�e � la poign�e.
			get
			{
				return this.propertyType;
			}

			set
			{
				this.propertyType = value;
			}
		}

		public int PropertyRank
		{
			//	Rang de la poign�e dans la propri�t�.
			get
			{
				return this.propertyRank;
			}

			set
			{
				this.propertyRank = value;
			}
		}

		protected void NotifyArea()
		{
			//	Notifie un changement de la poign�e.
			if ( this.document == null )  return;
			if ( this.document.Notifier == null )  return;
			if ( !this.isVisible )  return;
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
		}


		public void CopyTo(Handle dst)
		{
			//	Copie la poign�e courante dans une poign�e destination.
			dst.position           = this.position;
			dst.initialPosition    = this.initialPosition;
			dst.type               = this.type;
			dst.constrainType      = this.constrainType;
			dst.isVisible          = this.isVisible;
			dst.isHilited          = this.isHilited;
			dst.isGlobalSelected   = this.isGlobalSelected;
			dst.isManySelected     = this.isManySelected;
			dst.isShaperDeselected = this.isShaperDeselected;
			dst.propertyType       = this.propertyType;
			dst.propertyRank       = this.propertyRank;
		}

		public void SwapSelection(Handle h)
		{
			//	Permute les informations de s�lections entre 2 poign�es.
			Misc.Swap(ref this.isVisible,          ref h.isVisible         );
			Misc.Swap(ref this.isGlobalSelected,   ref h.isGlobalSelected  );
			Misc.Swap(ref this.isManySelected,     ref h.isManySelected    );
			Misc.Swap(ref this.isShaperDeselected, ref h.isShaperDeselected);
		}


		public bool Detect(Point pos)
		{
			//	D�tecte si la souris est dans la poign�e.
			if ( !this.isVisible || this.isGlobalSelected || this.isManySelected )  return false;
			if ( this.type == HandleType.Hide )  return false;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double scaleX     = context.ScaleX;
			double handleSize = context.HandleSize*2.0;

			if ( this.type == HandleType.Secondary ||
				 this.type == HandleType.Bezier    )
			{
				handleSize -= 4.0/scaleX;
			}

			if (this.type == HandleType.Property)
			{
				handleSize -= 2.0/scaleX;
			}

			if (this.type == HandleType.PropertyMove)
			{
				handleSize += 2.0/scaleX;
			}

			handleSize += 1.0/scaleX;

			return Handle.Detect(pos, this.position, handleSize);
		}

		static public bool Detect(Point mousePos, Point handlePos, double size)
		{
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = handlePos.X - size/2.0;
			rect.Right  = handlePos.X + size/2.0;
			rect.Bottom = handlePos.Y - size/2.0;
			rect.Top    = handlePos.Y + size/2.0;
			return rect.Contains(mousePos);
		}


		public Drawing.Rectangle BoundingBox
		{
			//	Retourne la bbox d'une poign�e.
			//	Il n'est pas n�cessaire de tenir compte des dimensions de la poign�e,
			//	car la zone � repeindre est toujours engraiss�e en cons�quence juste
			//	avant le Invalidate !
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


		public void Draw(Graphics graphics, DrawingContext context)
		{
			//	Dessine la poign�e.
			if ( !this.isVisible )  return;
			if ( this.isManySelected )  return;

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
				double hs = this.IsShaperDeselected ? handleSize*0.4 : handleSize*0.5;
				rect.Left   = pos.X-hs;
				rect.Right  = pos.X+hs;
				rect.Bottom = pos.Y-hs;
				rect.Top    = pos.Y+hs;
				graphics.Align(ref rect);

				Color color;
				if ( this.isHilited )
				{
					color = Color.FromAlphaColor(1.0, context.HiliteOutlineColor);
				}
				else
				{
					if ( this.IsShaperDeselected )
					{
						color = DrawingContext.ColorHandleGlobal;
					}
					else
					{
						switch ( this.type )
						{
							case HandleType.Starting:      color = DrawingContext.ColorHandleStart;     break;
							case HandleType.Ending:        color = DrawingContext.ColorHandleStart;     break;
							case HandleType.Property:
							case HandleType.PropertyZoom:
							case HandleType.PropertyMove:  color = DrawingContext.ColorHandleProperty;  break;
							default:                       color = DrawingContext.ColorHandleMain;      break;
						}
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
					double d = this.IsShaperDeselected ? 1.0 : 2.0;
					rect.Deflate(d/scaleX, d/scaleY);
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

				if ( this.type == HandleType.Add )
				{
					Drawing.Rectangle r1 = rect;
					Drawing.Rectangle r2 = rect;
					r1.Inflate(0.0/scaleX, -2.0/scaleY);
					r2.Inflate(-2.0/scaleX, 0.0/scaleY);

					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(this.Adapt(DrawingContext.ColorHandleOutline, context));

					r1.Deflate(1.0/scaleX, 1.0/scaleY);
					r2.Deflate(1.0/scaleX, 1.0/scaleY);
					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(this.Adapt(color, context));
				}

				if (this.type == HandleType.PropertyZoom)
				{
					rect.Inflate (0.5/scaleX, 0.5/scaleY);
					this.PaintCircle (graphics, rect, DrawingContext.ColorHandleOutline, context);
					rect.Deflate (1.0/scaleX, 1.0/scaleY);
					this.PaintCircle (graphics, rect, color, context);
				}

				if (this.type == HandleType.PropertyMove)
				{
					rect.Inflate (2.5/scaleX, 2.5/scaleY);
					this.PaintCircle (graphics, rect, DrawingContext.ColorHandleOutline, context);
					rect.Deflate (1.0/scaleX, 1.0/scaleY);
					this.PaintCircle (graphics, rect, color, context);
				}
			}

			graphics.LineWidth = initialWidth;
		}

		protected void PaintCircle(Graphics graphics, Drawing.Rectangle rect, Color color, DrawingContext context)
		{
			//	Dessine un cercle complet.
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			Path path = new Path();
			path.AppendCircle(rect.Center, rx, ry);
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.Adapt(color, context));
		}

		protected void PaintTriangle(Graphics graphics, Drawing.Rectangle rect, Color color, DrawingContext context)
		{
			//	Dessine un cercle complet.
			Path path = new Path();
			path.MoveTo((rect.Left+rect.Right)/2, rect.Top);
			path.LineTo(rect.Left, rect.Bottom);
			path.LineTo(rect.Right, rect.Bottom);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.Adapt(color, context));
		}

		protected Color Adapt(Color color, DrawingContext context)
		{
			//	Adapte une couleur au mode d'aper�u avant impression.
			if ( context.PreviewActive )
			{
				color = Color.FromAlphaColor(0.3, color);
			}
			return color;
		}


		#region Serialization
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise la poign�e.
			info.AddValue("Position", this.position);
			info.AddValue("Type", this.type);
			info.AddValue("ConstrainType", this.constrainType);
		}

		protected Handle(SerializationInfo info, StreamingContext context)
		{
			//	Constructeur qui d�s�rialise la poign�e.
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
		protected bool						isManySelected = false;
		protected bool						isShaperDeselected = false;
		protected Properties.Type			propertyType = Properties.Type.None;
		protected int						propertyRank;
	}
}
