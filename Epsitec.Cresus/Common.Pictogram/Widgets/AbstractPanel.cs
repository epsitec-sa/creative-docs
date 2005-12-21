using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Pictogram.Data;

namespace Epsitec.Common.Pictogram.Widgets
{
	/// <summary>
	/// La classe AbstractPanel est la classe de base pour tous les panels.
	/// </summary>
	
	[SuppressBundleSupport]
	
	public abstract class AbstractPanel : Epsitec.Common.Widgets.Widget
	{
		public AbstractPanel(Drawer drawer)
		{
			this.drawer = drawer;

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.extendedButton, "Etend ou r�duit le panneau");

			this.stylesButton = new GlyphButton(this);
			this.stylesButton.ButtonStyle = ButtonStyle.Icon;
			this.stylesButton.GlyphShape = GlyphShape.Dots;
			this.stylesButton.Clicked += new MessageEventHandler(this.StylesButtonClicked);
			this.stylesButton.TabIndex = 1000;
			this.stylesButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			ToolTip.Default.SetToolTip(this.stylesButton, "Styles");

			this.colorBlack = Drawing.Color.FromName("WindowFrame");
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.extendedButton.Clicked -= new MessageEventHandler(this.ExtendedButtonClicked);
				this.stylesButton.Clicked -= new MessageEventHandler(this.StylesButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		public override double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return 30;
			}
		}

		public virtual bool IsNormalAndExtended()
		{
			//	Indique si ce panneau poss�de 2 hauteurs diff�rentes.
			return this.isNormalAndExtended;
		}

		public virtual bool ExtendedSize
		{
			//	Indique si le panneau est r�duit (petite hauteur) ou �tendu (grande hauteur).
			get
			{
				return this.extendedSize;
			}

			set
			{
				this.extendedSize = value;
				this.extendedButton.GlyphShape = this.extendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
			}
		}

		public bool Multi
		{
			//	Indique si le panneau repr�sente des propri�t�s contradictoires.
			get
			{
				return this.multi;
			}

			set
			{
				if ( this.multi != value )
				{
					this.multi = value;
					this.Invalidate();
				}
			}
		}

		public bool StyleDirect
		{
			//	Indique si le panneau �dite directement un style.
			get { return this.styleDirect; }
			set { this.styleDirect = value; }
		}

		public bool LayoutDirect
		{
			//	Indique si le panneau �dite directement une propri�t� de calque.
			get { return this.layoutDirect; }
			set { this.layoutDirect = value; }
		}

		public bool PatternDirect
		{
			//	Indique si le panneau �dite directement une propri�t� d'un motif.
			get { return this.patternDirect; }
			set { this.patternDirect = value; }
		}


		public virtual void SetProperty(AbstractProperty property)
		{
			//	Propri�t� -> widget.
			this.type                = property.Type;
			this.text                = property.Text;
			this.textStyle           = property.TextStyle;
			this.backgroundIntensity = property.BackgroundIntensity;
			this.styleID             = property.StyleID;
			this.styleName           = property.StyleName;
		}

		public virtual AbstractProperty GetProperty()
		{
			//	Widget -> propri�t�.
			return null;
		}

		protected void GetProperty(AbstractProperty property)
		{
			property.Type      = this.type;
			property.StyleID   = this.styleID;
			property.StyleName = this.styleName;
		}

		public virtual PropertyType PropertyType
		{
			//	Retourne le type de la propri�t� �dit�e par le panneau.
			get
			{
				return this.type;
			}
		}


		public virtual bool DefaultFocus()
		{
			//	Met le focus par d�faut dans ce panneau.
			return false;
		}


		protected override void UpdateClientGeometry()
		{
			//	Met � jour la g�om�trie.
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Drawing.Rectangle rTop = this.Client.Bounds;
			rTop.Left += 1;
			rTop.Width = this.extendedZoneWidth-2;
			rTop.Top -= 8;
			rTop.Bottom = rTop.Top-13;
			this.extendedButton.Bounds = rTop;
			this.extendedButton.SetVisible(this.isNormalAndExtended && !this.styleDirect && !this.layoutDirect);

			rTop.Offset(this.Client.Width-this.extendedZoneWidth, 0);
			this.stylesButton.Bounds = rTop;
			this.stylesButton.SetVisible(!this.styleDirect && !this.layoutDirect && !this.patternDirect);
		}


