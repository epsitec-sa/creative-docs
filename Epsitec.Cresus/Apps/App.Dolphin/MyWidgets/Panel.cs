//	Copyright © 2003-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;
using Epsitec.App.Dolphin.MyWidgets;

[assembly: DependencyClass (typeof(Panel))]

namespace Epsitec.App.Dolphin.MyWidgets
{
	/// <summary>
	/// Simule un panneau conteneur, avec cadre visible ou non, et avec d'éventuelles vis aux quatre coins.
	/// </summary>
	public class Panel : FrameBox
	{
		public Panel() : base()
		{
			//	Détermine au hazard, une fois pour toutes, l'angle des quatre vis.
			System.Random r = new System.Random();
			this.screwAngles = new double[4];
			for (int i=0; i<4; i++)
			{
				this.screwAngles[i] = r.NextDouble()*360;
			}
		}

		public Panel(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		public double Brightness
		{
			get
			{
				return this.brightness;
			}
			set
			{
				if (this.brightness != value)
				{
					this.brightness = value;
					this.Invalidate();
				}
			}
		}

		public bool DrawScrew
		{
			//	Faut-il dessiner les vis dans les quatre coins ?
			get
			{
				return (bool) this.GetValue(Panel.DrawScrewProperty);
			}
			set
			{
				this.SetValue(Panel.DrawScrewProperty, value);
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;
			rect.Deflate(0.5);

			Path rr = new Path();
			rr.AppendRoundedRectangle(rect, 7);

			if (this.brightness != -1)
			{
				graphics.Rasterizer.AddSurface(rr);
				graphics.RenderSolid(DolphinApplication.FromBrightness(this.brightness));
			}

			if (this.DrawFullFrame)
			{
				graphics.Rasterizer.AddOutline(rr);
				graphics.RenderSolid(DolphinApplication.FromBrightness(0));
			}

			if (this.DrawScrew)
			{
				double offset = 7;
				this.PaintScrew(graphics, new Point(rect.Left+offset,  rect.Bottom+offset), 4, this.screwAngles[0]);
				this.PaintScrew(graphics, new Point(rect.Left+offset,  rect.Top-offset   ), 4, this.screwAngles[1]);
				this.PaintScrew(graphics, new Point(rect.Right-offset, rect.Bottom+offset), 4, this.screwAngles[2]);
				this.PaintScrew(graphics, new Point(rect.Right-offset, rect.Top-offset   ), 4, this.screwAngles[3]);
			}

			rr.Dispose();
		}

		protected void PaintScrew(Graphics graphics, Point center, double radius, double angle)
		{
			//	Dessine une vis de face.
			Point p1 = Transform.RotatePointDeg(center, angle+15, center+new Point( radius, 0));
			Point p2 = Transform.RotatePointDeg(center, angle-15, center+new Point(-radius, 0));
			Point p3 = Transform.RotatePointDeg(center, angle-15, center+new Point( radius, 0));
			Point p4 = Transform.RotatePointDeg(center, angle+15, center+new Point(-radius, 0));
			Point p5 = Transform.RotatePointDeg(center, angle,    center+new Point( radius, 0));
			Point p6 = Transform.RotatePointDeg(center, angle,    center+new Point(-radius, 0));

			Rectangle rect = new Rectangle(center.X-radius, center.Y-radius, radius*2, radius*2);

			graphics.AddFilledCircle(center, radius);
			Geometry.RenderVerticalGradient(graphics, rect, DolphinApplication.FromBrightness(0.4), DolphinApplication.FromBrightness(1.0));

			Path fence = new Path();  // chemin approximatif (hexagone irrégulier) pour la fente
			fence.MoveTo(p1);
			fence.LineTo(p2);
			fence.LineTo(p6);
			fence.LineTo(p4);
			fence.LineTo(p3);
			fence.LineTo(p5);
			fence.Close();

			graphics.Rasterizer.AddSurface(fence);
			graphics.RenderSolid(DolphinApplication.FromBrightness(0.5));

			graphics.AddCircle(center, radius);
			graphics.RenderSolid(DolphinApplication.FromBrightness(0));

			graphics.AddLine(p1, p2);
			graphics.AddLine(p3, p4);
			graphics.RenderSolid(DolphinApplication.FromBrightness(0));

			fence.Dispose();
		}


		public static readonly DependencyProperty DrawScrewProperty = DependencyProperty.Register("DrawScrew", typeof(bool), typeof(Panel), new VisualPropertyMetadata(false, VisualPropertyMetadataOptions.AffectsDisplay));

		protected double brightness = -1;
		protected double[] screwAngles;
	}
}
