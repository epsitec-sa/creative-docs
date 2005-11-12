using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;

namespace Epsitec.Common.Document.Widgets
{
	public struct Tab
	{
		public string			Tag;
		public double			Pos;
		public TextTabType		Type;
		public bool				Zombie;
	}

	/// <summary>
	/// La classe HRuler implémente la règle horizontale.
	/// </summary>
	public class HRuler : AbstractRuler
	{
		public HRuler() : base(false)
		{
			this.invalidTab.Tag = null;
			this.invalidTab.Pos = 0;
			this.invalidTab.Type = TextTabType.None;
			this.invalidTab.Zombie = false;
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


		// Position de la marge gauche pour la première ligne.
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

		// Position de la marge gauche pour le corps du texte.
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

		// Position de la marge droite.
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

		// Liste des tabulateurs.
		public Tab[] Tabs
		{
			get
			{
				return this.tabs;
			}

			set
			{
				if ( this.isDragging || !HRuler.TabCompare(this.tabs, value) )
				{
					this.tabs = value;
					this.Invalidate();
				}
			}
		}


		// Invalide la zone contenant le marqueur.
		protected override void InvalidateBoxMarker()
		{
			if ( !this.markerVisible )  return;

			Rectangle rect = this.Client.Bounds;
			double posx = this.DocumentToScreen(this.marker);
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

			double posx = this.DocumentToScreen(this.marker);

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

			double posx = this.GetHandleHorizontalPos("LeftFirst");
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

			double posx = this.GetHandleHorizontalPos("LeftBody");
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

			double posx = this.GetHandleHorizontalPos("Right");
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

			for ( int pass=1 ; pass<=3 ; pass++ )
			{
				if ( pass == 2 && this.draggingTabDest == null )  continue;

				foreach ( Tab tab in this.tabs )
				{
					if ( tab.Tag == this.draggingTabToDelete )  continue;
				
					Rectangle rect = this.Client.Bounds;

					double posx = this.GetHandleHorizontalPos(tab);
					rect.Left  = posx-rect.Height/2;
					rect.Right = posx+rect.Height/2;
					graphics.Align(ref rect);
					rect.Bottom += 3.0;

					Color colorBack = Color.FromBrightness(1);

					if ( tab.Tag == this.HiliteHandle )  // tabulateur survolé par la souris ?
					{
						colorBack = this.ColorBackgroundMargins(true);
					}
					else
					{
						colorBack = tab.Zombie ? DrawingContext.ColorTabZombie : this.ColorBackgroundEdited;
					}

					Color colorGlyph = Color.FromBrightness(0);  // noir
					if ( colorBack.GetBrightness() <= 0.4 )  // couleur foncée ?
					{
						colorGlyph = Color.FromBrightness(1);  // blanc
					}

					if ( pass == 1 )
					{
						graphics.AddFilledRectangle(rect);
						graphics.RenderSolid(colorBack);
					}
					else if ( pass == 2 )
					{
						if ( tab.Tag == this.draggingTabDest )
						{
							graphics.AddFilledCircle(rect.Center, rect.Width/2);
							graphics.RenderSolid(this.ColorBackgroundMargins(true));

							graphics.AddCircle(rect.Center, rect.Width/2);
							graphics.RenderSolid(colorGlyph);
						}
					}
					else if ( pass == 3 )
					{
						rect.Inflate(5);
						rect.Offset(0.5, 0);

						Common.Widgets.GlyphShape glyph = Common.Widgets.GlyphShape.TabRight;
						switch ( tab.Type )
						{
							case Drawing.TextTabType.Right:    glyph = Common.Widgets.GlyphShape.TabRight;    break;
							case Drawing.TextTabType.Left:     glyph = Common.Widgets.GlyphShape.TabLeft;     break;
							case Drawing.TextTabType.Center:   glyph = Common.Widgets.GlyphShape.TabCenter;   break;
							case Drawing.TextTabType.Decimal:  glyph = Common.Widgets.GlyphShape.TabDecimal;  break;
							case Drawing.TextTabType.Indent:   glyph = Common.Widgets.GlyphShape.TabIndent;   break;
						}
						Common.Widgets.WidgetState state = Common.Widgets.WidgetState.Enabled;
						adorner.PaintGlyph(graphics, rect, state, colorGlyph, glyph, Common.Widgets.PaintTextStyle.Button);
					}
				}
			}
		}

		// Dessine le "bouton" pour choisir le type de tabulateur.
		protected void PaintTabChoice(Graphics graphics)
		{
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			rect.Width = rect.Height;

			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.ColorBackgroundEdited);

			rect.Deflate(0.5);
			rect.Width += 1;
			graphics.AddRectangle(rect);
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
			rect.Width -= 1;
			rect.Inflate(0.5);
			rect.Bottom += 3.0;
			rect.Inflate(5);
			rect.Offset(0.5, -1);

			Common.Widgets.GlyphShape glyph = Common.Widgets.GlyphShape.TabRight;
			switch ( this.tabToCreate )
			{
				case Drawing.TextTabType.Right:    glyph = Common.Widgets.GlyphShape.TabRight;    break;
				case Drawing.TextTabType.Left:     glyph = Common.Widgets.GlyphShape.TabLeft;     break;
				case Drawing.TextTabType.Center:   glyph = Common.Widgets.GlyphShape.TabCenter;   break;
				case Drawing.TextTabType.Decimal:  glyph = Common.Widgets.GlyphShape.TabDecimal;  break;
				case Drawing.TextTabType.Indent:   glyph = Common.Widgets.GlyphShape.TabIndent;   break;
			}
			adorner.PaintGlyph(graphics, rect, Common.Widgets.WidgetState.Enabled, glyph, Common.Widgets.PaintTextStyle.Button);
		}
		
