using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	/// <summary>
	/// La classe HRuler implémente la règle horizontale.
	/// </summary>
	public class HRuler : AbstractRuler
	{
		public HRuler() : base(false)
		{
		}
		
		public HRuler(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		public override double DefaultHeight
		{
			get
			{
				return AbstractRuler.defaultBreadth;
			}
		}


		public double MarginLeftFirst
		{
			get
			{
				return this.marginLeftFirst;
			}

			set
			{
				if ( this.marginLeftFirst != value )
				{
					this.marginLeftFirst = value;
					this.Invalidate();
				}
			}
		}

		public double MarginLeftBody
		{
			get
			{
				return this.marginLeftBody;
			}

			set
			{
				if ( this.marginLeftBody != value )
				{
					this.marginLeftBody = value;
					this.Invalidate();
				}
			}
		}

		public double MarginRight
		{
			get
			{
				return this.marginRight;
			}

			set
			{
				if ( this.marginRight != value )
				{
					this.marginRight = value;
					this.Invalidate();
				}
			}
		}


		protected override void InvalidateBoxMarker()
		{
			if ( !this.markerVisible )  return;

			Rectangle rect = this.Client.Bounds;
			double scale = (this.ending-this.starting)/rect.Width;
			double posx = (this.marker-this.starting)/scale;
			rect.Left  = posx-4;
			rect.Right = posx+4;

			if ( rect.IntersectsWith(this.Client.Bounds) )
			{
				this.invalidateBox.MergeWith(rect);
			}
		}


		// Dessine la graduation.
		protected void PaintGrad(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			graphics.Align(ref rect);

			graphics.Color = adorner.ColorTextFieldBorder(this.IsEnabled);
			Font font = Font.GetFont("Tahoma", "Regular");

			double space = 3.0;
			double mul = 1.0;
			if ( this.ppm == 254.0 )
			{
				space = 1.0;
				mul = 2.54;
			}

			double scale = (this.ending-this.starting)/rect.Width;
			double step = System.Math.Pow(10.0, System.Math.Ceiling(System.Math.Log(scale*space, 10.0)))*mul;
			double grad = System.Math.Floor(this.starting/step)*step;

			graphics.SolidRenderer.Color = adorner.ColorText(this.PaintState);
			while ( grad < this.ending )
			{
				double posx = (grad-this.starting)/scale;
				int rank = (int) (System.Math.Floor(grad/step+0.5));

				if ( posx >= clipRect.Left-1.0  &&
					 posx <= clipRect.Right+1.0 )
				{
					double h = rect.Height;
					if ( rank%10 == 0 )  h *= 1.0;
					else if ( rank% 5 == 0 )  h *= 0.4;
					else                      h *= 0.2;
					graphics.AddLine(posx, 0, posx, h);
				}

				if ( rank%10 == 0 && posx <= clipRect.Right )
				{
					double value = grad/this.ppm;
					value *= 1000000.0;
					value = System.Math.Floor(value+0.5);  // arrondi à la 6ème décimale
					value /= 1000000.0;
					string text = value.ToString();

					double size = rect.Height*0.6;
					Rectangle bounds = font.GetTextBounds(text);
					bounds.Scale(size);
					bounds.Offset(posx+2, 0);

					if ( bounds.IntersectsWith(clipRect) )
					{
						graphics.PaintText(posx+2, rect.Top-size, text, font, size);
					}
				}
				
				grad += step;
			}
			graphics.RenderSolid(adorner.ColorText(this.PaintState));
		}

		// Dessine le marqueur de position de la souris.
		protected void PaintMarker(Graphics graphics)
		{
			if ( !this.markerVisible )  return;

			if ( this.marker < this.starting ||
				 this.marker > this.ending   )  return;

			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			Rectangle rect = this.Client.Bounds;
			graphics.Align(ref rect);

			double scale = (this.ending-this.starting)/rect.Width;
			double posx = (this.marker-this.starting)/scale;

			Path path = new Path();
			path.MoveTo(posx, 1);
			path.LineTo(posx-4, rect.Top);
			path.LineTo(posx+4, rect.Top);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(adorner.ColorCaption);

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}

		// Dessine la marge gauche pour la première ligne.
		protected void PaintMarginLeftFirst(Graphics graphics, bool hilite)
		{
			if ( !this.edited )  return;

			if ( this.marginLeftFirst < this.starting ||
				 this.marginLeftFirst > this.ending   )  return;

			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			double posx = this.GetHandleHorizontalPos(0);
			rect.Left  = posx-3;
			rect.Right = posx+3;
			graphics.Align(ref rect);

			Path path = new Path();
			path.MoveTo((rect.Left+rect.Right)/2, rect.Bottom+8);
			path.LineTo(rect.Left-0.5,  rect.Top-2);
			path.LineTo(rect.Left-0.5,  rect.Top-0);
			path.LineTo(rect.Right+0.5, rect.Top-0);
			path.LineTo(rect.Right+0.5, rect.Top-2);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.ColorBackgroundMargins(hilite));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.ColorBorderMargins);
		}

		// Dessine la marge gauche pour le corps du texte.
		protected void PaintMarginLeftBody(Graphics graphics, bool hilite)
		{
			if ( !this.edited )  return;

			if ( this.marginLeftBody < this.starting ||
				 this.marginLeftBody > this.ending   )  return;

			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			double posx = this.GetHandleHorizontalPos(1);
			rect.Left  = posx-3;
			rect.Right = posx+3;
			graphics.Align(ref rect);

			Path path = new Path();
			path.MoveTo((rect.Left+rect.Right)/2, rect.Top-4);
			path.LineTo(rect.Left-0.5,  rect.Bottom+6);
			path.LineTo(rect.Left-0.5,  rect.Bottom+0);
			path.LineTo(rect.Right+0.5, rect.Bottom+0);
			path.LineTo(rect.Right+0.5, rect.Bottom+6);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.ColorBackgroundMargins(hilite));

			graphics.Rasterizer.AddOutline(path);
			graphics.AddLine(rect.Left-0.5, rect.Bottom+4.5, rect.Right+0.5, rect.Bottom+4.5);
			graphics.RenderSolid(this.ColorBorderMargins);
		}

		// Dessine la marge droite.
		protected void PaintMarginRight(Graphics graphics, bool hilite)
		{
			if ( !this.edited )  return;

			if ( this.marginRight < this.starting ||
				 this.marginRight > this.ending   )  return;

			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			Rectangle rect = this.Client.Bounds;

			double posx = this.GetHandleHorizontalPos(3);
			rect.Left  = posx-3;
			rect.Right = posx+3;
			graphics.Align(ref rect);

			Path path = new Path();
			path.MoveTo((rect.Left+rect.Right)/2, rect.Top-4);
			path.LineTo(rect.Left-0.5,  rect.Bottom+6);
			path.LineTo(rect.Left-0.5,  rect.Bottom+0);
			path.LineTo(rect.Right+0.5, rect.Bottom+0);
			path.LineTo(rect.Right+0.5, rect.Bottom+6);
			path.Close();

			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(this.ColorBackgroundMargins(hilite));

			graphics.Rasterizer.AddOutline(path);
			graphics.RenderSolid(this.ColorBorderMargins);
		}

		// Dessine tous les tabulateurs.
		protected void PaintTabs(Graphics graphics)
		{
			if ( !this.edited )  return;
			if ( this.tabs == null || this.tabs.Length == 0 )  return;

			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;

			for ( int i=0 ; i<this.tabs.Length ; i++ )
			{
				Rectangle rect = this.Client.Bounds;

				double posx = this.GetHandleHorizontalPos(4+i);
				rect.Left  = posx-rect.Height/2;
				rect.Right = posx+rect.Height/2;

				Common.Widgets.GlyphShape glyph = Common.Widgets.GlyphShape.TabRight;
				switch ( this.tabs[i].Type )
				{
					case Drawing.TextTabType.Right:    glyph = Common.Widgets.GlyphShape.TabRight;    break;
					case Drawing.TextTabType.Left:     glyph = Common.Widgets.GlyphShape.TabLeft;     break;
					case Drawing.TextTabType.Center:   glyph = Common.Widgets.GlyphShape.TabCenter;   break;
					case Drawing.TextTabType.Decimal:  glyph = Common.Widgets.GlyphShape.TabDecimal;  break;
					case Drawing.TextTabType.Indent:   glyph = Common.Widgets.GlyphShape.TabIndent;   break;
				}
				adorner.PaintGlyph(graphics, rect, Common.Widgets.WidgetState.Enabled, glyph, Common.Widgets.PaintTextStyle.Button);
			}
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			
			Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.ColorBackground);  // dessine le fond

			if ( this.edited )
			{
				Rectangle zone = rect;
				double scale = (this.ending-this.starting)/rect.Width;
				zone.Left = (this.limitLow-this.starting)/scale;
				zone.Right = (this.limitHigh-this.starting)/scale;
				graphics.AddFilledRectangle(zone);
				graphics.RenderSolid(this.ColorBackgroundEdited);
			}

			this.PaintGrad(graphics, clipRect);
			this.PaintMarker(graphics);

			if ( this.edited )
			{
				this.PaintMarginLeftFirst(graphics, this.HiliteRank == 0 || this.HiliteRank == 2);
				this.PaintMarginLeftBody(graphics, this.HiliteRank == 1 || this.HiliteRank == 2);
				this.PaintMarginRight(graphics, this.HiliteRank == 3);
				this.PaintTabs(graphics);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);  // dessine le cadre
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}


		protected override int MoveDetect(Point pos)
		{
			if ( !this.edited )  return -1;
			if ( this.editObject == null )  return -1;

			int total = 4;
			if ( this.tabs != null )  total += this.tabs.Length;
			for ( int rank=total-1 ; rank>=0 ; rank-- )
			{
				double posx = this.GetHandleHorizontalPos(rank);
				if ( pos.X < posx-5 || pos.X > posx+5 )  continue;
				if ( rank == 0 && pos.Y < this.Client.Bounds.Top-4    )  continue;
				if ( rank == 1 && pos.Y > this.Client.Bounds.Bottom+8 )  continue;
				if ( rank == 2 && pos.Y > this.Client.Bounds.Bottom+4 )  continue;
				return rank;
			}
			return -1;
		}

		protected override void MoveBeginning(int rank, Point pos)
		{
			double initial = this.GetHandleHorizontalPos(rank);
			this.moveOffset = initial - pos.X;
		}

		protected override void MoveDragging(int rank, Point pos)
		{
			pos.X += this.moveOffset;

			if ( rank == 0 )  // left first ?
			{
				double posBody = this.GetHandleHorizontalPos(1);
				if ( System.Math.Abs(pos.X-posBody) < 3 )
				{
					pos.X = posBody;  // met pile sur le left body
				}
			}

			this.SetHandleHorizontalPos(rank, pos);

			pos.X = this.GetHandleHorizontalPos(rank);
			pos = this.document.Modifier.ActiveViewer.ScreenToInternal(pos);
			this.document.Modifier.ActiveViewer.MarkerVertical = pos.X;
		}

		protected override void MoveEnding(int rank, Point pos)
		{
			this.document.Modifier.ActiveViewer.MarkerVertical = double.NaN;
		}

		protected double GetHandleHorizontalPos(int rank)
		{
			double scale = (this.ending-this.starting)/this.Client.Bounds.Width;

			if ( rank == 0 )  return (this.marginLeftFirst-this.starting)/scale;
			if ( rank == 1 )  return (this.marginLeftBody-this.starting)/scale;
			if ( rank == 2 )  return (this.marginLeftBody-this.starting)/scale;
			if ( rank == 3 )  return (this.marginRight-this.starting)/scale;

			rank -= 4;  // 0..n
			if ( rank < this.tabs.Length )
			{
				return (this.tabs[rank].Pos-this.starting)/scale;
			}

			return 0;
		}

		protected void SetHandleHorizontalPos(int rank, Point pos)
		{
			Drawing.Rectangle bbox = this.editObject.BoundingBoxThin;
			double scale = (this.ending-this.starting)/this.Client.Bounds.Width;

			if ( rank < 4 )
			{
				double leftFirst, leftBody, right;
				Text.Properties.SizeUnits units;
				this.editObject.GetTextLeftMargins(out leftFirst, out leftBody, out units, true);
				this.editObject.GetTextRightMargins(out right, out units, true);
				units = Common.Text.Properties.SizeUnits.Points;

				if ( rank == 0 )  // left first ?
				{
					leftFirst = this.starting + pos.X*scale;
					leftFirst = leftFirst - bbox.Left;
					leftFirst = this.SnapGrid(leftFirst);
					leftFirst = System.Math.Max(leftFirst, 0);
					this.editObject.SetTextLeftMargins(leftFirst, leftBody, units);
				}

				if ( rank == 1 )  // left body ?
				{
					leftBody = this.starting + pos.X*scale;
					leftBody = leftBody - bbox.Left;
					leftBody = this.SnapGrid(leftBody);
					leftBody = System.Math.Max(leftBody, 0);
					this.editObject.SetTextLeftMargins(leftFirst, leftBody, units);
				}

				if ( rank == 2 )  // first + body ?
				{
					double initialBody = leftBody;
					leftBody = this.starting + pos.X*scale;
					leftBody = leftBody - bbox.Left;
					leftBody = this.SnapGrid(leftBody);
					leftBody = System.Math.Max(leftBody, 0);
					leftFirst += leftBody - initialBody;
					leftFirst = this.SnapGrid(leftFirst);
					leftFirst = System.Math.Max(leftFirst, 0);
					this.editObject.SetTextLeftMargins(leftFirst, leftBody, units);
				}

				if ( rank == 3 )  // right ?
				{
					right = this.starting + pos.X*scale;
					right = bbox.Right - right;
					right = this.SnapGrid(right);
					right = System.Math.Max(right, 0);
					this.editObject.SetTextRightMargins(right, units);
				}
			}
			else	// tabulateur ?
			{
				rank -= 4;  // 0..n
				if ( rank < this.tabs.Length )
				{
					double tabPos;
					TextTabType type;
					this.editObject.GetTextTab(rank, out tabPos, out type);

					tabPos = this.starting + pos.X*scale;
					tabPos = tabPos - bbox.Left;
					tabPos = this.SnapGrid(tabPos);
					tabPos = System.Math.Max(tabPos, 0);
					this.editObject.SetTextTab(rank, tabPos, type);
				}
			}
		}

		protected double SnapGrid(double value)
		{
			if ( this.document == null )  return value;

			Point pos = new Point(value, 0);
			this.document.Modifier.ActiveViewer.DrawingContext.SnapGrid(ref pos);
			return pos.X;
		}


		protected double					marginLeftFirst = 0.0;
		protected double					marginLeftBody = 0.0;
		protected double					marginRight = 0.0;
		protected double					moveOffset = 0.0;
	}
}
