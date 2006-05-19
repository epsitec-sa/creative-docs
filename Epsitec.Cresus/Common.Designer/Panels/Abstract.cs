using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Designer.Panels
{
	/// <summary>
	/// La classe Abstract est la classe de base pour tous les panels.
	/// </summary>
	public abstract class Abstract : Common.Widgets.Widget
	{
		public Abstract()
		{
			this.Entered += new MessageEventHandler(this.HandleMouseEntered);
			this.Exited += new MessageEventHandler(this.HandleMouseExited);

			this.PreferredHeight = this.DefaultHeight;
			
			this.label = new StaticText (this);
			this.fixIcon = new StaticText(this);

			this.hiliteButton = new GlyphButton(this);
			this.hiliteButton.ButtonStyle = ButtonStyle.None;
			this.hiliteButton.GlyphShape = GlyphShape.ArrowRight;
			this.hiliteButton.AutoFocus = false;

			this.extendedButton = new GlyphButton(this);
			this.extendedButton.ButtonStyle = ButtonStyle.Icon;
			this.extendedButton.GlyphShape = GlyphShape.ArrowDown;
			this.extendedButton.AutoFocus = false;
			this.extendedButton.Clicked += new MessageEventHandler(this.ExtendedButtonClicked);
			this.extendedButton.TabIndex = 0;
			this.extendedButton.TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			//?ToolTip.Default.SetToolTip(this.extendedButton, Res.Strings.Panel.Abstract.Extend);

			this.colorBlack = Color.FromName("WindowFrame");
			this.UpdateButtons();
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.Entered -= new MessageEventHandler(this.HandleMouseEntered);
				this.Exited -= new MessageEventHandler(this.HandleMouseExited);
				this.extendedButton.Clicked -= new MessageEventHandler(this.ExtendedButtonClicked);
			}
			
			base.Dispose(disposing);
		}

		
		public virtual double DefaultHeight
		{
			//	Retourne la hauteur standard.
			get
			{
				return this.LabelHeight+30;
			}
		}

		protected double LabelHeight
		{
			//	Retourne la hauteur pour le label supérieur.
			get
			{
				return 14;
			}
		}

		protected virtual bool IsNormalAndExtended()
		{
			//	Indique si ce panneau possède 2 hauteurs différentes.
			return this.isNormalAndExtended;
		}

		public bool IsExtendedSize
		{
			//	Indique si le panneau est réduit (petite hauteur) ou étendu (grande hauteur).
			get
			{
				return this.isExtendedSize;
			}

			set
			{
				if ( this.isExtendedSize != value )
				{
					this.isExtendedSize = value;
					this.UpdateButtons();
					this.HeightChanged();
				}
			}
		}

		public void HeightChanged()
		{
			//	Indique que la hauteur du panneau a changé.
			this.PreferredHeight = this.DefaultHeight;
		}

		public bool IsLayoutDirect
		{
			//	Indique si le panneau édite directement une propriété de calque.
			get { return this.isLayoutDirect; }
			set { this.isLayoutDirect = value; }
		}

		public bool IsObjectHilite
		{
			//	Indique si la souris montre un objet.
			get
			{
				return this.isObjectHilite;
			}

			set
			{
				if ( this.isObjectHilite != value )
				{
					this.isObjectHilite = value;
					this.UpdateButtons();
				}
			}
		}

		public bool IsHilite
		{
			//	Etat survolé du panneau.
			get
			{
				return this.isHilite;
			}

			set
			{
				if ( this.isHilite != value )
				{
					this.isHilite = value;
					this.UpdateButtons();
					this.Invalidate();
				}
			}
		}


		private void HandleMouseEntered(object sender, MessageEventArgs e)
		{
			//	La souris est entrée dans le panneau.
		}

		private void HandleMouseExited(object sender, MessageEventArgs e)
		{
			//	La souris est sortie du panneau.
		}


		public virtual bool DefaultFocus()
		{
			//	Met le focus par défaut dans ce panneau.
			return false;
		}


		protected Rectangle UsefulZone
		{
			//	Retourne la zone rectangulaire utile pour les widgets.
			get
			{
				Rectangle rect = this.Client.Bounds;
				rect.Top -= this.LabelHeight;
				rect.Left += this.extendedZoneWidth;
				rect.Right -= this.extendedZoneWidth;
				rect.Deflate(5);
				return rect;
			}
		}

		protected override void UpdateClientGeometry()
		{
			//	Met à jour la géométrie.
			base.UpdateClientGeometry();

			if ( this.extendedButton == null )  return;

			Rectangle rect = this.Client.Bounds;
			rect.Left += this.extendedZoneWidth+5;
			rect.Right -= this.extendedZoneWidth+5;
			rect.Top -= 1;
			rect.Bottom = rect.Top-this.LabelHeight;
			this.label.SetManualBounds(rect);
			this.label.Visibility = true;

			rect = this.Client.Bounds;
			rect.Left += 1;
			rect.Width = this.extendedZoneWidth;
			rect.Top -= 3;
			rect.Bottom = rect.Top-13;
			this.fixIcon.SetManualBounds(rect);
			this.hiliteButton.SetManualBounds(rect);

			rect.Left = this.Client.Bounds.Right-this.extendedZoneWidth+1;
			rect.Width = this.extendedZoneWidth-3;
			this.extendedButton.SetManualBounds(rect);

			this.UpdateButtons();
		}

		protected void UpdateButtons()
		{
			//	Met à jour les boutons.
			if ( this.isObjectHilite )
			{
				this.fixIcon.Visibility = false;
				this.hiliteButton.Visibility = (this.isHilite);
			}
			else
			{
				this.fixIcon.Visibility = true;
				this.hiliteButton.Visibility = false;
			}

			this.extendedButton.Visibility = (this.isNormalAndExtended && !this.isLayoutDirect);
			this.extendedButton.GlyphShape = this.isExtendedSize ? GlyphShape.ArrowUp : GlyphShape.ArrowDown;
		}


		public virtual void OriginColorDeselect()
		{
			//	Désélectionne toutes les origines de couleurs possibles.
		}

		public virtual void OriginColorSelect(int rank)
		{
			//	Sélectionne l'origine de couleur.
		}

		public virtual int OriginColorRank()
		{
			//	Retourne le rang de la couleur d'origine.
			return -1;
		}

		public virtual void OriginColorChange(RichColor color)
		{
			//	Modifie la couleur d'origine.
		}

		public virtual RichColor OriginColorGet()
		{
			//	Donne la couleur d'origine.
			return RichColor.FromBrightness(0);
		}


		//	Le bouton pour étendre/réduire le panneau a été cliqué.
		private void ExtendedButtonClicked(object sender, MessageEventArgs e)
		{
			this.IsExtendedSize = !this.isExtendedSize;
		}

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			IAdorner adorner = Epsitec.Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			graphics.AddFilledRectangle(rect);
			Color cap = adorner.ColorCaption;
			Color color = Color.FromAlphaRgb(1.0-this.backgroundIntensity, 0.5+cap.R*0.5, 0.5+cap.G*0.5, 0.5+cap.B*0.5);
			graphics.RenderSolid(color);

			if ( this.isHilite )
			{
				Rectangle part = rect;
				part.Width = this.extendedZoneWidth;
				graphics.AddFilledRectangle(part);
				//?graphics.RenderSolid(context.HiliteSurfaceColor);
			}

			rect.Deflate(0.5, 0.5);
			graphics.AddLine(rect.Left, rect.Bottom-0.5, rect.Left, rect.Top-0.5);
			graphics.AddLine(rect.Left+this.extendedZoneWidth, rect.Bottom-0.5, rect.Left+this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right-this.extendedZoneWidth, rect.Bottom-0.5, rect.Right-this.extendedZoneWidth, rect.Top-0.5);
			graphics.AddLine(rect.Right, rect.Bottom-0.5, rect.Right, rect.Top-0.5);
			graphics.AddLine(rect.Left-0.5, rect.Top, rect.Right+0.5, rect.Top);
			graphics.AddLine(rect.Left-0.5, rect.Bottom, rect.Right+0.5, rect.Bottom);
			graphics.RenderSolid(adorner.ColorBorder);
		}


		public static void SetText(Widget widget, string text)
		{
			//	Met un texte dans un widget quelconque.
			if ( widget.Text != text )
			{
				widget.Text = text;
			}
		}


		protected Color						colorBlack;
		protected double					backgroundIntensity = 1.0;
		protected bool						isExtendedSize = false;
		protected bool						isNormalAndExtended = false;
		protected double					extendedZoneWidth = 16;
		protected StaticText				label;
		protected StaticText				fixIcon;
		protected GlyphButton				hiliteButton;
		protected GlyphButton				extendedButton;
		protected bool						isLayoutDirect = false;
		protected bool						isHilite = false;
		protected bool						isObjectHilite = false;
		protected bool						ignoreChanged = false;
	}
}
