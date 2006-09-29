using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.MetaButton))]

namespace Epsitec.Common.Widgets
{
	public enum ButtonDisplayMode
	{
		Automatic,		// icône et/ou texte selon la taille disponible
		Icon,			// icône seule
		Text,			// texte seul
		IconAndText,	// icône à gauche et texte à droite
	}

	public enum ButtonMarkDisposition
	{
		None,			// pas de marque
		Below,			// marque au dessous
		Above,			// marque au dessus
		Left,			// marque à gauche
		Right,			// marque à droite
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

		
		public ButtonDisplayMode DisplayMode
		{
			//	Mode d'affichage du contenu du bouton.
			get
			{
				return (ButtonDisplayMode) this.GetValue(MetaButton.DisplayModeProperty);
			}

			set
			{
				this.SetValue(MetaButton.DisplayModeProperty, value);
			}
		}

		public ButtonMarkDisposition MarkDisposition
		{
			//	Emplacement de la marque.
			get
			{
				return (ButtonMarkDisposition) this.GetValue(MetaButton.MarkDispositionProperty);
			}

			set
			{
				this.SetValue(MetaButton.MarkDispositionProperty, value);
			}
		}

		public double MarkDimension
		{
			//	Dimension de la marque.
			get
			{
				return (double) this.GetValue(MetaButton.MarkDimensionProperty);
			}

			set
			{
				this.SetValue(MetaButton.MarkDimensionProperty, value);
			}
		}

		public Color BulletColor
		{
			//	Couleur de la puce éventuelle (si différent de Color.Empty).
			get
			{
				return (Color) this.GetValue(MetaButton.BulletColorProperty);
			}

			set
			{
				this.SetValue(MetaButton.BulletColorProperty, value);
			}
		}

		public string PreferredIconLanguage
		{
			get
			{
				return (string) this.GetValue(MetaButton.PreferredIconLanguageProperty);
			}

			set
			{
				this.SetValue(MetaButton.PreferredIconLanguageProperty, value);
			}
		}

		public string PreferredIconStyle
		{
			get
			{
				return (string) this.GetValue(MetaButton.PreferredIconStyleProperty);
			}

			set
			{
				this.SetValue(MetaButton.PreferredIconStyleProperty, value);
			}
		}

