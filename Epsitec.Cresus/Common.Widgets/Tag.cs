//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 12/02/2004

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe Tag implémente une petite étiquette (pastille) qui peut servir
	/// à l'implémentation de "smart tags".
	/// </summary>
	public class Tag : IconButton
	{
		public Tag()
		{
			this.ButtonStyle = ButtonStyle.Flat;
			this.ResetDefaultColors ();
		}
		
		public Tag(Widget embedder) : this ()
		{
			this.SetEmbedder (embedder);
		}
		
		public Tag(string icon) : base (icon)
		{
			this.ResetDefaultColors  ();
		}
		
		public Tag(string command, string icon) : base (command, icon)
		{
			this.ResetDefaultColors  ();
		}
		
		public Tag(string command, string icon, string name) : base (command, icon, name)
		{
			this.ResetDefaultColors  ();
		}
		

		public void ResetDefaultColors()
		{
			this.BackColor = Drawing.Color.Empty; //Drawing.Color.FromHSV (200, 0.40, 1.00);
			this.Color     = Drawing.Color.Empty; //Drawing.Color.FromHSV (200, 1.00, 1.00);
		}
		
		
		public Drawing.Color					Color
		{
			get
			{
				return this.color;
			}
			set
			{
				if (this.color != value)
				{
					this.color = value;
					this.Invalidate ();
				}
			}
		}
		
		
		protected Drawing.Color GetColorLight(bool disable)
		{
			Drawing.Color color;
			
			if (this.BackColor.IsValid)
			{
				color = this.BackColor;
			}
			else
			{
				double h, s, v;
				color = this.GetColorDark (false);
				color.GetHSV (out h, out s, out v);
				color = Drawing.Color.FromAHSV (color.A, h, s * 0.4, v);
			}
			
			if (disable)
			{
				color = Drawing.Color.FromBrightness (color.GetBrightness ());
			}
			
			return color;
		}
		
		protected Drawing.Color GetColorDark(bool disable)
		{
			Drawing.Color color;
			
			if (this.color.IsValid)
			{
				color = this.color;
			}
			else
			{
				double h, s, v;
				color = Adorner.Factory.Active.ColorCaption;
				color.GetHSV (out h, out s, out v);
				color = Drawing.Color.FromAHSV (color.A, h, s, v * 0.8);
			}
			
			if (disable)
			{
				color = Drawing.Color.FromBrightness (color.GetBrightness ());
			}
			
			return color;
		}
		
		protected Drawing.Color GetColorIcon(bool disable)
		{
			double h, s, v;
			Drawing.Color color = this.GetColorLight (false);
			
			color.GetHSV (out h, out s, out v);
			
			if (v < 0.5)
			{
				v += 0.5;
			}
			else
			{
				v -= 0.5;
			}
			
			color = Drawing.Color.FromAHSV (color.A, h, s, v);
			
			if (disable)
			{
				color = Drawing.Color.FromBrightness (color.GetBrightness ());
			}
			
			return color;
		}
		
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clip_rect)
		{
			Drawing.Path path = new Drawing.Path ();
			
			double dx = this.Client.Width;
			double dy = this.Client.Height;
			double cx = dx / 2;
			double cy = dy / 2;
			double r  = System.Math.Min (cx, cy) - 1;
			
			path.AppendCircle (cx, cy, r);
			
			double ox = cx * 0.8;
			double oy = cy * 1.2;
			
			if ((this.State & WidgetState.Engaged) != 0)
			{
				ox = cx * 1.2;
				oy = cy * 0.8;
			}
			else if ((this.State & WidgetState.Entered) != 0)
			{
				ox = cx * 1.0;
				oy = cy * 1.0;
			}
			
			bool disabled = ! this.IsEnabled;
			
			this.DefineGradientShape (graphics, disabled);
			this.DefineGradientOffset (graphics, ox, oy, r);
			
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderGradient ();
			graphics.Rasterizer.AddOutline (path, 1);
			graphics.RenderSolid (this.GetColorLight (disabled));
			
			path.Clear ();
			path.MoveTo (cx*0.7, cy*1.3);
			path.LineTo (cx*1.0, cy*0.6);
			path.LineTo (cx*1.3, cy*1.3);
			path.Close ();
			
			graphics.Rasterizer.AddSurface (path);
			graphics.RenderSolid (this.GetColorIcon (disabled));
		}
		
		protected void DefineGradientShape(Drawing.Graphics graphics, bool disabled)
		{
			double[] r = new double[256];
			double[] g = new double[256];
			double[] b = new double[256];
			double[] a = new double[256];
			
			Drawing.Color color = this.GetColorDark (disabled);
			Drawing.Color spot  = this.GetColorLight (disabled);
			
			for (int i = 0; i < 256; i++)
			{
				double mix1 = (i/255.0); // * 0.5 + 0.5;
				double mix2 = 1.0 - mix1;
				
				r[i] = mix1*color.R + mix2*spot.R;
				g[i] = mix1*color.G + mix2*spot.G;
				b[i] = mix1*color.B + mix2*spot.B;
				a[i] = mix1*color.A + mix2*spot.A;
			}
			
			graphics.GradientRenderer.SetParameters (0, 100);
			graphics.GradientRenderer.SetColors (r, g, b, a);
			graphics.GradientRenderer.Fill = Drawing.GradientFill.Circle;
		}
		
		protected void DefineGradientOffset(Drawing.Graphics graphics, double cx, double cy, double r)
		{
			Drawing.Transform t = new Drawing.Transform ();
			t.Scale (r / 100.0, r / 100.0);
			t.Translate (cx, cy);
			
			graphics.GradientRenderer.Transform = t;
		}
		
		
		protected Drawing.Color					color;
	}
}
