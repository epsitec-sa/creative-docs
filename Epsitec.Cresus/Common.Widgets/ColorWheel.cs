namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorWheel permet de choisir une couleur rgb.
	/// </summary>
	public class ColorWheel : Widget
	{
		public ColorWheel()
		{
			this.colorBlack = Drawing.Color.FromName("WindowFrame");
		}
		
		public ColorWheel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		// Retourne la largeur standard.
		public override double DefaultWidth
		{
			get
			{
				return 100;
			}
		}

		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return 100;
			}
		}

		// Couleur.
		public Drawing.Color Color
		{
			get
			{
				Drawing.Color color = Drawing.Color.FromHSV(this.h, this.s, this.v);
				color.A = this.a;
				return color;
			}

			set
			{
				double h,s,v,a;
				value.GetHSV(out h, out s, out v);
				a = value.A;
				if ( h != this.h || s != this.s || v != this.v || a != this.a )
				{
					this.h = h;
					this.s = s;
					this.v = v;
					this.a = a;
					this.ComputePosHandler();
					this.Invalidate();
				}
			}
		}

		
		public void GetHSV(out double h, out double s, out double v)
		{
			h = this.h;
			s = this.s;
			v = this.v;
		}
		
		public void SetHSV(double h, double s, double v)
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
		
		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

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
			Drawing.Point p2 = Drawing.Transform.RotatePoint(this.centerCircle, (180.0+60.0)*System.Math.PI/180.0, p1);
			this.rectTriangle.Left   = p2.X;
			this.rectTriangle.Right  = p1.X;
			this.rectTriangle.Bottom = p2.Y;
			this.rectTriangle.Top    = p2.Y+(p1.Y-p2.Y)*2;

			Drawing.Point p3 = Drawing.Transform.RotatePoint(this.centerCircle, (180.0+45.0)*System.Math.PI/180.0, p1);
			Drawing.Point p4 = Drawing.Transform.RotatePoint(this.centerCircle, 45.0*System.Math.PI/180.0, p1);
			this.rectSquare.Left   = p3.X;
			this.rectSquare.Right  = p4.X;
			this.rectSquare.Bottom = p3.Y;
			this.rectSquare.Top    = p4.Y;

			this.ComputePosHandler();
		}

		// Calcule la position des poignées.
		protected void ComputePosHandler()
		{
			double radius = (this.radiusCircleMax+this.radiusCircleMin)/2;
			double angle = this.h*System.Math.PI*2/360;  // 0..2*PI
			this.posHandlerH = this.centerCircle + Drawing.Transform.RotatePoint(angle, new Drawing.Point(0, radius));

			this.posHandlerSV.X = this.rectSquare.Left + this.rectSquare.Width*this.s;
			this.posHandlerSV.Y = this.rectSquare.Bottom + this.rectSquare.Height*this.v;
		}

		
		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
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
					break;

				case MessageType.MouseMove:
					if ( this.mouseDownH )
					{
						if ( this.DetectH(pos, false, ref this.h) )
						{
							this.mouseDownH = true;
							this.ComputePosHandler();
							this.OnChanged();
							this.Invalidate();
						}
					}
					if ( this.mouseDownSV )
					{
						if ( this.DetectSV(pos, false, ref this.s, ref this.v) )
						{
							this.mouseDownSV = true;
							this.ComputePosHandler();
							this.OnChanged();
							this.Invalidate();
						}
					}
					break;

				case MessageType.MouseUp:
					this.mouseDownH = false;
					this.mouseDownSV = false;
					break;
			}
			
			message.Consumer = this;
		}

		// Détecte la teinte dans le cercle des couleurs.
		protected bool DetectH(Drawing.Point pos, bool restricted, ref double h)
		{
			if ( restricted )
			{
				double dist = Drawing.Point.Distance(this.centerCircle, pos);
				if ( dist > this.radiusCircleMax || dist < this.radiusCircleMin )
				{
					return false;
				}
			}

			double angle = Drawing.Point.ComputeAngle(this.centerCircle, pos);
			angle = Epsitec.Common.Math.ClipAngle(angle-System.Math.PI/2);
			h = angle/(System.Math.PI*2)*360;  // 0..360
			return true;
		}

		
		// Détecte la saturation et l'intensité dans le carré des couleurs.
		protected bool DetectSV(Drawing.Point pos, bool restricted, ref double s, ref double v)
		{
			if ( restricted )
			{
				Drawing.Rectangle rect = this.rectSquare;
				rect.Inflate(this.radiusHandler, this.radiusHandler);
				if ( !rect.Contains(pos) )
				{
					return false;
				}
			}

			s = Epsitec.Common.Math.Clip((pos.X-this.rectSquare.Left)/this.rectSquare.Width);
			v = Epsitec.Common.Math.Clip((pos.Y-this.rectSquare.Bottom)/this.rectSquare.Height);
			return true;
		}


		// Génère un événement pour dire ça a changé.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		// Dessine un cercle inscrit dans un rectangle.
		protected void PathAddCircle(Drawing.Path path,
									 Drawing.Rectangle rect)
		{
			Drawing.Point c = new Drawing.Point((rect.Left+rect.Right)/2, (rect.Bottom+rect.Top)/2);
			double rx = rect.Width/2;
			double ry = rect.Height/2;
			path.MoveTo(c.X-rx, c.Y);
			path.CurveTo(c.X-rx, c.Y+ry*0.56, c.X-rx*0.56, c.Y+ry, c.X, c.Y+ry);
			path.CurveTo(c.X+rx*0.56, c.Y+ry, c.X+rx, c.Y+ry*0.56, c.X+rx, c.Y);
			path.CurveTo(c.X+rx, c.Y-ry*0.56, c.X+rx*0.56, c.Y-ry, c.X, c.Y-ry);
			path.CurveTo(c.X-rx*0.56, c.Y-ry, c.X-rx, c.Y-ry*0.56, c.X-rx, c.Y);
			path.Close();
		}

		// Dessine un cercle dégradé pour la teinte (H).
		protected void PaintGradientCircle(Drawing.Graphics graphics,
										   Drawing.Rectangle rect,
										   Drawing.Color colorBorder,
										   Drawing.Color colorWindow)
		{
			if ( this.IsEnabled )
			{
				double cx = rect.Left+rect.Width/2;
				double cy = rect.Bottom+rect.Height/2;
			
				Drawing.Path path = new Drawing.Path();
				this.PathAddCircle(path, rect);
			
				double[] r = new double[256];
				double[] g = new double[256];
				double[] b = new double[256];
				double[] a = new double[256];
			
				for ( int i=0 ; i<256 ; i++ )
				{
					Drawing.Color.ConvertHSVtoRGB(i/256.0*360.0, 1.0, 1.0, out r[i], out g[i], out b[i]);
					a[i] = 1.0;
				}
			
				graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;
				graphics.Rasterizer.AddSurface(path);
				graphics.GradientRenderer.Fill = Drawing.GradientFill.Conic;
				graphics.GradientRenderer.SetParameters(0, 250);
				graphics.GradientRenderer.SetColors(r, g, b, a);
			
				Drawing.Transform t = new Drawing.Transform();
				t.Translate(cx, cy);
				t.Rotate(-90, cx, cy);  // rouge en haut
				graphics.GradientRenderer.Transform = t;
			
				graphics.RenderGradient();

				// Dessine l'échantillon au milieu.
				Drawing.Rectangle rInside = rect;
				rInside.Inflate(-this.rectCircle.Width*0.125, -this.rectCircle.Width*0.125);
				path = new Drawing.Path();
				this.PathAddCircle(path, rInside);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(colorWindow);

				rInside.Inflate(-0.5, -0.5);
				graphics.AddLine(rInside.Left, (rInside.Bottom+rInside.Top)/2, rInside.Right, (rInside.Bottom+rInside.Top)/2);
				graphics.AddLine((rInside.Left+rInside.Right)/2, rInside.Bottom, (rInside.Left+rInside.Right)/2, rInside.Top);
				graphics.RenderSolid(colorBorder);

				rInside.Inflate(0.5, 0.5);
				path = new Drawing.Path();
				this.PathAddCircle(path, rInside);
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(this.Color);
			}

			rect.Inflate(-0.5, -0.5);
			this.PaintCircle(graphics, rect, colorBorder);
			rect.Inflate(-this.rectCircle.Width*0.125, -this.rectCircle.Width*0.125);
			this.PaintCircle(graphics, rect, colorBorder);
		}

		// Dessine un cercle inscrit dans un rectangle.
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
			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(color);
		}

		// Dessine un carré dégradé pour représenter SV.
		protected void PaintGradientSquare(Drawing.Graphics graphics,
										   Drawing.Rectangle rect,
										   Drawing.Color colorBorder)
	{
			if ( this.IsEnabled )
			{
				for ( double posy=rect.Bottom ; posy<=rect.Top ; posy++ )
				{
					Drawing.Path path = new Drawing.Path();
					path.MoveTo(rect.Left, posy-0.5);
					path.LineTo(rect.Right, posy-0.5);
					path.LineTo(rect.Right, posy+0.5);
					path.LineTo(rect.Left, posy+0.5);
					path.Close();
			
					graphics.Rasterizer.FillMode = Drawing.FillMode.NonZero;
					graphics.Rasterizer.AddSurface(path);
					graphics.GradientRenderer.Fill = Drawing.GradientFill.X;
					graphics.GradientRenderer.SetParameters(rect.Left, rect.Right);

					double factor = (posy-rect.Bottom)/rect.Height;
					Drawing.Color c1 = Drawing.Color.FromBrightness(factor);
					Drawing.Color c2 = Drawing.Color.FromHSV(this.h,1,factor);
					graphics.GradientRenderer.SetColors(c1, c2);

					Drawing.Transform t = new Drawing.Transform();
					t.Translate(0, 0);
					t.Rotate(0, 0, 0);
					graphics.GradientRenderer.Transform = t;

					graphics.RenderGradient();
				}
			}

			graphics.AddRectangle(rect);
			graphics.RenderSolid(colorBorder);
		}

		// Dessine une poignée.
		protected void PaintHandler(Drawing.Graphics graphics,
									Drawing.Point center,
									double radius,
									Drawing.Color color)
		{
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

		// Dessine une poignée.
		protected void PaintHandler(Drawing.Graphics graphics,
									Drawing.Point center,
									double radius)
		{
			double originalWidth = graphics.LineWidth;
			graphics.LineWidth = 2;
			this.PaintHandler(graphics, center, radius+1, Drawing.Color.FromBrightness(1));
			graphics.LineWidth = originalWidth;

			this.PaintHandler(graphics, center, radius, this.colorBlack);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.rectCircle;
			Drawing.Color colorBorder = adorner.ColorBorder;
			Drawing.Color colorWindow = adorner.ColorWindow;
			this.PaintGradientCircle(graphics, rect, colorBorder, colorWindow);

			rect = this.rectSquare;
			graphics.Align(ref rect);
			rect.Inflate(-0.5, -0.5);
			this.PaintGradientSquare(graphics, rect, colorBorder);

			this.PaintHandler(graphics, this.posHandlerH,  this.radiusHandler);
			this.PaintHandler(graphics, this.posHandlerSV, this.radiusHandler);
		}


		protected double					h,s,v,a;
		protected Drawing.Color				colorBlack;
		protected Drawing.Rectangle			rectCircle;
		protected Drawing.Point				centerCircle;
		protected double					radiusCircleMax;
		protected double					radiusCircleMin;
		protected double					radiusHandler;
		protected Drawing.Rectangle			rectTriangle;
		protected Drawing.Rectangle			rectSquare;
		protected Drawing.Point				posHandlerH;
		protected Drawing.Point				posHandlerSV;
		protected bool						mouseDownH = false;
		protected bool						mouseDownSV = false;
	}
}