		public override bool HasTextLabel
		{
			get
			{
				return (this.DisplayMode == ButtonDisplayMode.Text || this.DisplayMode == ButtonDisplayMode.IconAndText);
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
			if (string.IsNullOrEmpty(iconName) || this.DisplayMode == ButtonDisplayMode.Text)
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

				if (string.IsNullOrEmpty(this.PreferredIconLanguage) == false)
				{
					builder.Append(@" lang=""");
					builder.Append(this.PreferredIconLanguage);
					builder.Append(@"""");
				}

				if (string.IsNullOrEmpty(this.PreferredIconStyle) == false)
				{
					builder.Append(@" style=""");
					builder.Append(this.PreferredIconStyle);
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
					Rectangle rect = this.InsideButtonBounds;
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
				Rectangle rect = this.InsideButtonBounds;

				if (this.iconLayout != null)
				{
					rect.Left += rect.Height;
				}

				switch (this.ContentAlignment)
				{
					case ContentAlignment.BottomLeft:
					case ContentAlignment.MiddleLeft:
					case ContentAlignment.TopLeft:
						rect.Left += 5;  // espace entre le bord gauche ou l'icône et le texte
						break;
				}

				return rect;
			}
		}

		protected Rectangle InsideButtonBounds
		{
			//  Donne le rectangle utilisable à l'intérieur du cadre du bouton.
			get
			{
				Rectangle bounds = this.ButtonBounds;
				
				switch (this.ButtonStyle)
				{
					case ButtonStyle.Normal:
					case ButtonStyle.DefaultAccept:
					case ButtonStyle.DefaultAcceptAndCancel:
					case ButtonStyle.DefaultCancel:
						bounds.Deflate (4);
						break;
				}

				return bounds;
			}
		}

		protected Rectangle ButtonBounds
		{
			//	Donne le rectangle à utiliser pour le cadre du bouton.
			get
			{
				Rectangle rect = this.Client.Bounds;

				switch (this.MarkDisposition)
				{
					case ButtonMarkDisposition.Below:
						rect.Bottom += this.MarkDimension;
						break;

					case ButtonMarkDisposition.Above:
						rect.Top -= this.MarkDimension;
						break;

					case ButtonMarkDisposition.Left:
						rect.Left += this.MarkDimension;
						break;

					case ButtonMarkDisposition.Right:
						rect.Right -= this.MarkDimension;
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

			if (this.ActiveState == ActiveState.Yes && this.MarkDisposition != ButtonMarkDisposition.None)  // dessine la marque triangulaire ?
			{
				Path path = new Path();
				double middle;
				double factor = 1.0;
				double m = this.MarkDimension;

				switch ( this.MarkDisposition )
				{
					case ButtonMarkDisposition.Below:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Bottom);
						path.LineTo(middle-m*factor, rect.Bottom+m);
						path.LineTo(middle+m*factor, rect.Bottom+m);
						break;

					case ButtonMarkDisposition.Above:
						middle = (rect.Left+rect.Right)/2;
						path.MoveTo(middle, rect.Top);
						path.LineTo(middle-m*factor, rect.Top-m);
						path.LineTo(middle+m*factor, rect.Top-m);
						break;

					case ButtonMarkDisposition.Left:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Left, middle);
						path.LineTo(rect.Left+m, middle-m*factor);
						path.LineTo(rect.Left+m, middle+m*factor);
						break;

					case ButtonMarkDisposition.Right:
						middle = (rect.Bottom+rect.Top)/2;
						path.MoveTo(rect.Right, middle);
						path.LineTo(rect.Right-m, middle-m*factor);
						path.LineTo(rect.Right-m, middle+m*factor);
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
			if (!this.BulletColor.IsEmpty)
			{
				Rectangle r = rect;
				r.Deflate(3.5);
				r.Width = r.Height;

				graphics.AddFilledRectangle(r);
				graphics.RenderSolid(this.BulletColor);

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


		private static void HandleGeometryChanged(DependencyObject obj, object oldValue, object newObject)
		{
			MetaButton that = obj as MetaButton;

			that.UpdateIcon(that.IconName);
			that.Invalidate();
		}

		public static readonly DependencyProperty DisplayModeProperty           = DependencyProperty.Register("DisplayMode", typeof(ButtonDisplayMode), typeof(MetaButton), new DependencyPropertyMetadata(ButtonDisplayMode.Automatic, MetaButton.HandleGeometryChanged));
		public static readonly DependencyProperty MarkDispositionProperty       = DependencyProperty.Register("MarkDisposition", typeof(ButtonMarkDisposition), typeof(MetaButton), new DependencyPropertyMetadata(ButtonMarkDisposition.None, MetaButton.HandleGeometryChanged));
		public static readonly DependencyProperty MarkDimensionProperty         = DependencyProperty.Register("MarkDimension", typeof(double), typeof(MetaButton), new DependencyPropertyMetadata(8.0, MetaButton.HandleGeometryChanged));
		public static readonly DependencyProperty BulletColorProperty           = DependencyProperty.Register("BulletColor", typeof(Color), typeof(MetaButton), new DependencyPropertyMetadata(Color.Empty, MetaButton.HandleGeometryChanged));
		public static readonly DependencyProperty PreferredIconLanguageProperty = DependencyProperty.Register("PreferredIconLanguage", typeof(string), typeof(MetaButton), new DependencyPropertyMetadata(System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName, MetaButton.HandleGeometryChanged));
		public static readonly DependencyProperty PreferredIconStyleProperty    = DependencyProperty.Register("PreferredIconStyle", typeof(string), typeof(MetaButton), new DependencyPropertyMetadata(null, MetaButton.HandleGeometryChanged));

		protected TextLayout			iconLayout;
	}
}
