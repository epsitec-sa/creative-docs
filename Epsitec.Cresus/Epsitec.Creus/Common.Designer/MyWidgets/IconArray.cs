using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// La classe IconArray permet de choisir une icône dans un tableau avec un
	/// ascenseur vertical. Afin d'éviter de créer un grand nombre de widgets,
	/// les différentes cellules ne sont pas programmées avec des instances de
	/// widgets, mais directement dessinées ici.
	/// </summary>
	public class IconArray : Widget, Widgets.Helpers.IToolTipHost
	{
		public IconArray()
		{
			this.AutoFocus  = true;
			this.AutoEngage = true;
			this.AutoDoubleClick = true;
			
			this.InternalState |= WidgetInternalState.Focusable;
			this.InternalState |= WidgetInternalState.Engageable;
			
			this.scroller = new VScroller(this);
			this.scroller.MinValue          = 0.0M;
			this.scroller.MaxValue          = 1.0M;
			this.scroller.VisibleRangeRatio = 0.1M;
			this.scroller.IsInverted        = true;
			this.scroller.ValueChanged += this.HandleScrollerValueChanged;
		}
		
		public IconArray(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.scroller.ValueChanged -= this.HandleScrollerValueChanged;
				this.scroller.Dispose();
				this.scroller = null;
			}
			
			base.Dispose(disposing);
		}


		public double CellSize
		{
			//	Taille d'une cellule. IconArray s'arrange pour que les cellules
			//	soient à peu près carrées. La taille d'une cellule détermine le
			//	nombre de cellules qu'il sera possible de placer horizontalement
			//	et verticalement.
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

		public void SetIcons(List<string> list)
		{
			//	Choix de la liste des icônes.
			this.icons = list;
			this.Invalidate();
		}

		public int SelectedIndex
		{
			//	Choix de l'index de l'icône sélectionnée.
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


		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie de l'ascenseur.
			base.UpdateClientGeometry();

			if ( this.scroller != null )
			{
				this.scroller.SetManualBounds(new Rectangle(this.Client.Size.Width-this.scroller.ActualWidth, 0, this.scroller.ActualWidth, this.Client.Size.Height));
			}
		}
		
		
		private void HandleScrollerValueChanged(object sender)
		{
			//	Appelé lorsque l'ascenseur est déplacé.
			System.Diagnostics.Debug.Assert(this.scroller == sender);
			this.Invalidate();  // redessine le tableau des icônes
		}
		
		
		protected override void ProcessMessage(Message message, Point pos)
		{
			//	Gestion des événements.
			if ( !this.IsEnabled )  return;

			switch ( message.MessageType )
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

		protected virtual bool ProcessKeyEvent(Message message)
		{
			//	Gestion d'une touche clavier pressée.
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

		protected bool MoveSelectedCell(int move)
		{
			//	Déplace la cellule sélectionnée.
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
			sel = System.Math.Min(sel, this.TotalCell-1);
			this.SelectedIndex = sel;
			this.ShowSelectedCell();

			return true;
		}

		public void ShowSelectedCell()
		{
			//	Si nécessaire, bouge l'ascenseur pour montrer la cellule sélectionnée.
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

		protected int Detect(Point pos)
		{
			//	Détection de l'icône visée.
			Rectangle area = this.DrawingArea();
			if ( !area.Contains(pos) )  return -1;

			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();

			int x = (int) ((pos.X-area.Left)/this.CellWidth());
			int y = (int) ((pos.Y-area.Bottom)/this.CellHeight());
			y = dy-y-1;

			int cell = this.First() + dx*y + x;
			if ( cell >= this.TotalCell )  return -1;
			return cell;
		}


		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Peinture du widget.
			IAdorner adorner = Widgets.Adorners.Factory.Active;
			WidgetPaintState state = this.PaintState;
			
			Rectangle area = this.DrawingArea();
			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();
			double cellWidth  = this.CellWidth();
			double cellHeight = this.CellHeight();

			graphics.AddFilledRectangle(area);
			graphics.RenderSolid(adorner.ColorTextBackground);

			//	Dessine les icônes.
			if ( this.icons != null )
			{
				TextLayout layout = new TextLayout();
				layout.LayoutSize = new Size(cellWidth, cellHeight);
				layout.Alignment = ContentAlignment.MiddleCenter;

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

						WidgetPaintState cellState = WidgetPaintState.Enabled;
						if ( first == this.selectedIndex && first < this.TotalCell )
						{
							TextLayout.SelectedArea[] areas = new TextLayout.SelectedArea[1];
							areas[0] = new TextLayout.SelectedArea(rect);
							adorner.PaintTextSelectionBackground(graphics, areas, state, PaintTextStyle.TextField, TextFieldDisplayMode.Default);

							cellState |= WidgetPaintState.Selected;
						}

						if ( first < this.TotalCell )
						{
							if (first == 0)  // pas d'icône ?
							{
								layout.Text = null;
								layout.Paint(rect.BottomLeft, graphics);
							}
							else if (first >= 1)
							{
								layout.Text = Misc.ImageFull(this.icons[first-1]);
								layout.Paint(rect.BottomLeft, graphics);
							}

							first++;
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

			//	Dessine le quadrillage et le cadre.
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


		protected int First()
		{
			//	En fonction de la position de l'ascenseur (0..1), retourne la
			//	première icône visible.
			if ( this.icons == null )  return 0;

			int dx = this.TotalCellVisibleX();
			int dy = this.TotalCellVisibleY();

			int total = (this.TotalCell+dx-1)/dx*dx;
			int max = total-dx*dy;

			if ( max <= 0 )
			{
				this.scroller.Enable = false;
				return 0;
			}
			else
			{
				this.scroller.Enable = true;
				this.scroller.SmallChange = (decimal) ((double)dx/max);
				this.scroller.LargeChange = (decimal) ((double)dx*dy/max);
				this.scroller.VisibleRangeRatio = (decimal) ((double)dx*dy/total);

				int first = (int) (this.scroller.DoubleValue*max);
				return (first+dx/2)/dx*dx;
			}
		}

		protected double CellWidth()
		{
			//	Largeur d'une cellule.
			Rectangle rect = this.DrawingArea();
			return rect.Width/this.TotalCellVisibleX();
		}

		protected double CellHeight()
		{
			//	Hauteur d'une cellule.
			Rectangle rect = this.DrawingArea();
			return rect.Height/this.TotalCellVisibleY();
		}

		protected int TotalCellVisibleX()
		{
			//	Nombre de cellules visibles horizontalement.
			Rectangle rect = this.DrawingArea();
			int total = (int) (rect.Width/this.cellSize);
			if ( total == 0 )  total++;
			return total;
		}

		protected int TotalCellVisibleY()
		{
			//	Nombre de cellules visibles verticalement.
			Rectangle rect = this.DrawingArea();
			int total = (int) (rect.Height/this.cellSize);
			if ( total == 0 )  total++;
			return total;
		}

		protected Rectangle DrawingArea()
		{
			//	Rectangle où dessiner les cellules.
			Rectangle rect = this.Client.Bounds;
			rect.Right -= this.scroller.ActualWidth+1.0;
			return rect;
		}


		protected int TotalCell
		{
			//	Retourne le nombre total de cases.
			get
			{
				return 1 + this.icons.Count;
			}
		}


		protected static Drawing.Font GetFont(string fontFace, string fontStyle)
		{
			//	Donne une fonte d'après son nom.
			Drawing.Font font = Drawing.Font.GetFont(fontFace, fontStyle);

			if ( font == null )
			{
				font = Drawing.Font.GetFontFallback(fontFace);
			}
			
			return font;
		}


		#region Helpers.IToolTipHost
		public object GetToolTipCaption(Point pos)
		{
			//	Donne l'objet (string ou widget) pour le tooltip en fonction de la position.
			return this.GetTooltipEditedText(pos);
		}

		protected string GetTooltipEditedText(Point pos)
		{
			//	Donne le texte du tooltip en fonction de la position.
			int index = this.Detect(pos);

			if (index == 0)  // pas d'icône ?
			{
				return Res.Strings.Dialog.Icon.None;
			}

			if (index >= 1 && index < this.TotalCell)
			{
				string module, name;
				Misc.GetIconNames(this.icons[index-1], out module, out name);
				return Misc.CompactModuleAndName (module, name);
			}

			return null;  // pas de tooltip
		}
		#endregion


		protected virtual void OnChangeSelected()
		{
			//	Appelé lorsque l'icône sélectionnée change.
			if ( this.ChangeSelected != null )
			{
				this.ChangeSelected(this);
			}
		}

		public event Support.EventHandler		ChangeSelected;
		
		protected double						cellSize = 25;
		protected VScroller						scroller;
		protected List<string>					icons;
		protected int							selectedIndex = -1;
		protected bool							mouseDown = false;
	}
}
