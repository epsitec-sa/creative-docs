namespace Epsitec.Common.Widgets
{
	public enum ScrollListAdjust
	{
		MoveUp,			// d�place le haut
		MoveDown,		// d�place le bas
	}

	/// <summary>
	/// La classe ScrollList r�alise une liste d�roulante simple.
	/// </summary>
	public class ScrollList : Widget
	{
		public ScrollList()
		{
			this.lineHeight = this.GetLineHeight();
			this.scroller = new Scroller();
			this.scroller.Moved += new EventHandler(this.HandleScroller);
			this.Children.Add(this.scroller);
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

		// Ajoute un texte � la fin de la liste.
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

		// Ligne s�lectionn�e, -1 si aucune.
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
				if ( value != this.selectedLine )
				{
					this.selectedLine = value;
					this.isDirty = true;
					this.Invalidate();
					OnSelectChanged();
				}
			}
		}

		// Premi�re ligne visible.
		public int FirstLine
		{
			get
			{
				return this.firstLine;
			}

			set
			{
				value = System.Math.Max(value, 0);
				value = System.Math.Min(value, this.list.Count-this.visibleLines);
				if ( value != this.firstLine )
				{
					this.firstLine = value;
					this.UpdateTextlayouts();
					this.Invalidate();
				}
			}
		}

		// Indique si la ligne s�lectionn�e est visible.
		public bool IsShowSelect()
		{
			if ( this.selectedLine == -1 )  return true;
			if ( this.selectedLine >= this.firstLine &&
				 this.selectedLine <  this.firstLine+this.visibleLines )  return true;
			return false;
		}

		// Rend la ligne s�lectionn�e visible.
		public void ShowSelect()
		{
			if ( this.selectedLine == -1 )  return;

			int display = System.Math.Min(this.visibleLines, this.list.Count);
			int fl = System.Math.Min(this.selectedLine+display/2, this.list.Count-1);
			fl = System.Math.Max(fl-display+1, 0);
			this.FirstLine = fl;
		}

		// Ajuste la hauteur pour afficher pile un nombre entier de lignes.
		public bool Adjust(ScrollListAdjust mode)
		{
			double h = this.Height-this.margin*2;
			int nbLines = (int)(h/this.lineHeight);
			double adjust = h - nbLines*this.lineHeight;
			if ( adjust == 0 )  return false;

			if ( mode == ScrollListAdjust.MoveUp )
			{
				this.Top -= adjust;
			}
			if ( mode == ScrollListAdjust.MoveDown )
			{
				this.Bottom += adjust;
			}
			this.Invalidate();
			return true;
		}


		// Appel� lorsque l'ascenseur a boug�.
		private void HandleScroller(object sender)
		{
			this.FirstLine = (int)(this.scroller.Range-this.scroller.Position);
		}


		// Gestion d'un �v�nement.
		protected override void ProcessMessage(Message message, Drawing.Point pos)
		{
			switch ( message.Type )
			{
				case MessageType.MouseDown:
					this.mouseDown = true;
					MouseSelect(pos.Y);
					break;
				
				case MessageType.MouseMove:
					if ( this.mouseDown )
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
			}
			
			message.Consumer = this;
		}

		// S�lectionne la ligne selon la souris.
		protected bool MouseSelect(double pos)
		{
			pos = this.Client.Height-pos;
			int line = (int)((pos-this.margin)/this.lineHeight);
			if ( line < 0 || line >= this.visibleLines )  return false;
			this.Select = this.firstLine+line;
			return true;
		}


		// Met � jour l'ascenseur en fonction de la liste.
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

		// Met � jour les textes.
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


		// Met � jour la g�om�trie de l'ascenseur de la liste.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

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


		// G�n�re un �v�nement pour dire que la s�lection dans la liste a chang�.
		protected virtual void OnSelectChanged()
		{
			if ( this.SelectChanged != null )  // qq'un �coute ?
			{
				this.SelectChanged(this);
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

		protected bool							isDirty;
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
