namespace Epsitec.Common.Widgets
{
	public enum ScrollListShow
	{
		Extremity,		// déplacement minimal aux extrémités
		Middle,			// déplacement central
	}

	public enum ScrollListAdjust
	{
		MoveUp,			// déplace le haut
		MoveDown,		// déplace le bas
	}

	/// <summary>
	/// La classe ScrollList réalise une liste déroulante simple.
	/// </summary>
	public class ScrollList : Widget
	{
		public ScrollList()
		{
			this.internal_state |= InternalState.AutoFocus;
			this.internal_state |= InternalState.Focusable;

			this.lineHeight = this.GetLineHeight();
			this.scroller = new Scroller();
			this.scroller.Moved += new EventHandler(this.HandleScroller);
			this.Children.Add(this.scroller);
		}

		public bool ComboMode
		{
			set
			{
				this.isComboList = value;
			}

			get
			{
				return this.isComboList;
			}
		}


		// Vide toute la liste.
		public void Reset()
		{
			this.firstLine = 0;
			this.selectedLine = -1;
			this.list.Clear();
			this.isDirty = true;
			this.Invalidate();
			OnSelectChanged();
		}

		// Ajoute un texte à la fin de la liste.
		public void AddText(string text)
		{
			this.list.Add(text);
			this.isDirty = true;
			this.Invalidate();
		}

		// Donne un texte de la liste.
		public string GetText(int index)
		{
			if ( index < 0 || index >= this.list.Count )  return "";
			return (string)this.list[index];
		}

		// Ligne sélectionnée, -1 si aucune.
		public int Select
		{
			get
			{
				return this.selectedLine;
			}

			set
			{
				if ( value != -1 )
				{
					value = System.Math.Max(value, 0);
					value = System.Math.Min(value, this.list.Count-1);
				}
				if ( value != this.selectedLine || this.isSendSelectChanged )
				{
					this.selectedLine = value;
					this.isDirty = true;
					this.Invalidate();
					OnSelectChanged();
				}
			}
		}

		// Première ligne visible.
		public int FirstLine
		{
			get
			{
				return this.firstLine;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, System.Math.Max(this.list.Count-this.visibleLines, 0));
				if ( value != this.firstLine )
				{
					this.firstLine = value;
					this.UpdateTextlayouts();
					this.Invalidate();
				}
			}
		}

		// Indique si la ligne sélectionnée est visible.
		public bool IsShowSelect()
		{
			if ( this.selectedLine == -1 )  return true;
			if ( this.selectedLine >= this.firstLine &&
				 this.selectedLine <  this.firstLine+this.visibleLines )  return true;
			return false;
		}

		// Rend la ligne sélectionnée visible.
		public void ShowSelect(ScrollListShow mode)
		{
			if ( this.selectedLine == -1 )  return;

			int fl = this.FirstLine;
			if ( mode == ScrollListShow.Extremity )
			{
				if ( this.selectedLine < this.firstLine )
				{
					fl = this.selectedLine;
				}
				if ( this.selectedLine > this.firstLine+this.visibleLines-1 )
				{
					fl = this.selectedLine-(this.visibleLines-1);
				}
			}
			if ( mode == ScrollListShow.Middle )
			{
				int display = System.Math.Min(this.visibleLines, this.list.Count);
				fl = System.Math.Min(this.selectedLine+display/2, this.list.Count-1);
				fl = System.Math.Max(fl-display+1, 0);
			}
			this.FirstLine = fl;
		}

		// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
		public bool AdjustToMultiple(ScrollListAdjust mode)
		{
			double h = this.Height-this.margin*2;
			int nbLines = (int)(h/this.lineHeight);
			double adjust = h - nbLines*this.lineHeight;
			if ( adjust == 0 )  return false;

			if ( mode == ScrollListAdjust.MoveUp )
			{
				this.Top = System.Math.Floor(this.Top-adjust);
			}
			if ( mode == ScrollListAdjust.MoveDown )
			{
				this.Bottom = System.Math.Floor(this.Bottom+adjust);
			}
			this.Invalidate();
			return true;
		}

		// Ajuste la hauteur pour afficher exactement le nombre de lignes contenues.
		public bool AdjustToContent(ScrollListAdjust mode, double hMin, double hMax)
		{
			double h = this.lineHeight*this.list.Count+this.margin*2;
			double hope = h;
			h = System.Math.Max(h, hMin);
			h = System.Math.Min(h, hMax);
			if ( h == this.Height )  return false;

			if ( mode == ScrollListAdjust.MoveUp )
			{
				this.Top = this.Bottom+h;
			}
			if ( mode == ScrollListAdjust.MoveDown )
			{
				this.Bottom = this.Top-h;
			}
			this.Invalidate();
			if ( h != hope )  AdjustToMultiple(mode);
			return true;
		}


		// Appelé lorsque l'ascenseur a bougé.
		private void HandleScroller(object sender)
		{
			this.FirstLine = (int)(this.scroller.Range-this.scroller.Position);
		}


