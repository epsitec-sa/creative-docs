using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using Epsitec.Common.OpenType;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe GlyphArray permet de choisir le glyphe Unicode d'une police
	/// dans un tableau avec un ascenseur vertical. Afin d'éviter de créer un
	/// grand nombre de widgets, les différentes cellules ne sont pas programmées
	/// avec des instances de widgets, mais directement dessinées ici.
	/// </summary>
	public class GlyphArray : Widget
	{
		public GlyphArray()
		{
			this.AutoFocus  = true;
			this.AutoEngage = true;
			this.AutoDoubleClick = true;
			
			this.InternalState |= InternalState.Focusable;
			this.InternalState |= InternalState.Engageable;
			
			this.scroller = new VScroller(this);
			this.scroller.MinValue          = 0.0M;
			this.scroller.MaxValue          = 1.0M;
			this.scroller.VisibleRangeRatio = 0.1M;
			this.scroller.IsInverted        = true;
			this.scroller.ValueChanged += new Support.EventHandler(this.HandleScrollerValueChanged);
		}
		
		public GlyphArray(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.scroller.ValueChanged -= new Support.EventHandler(this.HandleScrollerValueChanged);
				this.scroller.Dispose();
				this.scroller = null;
			}
			
			base.Dispose(disposing);
		}


		// Taille d'une cellule. GlyphArray s'arrange pour que les cellules
		// soient à peu près carrées. La taille d'une cellule détermine le
		// nombre de cellules qu'il sera possible de placer horizontalement
		// et verticalement.
		public double CellSize
		{
			get
			{
				return this.cellSize;
			}

			set
			{
				if ( this.cellSize != value )
				{
					this.cellSize = value;
					this.Invalidate();
				}
			}
		}

		// Choix de la fonte.
		public void SetFont(string fontFace, string fontStyle)
		{
			if ( this.fontFace != fontFace || this.fontStyle != fontStyle )
			{
				this.fontFace = fontFace;
				this.fontStyle = fontStyle;
				this.UpdateUnicodes();
				this.scroller.Value = 0.0M;
				this.Invalidate();
			}
		}

		// Choix de l'index du glyphe sélectionné.
		public int SelectedIndex
		{
			get
			{
				return this.selectedIndex;
			}

			set
			{
				if ( this.selectedIndex != value )
				{
					this.selectedIndex = value;
					this.OnChangeSelected();
					this.Invalidate();
				}
			}
		}

		// Retourne le caractère Unicode correspondant à un index.
		public int IndexToUnicode(int index)
		{
			if ( index < 0 || index >= this.unicodes.Length )  return 0;
			return this.unicodes[index];
		}

		// Retourne l'index correspondant à un caractère Unicode.
		public int UnicodeToIndex(int code)
		{
			for ( int i=0 ; i<this.unicodes.Length ; i++ )
			{
				if ( code == this.unicodes[i] )  return i;
			}
			return -1;
		}


		// Met à jour la liste des glyphes Unicode en fonction de la fonte.
		protected void UpdateUnicodes()
		{
			this.unicodes = null;

			Drawing.Font font = GlyphArray.GetFont(this.fontFace, this.fontStyle);
			if ( font == null )  return;

			int total = 0;
			int ii = 0;
			for ( int pass=0 ; pass<2 ; pass++ )
			{
				for ( int i=0x0020 ; i<=0xFB06 ; i++ )
				{
					if ( i >= 0x0400 && i <= 0x04FF )  continue;  // cyrillique ?
					if ( i >= 0x0500 && i <= 0x05FF )  continue;  // araménien et hébreu ?
					if ( i >= 0x0600 && i <= 0x06FF )  continue;  // arabe ?
					if ( i >= 0x0900 && i <= 0x09FF )  continue;  // bengali ?
					if ( i >= 0x0A00 && i <= 0x0AFF )  continue;  // pendjabi ?
					if ( i >= 0x0B00 && i <= 0x0BFF )  continue;  // tamoul ?
					if ( i >= 0x0C00 && i <= 0x0CFF )  continue;  // télougou ?
					if ( i >= 0x0D00 && i <= 0x0DFF )  continue;  // malayalam ?
					if ( i >= 0x0E00 && i <= 0x0EFF )  continue;  // thaï ?
					if ( i >= 0x1000 && i <= 0x10FF )  continue;  // géorgien ?
					if ( i >= 0x1100 && i <= 0x11FF )  continue;  // jamos ?
					if ( i >= 0x3000 && i <= 0x30FF )  continue;  // katakana ?
					if ( i >= 0x3100 && i <= 0x31FF )  continue;  // bopomofo ?
					//?if ( i >= 0xE000 && i <= 0xF0FF )  continue;  // réservé ?

					int glyph = font.GetGlyphIndex(i);
					if ( glyph != 0 )
					{
						if ( pass == 0 )  // 1ère passe ?
						{
							total ++;  // compte le nombre de glyphes existants
						}
						if ( pass == 1 )  // 2ème passe ?
						{
							this.unicodes[ii++] = i;  // rempli le tableau
						}
					}
				}

				if ( pass == 0 )  // fin de la 1ère passe ?
				{
					this.unicodes = new int[total];  // alloue le tableau
				}
			}
		}


		// Met à jour la géométrie de l'ascenseur.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

			if ( this.scroller != null )
			{
				this.scroller.Bounds = new Rectangle(this.Client.Width-this.scroller.Width, 0, this.scroller.Width, this.Client.Height);
			}
		}
		
		
		// Appelé lorsque l'ascenseur est déplacé.
		private void HandleScrollerValueChanged(object sender)
		{
			System.Diagnostics.Debug.Assert(this.scroller == sender);
			this.Invalidate();  // redessine le tableau de glyphes
		}
		
		
		// Gestion des événements.
		protected override void ProcessMessage(Message message, Point pos)
		{
			if ( !this.IsEnabled )  return;

			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					this.SelectedIndex = this.Detect(pos);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
					{
						this.SelectedIndex = this.Detect(pos);
					}
					break;

				case MessageType.MouseUp:
					this.mouseDown = false;
					break;

				case MessageType.MouseWheel:
					if ( message.Wheel < 0 )  this.scroller.Value += this.scroller.SmallChange;
					if ( message.Wheel > 0 )  this.scroller.Value -= this.scroller.SmallChange;
					break;

				case MessageType.KeyDown:
					if ( !this.ProcessKeyEvent(message) )
					{
						return;
					}
					break;
				
				default:
					return;
			}
			
			message.Consumer = this;
		}

		// Gestion d'une touche clavier pressée.
		protected virtual bool ProcessKeyEvent(Message message)
		{
			switch ( message.KeyCode )
			{
				case KeyCode.ArrowLeft:   return this.MoveSelectedCell(-1);
				case KeyCode.ArrowRight:  return this.MoveSelectedCell( 1);
				case KeyCode.ArrowUp:     return this.MoveSelectedCell(-2);
				case KeyCode.ArrowDown:   return this.MoveSelectedCell( 2);

				default:
					return false;
			}
		}

		// Déplace la cellule sélectionnée.
		protected bool MoveSelectedCell(int move)
		{
			int sel = this.selectedIndex;
			if ( sel == -1 )  return false;

			int dx = this.TotalCellVisibleX();

			switch ( move )
			{
				case -1:  sel --;     break;
				case  1:  sel ++;     break;
				case -2:  sel -= dx;  break;
				case  2:  sel += dx;  break;

				default:
					return false;
			}

			sel = System.Math.Max(sel, 0);
			sel = System.Math.Min(sel, this.unicodes.Length-1);
			this.SelectedIndex = sel;
			this.ShowSelectedCell();

			return true;
		}

		// Si nécessaire, bouge l'ascenseur pour montrer la cellule sélectionnée.
		public void ShowSelectedCell()
		{
			int sel = this.selectedIndex;
			if ( sel == -1 )  return;

			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();

			for ( int i=0 ; i<1000 ; i++ )
			{
				int first = this.First();

				if ( sel < first )
				{
					this.scroller.Value -= this.scroller.SmallChange;
					continue;
				}

				if ( sel >= first+dx*dy )
				{
					this.scroller.Value += this.scroller.SmallChange;
					continue;
				}

				break;
			}
		}

		// Détection du glyphe visé.
		protected int Detect(Point pos)
		{
			Rectangle area = this.DrawingArea();
			if ( !area.Contains(pos) )  return -1;

			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();

			int x = (int) ((pos.X-area.Left)/this.CellWidth());
			int y = (int) ((pos.Y-area.Bottom)/this.CellHeight());
			y = dy-y-1;

			int cell = this.First() + dx*y + x;
			if ( cell >= this.unicodes.Length )  return -1;
			return cell;
		}


		// Peinture du widget.
		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner    adorner = Widgets.Adorner.Factory.Active;
			WidgetState state   = this.PaintState;
			
			Rectangle area = this.DrawingArea();
			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();
			double cellWidth  = this.CellWidth();
			double cellHeight = this.CellHeight();

			// Dessine les glyphes.
			if ( this.unicodes != null )
			{
				Drawing.Font font = GlyphArray.GetFont(this.fontFace, this.fontStyle);
				double fontSize = this.cellSize*0.6;
				int first = this.First();

				for ( int y=0 ; y<dy ; y++ )
				{
					for ( int x=0 ; x<dx ; x++ )
					{
						Rectangle rect = new Rectangle();

						rect.Left = area.Left + cellWidth*x;
						rect.Width = cellWidth;

						rect.Bottom = area.Top - cellHeight*(y+1);
						rect.Height = cellHeight;

						WidgetState cellState = WidgetState.Enabled;
						if ( first == this.selectedIndex && first < this.unicodes.Length )
						{
							TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
							areas[0] = new TextLayout.SelectedArea();
							areas[0].Rect = rect;
							adorner.PaintTextSelectionBackground(graphics, areas, state);

							cellState |= WidgetState.Selected;
						}

						if ( font != null && first < this.unicodes.Length )
						{
							char c = (char) this.unicodes[first++];
							int glyph = font.GetGlyphIndex(c);
							double width = font.GetGlyphAdvance(glyph)*fontSize;
							graphics.AddText(rect.Center.X-width/2.0, rect.Bottom+rect.Height*0.35, c.ToString(), font, fontSize);
							graphics.RenderSolid(adorner.ColorText(cellState));
						}
						else
						{
							Rectangle inside = rect;
							inside.Deflate(0.5);
							graphics.AddLine(inside.BottomLeft, inside.TopRight);
							graphics.AddLine(inside.TopLeft, inside.BottomRight);
							graphics.RenderSolid(adorner.ColorBorder);  // x
						}
					}
				}
			}

			// Dessine le quadrillage et le cadre.
			for ( int x=1 ; x<dx ; x++ )
			{
				double posx = System.Math.Floor(area.Left + cellWidth*x) + 0.5;
				graphics.AddLine(posx, area.Bottom+1.0, posx, area.Top-1.0);
			}

			for ( int y=1 ; y<dy ; y++ )
			{
				double posy = System.Math.Floor(area.Bottom + cellHeight*y) + 0.5;
				graphics.AddLine(area.Left+1.0, posy, area.Right-1.0, posy);
			}

			area.Deflate(0.5);
			graphics.AddRectangle(area);

			graphics.RenderSolid(adorner.ColorBorder);
		}


		// En fonction de la position de l'ascenseur (0..1), retourne le
		// premier glyphe visible.
		protected int First()
		{
			if ( this.unicodes == null )  return 0;

			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();

			int total = (this.unicodes.Length+dx-1)/dx*dx;
			int max = total-dx*dy;

			if ( max <= 0 )
			{
				this.scroller.SetEnabled(false);
				return 0;
			}
			else
			{
				this.scroller.SetEnabled(true);
				this.scroller.SmallChange = (decimal) ((double)dx/max);
				this.scroller.LargeChange = (decimal) ((double)dx*dy/max);
				this.scroller.VisibleRangeRatio = (decimal) ((double)dx*dy/total);

				int first = (int) (this.scroller.DoubleValue*max);
				return (first+dx/2)/dx*dx;
			}
		}

		// Largeur d'une cellule.
		protected double CellWidth()
		{
			Rectangle rect = this.DrawingArea();
			return rect.Width/this.TotalCellVisibleX();
		}

		// Hauteur d'une cellule.
		protected double CellHeight()
		{
			Rectangle rect = this.DrawingArea();
			return rect.Height/this.TotalCellVisibleY();
		}

		// Nombre de cellules visibles horizontalement.
		protected int TotalCellVisibleX()
		{
			Rectangle rect = this.DrawingArea();
			int total = (int) (rect.Width/this.cellSize);
			if ( total == 0 )  total++;
			return total;
		}

		// Nombre de cellules visibles verticalement.
		protected int TotalCellVisibleY()
		{
			Rectangle rect = this.DrawingArea();
			int total = (int) (rect.Height/this.cellSize);
			if ( total == 0 )  total++;
			return total;
		}

		// Rectangle où dessiner les cellules.
		protected Rectangle DrawingArea()
		{
			Rectangle rect = this.Client.Bounds;
			rect.Right -= this.scroller.Width+1.0;
			return rect;
		}


		// Donne une fonte d'après son nom.
		protected static Drawing.Font GetFont(string fontFace, string fontStyle)
		{
			Drawing.Font font = Drawing.Font.GetFont(fontFace, fontStyle);

			if ( font == null )
			{
				font = Drawing.Font.GetFontFallback(fontFace);
			}
			
			return font;
		}


		// Appelé lorsque le glyphe sélectionné change.
		protected virtual void OnChangeSelected()
		{
			if ( this.ChangeSelected != null )
			{
				this.ChangeSelected(this);
			}
		}

		public event Support.EventHandler		ChangeSelected;
		
		protected double						cellSize = 25;
		protected VScroller						scroller;
		protected string						fontFace;
		protected string						fontStyle;
		protected int							selectedIndex = -1;
		protected int[]							unicodes;
		protected bool							mouseDown = false;
	}
}
