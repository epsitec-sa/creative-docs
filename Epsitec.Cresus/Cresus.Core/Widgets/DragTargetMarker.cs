﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget sert simplement à dessiner le marqueur '>-----' qui montre la destination d'un drag d'une tuile.
	/// </summary>
	public class DragTargetMarker : Widget
	{
		public DragTargetMarker()
		{
			this.markerColor = Color.FromName ("Red");
		}


		public Color MarkerColor
		{
			get
			{
				return this.markerColor;
			}
			set
			{
				this.markerColor = value;
			}
		}

		public Point HotSpot
		{
			get
			{
				return new Point (System.Math.Floor(this.PreferredHeight/7), this.PreferredHeight/2);
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle rect = this.Client.Bounds;

			//	Dessine le triangle '>'.
			Rectangle triangleRect = new Rectangle (rect.Left, rect.Bottom, rect.Height/2, rect.Height);

			Path trianglePath = new Path ();
			trianglePath.MoveTo (new Point (triangleRect.Right, triangleRect.Center.Y));
			trianglePath.LineTo (triangleRect.BottomLeft);
			trianglePath.LineTo (triangleRect.TopLeft);
			trianglePath.Close ();

			graphics.Rasterizer.AddSurface (trianglePath);
			graphics.RenderSolid (this.markerColor);

			//	Dessine le trait '-------' dégradé.
			Rectangle lineRect = new Rectangle (rect.Left+rect.Height/2, rect.Bottom, rect.Width-rect.Height/2, rect.Height);

			Path linePath = new Path ();
			linePath.MoveTo (new Point (lineRect.Left,  lineRect.Center.Y-0.5));
			linePath.LineTo (new Point (lineRect.Right, lineRect.Center.Y-0.5));
			linePath.LineTo (new Point (lineRect.Right, lineRect.Center.Y+0.5));
			linePath.LineTo (new Point (lineRect.Left,  lineRect.Center.Y+0.5));
			linePath.Close ();

			graphics.Rasterizer.AddSurface (linePath);

			graphics.GradientRenderer.Fill = GradientFill.X;
			graphics.GradientRenderer.SetParameters (-100, 100);
			graphics.GradientRenderer.SetColors (Color.FromAlphaColor (1.0, this.markerColor), Color.FromAlphaColor (0.0, this.markerColor));

			Transform t = Transform.Identity;
			Point center = lineRect.Center;
			t = t.Scale (lineRect.Width/100/2, lineRect.Height/100/2);
			t = t.Translate (center);
			graphics.GradientRenderer.Transform = t;
			graphics.RenderGradient ();  // dégradé de gauche à droite
		}


		private Color markerColor;
	}
}
