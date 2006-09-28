using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.MetaButton))]

namespace Epsitec.Common.Widgets
{
	public enum DisplayMode
	{
		Automatic,		// icône et/ou texte selon la taille disponible
		Icon,			// icône seule
		Text,			// texte seul
		IconAndText,	// icône à gauche et texte à droite
	}

	public enum SiteMark
	{
		None,			// pas de marque
		OnBottom,		// marque en bas
		OnTop,			// marque en haut
		OnLeft,			// marque à gauche
		OnRight,		// marque à droite
	}


	/// <summary>
	/// La classe MetaButton est un Button pouvant contenir une icône et/ou un texte.
	/// </summary>
	public class MetaButton : Button
	{
		public MetaButton()
		{
			this.ButtonStyle = ButtonStyle.ToolItem;
		}

		public MetaButton(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}


		static MetaButton()
		{
			Helpers.VisualPropertyMetadata metadataAlign = new Helpers.VisualPropertyMetadata(ContentAlignment.MiddleLeft, Helpers.VisualPropertyMetadataOptions.AffectsTextLayout);
			Helpers.VisualPropertyMetadata metadataDy = new Helpers.VisualPropertyMetadata(Widget.DefaultFontHeight+10, Helpers.VisualPropertyMetadataOptions.AffectsMeasure);

			Visual.ContentAlignmentProperty.OverrideMetadata(typeof(MetaButton), metadataAlign);
			Visual.PreferredHeightProperty.OverrideMetadata(typeof(MetaButton), metadataDy);
		}

		
		public DisplayMode DisplayMode
		{
			//	Mode d'affichage du contenu du bouton.
			get
			{
				return (DisplayMode) this.GetValue (MetaButton.DisplayModeProperty);
			}

			set
			{
				this.SetValue (MetaButton.DisplayModeProperty, value);
			}
		}

		public SiteMark SiteMark
		{
			//	Emplacement de la marque.
			get
			{
				return this.siteMark;
			}

			set
			{
				if (this.siteMark != value)
				{
					this.siteMark = value;
					this.Invalidate();
				}
			}
		}

		public double MarkDimension
		{
			//	Dimension de la marque.
			get
			{
				return this.markDimension;
			}

			set
			{
				if ( this.markDimension != value )
				{
					this.markDimension = value;
					this.Invalidate();
				}
			}
		}

		public Color BulletColor
		{
			//	Couleur de la puce éventuelle (si différent de Color.Empty).
			get
			{
				return this.bulletColor;
			}

			set
			{
				if ( this.bulletColor != value )
				{
					this.bulletColor = value;
					this.Invalidate();
				}
			}
		}

		public string PreferredIconLanguage
		{
			get
			{
				return this.preferredIconLanguage;
			}

			set
			{
				if (this.preferredIconLanguage != value)
				{
					this.preferredIconLanguage = value;
					this.UpdateIcon(this.IconName);
				}
			}
		}

		public string PreferredIconStyle
		{
			get
			{
				return this.preferredIconStyle;
			}

			set
			{
				if (this.preferredIconStyle != value)
				{
					this.preferredIconStyle = value;
					this.UpdateIcon(this.IconName);
				}
			}
		}

		public override bool HasTextLabel
		{
			get
			{
				return (this.DisplayMode == DisplayMode.Text || this.DisplayMode == DisplayMode.IconAndText);
			}
		}


		public Rectangle InnerTextBounds
		{
			get
			{
				return this.TextBounds;
			}
		}

		protected override Size GetTextLayoutSize()
		{
			return this.TextBounds.Size;
		}

		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();
			this.UpdateIcon(this.IconName);
		}

		protected override void OnIconNameChanged(string oldIconName, string newIconName)
		{
			base.OnIconNameChanged(oldIconName, newIconName);

			if (string.IsNullOrEmpty(oldIconName) &&
				string.IsNullOrEmpty(newIconName))
			{
				//	Nothing to do. Change is not significant : the text remains
				//	empty if we swap "" for null.
			}
			else
			{
				this.UpdateIcon(newIconName);
			}
		}