		// Dessine toute la règle.
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Common.Widgets.Adorner.Factory.Active;
			
			Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			graphics.RenderSolid(this.ColorBackground);  // dessine le fond

			if ( this.edited )
			{
				Rectangle zone = rect;
				zone.Left  = this.DocumentToScreen(this.limitLow);
				zone.Right = this.DocumentToScreen(this.limitHigh);
				graphics.AddFilledRectangle(zone);
				graphics.RenderSolid(this.ColorBackgroundEdited);
			}

			this.PaintGrad(graphics, clipRect);
			this.PaintMarker(graphics);

			if ( this.edited )  // édition en cours ?
			{
				this.PaintTabs(graphics);
				this.PaintMarginLeftFirst(graphics, this.HiliteHandle == "LeftFirst" || this.HiliteHandle == "FirstBody");
				this.PaintMarginLeftBody (graphics, this.HiliteHandle == "LeftBody"  || this.HiliteHandle == "FirstBody");
				this.PaintMarginRight    (graphics, this.HiliteHandle == "Right");
				this.PaintTabChoice(graphics);
			}

			rect.Deflate(0.5);
			graphics.AddRectangle(rect);  // dessine le cadre
			graphics.RenderSolid(adorner.ColorTextFieldBorder(this.IsEnabled));
		}


		// Détecte les tabulateurs visés par la souris.
		protected string[] DetectTabs(Point pos, string exclude)
		{
			int total = 0;
			for ( int i=0 ; i<this.tabs.Length ; i++ )
			{
				Tab tab = this.tabs[i];
				if ( tab.Tag == exclude )  continue;

				double posx = this.GetHandleHorizontalPos(tab);
				if ( pos.X < posx-5 || pos.X > posx+5 )  continue;
				
				total ++;
			}
			if ( total == 0 )  return null;

			string[] list = new string[total];
			int j = 0;
			for ( int i=0 ; i<this.tabs.Length ; i++ )
			{
				Tab tab = this.tabs[i];
				if ( tab.Tag == exclude )  continue;

				double posx = this.GetHandleHorizontalPos(tab);
				if ( pos.X < posx-5 || pos.X > posx+5 )  continue;
				
				list[j++] = tab.Tag;
			}

			return list;
		}

		// Détecte la poignée visée par la souris.
		protected override string DraggingDetect(Point pos, string exclude)
		{
			if ( !this.edited )  return null;
			if ( this.editObject == null )  return null;

			if ( this.DetectHandle("Right",     pos) )  return "Right";
			if ( this.DetectHandle("FirstBody", pos) )  return "FirstBody";
			if ( this.DetectHandle("LeftBody",  pos) )  return "LeftBody";
			if ( this.DetectHandle("LeftFirst", pos) )  return "LeftFirst";

			for ( int i=this.tabs.Length-1 ; i>=0 ; i-- )
			{
				Tab tab = this.tabs[i];
				if ( tab.Tag == exclude )  continue;

				double posx = this.GetHandleHorizontalPos(tab);
				if ( pos.X < posx-5 || pos.X > posx+5 )  continue;
				
				return tab.Tag;
			}
			return null;
		}

