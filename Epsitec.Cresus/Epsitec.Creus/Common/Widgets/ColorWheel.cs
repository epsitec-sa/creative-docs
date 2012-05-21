using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ColorWheel permet de choisir une couleur.
	/// </summary>
	public class ColorWheel : Widget
	{
		public ColorWheel()
		{
			this.black = Drawing.Color.FromName("WindowFrame");
		}
		
		public ColorWheel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		static ColorWheel()
		{
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (100.0);
			metadataDy.DefineDefaultValue (100.0);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (ColorWheel), metadataDx);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (ColorWheel), metadataDy);
		}

		public Drawing.RichColor Color
		{
			//	Couleur.
			get
			{
				if ( this.colorSpace == Drawing.ColorSpace.Gray )
				{
					return Drawing.RichColor.FromAGray(this.a, this.g);
				}
				else
				{
					Drawing.RichColor color = Drawing.RichColor.FromAlphaHsv(this.a, this.h, this.s, this.v);
					color.ColorSpace = this.colorSpace;
					return color;
				}
			}

			set
			{
				if ( value.ColorSpace == Drawing.ColorSpace.Gray )
				{
					double g = value.Gray;
					double a = value.A;

					if ( value.ColorSpace != this.colorSpace ||
						 g != this.g || a != this.a )
					{
						this.colorSpace = value.ColorSpace;
						this.g = g;
						this.a = a;
						this.ComputePosHandler();
						this.Invalidate();
					}
				}
				else
				{
					double h,s,v,a;
					value.Basic.GetHsv(out h, out s, out v);
					a = value.A;

					if ( value.ColorSpace != this.colorSpace ||
						 h != this.h || s != this.s || v != this.v || a != this.a )
					{
						this.colorSpace = value.ColorSpace;
						this.h = h;
						this.s = s;
						this.v = v;
						this.a = a;
						this.ComputePosHandler();
						this.Invalidate();
					}
				}
			}
		}

		
		public void GetHsv(out double h, out double s, out double v)
		{
			h = this.h;
			s = this.s;
			v = this.v;
		}
		
		public void SetHsv(double h, double s, double v)
		{
			if ( h != this.h || s != this.s || v != this.v )
			{
				this.h = h;
				this.s = s;
				this.v = v;
				this.ComputePosHandler();
				this.Invalidate();
			}
		}
		
		public void SetAGray(double a, double g)
		{
			if ( a != this.a || g != this.g )
			{
				this.a = a;
				this.g = g;
				this.ComputePosHandler();
				this.Invalidate();
			}
		}

		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			//	Met à jour la géométrie.

			Drawing.Rectangle rect = this.Client.Bounds;
			double dim = System.Math.Min(rect.Width, rect.Height);
			rect.Width = dim;
			rect.Height = dim;

			this.radiusCircleMax = dim*0.500;
			this.radiusCircleMin = dim*0.375;
			this.radiusHandler = (this.radiusCircleMax-this.radiusCircleMin)*0.2;
			this.rectCircle = rect;

			this.centerCircle.X = this.rectCircle.Left+this.rectCircle.Width/2;
			this.centerCircle.Y = this.rectCircle.Bottom+this.rectCircle.Height/2;

			Drawing.Point p1 = this.centerCircle + new Drawing.Point(this.radiusCircleMin*0.95, 0);
			Drawing.Point p2 = Drawing.Transform.RotatePointDeg(this.centerCircle, 180.0+60.0, p1);
			this.rectTriangle.Left   = p2.X;
			this.rectTriangle.Right  = p1.X;
			this.rectTriangle.Bottom = p2.Y;
			this.rectTriangle.Top    = p2.Y+(p1.Y-p2.Y)*2;

			Drawing.Point p3 = Drawing.Transform.RotatePointDeg(this.centerCircle, 180.0+45.0, p1);
			Drawing.Point p4 = Drawing.Transform.RotatePointDeg(this.centerCircle, 45.0, p1);
			this.rectSquare.Left   = p3.X;
			this.rectSquare.Right  = p4.X;
			this.rectSquare.Bottom = p3.Y;
			this.rectSquare.Top    = p4.Y;

			this.ComputePosHandler();
		}

		protected void ComputePosHandler()
		{
			//	Calcule la position des poignées.
			double radius = (this.radiusCircleMax+this.radiusCircleMin)/2;
			
			this.posHandlerH    = this.centerCircle + Drawing.Transform.RotatePointDeg(this.h, new Drawing.Point(0, radius));
			this.posHandlerSV.X = this.rectSquare.Left + this.rectSquare.Width*this.s;
			this.posHandlerSV.Y = this.rectSquare.Bottom + this.rectSquare.Height*this.v;
			this.posHandlerG.X  = this.centerCircle.X;
			this.posHandlerG.Y  = this.rectSquare.Bottom + this.rectSquare.Height*this.g;
		}

		
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un événement.
			switch ( message.MessageType )
			{
				case MessageType.MouseDown:
					if ( this.DetectH(pos, true, ref this.h) )
					{
						this.mouseDownH = true;
						this.ComputePosHandler();
						this.OnChanged();
						this.Invalidate();
					}
					else if ( this.DetectSV(pos, true, ref this.s, ref this.v) )
					{
						this.mouseDownSV = true;
						this.ComputePosHandler();
						this.OnChanged();
						this.Invalidate();
					}
					else if ( this.DetectG(pos, true, ref this.g) )
					{
						this.mouseDownG = true;
						this.ComputePosHandler();
						this.OnChanged();
						this.Invalidate();
					}
					break;

				case MessageType.MouseMove:
					if ( this.mouseDownH )
					{
						if ( this.DetectH(pos, false, ref this.h) )
						{
							this.ComputePosHandler();
							this.OnChanged();
							this.Invalidate();
						}
					}
					if ( this.mouseDownSV )
					{
						if ( this.DetectSV(pos, false, ref this.s, ref this.v) )
						{
							this.ComputePosHandler();
							this.OnChanged();
							this.Invalidate();
						}
					}
					if ( this.mouseDownG )
					{
						if ( this.DetectG(pos, false, ref this.g) )
						{
							this.ComputePosHandler();
							this.OnChanged();
							this.Invalidate();
						}
					}
					break;

				case MessageType.MouseUp:
					this.mouseDownH  = false;
					this.mouseDownSV = false;
					this.mouseDownG  = false;
					break;
			}
			
			message.Consumer = this;
		}

		protected bool DetectH(Drawing.Point pos, bool restricted, ref double h)
		{
			//	Détecte la teinte dans le cercle des couleurs.
			if ( this.colorSpace == Drawing.ColorSpace.Gray )  return false;
			if ( this.IsGrey )  return false;

			if ( restricted )
			{
				double dist = Drawing.Point.Distance(this.centerCircle, pos);
				if ( dist > this.radiusCircleMax || dist < this.radiusCircleMin )
				{
					return false;
				}
			}

			double angle = Drawing.Point.ComputeAngleDeg(this.centerCircle, pos);
			angle = Epsitec.Common.Math.ClipAngleDeg(angle-90);
			h = angle;  // 0..360
			return true;
		}
		
		protected bool DetectSV(Drawing.Point pos, bool restricted, ref double s, ref double v)
		{
			//	Détecte la saturation et l'intensité dans le carré des couleurs.
			if ( this.colorSpace == Drawing.ColorSpace.Gray )  return false;

			if ( restricted )
			{
				Drawing.Rectangle rect = this.rectSquare;
				rect.Inflate(this.radiusHandler);
				if ( !rect.Contains(pos) )
				{
					return false;
				}
			}

			s = Epsitec.Common.Math.Clip((pos.X-this.rectSquare.Left)/this.rectSquare.Width);
			v = Epsitec.Common.Math.Clip((pos.Y-this.rectSquare.Bottom)/this.rectSquare.Height);
			return true;
		}

		protected bool DetectG(Drawing.Point pos, bool restricted, ref double g)
		{
			//	Détecte le niveau de gris.
			if ( this.colorSpace != Drawing.ColorSpace.Gray )  return false;

			if ( restricted )
			{
				Drawing.Rectangle rect = this.rectSquare;
				rect.Inflate(this.radiusHandler);
				rect.Left = rect.Center.X-10.0;
				rect.Width = 2.0*10.0;
				if ( !rect.Contains(pos) )
				{
					return false;
				}
			}

			g = Epsitec.Common.Math.Clip((pos.Y-this.rectSquare.Bottom)/this.rectSquare.Height);
			return true;
		}


		protected virtual void OnChanged()
		{
			//	Génère un événement pour dire ça a changé.
			var handler = this.GetUserEventHandler("Changed");
			if (handler != null)
			{
				handler(this);
			}
		}

		public event EventHandler			Changed
		{
			add
			{
				this.AddUserEventHandler("Changed", value);
			}
			remove
			{
				this.RemoveUserEventHandler("Changed", value);
			}
		}


		//	Dessine un cercle inscrit dans un rectangle.
		protected void PathAddCircle(Drawing.Path path,
									 Drawing.Rectangle rect)
		{
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			
			path.AppendCircle(c.X, c.Y, rx, ry);
		}

		protected bool IsGrey
		{
			//	Indique si la couleur représente un niveau de gris.
			get
			{
				return ( this.s == 0.0 || this.v == 0.0 );
			}
		}

		protected void PaintGradientCircle(Drawing.Graphics graphics,
										   Drawing.Rectangle rect,
										   Drawing.Color colorBorder,
										   Drawing.Color colorWindow)
		{
			//	Dessine un cercle dégradé pour la teinte (H).
			if ( this.IsEnabled )
			{
				double cx = rect.Left+rect.Width/2;
				double cy = rect.Bottom+rect.Height/2;
			
				Drawing.Path path;

				if ( !this.IsGrey )
				{
					path = new Drawing.Path();
					this.PathAddCircle(path, rect);
			
					double[] r = new double[256];
					double[] g = new double[256];
					double[] b = new double[256];
					double[] a = new double[256];
			
					for ( int i=0 ; i<256 ; i++ )
					{
						Drawing.Color.ConvertHsvToRgb(i/256.0*360.0, 1.0, 1.0, out r[i], out g[i], out b[i]);
						a[i] = 1.0;
					}
			
					graphics.FillMode = Drawing.FillMode.NonZero;
					graphics.Rasterizer.AddSurface(path);
					graphics.GradientRenderer.Fill = Drawing.GradientFill.Conic;
					graphics.GradientRenderer.SetParameters(0, 250);
					graphics.GradientRenderer.SetColors(r, g, b, a);

					Drawing.Transform t = Drawing.Transform.Identity;
					t = t.Translate (cx, cy);
					t = t.RotateDeg (-90, cx, cy);  // rouge en haut
					graphics.GradientRenderer.Transform = t;
			
					graphics.RenderGradient();
				}

				//	Dessine l'échantillon au milieu.
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(this.rectCircle.Width*0.125);
				path = new Drawing.Path();
				this.PathAddCircle(path, rInside);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(colorWindow);

				rInside.Deflate(0.5);
				graphics.AddLine(rInside.Left, (rInside.Bottom+rInside.Top)/2, rInside.Right, (rInside.Bottom+rInside.Top)/2);
				graphics.AddLine((rInside.Left+rInside.Right)/2, rInside.Bottom, (rInside.Left+rInside.Right)/2, rInside.Top);
				graphics.RenderSolid(colorBorder);

				rInside.Inflate(0.5);
				path = new Drawing.Path();
				this.PathAddCircle(path, rInside);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.Color.Basic);
			}

			rect.Deflate(0.5);
			this.PaintCircle(graphics, rect, colorBorder);
			rect.Deflate(this.rectCircle.Width*0.125);
			this.PaintCircle(graphics, rect, colorBorder);
		}

		protected void PaintGrayCircle(Drawing.Graphics graphics,
									   Drawing.Rectangle rect,
									   Drawing.Color colorBorder,
									   Drawing.Color colorWindow)
		{
			//	Dessine un cercle gris échantillon.
			if ( this.IsEnabled )
			{
				double cx = rect.Left+rect.Width/2;
				double cy = rect.Bottom+rect.Height/2;
			
				Drawing.Path path;

				//	Dessine l'échantillon au milieu.
				Drawing.Rectangle rInside = rect;
				rInside.Deflate(this.rectCircle.Width*0.125);
				path = new Drawing.Path();
				this.PathAddCircle(path, rInside);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(colorWindow);

				rInside.Deflate(0.5);
				graphics.AddLine(rInside.Left, (rInside.Bottom+rInside.Top)/2, rInside.Right, (rInside.Bottom+rInside.Top)/2);
				graphics.AddLine((rInside.Left+rInside.Right)/2, rInside.Bottom, (rInside.Left+rInside.Right)/2, rInside.Top);
				graphics.RenderSolid(colorBorder);

				rInside.Inflate(0.5);
				path = new Drawing.Path();
				this.PathAddCircle(path, rInside);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.Color.Basic);
			}

			rect.Deflate(0.5);
			this.PaintCircle(graphics, rect, colorBorder);
			rect.Deflate(this.rectCircle.Width*0.125);
			this.PaintCircle(graphics, rect, colorBorder);
		}

		protected void PaintCircle(Drawing.Graphics graphics,
								   Drawing.Rectangle rect,
								   Drawing.Color color)
		{
			//	Dessine un cercle inscrit dans un rectangle.
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			
			using (Drawing.Path path = Drawing.Path.CreateCircle(c.X, c.Y, rx, ry))
			{
				graphics.Rasterizer.AddOutline(path);
				graphics.RenderSolid(color);
			}
		}

		protected void PaintGradientSquare(Drawing.Graphics graphics,
										   Drawing.Rectangle rect,
										   Drawing.Color colorBorder)
	{
			//	Dessine un carré dégradé pour représenter SV.
			if ( this.IsEnabled )
			{
#if true
				Drawing.Color c1 = Drawing.Color.FromBrightness(0);
				Drawing.Color c4 = Drawing.Color.FromBrightness(1);
				Drawing.Color c2 = c1;
				Drawing.Color c3 = Drawing.Color.FromHsv(this.h,1,1);
				
				double x1 = rect.Left;
				double y1 = rect.Bottom;
				double x2 = rect.Right;
				double y2 = rect.Top;
				
				Drawing.Transform transform = graphics.Transform;
				
				transform.TransformDirect(ref x1, ref y1);
				transform.TransformDirect(ref x2, ref y2);
				
				int x  = (int) x1;
				int y  = (int) y1;
				int dx = (int) (x2-x1);
				int dy = (int) (y2-y1);
				
				graphics.SolidRenderer.Clear4Colors(x, y, dx, dy, c1, c2, c3, c4);
#else
				for ( double posy=rect.Bottom ; posy<=rect.Top ; posy++ )
				{
					Drawing.Path path = new Drawing.Path();
					path.MoveTo(rect.Left, posy-0.5);
					path.LineTo(rect.Right, posy-0.5);
					path.LineTo(rect.Right, posy+0.5);
					path.LineTo(rect.Left, posy+0.5);
					path.Close();
			
					graphics.FillMode = Drawing.FillMode.NonZero;
					graphics.Rasterizer.AddSurface(path);
					graphics.GradientRenderer.Fill = Drawing.GradientFill.X;
					graphics.GradientRenderer.SetParameters(rect.Left, rect.Right);

					double factor = (posy-rect.Bottom)/rect.Height;
					Drawing.Color c1 = Drawing.Color.FromBrightness(factor);
					Drawing.Color c2 = Drawing.Color.FromHsv(this.h,1,factor);
					graphics.GradientRenderer.SetColors(c1, c2);

					Drawing.Transform t = new Drawing.Transform();
					t.Translate(0, 0);
					t.Rotate(0, 0, 0);
					graphics.GradientRenderer.Transform = t;

					graphics.RenderGradient();
				}
#endif
			}

			graphics.AddRectangle(rect);
			graphics.RenderSolid(colorBorder);
		}

		protected void PaintGradientGray(Drawing.Graphics graphics,
										 Drawing.Rectangle rect,
										 Drawing.Color colorBorder)
		{
			//	Dessine un carré dégradé pour représenter le niveau de gris.
			if ( this.IsEnabled )
			{
				Drawing.Color c1 = Drawing.Color.FromBrightness(0);
				Drawing.Color c2 = Drawing.Color.FromBrightness(0);
				Drawing.Color c3 = Drawing.Color.FromBrightness(1);
				Drawing.Color c4 = Drawing.Color.FromBrightness(1);
				
				double x1 = rect.Left;
				double y1 = rect.Bottom;
				double x2 = rect.Right;
				double y2 = rect.Top;
				
				Drawing.Transform transform = graphics.Transform;
				
				transform.TransformDirect(ref x1, ref y1);
				transform.TransformDirect(ref x2, ref y2);
				
				int x  = (int) x1;
				int y  = (int) y1;
				int dx = (int) (x2-x1);
				int dy = (int) (y2-y1);
				
				graphics.SolidRenderer.Clear4Colors(x, y, dx, dy, c1, c2, c3, c4);
			}

			graphics.AddRectangle(rect);
			graphics.RenderSolid(colorBorder);
		}

		protected void PaintHandler(Drawing.Graphics graphics,
									Drawing.Point center,
									double radius,
									Drawing.Color color)
		{
			//	Dessine une poignée.
			if ( this.IsEnabled )
			{
				Drawing.Rectangle rect = new Drawing.Rectangle();
				rect.Left   = center.X-radius;
				rect.Right  = center.X+radius;
				rect.Bottom = center.Y-radius;
				rect.Top    = center.Y+radius;
				this.PaintCircle(graphics, rect, color);
			}
		}

		protected void PaintHandler(Drawing.Graphics graphics,
									Drawing.Point center,
									double radius)
		{
			//	Dessine une poignée.
			double originalWidth = graphics.LineWidth;
			graphics.LineWidth = 2;
			this.PaintHandler(graphics, center, radius+1, Drawing.Color.FromBrightness(1));
			graphics.LineWidth = originalWidth;

			this.PaintHandler(graphics, center, radius, this.black);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorners.Factory.Active;

			Drawing.Rectangle rect = this.rectCircle;
			Drawing.Color colorBorder = adorner.ColorBorder;
			Drawing.Color colorWindow = adorner.ColorWindow;

			if ( this.colorSpace == Drawing.ColorSpace.Gray )
			{
				this.PaintGrayCircle(graphics, rect, colorBorder, colorWindow);

				rect = this.rectSquare;
				rect.Left = rect.Center.X-10.0;
				rect.Width = 2.0*10.0;
				graphics.Align(ref rect);
				rect.Deflate(0.5);
				this.PaintGradientGray(graphics, rect, colorBorder);

				this.PaintHandler(graphics, this.posHandlerG, this.radiusHandler);
			}
			else
			{
				this.PaintGradientCircle(graphics, rect, colorBorder, colorWindow);

				rect = this.rectSquare;
				graphics.Align(ref rect);
				rect.Deflate(0.5);
				this.PaintGradientSquare(graphics, rect, colorBorder);

				if ( !this.IsGrey )
				{
					this.PaintHandler(graphics, this.posHandlerH,  this.radiusHandler);
				}
				this.PaintHandler(graphics, this.posHandlerSV, this.radiusHandler);
			}
		}


		protected Drawing.ColorSpace		colorSpace;
		protected double					h,s,v,a,g;
		protected Drawing.Color				black;
		protected Drawing.Rectangle			rectCircle;
		protected Drawing.Point				centerCircle;
		protected double					radiusCircleMax;
		protected double					radiusCircleMin;
		protected double					radiusHandler;
		protected Drawing.Rectangle			rectTriangle;
		protected Drawing.Rectangle			rectSquare;
		protected Drawing.Point				posHandlerH;
		protected Drawing.Point				posHandlerSV;
		protected Drawing.Point				posHandlerG;
		protected bool						mouseDownH  = false;
		protected bool						mouseDownSV = false;
		protected bool						mouseDownG  = false;
	}
}
