using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	public enum HandleType
	{
		Primary,		// poignée principale
		Secondary,		// poignée secondaire
		Bezier,			// poignée secondaire pour courbe de Bézier
		Starting,		// poignée de départ
		Ending,			// poignée d'arrivée
		Hide,			// poignée invisible
		Property,		// poignée d'une propriété
		Center,			// poignée du centre de rotation
		Rotate,			// poignée de l'angle de rotation
	}

	public enum HandleConstrainType
	{
		Symmetric,		// symétrique
		Smooth,			// lisse
		Corner,			// anguleux
		Simply,			// simple (sans coin fantaisie)
	}

	/// <summary>
	/// La classe Handle représente une poignée d'un objet graphique.
	/// </summary>
	public class Handle
	{
		public Handle()
		{
		}

		// Position de la poignée.
		public Drawing.Point Position
		{
			get { return this.position; }
			set { this.position = value; }
		}

		// Type de la poignée.
		[XmlAttribute]
		public HandleType Type
		{
			get { return this.type; }
			set { this.type = value; }
		}

		// Type de la poignée.
		[XmlAttribute]
		public HandleConstrainType ConstrainType
		{
			get { return this.constrainType; }
			set { this.constrainType = value; }
		}

		// Etat "sélectionné" de la poignée.
		[XmlIgnore]
		public bool IsSelected
		{
			get { return this.isSelected; }
			set { this.isSelected = value; }
		}

		// Etat "survolé" de la poignée.
		[XmlIgnore]
		public bool IsHilited
		{
			get { return this.isHilited; }
			set { this.isHilited = value; }
		}

		// Etat "sélectionné global" de la poignée.
		[XmlIgnore]
		public bool IsGlobalSelected
		{
			get { return this.isGlobalSelected; }
			set { this.isGlobalSelected = value; }
		}


		// Copie la poignée courante dans une poignée destination.
		public void CopyTo(Handle dst)
		{
			dst.Position         = this.Position;
			dst.Type             = this.Type;
			dst.ConstrainType    = this.ConstrainType;
			dst.IsSelected       = this.IsSelected;
			dst.IsHilited        = this.IsHilited;
			dst.IsGlobalSelected = this.IsGlobalSelected;
		}


		// Détecte si la souris est dans la poignée.
		public bool Detect(Drawing.Point pos)
		{
			if ( !this.isSelected || this.isGlobalSelected )  return false;
			if ( this.type == HandleType.Hide )  return false;
			Drawing.Rectangle rect = new Drawing.Rectangle();
			rect.Left   = this.position.X - this.handleSize/2 - 3.0/this.scaleX;
			rect.Right  = this.position.X + this.handleSize/2 + 1.0/this.scaleX;
			rect.Bottom = this.position.Y - this.handleSize/2 - 3.0/this.scaleY;
			rect.Top    = this.position.Y + this.handleSize/2 + 1.0/this.scaleY;
			return rect.Contains(pos);
		}


		// Tient compte de la bbox d'une poignée.
		public void BoundingBox(IconContext iconContext, ref Drawing.Rectangle bbox)
		{
			Drawing.Rectangle rect = new Drawing.Rectangle(this.position, Drawing.Size.Empty);
			//rect.Inflate(iconContext.HandleSize/2);  // inutile, fait par Drawer.InvalidateAll
			bbox.MergeWith(rect);
		}


		// Dessine la poignée.
		public void Draw(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( !this.isSelected )  return;

			this.scaleX = iconContext.ScaleX;
			this.scaleY = iconContext.ScaleY;
			this.handleSize = iconContext.HandleSize;

			double initialWidth = graphics.LineWidth;
			graphics.LineWidth = 1.0/this.scaleX;

			Drawing.Point pos = this.position;
			graphics.Align(ref pos);

			if ( this.isGlobalSelected )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = pos.X-this.handleSize*0.25;
				rect.Right  = pos.X+this.handleSize*0.25;
				rect.Bottom = pos.Y-this.handleSize*0.25;
				rect.Top    = pos.Y+this.handleSize*0.25;
				graphics.Align(ref rect);

				if ( this.type == HandleType.Primary )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleGlobal);

					rect.Deflate(0.5/this.scaleX, 0.5/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleOutline);
				}
			}
			else
			{
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = pos.X-this.handleSize*0.5;
				rect.Right  = pos.X+this.handleSize*0.5;
				rect.Bottom = pos.Y-this.handleSize*0.5;
				rect.Top    = pos.Y+this.handleSize*0.5;
				graphics.Align(ref rect);

				Drawing.Color color;
				if ( this.isHilited )
				{
					color = iconContext.HiliteOutlineColor;
					color.A = 1.0;
				}
				else
				{
					switch ( this.type )
					{
						case HandleType.Starting:  color = IconContext.ColorHandleStart;     break;
						case HandleType.Ending:    color = IconContext.ColorHandleStart;     break;
						case HandleType.Property:  color = IconContext.ColorHandleProperty;  break;
						default:                   color = IconContext.ColorHandleMain;      break;
					}
				}

				if ( this.type == HandleType.Primary )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/this.scaleX, 0.5/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Secondary ||
					 this.type == HandleType.Bezier    )
				{
					rect.Deflate(2.0/this.scaleX, 2.0/this.scaleY);
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/this.scaleX, 0.5/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Starting )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/this.scaleX, 0.5/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Ending )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/this.scaleX, 0.5/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleOutline);

					rect.Inflate(2.0/this.scaleX, 2.0/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(color);
					rect.Inflate(1.0/this.scaleX, 1.0/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(color);
				}

				if ( this.type == HandleType.Property )
				{
					graphics.AddFilledRectangle(rect);
					graphics.RenderSolid(color);

					rect.Deflate(0.5/this.scaleX, 0.5/this.scaleY);
					graphics.AddRectangle(rect);
					graphics.RenderSolid(IconContext.ColorHandleOutline);
				}

				if ( this.type == HandleType.Center )
				{
					Drawing.Rectangle r1 = rect;
					Drawing.Rectangle r2 = rect;
					r1.Inflate(2.0/this.scaleX, -2.0/this.scaleY);
					r2.Inflate(-2.0/this.scaleX, 2.0/this.scaleY);

					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(IconContext.ColorHandleOutline);

					r1.Deflate(1.0/this.scaleX, 1.0/this.scaleY);
					r2.Deflate(1.0/this.scaleX, 1.0/this.scaleY);
					graphics.AddFilledRectangle(r1);
					graphics.AddFilledRectangle(r2);
					graphics.RenderSolid(color);
				}

				if ( this.type == HandleType.Rotate )
				{
					rect.Inflate(0.5/this.scaleX, 0.5/this.scaleY);
					this.PaintCircle(graphics, rect, IconContext.ColorHandleOutline);
					rect.Deflate(1.0/this.scaleX, 1.0/this.scaleY);
					this.PaintCircle(graphics, rect, color);
				}
			}

			graphics.LineWidth = initialWidth;
		}

		// Dessine un cercle complet.
		protected void PaintCircle(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Drawing.Color color)
		{
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(c.X-rx, c.Y);
			path.CurveTo(c.X-rx, c.Y+ry*0.56, c.X-rx*0.56, c.Y+ry, c.X, c.Y+ry);
			path.CurveTo(c.X+rx*0.56, c.Y+ry, c.X+rx, c.Y+ry*0.56, c.X+rx, c.Y);
			path.CurveTo(c.X+rx, c.Y-ry*0.56, c.X+rx*0.56, c.Y-ry, c.X, c.Y-ry);
			path.CurveTo(c.X-rx*0.56, c.Y-ry, c.X-rx, c.Y-ry*0.56, c.X-rx, c.Y);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(color);
		}


		protected bool					isSelected = false;
		protected bool					isHilited = false;
		protected bool					isGlobalSelected = false;
		protected double				handleSize;
		protected double				scaleX;
		protected double				scaleY;

		protected HandleType			type = HandleType.Primary;
		protected HandleConstrainType	constrainType = HandleConstrainType.Symmetric;

		protected Drawing.Point			position = new Drawing.Point(0, 0);
	}
}