		// Détecte si la souris est sur une poignée.
		protected bool DetectHandle(string handle, Point pos)
		{
			double posx = this.GetHandleHorizontalPos(handle);
			if ( pos.X < posx-5 || pos.X > posx+5 )  return false;

			if ( handle == "LeftFirst" && pos.Y < this.Client.Bounds.Top-4    )  return false;
			if ( handle == "LeftBody"  && pos.Y > this.Client.Bounds.Bottom+8 )  return false;
			if ( handle == "FirstBody" && pos.Y > this.Client.Bounds.Bottom+4 )  return false;

			return true;
		}

		// Début du drag d'une poignée.
		protected override void DraggingStart(ref string handle, Point pos)
		{
			this.draggingTabToDelete = null;
			this.draggingTabDest = null;
			this.draggingFirstMove = true;
			
			if ( handle == null )
			{
				Rectangle rect = this.Client.Bounds;
				rect.Width = rect.Height;
				if ( rect.Contains(pos) )  // change le type de tabulateur à insérer ?
				{
					switch ( this.tabToCreate )
					{
						case Drawing.TextTabType.Right:    this.tabToCreate = Drawing.TextTabType.Center;   break;
						case Drawing.TextTabType.Center:   this.tabToCreate = Drawing.TextTabType.Left;     break;
						case Drawing.TextTabType.Left:     this.tabToCreate = Drawing.TextTabType.Decimal;  break;
						case Drawing.TextTabType.Decimal:  this.tabToCreate = Drawing.TextTabType.Indent;   break;
						case Drawing.TextTabType.Indent:   this.tabToCreate = Drawing.TextTabType.Right;    break;
					}
					this.Invalidate(rect);
				}
				else
				{
					Drawing.Rectangle bbox = this.editObject.BoundingBoxThin;
					double tabPos = this.ScreenToDocument(pos.X);
					tabPos = tabPos - bbox.Left;
					handle = this.editObject.NewTextTab(tabPos, this.tabToCreate);
					this.draggingOffset = 0.0;
					this.draggingFirstMove = false;
				}
			}
			else
			{
				double initial = this.GetHandleHorizontalPos(handle);
				this.draggingOffset = initial - pos.X;
			}
		}

		// Déplace une poignée.
		protected override void DraggingMove(ref string handle, Point pos)
		{
			if ( handle == null )  return;

			pos.X += this.draggingOffset;

			if ( handle == "LeftFirst" )  // left first ?
			{
				double posBody = this.GetHandleHorizontalPos("LeftBody");
				if ( System.Math.Abs(pos.X-posBody) < 3 )
				{
					pos.X = posBody;  // magnétise pile sur le left body
				}
			}

			if ( this.IsTab(handle) )  // déplacement d'un tab ?
			{
				Rectangle rect = this.Client.Bounds;
				if ( pos.Y < rect.Bottom || pos.Y > rect.Top )  // hors de la règle ?
				{
					this.draggingTabToDelete = handle;
					this.draggingTabDest = null;
				}
				else
				{
					this.draggingTabToDelete = null;
					this.draggingTabDest = this.DraggingDetect(pos, handle);
				}
			}

			this.SetHandleHorizontalPos(ref handle, pos);

			this.draggingFirstMove = false;
			
			pos.X = this.GetHandleHorizontalPos(handle);
			pos = this.document.Modifier.ActiveViewer.ScreenToInternal(pos);
			this.document.Modifier.ActiveViewer.MarkerVertical = (this.draggingTabToDelete == null) ? pos.X : double.NaN;
		}