		// Gestion d'un événement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					if ( this.isComboList )
					{
						this.isSendSelectChanged = true;
					}
					this.mouseDown = true;
					MouseSelect(pos.Y);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown || this.isComboList )
					{
						MouseSelect(pos.Y);
					}
					break;

				case MessageType.MouseUp:
					if ( this.mouseDown )
					{
						MouseSelect(pos.Y);
						this.mouseDown = false;
					}
					break;

				case MessageType.KeyDown:
					//System.Diagnostics.Debug.WriteLine("KeyDown "+message.KeyChar+" "+message.KeyCode);
					ProcessKeyDown(message.KeyCode, message.IsShiftPressed, message.IsCtrlPressed);
					break;
			}
			
			message.Consumer = this;
		}

		// Sélectionne la ligne selon la souris.
		protected bool MouseSelect(double pos)
		{
			pos = this.Client.Height-pos;
			int line = (int)((pos-this.margin)/this.lineHeight);
			if ( line < 0 || line >= this.visibleLines )  return false;
			this.Select = this.firstLine+line;
			return true;
		}

		// Gestion d'une touche pressée avec KeyDown dans la liste.
		protected void ProcessKeyDown(int key, bool isShiftPressed, bool isCtrlPressed)
		{
			int		sel;

			switch ( key )
			{
				case (int)System.Windows.Forms.Keys.Up:
					sel = this.Select-1;
					if ( sel >= 0 )
					{
						Select = sel;
						if ( !this.IsShowSelect() )  this.ShowSelect(ScrollListShow.Extremity);
					}
					break;

				case (int)System.Windows.Forms.Keys.Down:
					sel = this.Select+1;
					if ( sel < this.list.Count )
					{
						Select = sel;
						if ( !this.IsShowSelect() )  this.ShowSelect(ScrollListShow.Extremity);
					}
					break;
			}
		}


		// Met à jour l'ascenseur en fonction de la liste.
		protected void UpdateScroller()
		{
			if ( this.scroller == null )  return;

			if ( !this.isDirty )  return;
			this.isDirty = false;

			int total = this.list.Count;
			if ( total <= this.visibleLines )
			{
				this.scroller.Hide();
			}
			else
			{
				this.scroller.Show();
				this.scroller.Range = total-this.visibleLines;
				this.scroller.Display = this.scroller.Range*((double)this.visibleLines/total);
				this.scroller.Position = this.scroller.Range-this.firstLine;
				this.scroller.ButtonStep = 1;
				this.scroller.PageStep = this.visibleLines/2;
			}

			this.UpdateClientGeometry();
			this.UpdateTextlayouts();
		}

		// Met à jour les textes.
		protected void UpdateTextlayouts()
		{
			int max = System.Math.Min(this.visibleLines, this.list.Count);
			for ( int i=0 ; i<max ; i++ )
			{
				this.textLayouts[i] = new TextLayout();
				this.textLayouts[i].Text = (string)this.list[i+this.firstLine];
				this.textLayouts[i].Font = this.DefaultFont;
				this.textLayouts[i].FontSize = this.DefaultFontSize;
				this.textLayouts[i].LayoutSize = new Drawing.Size(this.Width-this.margin*2-this.rightMargin, this.lineHeight);
			}
		}


		// Met à jour la géométrie de l'ascenseur de la liste.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			
			if ( this.lineHeight == 0 )
			{
				return;  // PA
			}

			Drawing.Rectangle rect = this.Bounds;
			this.margin = 3;
			rect.Inflate(-this.margin, -this.margin);

			this.visibleLines = (int)(rect.Height/this.lineHeight);
			this.textLayouts = new TextLayout[this.visibleLines];

			if ( this.scroller == null )
			{
				this.rightMargin = 0;
			}
			else
			{
				this.rightMargin = this.scroller.IsVisible ? Scroller.StandardWidth : 0;
				Drawing.Rectangle aRect = new Drawing.Rectangle(this.margin+rect.Width-this.rightMargin, this.margin, this.rightMargin, rect.Height);
				this.scroller.Bounds = aRect;
			}
		}


		// Génère un événement pour dire que la sélection dans la liste a changé.
		protected virtual void OnSelectChanged()
		{
			if ( !this.isComboList || this.isSendSelectChanged )
			{
				if ( this.SelectChanged != null )  // qq'un écoute ?
				{
					this.SelectChanged(this);
				}
			}
		}


		// Retourne la hauteur de l'interligne.
		protected double GetLineHeight()
		{
			Drawing.Font font = this.DefaultFont;
			double fontSize = this.DefaultFontSize;
			return font.LineHeight*fontSize;
		}


		// Dessine la liste.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			WidgetState       state = this.PaintState;
			Direction         dir   = this.RootDirection;
			
			adorner.PaintTextFieldBackground(graphics, rect, state, dir, TextFieldStyle.Normal);

			this.UpdateScroller();
			Drawing.Point pos = new Drawing.Point(this.margin, rect.Height-this.margin-this.lineHeight);
			int max = System.Math.Min(this.visibleLines, this.list.Count);
			for ( int i=0 ; i<max ; i++ )
			{
				Drawing.Color color = Drawing.Color.Empty;

				if ( i+this.firstLine == this.selectedLine )
				{
					Drawing.Rectangle[] rects = new Drawing.Rectangle[1];
					rects[0].Left   = this.margin;
					rects[0].Right  = this.Client.Width-this.margin-this.rightMargin-(this.scroller.IsVisible?2:0);
					rects[0].Bottom = pos.Y;
					rects[0].Top    = pos.Y+this.lineHeight;
					adorner.PaintTextSelectionBackground(graphics, new Drawing.Point(0,0), rects);

					color = Drawing.Color.FromName("ActiveCaptionText");
				}

				this.textLayouts[i].Paint(pos, graphics, Drawing.Rectangle.Empty, color);
				pos.Y -= this.lineHeight;
			}
		}


		public event EventHandler SelectChanged;

		protected bool							isComboList = false;
		protected bool							isDirty;
		protected bool							isSendSelectChanged = false;
		protected bool							mouseDown = false;
		protected System.Collections.ArrayList	list = new System.Collections.ArrayList();
		protected TextLayout[]					textLayouts;
		protected double						margin;
		protected double						rightMargin;
		protected double						lineHeight;
		protected Scroller						scroller;
		protected int							visibleLines;
		protected int							firstLine = 0;
		protected int							selectedLine = -1;
	}
}