		public virtual void OriginColorDeselect()
		{
			//	D�s�lectionne toutes les origines de couleurs possibles.
		}

		public virtual void OriginColorSelect(int rank)
		{
			//	S�lectionne l'origine de couleur.
		}

		public virtual int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return -1;
		}

		public virtual void OriginColorChange(Drawing.Color color)
		{
			//	Modifie la couleur d'origine.
		}

		public virtual Drawing.Color OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return Drawing.Color.FromBrightness(0);
		}


		protected virtual void OnChanged()
		{
			//	G�n�re un �v�nement pour dire que �a a chang�.
			if ( this.Changed != null )  // qq'un �coute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		//	G�n�re un �v�nement pour dire que la couleur d'origine a chang�.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un �coute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		//	Le bouton pour �tendre/r�duire le panneau a �t� cliqu�.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.ExtendedSize = !this.extendedSize;
			this.OnExtendedChanged();
		}

		private void StylesButtonClicked(object sender, MessageEventArgs e)
		{
			//	Le bouton des styles a �t� cliqu�.
			GlyphButton button = sender as GlyphButton;
			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
			VMenu menu = this.Styles.CreateMenu(this.type, this.styleID);
			menu.Host = this;
			pos.X -= menu.Width;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		protected virtual void OnExtendedChanged()
		{
			//	G�n�re un �v�nement pour dire que la hauteur a chang�.
			if ( this.ExtendedChanged != null )  // qq'un �coute ?
			{
				this.ExtendedChanged(this);
			}
		}

		public event EventHandler ExtendedChanged;


		//	Retourne la collection des styles.
		protected StylesCollection Styles
		{
			get { return this.drawer.IconObjects.StylesCollection; }
		}


		public override Drawing.Rectangle GetShapeBounds()
		{
			Drawing.Rectangle rect = base.GetShapeBounds();
			rect.Left  -= 1;
			rect.Right += 1;
			rect.Top   += 1;
			return rect;
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
#if false
			Drawing.Color color = adorner.ColorWindow;
			color = Drawing.Color.FromRGB(color.R*this.backgroundIntensity, color.G*this.backgroundIntensity, color.B*this.backgroundIntensity);
#else
			Drawing.Color cap = adorner.ColorCaption;
			Drawing.Color color = Drawing.Color.FromARGB(1.0-this.backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
#endif
			graphics.RenderSolid(color);

			if ( this.multi )
			{
				Drawing.Rectangle part = rect;
				part.Width = this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(IconContext.ColorMulti);
			}

			if ( this.styleID != 0 || styleDirect )
			{
				Drawing.Rectangle part = rect;
				part.Left = part.Right-this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(IconContext.ColorStyle);

				part.Left = rect.Left+this.extendedZoneWidth;
				part.Right = rect.Right-this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				graphics.RenderSolid(IconContext.ColorStyleBack);
			}

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left-1, rect.Bottom-0.5, rect.Left-1, rect.Top+0.5);
			graphics.AddLine(rect.Left+this.extendedZoneWidth, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth, rect.Top+0.5);
			graphics.AddLine(rect.Right-this.extendedZoneWidth, rect.Bottom-0.5, rect.Right-this.extendedZoneWidth, rect.Top+0.5);
			graphics.AddLine(rect.Right+1, rect.Bottom-0.5, rect.Right+1, rect.Top+0.5);
			graphics.AddLine(rect.Left-1.5, rect.Top+1, rect.Right+1.5, rect.Top+1);
			graphics.AddLine(rect.Left-1.5, rect.Bottom, rect.Right+1.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		protected Drawer					drawer;
		protected Drawing.Color				colorBlack;
		protected double					backgroundIntensity;
		protected bool						extendedSize = false;
		protected PropertyType				type;
		protected string					text;
		protected string					textStyle;
		protected bool						isNormalAndExtended = false;
		protected double					extendedZoneWidth = 15;
		protected GlyphButton				extendedButton;
		protected GlyphButton				stylesButton;
		protected bool						multi = false;
		protected int						styleID = 0;
		protected string					styleName = "";
		protected bool						styleDirect = false;
		protected bool						layoutDirect = false;
		protected bool						patternDirect = false;
	}
}