		// Fin du drag d'une poignée.
		protected override void DraggingEnd(ref string handle, Point pos)
		{
			if ( handle == null )  return;

			this.document.Modifier.ActiveViewer.MarkerVertical = double.NaN;

			if ( this.IsTab(handle) )  // fin du déplacement d'un tab ?
			{
				Rectangle rect = this.Client.Bounds;
				if ( pos.Y < rect.Bottom || pos.Y > rect.Top )  // hors de la règle ?
				{
					this.editObject.DeleteTextTab(handle);
				}
				else
				{
					string[] list = this.DetectTabs(pos, handle);
					if ( list != null )
					{
						this.editObject.RenameTextTabs(list, handle);
					}
				}
				
				this.draggingTabToDelete = null;
				this.draggingTabDest = null;
			}
		}

		protected double GetHandleHorizontalPos(Tab tab)
		{
			return this.DocumentToScreen(tab.Pos);
		}

		protected double GetHandleHorizontalPos(string handle)
		{
			if ( handle == "LeftFirst" )  return this.DocumentToScreen(this.marginLeftFirst);
			if ( handle == "LeftBody"  )  return this.DocumentToScreen(this.marginLeftBody);
			if ( handle == "FirstBody" )  return this.DocumentToScreen(this.marginLeftBody);
			if ( handle == "Right"     )  return this.DocumentToScreen(this.marginRight);

			Tab tab = this.GetTab(handle);
			return this.DocumentToScreen(tab.Pos);
		}

		protected void SetHandleHorizontalPos(ref string handle, Point pos)
		{
			Drawing.Rectangle bbox = this.editObject.BoundingBoxThin;

			Tab tab = this.GetTab(handle);
			if ( tab.Tag == null )
			{
				double leftFirst, leftBody, right;
				Text.Properties.SizeUnits units;
				this.editObject.GetTextLeftMargins(out leftFirst, out leftBody, out units, true);
				this.editObject.GetTextRightMargins(out right, out units, true);
				units = Common.Text.Properties.SizeUnits.Points;

				if ( handle == "LeftFirst" )
				{
					leftFirst = this.ScreenToDocument(pos.X);
					leftFirst = leftFirst - bbox.Left;
					leftFirst = this.SnapGrid(leftFirst);
					leftFirst = System.Math.Max(leftFirst, 0);
					this.editObject.SetTextLeftMargins(leftFirst, leftBody, units, this.draggingFirstMove);
				}

				if ( handle == "LeftBody" )
				{
					leftBody = this.ScreenToDocument(pos.X);
					leftBody = leftBody - bbox.Left;
					leftBody = this.SnapGrid(leftBody);
					leftBody = System.Math.Max(leftBody, 0);
					this.editObject.SetTextLeftMargins(leftFirst, leftBody, units, this.draggingFirstMove);
				}

				if ( handle == "FirstBody" )
				{
					double initialBody = leftBody;
					leftBody = this.ScreenToDocument(pos.X);
					leftBody = leftBody - bbox.Left;
					leftBody = this.SnapGrid(leftBody);
					leftBody = System.Math.Max(leftBody, 0);
					leftFirst += leftBody - initialBody;
					leftFirst = this.SnapGrid(leftFirst);
					leftFirst = System.Math.Max(leftFirst, 0);
					this.editObject.SetTextLeftMargins(leftFirst, leftBody, units, this.draggingFirstMove);
				}

				if ( handle == "Right" )
				{
					right = this.ScreenToDocument(pos.X);
					right = bbox.Right - right;
					right = this.SnapGrid(right);
					right = System.Math.Max(right, 0);
					this.editObject.SetTextRightMargins(right, units, this.draggingFirstMove);
				}
			}
			else	// tabulateur ?
			{
				double tabPos;
				TextTabType type;
				this.editObject.GetTextTab(handle, out tabPos, out type);
				
				tabPos = this.ScreenToDocument(pos.X);
				tabPos = tabPos - bbox.Left;
				tabPos = this.SnapGrid(tabPos);
				tabPos = System.Math.Max(tabPos, 0);
				
				this.editObject.SetTextTab(ref handle, tabPos, type, this.draggingFirstMove);
			}
		}

		// Indique si un tag ou un handle correspond à un tabulateur.
		protected bool IsTab(string tag)
		{
			foreach ( Tab tab in this.tabs )
			{
				if ( tab.Tag == tag )  return true;
			}
			return false;
		}

		// Donne un tabulateur Tab d'après son tag ou handle.
		protected Tab GetTab(string tag)
		{
			foreach ( Tab tab in this.tabs )
			{
				if ( tab.Tag == tag )  return tab;
			}
			return this.invalidTab;
		}