		protected void UpdateIcon(string iconName)
		{
			//	Met à jour le texte du bouton, qui est un tag <img.../> contenant le nom de l'image
			//	suivi des différentes préférences (taille, langue et style).
			if (string.IsNullOrEmpty(iconName) || this.DisplayMode == DisplayMode.Text)
			{
				this.iconLayout = null;
			}
			else
			{
				System.Text.StringBuilder builder = new System.Text.StringBuilder();

				builder.Append(@"<img src=""");
				builder.Append(iconName);
				builder.Append(@"""");

				Rectangle rect = this.IconBounds;
				builder.Append(@" dx=""");
				builder.Append(rect.Width.ToString(System.Globalization.CultureInfo.InvariantCulture));
				builder.Append(@""" dy=""");
				builder.Append(rect.Height.ToString(System.Globalization.CultureInfo.InvariantCulture));
				builder.Append(@"""");

				if (string.IsNullOrEmpty(this.preferredIconLanguage) == false)
				{
					builder.Append(@" lang=""");
					builder.Append(this.preferredIconLanguage);
					builder.Append(@"""");
				}

				if (string.IsNullOrEmpty(this.preferredIconStyle) == false)
				{
					builder.Append(@" style=""");
					builder.Append(this.preferredIconStyle);
					builder.Append(@"""");
				}

				builder.Append(@"/>");

				if (this.iconLayout == null)
				{
					this.iconLayout = new TextLayout();
				}

				this.iconLayout.Text = builder.ToString();
				this.iconLayout.Alignment = ContentAlignment.MiddleCenter;
			}
		}

		protected Rectangle IconBounds
		{
			//	Donne le rectangle carré à utiliser pour l'icône du bouton.
			get
			{
				if (this.iconLayout == null)
				{
					return Rectangle.Empty;
				}
				else
				{
					Rectangle rect = this.ButtonBounds;
					rect.Width = rect.Height;  // forcément un carré
					return rect;
				}
			}
		}

		protected Rectangle TextBounds
		{
			//	Donne le rectangle à utiliser pour le texte du bouton.
			get
			{
				Rectangle rect = this.ButtonBounds;

				if (this.iconLayout != null)
				{
					rect.Left += rect.Height;
				}

				if (this.ContentAlignment == ContentAlignment.MiddleLeft)
				{
					rect.Left += 5;  // espace entre le bord gauche ou l'icône et le texte
				}

				return rect;
			}
		}

		protected Rectangle ButtonBounds
		{
			//	Donne le rectangle à utiliser pour le cadre du bouton.
			get
			{
				Rectangle rect = this.Client.Bounds;

				switch (this.siteMark)
				{
					case SiteMark.OnBottom:
						rect.Bottom += this.markDimension;
						break;

					case SiteMark.OnTop:
						rect.Top -= this.markDimension;
						break;

					case SiteMark.OnLeft:
						rect.Left += this.markDimension;
						break;

					case SiteMark.OnRight:
						rect.Right -= this.markDimension;
						break;
				}

				return rect;
			}
		}

		
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			//	Dessine le bouton.
			IAdorner adorner = Common.Widgets.Adorners.Factory.Active;

			Rectangle rect = this.Client.Bounds;
			WidgetPaintState state = this.PaintState;

			bool enable = ((state & WidgetPaintState.Enabled) != 0);
			if (!enable)
			{
				state &= ~WidgetPaintState.Focused;
				state &= ~WidgetPaintState.Entered;
				state &= ~WidgetPaintState.Engaged;
			}

			if (this.ActiveState == ActiveState.Yes && this.siteMark != SiteMark.None)  // dessine la marque triangulaire ?
			{
				Path path = new Path();
				double middle;
				double factor = 1.0;

				switch ( this.siteMark )
				{
					case SiteMark.OnBottom:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Bottom);
						path.LineTo(middle-this.markDimension*factor, rect.Bottom+this.markDimension);
						path.LineTo(middle+this.markDimension*factor, rect.Bottom+this.markDimension);
						break;

					case SiteMark.OnTop:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Top);
						path.LineTo(middle-this.markDimension*factor, rect.Top-this.markDimension);
						path.LineTo(middle+this.markDimension*factor, rect.Top-this.markDimension);
						break;

					case SiteMark.OnLeft:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Left, middle);
						path.LineTo(rect.Left+this.markDimension, middle-this.markDimension*factor);
						path.LineTo(rect.Left+this.markDimension, middle+this.markDimension*factor);
						break;

					case SiteMark.OnRight:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Right, middle);
						path.LineTo(rect.Right-this.markDimension, middle-this.markDimension*factor);
						path.LineTo(rect.Right-this.markDimension, middle+this.markDimension*factor);
						break;
				}
				path.Close();

				graphics.Color = adorner.ColorTextFieldBorder(enable);
				graphics.PaintSurface(path);
			}
			
			rect = this.ButtonBounds;
			state &= ~WidgetPaintState.Selected;
			adorner.PaintButtonBackground(graphics, rect, state, Direction.Down, this.ButtonStyle);

			//	Dessine la puce carrée à gauche.
			if (!this.bulletColor.IsEmpty)
			{
				Rectangle r = rect;
				r.Deflate(3.5);
				r.Width = r.Height;

				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(this.bulletColor);

				graphics.AddRectangle(r);
				graphics.RenderSolid(adorner.ColorTextFieldBorder(enable));

				rect.Left += rect.Height;
			}

			//	Dessine l'icône.
			if (this.iconLayout != null)
			{
				Rectangle ricon = this.IconBounds;

				if (this.innerZoom != 1.0)
				{
					double zoom = (this.innerZoom-1)/2+1;
					this.iconLayout.LayoutSize = ricon.Size/this.innerZoom;
					Transform transform = graphics.Transform;
					graphics.ScaleTransform(zoom, zoom, 0, -this.Client.Size.Height*zoom);
					adorner.PaintButtonTextLayout(graphics, ricon.BottomLeft, this.iconLayout, state, this.ButtonStyle);
					graphics.Transform = transform;
				}
				else
				{
					this.iconLayout.LayoutSize = ricon.Size;
					adorner.PaintButtonTextLayout(graphics, ricon.BottomLeft, this.iconLayout, state, this.ButtonStyle);
				}
			}

			//	Dessine le texte.
			rect = this.TextBounds;

			if (this.innerZoom != 1.0)
			{
				this.TextLayout.LayoutSize = rect.Size/this.innerZoom;
				Transform transform = graphics.Transform;
				graphics.ScaleTransform(this.innerZoom, this.innerZoom, this.Client.Size.Width / 2, this.Client.Size.Height / 2);
				adorner.PaintButtonTextLayout(graphics, rect.BottomLeft, this.TextLayout, state, this.ButtonStyle);
				graphics.Transform = transform;
			}
			else
			{
				this.TextLayout.LayoutSize = rect.Size;
				adorner.PaintButtonTextLayout(graphics, rect.BottomLeft, this.TextLayout, state, this.ButtonStyle);
			}
		}

		private static void HandleDisplayModeChanged(DependencyObject obj, object oldValue, object newObject)
		{
			MetaButton that = obj as MetaButton;
			
			that.UpdateIcon (that.IconName);
			that.Invalidate ();
		}

		public static readonly DependencyProperty DisplayModeProperty = DependencyProperty.Register ("DisplayMode", typeof (DisplayMode), typeof (MetaButton), new DependencyPropertyMetadata (DisplayMode.Automatic, MetaButton.HandleDisplayModeChanged));

		protected SiteMark				siteMark = SiteMark.None;
		protected double				markDimension = 8;
		protected Color					bulletColor = Color.Empty;
		protected string				preferredIconLanguage = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
		protected string				preferredIconStyle;
		protected TextLayout			iconLayout;
	}
}
