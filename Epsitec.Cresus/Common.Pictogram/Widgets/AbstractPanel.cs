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
			ToolTip.Default.SetToolTip(this.extendedButton, "Etend ou réduit le panneau");

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

		
		// Retourne la hauteur standard.
		public override double DefaultHeight
		{
			get
			{
				return 30;
			}
		}

		// Indique si ce panneau possède 2 hauteurs différentes.
		public virtual bool IsNormalAndExtended()
		{
			return this.isNormalAndExtended;
		}

		// Indique si le panneau est réduit (petite hauteur) ou étendu (grande hauteur).
		public virtual bool ExtendedSize
		{
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

		// Indique si le panneau représente des propriétés contradictoires.
		public bool Multi
		{
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

		// Indique si le panneau édite directement un style.
		public bool StyleDirect
		{
			get { return this.styleDirect; }
			set { this.styleDirect = value; }
		}

		// Indique si le panneau édite directement une propriété de calque.
		public bool LayoutDirect
		{
			get { return this.layoutDirect; }
			set { this.layoutDirect = value; }
		}

		// Indique si le panneau édite directement une propriété d'un motif.
		public bool PatternDirect
		{
			get { return this.patternDirect; }
			set { this.patternDirect = value; }
		}


		// Propriété -> widget.
		public virtual void SetProperty(AbstractProperty property)
		{
			this.type                = property.Type;
			this.text                = property.Text;
			this.textStyle           = property.TextStyle;
			this.backgroundIntensity = property.BackgroundIntensity;
			this.styleID             = property.StyleID;
			this.styleName           = property.StyleName;
		}

		// Widget -> propriété.
		public virtual AbstractProperty GetProperty()
		{
			return null;
		}

		protected void GetProperty(AbstractProperty property)
		{
			property.Type      = this.type;
			property.StyleID   = this.styleID;
			property.StyleName = this.styleName;
		}

		// Retourne le type de la propriété éditée par le panneau.
		public virtual PropertyType PropertyType
		{
			get
			{
				return this.type;
			}
		}


		// Met le focus par défaut dans ce panneau.
		public virtual bool DefaultFocus()
		{
			return false;
		}


		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
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


		// Désélectionne toutes les origines de couleurs possibles.
		public virtual void OriginColorDeselect()
		{
		}

		// Sélectionne l'origine de couleur.
		public virtual void OriginColorSelect(int rank)
		{
		}

		// Retourne le rang de la couleur d'origine.
		public virtual int OriginColorRank()
		{
			return -1;
		}

		// Modifie la couleur d'origine.
		public virtual void OriginColorChange(Drawing.Color color)
		{
		}

		// Donne la couleur d'origine.
		public virtual Drawing.Color OriginColorGet()
		{
			return Drawing.Color.FromBrightness(0);
		}


		// Génère un événement pour dire que ça a changé.
		protected virtual void OnChanged()
		{
			if ( this.Changed != null )  // qq'un écoute ?
			{
				this.Changed(this);
			}
		}

		public event EventHandler Changed;


		// Génère un événement pour dire que la couleur d'origine a changé.
		protected virtual void OnOriginColorChanged()
		{
			if ( this.OriginColorChanged != null )  // qq'un écoute ?
			{
				this.OriginColorChanged(this);
			}
		}

		public event EventHandler OriginColorChanged;


		// Le bouton pour étendre/réduire le panneau a été cliqué.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.ExtendedSize = !this.extendedSize;
			this.OnExtendedChanged();
		}

		// Le bouton des styles a été cliqué.
		private void StylesButtonClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;
			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
			VMenu menu = this.Styles.CreateMenu(this.type, this.styleID);
			menu.Host = this;
			pos.X -= menu.Width;
			menu.ShowAsContextMenu(this.Window, pos);
		}

		// Génère un événement pour dire que la hauteur a changé.
		protected virtual void OnExtendedChanged()
		{
			if ( this.ExtendedChanged != null )  // qq'un écoute ?
			{
				this.ExtendedChanged(this);
			}
		}

		public event EventHandler ExtendedChanged;


		// Retourne la collection des styles.
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