		// Conversion d'une position relative dans le texte selon la grille.
		protected double SnapGrid(double value)
		{
			if ( this.document == null )  return value;

			Point pos = new Point(value, 0);
			this.document.Modifier.ActiveViewer.DrawingContext.SnapGrid(ref pos);
			return pos.X;
		}


		// Conversion d'une position dans le document en position en pixel dans l'écran.
		protected double DocumentToScreen(double value)
		{
			double scale = (this.ending-this.starting)/this.Client.Bounds.Width;
			return (value-this.starting)/scale;
		}

		// Conversion d'une position en pixel dans l'écran en position dans le document.
		protected double ScreenToDocument(double value)
		{
			double scale = (this.ending-this.starting)/this.Client.Bounds.Width;
			return value*scale + this.starting;
		}


		// Compare deux listes de tabulateurs.
		// Retourne true si les deux listes sont équivalentes (même contenu).
		protected static bool TabCompare(Tab[] list1, Tab[] list2)
		{
			if ( list1 == null && list2 == null )  return true;
			if ( list1 == null && list2 != null )  return false;
			if ( list1 != null && list2 == null )  return false;
			if ( list1.Length  != list2.Length  )  return false;

			for ( int i=0 ; i<list1.Length ; i++ )
			{
				if ( list1[i].Pos    != list2[i].Pos    )  return false;
				if ( list1[i].Type   != list2[i].Type   )  return false;
				if ( list1[i].Zombie != list2[i].Zombie )  return false;
			}
			return true;
		}


		// Donne le texte du tooltip d'édition en fonction de la position.
		protected override string GetTooltipEditedText(Point pos)
		{
			Drawing.Rectangle bbox = this.editObject.BoundingBoxThin;
			double x = this.ScreenToDocument(pos.X) - bbox.Left;

			if ( this.isDragging )  // déplacement en cours ?
			{
				x = this.SnapGrid(x);
				return this.document.Modifier.RealToString(x);
			}

			Rectangle rect = this.Client.Bounds;
			rect.Width = rect.Height;
			if ( rect.Contains(pos) )  // change le type de tabulateur à insérer ?
			{
				return Res.Strings.Action.Text.Ruler.TabChoice;
			}
			
			string handle = this.DraggingDetect(pos);

			if ( handle == "LeftFirst" )  return Res.Strings.Action.Text.Ruler.HandleLeftFirst;
			if ( handle == "LeftBody"  )  return Res.Strings.Action.Text.Ruler.HandleLeftBody;
			if ( handle == "FirstBody" )  return Res.Strings.Action.Text.Ruler.HandleFirstBody;
			if ( handle == "Right"     )  return Res.Strings.Action.Text.Ruler.HandleRight;

			Tab tab = this.GetTab(handle);
			if ( tab.Tag != null )
			{
				if ( tab.Zombie )
				{
					return Res.Strings.Action.Text.Ruler.TabZombie;
				}
				else
				{
					switch ( tab.Type )
					{
						case Drawing.TextTabType.Right:    return Res.Strings.Action.Text.Ruler.TabRight;
						case Drawing.TextTabType.Left:     return Res.Strings.Action.Text.Ruler.TabLeft;
						case Drawing.TextTabType.Center:   return Res.Strings.Action.Text.Ruler.TabCenter;
						case Drawing.TextTabType.Decimal:  return Res.Strings.Action.Text.Ruler.TabDecimal;
						case Drawing.TextTabType.Indent:   return Res.Strings.Action.Text.Ruler.TabIndent;
				
					}
				}
			}

			if ( x >= 0.0 && x <= bbox.Width )
			{
				return Res.Strings.Action.Text.Ruler.TabCreate;
			}

			return null;  // pas de tooltip
		}

		
		protected double					marginLeftFirst = 0.0;
		protected double					marginLeftBody = 0.0;
		protected double					marginRight = 0.0;
		protected Tab[]						tabs = null;
		protected Tab						invalidTab;

		protected Drawing.TextTabType		tabToCreate = Drawing.TextTabType.Right;
		protected double					draggingOffset = 0.0;
		protected string					draggingTabToDelete = null;
		protected string					draggingTabDest = null;
		protected bool						draggingFirstMove = false;
	}
}
